# SEC-01 — JWT Authentication y Tenant Isolation en WorkflowService

## 1. Objetivo

Implementar y validar autenticación JWT y aislamiento por tenant en las operaciones HTTP de `WorkflowService`.

La implementación debe garantizar que:

- Los endpoints protegidos requieran un usuario autenticado.
- La autorización por rol continúe funcionando.
- `TenantId` sea obtenido del JWT autenticado.
- Un `TenantId` proporcionado externamente por el cliente no sea considerado autoridad.
- Un usuario de un tenant no pueda operar procesos pertenecientes a otro tenant.
- El procesamiento asíncrono mediante RabbitMQ permanezca independiente del contexto HTTP.
- `/health` pueda permanecer accesible sin autenticación.
- Swagger soporte autenticación Bearer en Development.
- Existan Integration Tests para validar el comportamiento HTTP real.

---

## 2. Alcance

SEC-01 protege las operaciones HTTP de `WorkflowService`.

El flujo HTTP queda conceptualmente:

```text
Client
  ↓
JWT Bearer
  ↓
Authentication
  ↓
Authorization
  ↓
TenantContext
  ↓
Controller
  ↓
UseCase
  ↓
DataStore
  ↓
WorkflowDbContext
```

El procesamiento asíncrono mediante RabbitMQ conserva su flujo independiente:

```text
Integration Event
  ↓
MassTransit Consumer
  ↓
UseCase
  ↓
DataStore
  ↓
WorkflowDbContext
```

No se introdujo dependencia de `HttpContext` en:

- Domain.
- UseCases.
- DataStores.
- `WorkflowDbContext`.
- Consumers.
- Procesamiento RabbitMQ.
- Outbox.

---

## 3. Autenticación JWT

`WorkflowService` utiliza JWT Bearer Authentication.

La configuración se realiza mediante:

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
```

Se creó `JwtOptionsSetup` para configurar `JwtBearerOptions` mediante `IConfigureNamedOptions<JwtBearerOptions>`.

Esta configuración valida:

- Signing key.
- Issuer.
- Audience.
- Lifetime.
- Claim de usuario `sub`.
- Claim de tenant `tenant_id`.

Los claims `sub` y `tenant_id` deben representar `Guid` válidos y diferentes de `Guid.Empty`.

Conceptualmente:

```text
JWT
 ├── sub
 ├── tenant_id
 ├── role
 ├── issuer
 ├── audience
 └── signature
      ↓
JwtBearerHandler
      ↓
OnTokenValidated
 ├── sub válido
 └── tenant_id válido
```

La validación del rol permanece como responsabilidad de Authorization y no de `OnTokenValidated`.

---

## 4. TenantContext

Se agregó la abstracción:

```csharp
public interface ITenantContext
{
    Guid TenantId { get; }
}
```

Su implementación HTTP obtiene `tenant_id` desde el usuario autenticado mediante `IHttpContextAccessor`.

El contexto rechaza:

- Claim `tenant_id` ausente.
- Claim que no representa un `Guid`.
- `Guid.Empty`.

Se registraron las dependencias:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();
```

Los Controllers utilizan `ITenantContext` para obtener el tenant de la petición autenticada.

El cliente no proporciona un `TenantId` autoritativo mediante el Request.

El flujo queda:

```text
JWT
 ↓
tenant_id
 ↓
TenantContext
 ↓
Controller
 ↓
Command.TenantId
 ↓
UseCase
```

---

## 5. Identidad del usuario

El identificador del usuario se obtiene desde el claim:

```text
sub
```

La extensión utilizada para recuperar el usuario valida también que el identificador:

- Sea un `Guid`.
- No sea `Guid.Empty`.

En las operaciones de decisión, este identificador se propaga como:

```text
JWT.sub
   ↓
DecidedBy
```

Por tanto, el usuario que toma la decisión proviene de la identidad autenticada y no de información proporcionada por el cliente.

---

## 6. Authorization

Los endpoints de decisión de `ReviewProcessesController` permanecen protegidos para el rol correspondiente.

Para las operaciones evaluadas en SEC-01:

```text
Role = Coordinator
```

Authentication y Authorization conservan responsabilidades separadas:

```text
Authentication
 ├── JWT válido
 ├── sub válido
 └── tenant_id válido

Authorization
 └── Role = Coordinator
```

---

## 7. Tenant isolation en ReviewProcess

Las operaciones de decisión utilizan `TenantId` como parte del criterio de recuperación del Aggregate.

`ApproveUseCase`, por ejemplo, obtiene el proceso mediante:

```csharp
_store.GetByIdAsync(
    command.TenantId,
    command.ProcessId,
    cancellationToken);
```

`ReviewStore` aplica ambos identificadores:

```csharp
process =>
    process.TenantId == tenantId &&
    process.Id == processId
```

Por tanto, el identificador de un `ReviewProcess` por sí solo no es suficiente para recuperar un proceso.

Conceptualmente:

```text
ReviewProcess identity for HTTP operation
        =
TenantId + ProcessId
```

Esto evita que un usuario autenticado para Tenant A pueda operar un proceso perteneciente a Tenant B aunque conozca su `ProcessId`.

---

## 8. Comportamiento cross-tenant

Para evitar revelar la existencia de recursos pertenecientes a otro tenant, un proceso que existe físicamente pero pertenece a otro tenant no es recuperado por el DataStore.

Ejemplo:

```text
JWT
TenantId = Tenant A

ReviewProcess
TenantId = Tenant B
ProcessId = X

        ↓

GetByIdAsync(Tenant A, X)

        ↓

No encontrado

        ↓

404 Not Found
```

No se realiza una consulta adicional para determinar si el `ProcessId` existe en otro tenant.

De esta manera, desde la perspectiva de Tenant A, el recurso de Tenant B no existe.

---

## 9. Outbox

La arquitectura existente de Outbox no fue modificada.

Cuando una decisión válida es aplicada:

```text
ReviewProcess.Approve()
        ↓
ApprovedEvent
        ↓
ReviewStore.SaveDecisionAsync()
        ↓
WorkflowDbContext
 ├── UPDATE ReviewProcess
 └── INSERT OutboxMessage
        ↓
SaveChangesAsync()
```

El cambio del Aggregate y la creación del `OutboxMessage` permanecen dentro del mismo `WorkflowDbContext` y se persisten mediante el mismo `SaveChangesAsync`.

SEC-01 no modifica el mecanismo de publicación posterior del Outbox.

---

## 10. RabbitMQ

La implementación de `TenantContext` se limita al boundary HTTP.

No se agregó dependencia de `ITenantContext` o `HttpContext` al flujo RabbitMQ.

El procesamiento asíncrono continúa obteniendo la información necesaria desde el Integration Event:

```text
RabbitMQ
   ↓
Integration Event
   ↓
Consumer
   ↓
UseCase
   ↓
ReviewProcess
```

Esto evita acoplar Consumers o procesos background a un contexto HTTP que no existe durante el procesamiento de mensajes.

---

## 11. Integration Tests HTTP

Se creó un proyecto de Integration Tests en:

```text
tests/
└── integration/
    └── SIA.WorkflowService.IntegrationTests/
```

Los tests utilizan:

- `WebApplicationFactory<Program>`.
- ASP.NET Core real.
- Authentication real.
- Authorization real.
- JWT firmado.
- `TenantContext` real.
- Controllers reales.
- UseCases reales.
- `ReviewStore` real.
- EF Core.
- SQLite in-memory.

No se utilizan mocks para reemplazar Authentication, Authorization, Controller, UseCase o DataStore.

---

## 12. Base de datos de Integration Tests

La base SQL Server de producción se reemplaza por SQLite in-memory.

Durante la configuración del `WebApplicationFactory` se eliminan tanto:

```text
DbContextOptions<WorkflowDbContext>
```

como:

```text
IDbContextOptionsConfiguration<WorkflowDbContext>
```

antes de registrar SQLite.

Esto es necesario para evitar que EF Core intente utilizar simultáneamente:

```text
SqlServer + SQLite
```

lo cual produce:

```text
Only a single database provider can be registered
in a service provider.
```

La conexión SQLite permanece abierta durante la vida del `WorkflowApiFactory`.

El esquema se crea mediante:

```csharp
await dbContext.Database.EnsureCreatedAsync();
```

por lo que se utilizan las configuraciones EF Core reales de `WorkflowService`.

---

## 13. Aislamiento de infraestructura externa

Los Integration Tests HTTP no requieren:

- SQL Server real.
- RabbitMQ real.
- Docker.
- Servicios externos.

Se deshabilitan específicamente:

```text
OutboxPublisherService
MassTransit Hosted Service
```

No se eliminan indiscriminadamente todos los `IHostedService`.

La infraestructura cubierta queda:

```text
REAL
────────────────────────
ASP.NET Core
JWT Authentication
Authorization
TenantContext
Controller
UseCase
ReviewStore
EF Core
SQLite
Outbox persistence


AISLADO
────────────────────────
SQL Server
RabbitMQ
MassTransit Bus
OutboxPublisherService
```

El objetivo de estos tests es validar el flujo HTTP y la persistencia del Outbox, no demostrar la publicación física del Integration Event hacia RabbitMQ.

---

## 14. JWT utilizado en Integration Tests

Los tests generan JWT reales firmados con una signing key exclusiva para pruebas.

Los tokens incluyen:

```text
sub
tenant_id
role
issuer
audience
expiration
```

El JWT debe atravesar el `JwtBearerHandler` real.

No se reemplaza Authentication mediante mocks, fake handlers o identidades insertadas directamente en `HttpContext`.

La signing key utilizada en los tests no corresponde a un secret real de ningún ambiente.

---

## 15. Casos de prueba

### 15.1 Health sin autenticación

```text
Health_WithoutAuthentication_ShouldReturnOk
```

Escenario:

```text
GET /health
sin JWT
```

Resultado esperado:

```text
200 OK
```

Confirma que el Health Check puede permanecer anónimo.

---

### 15.2 Endpoint protegido sin autenticación

```text
Approve_WithoutAuthentication_ShouldReturnUnauthorized
```

Escenario:

```text
POST /api/review-processes/{processId}/approve
sin JWT
```

Resultado esperado:

```text
401 Unauthorized
```

Confirma que Authentication detiene la petición antes de ejecutar la operación protegida.

---

### 15.3 JWT válido y mismo tenant

```text
Approve_WithValidTenantToken_ShouldReturnNoContent
```

Se crea:

```text
JWT
 ├── sub       = User A
 ├── tenant_id = Tenant A
 └── role      = Coordinator

ReviewProcess
 └── TenantId  = Tenant A
```

Se ejecuta:

```text
POST /api/review-processes/{processId}/approve
```

Resultado:

```text
204 No Content
```

Además se verifica:

- El `ReviewProcess` cambia a `Approved`.
- `DecidedBy` corresponde al `sub` del JWT.
- `DecisionCorrelationId` corresponde al CorrelationId de la petición.
- Se persiste el `OutboxMessage`.
- El tipo del evento corresponde a `ProposalApprovedV1`.

Este test valida el recorrido:

```text
JWT
 ↓
Authentication
 ↓
Authorization
 ↓
TenantContext
 ↓
Controller
 ↓
ApproveCommand
 ↓
ApproveUseCase
 ↓
ReviewStore
 ↓
ReviewProcess
 ↓
Outbox
```

---

### 15.4 Intento cross-tenant

Se agregó adicionalmente:

```text
Approve_WithDifferentTenantToken_ShouldReturnNotFound
```

Este escenario va más allá del mínimo solicitado y prueba explícitamente el aislamiento entre tenants.

Se prepara:

```text
JWT
tenant_id = Tenant A

ReviewProcess
TenantId = Tenant B
```

El `ProcessId` utilizado en la petición sí existe físicamente.

Resultado esperado:

```text
404 Not Found
```

Además se verifica que:

```text
ReviewProcess.Status                = InReview
ReviewProcess.DecidedBy             = null
ReviewProcess.DecidedAtUtc          = null
ReviewProcess.DecisionCorrelationId = null
OutboxMessage                       = no creado
```

Esto demuestra que el intento cross-tenant no solamente es rechazado, sino que tampoco produce efectos secundarios.

---

## 16. Incidencias encontradas durante la implementación

### 16.1 Configuración JWT demasiado temprana

Inicialmente la configuración JWT era resuelta directamente durante el bootstrap de `Program.cs`.

Esto dificultaba que `WebApplicationFactory` sustituyera la configuración de JWT para los Integration Tests.

Se refactorizó la configuración a:

```text
JwtOptionsSetup
```

mediante:

```text
IConfigureNamedOptions<JwtBearerOptions>
```

De esta manera, las opciones JWT son resueltas mediante el sistema de Options y pueden utilizar la configuración proporcionada por el host de Integration Tests.

La validación de seguridad existente se conservó.

---

### 16.2 Uso incorrecto de CreateHostBuilder en WebApplicationFactory

Durante la construcción inicial de `WorkflowApiFactory` se intentó sobrescribir:

```csharp
CreateHostBuilder()
```

Esto provocó el error:

```text
No application configured...
```

La sobrescritura fue eliminada.

`WebApplicationFactory<Program>` utiliza el bootstrap real de la aplicación y las modificaciones necesarias se realizan mediante:

```csharp
ConfigureWebHost(...)
```

---

### 16.3 SQL Server y SQLite registrados simultáneamente

Inicialmente se eliminó únicamente:

```text
DbContextOptions<WorkflowDbContext>
```

antes de registrar SQLite.

Al ejecutar realmente `WorkflowDbContext`, EF Core detectó:

```text
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Sqlite
```

y produjo el error:

```text
Only a single database provider can be registered
in a service provider.
```

La solución fue eliminar también:

```text
IDbContextOptionsConfiguration<WorkflowDbContext>
```

antes de registrar SQLite.

Después de este ajuste, `WorkflowDbContext` pudo operar correctamente utilizando exclusivamente SQLite durante los Integration Tests.

---

## 17. Resultado final

Los acceptance criteria de SEC-01 quedaron cubiertos:

| Criterio | Resultado |
|---|---|
| JWT Authentication configurado | OK |
| Authorization configurado | OK |
| Endpoints protegidos requieren autenticación | OK |
| TenantId obtenido del contexto autenticado | OK |
| TenantId del cliente no es autoridad | OK |
| Health puede permanecer anónimo | OK |
| Swagger soporta Bearer en Development | OK |
| Integration Test de 401 | OK |
| Integration Test con tenant válido | OK |
| Validación adicional cross-tenant | OK |
| Sin efectos secundarios cross-tenant | OK |
| RabbitMQ independiente de HttpContext | OK |
| Outbox preservado | OK |

La suite completa de Integration Tests fue ejecutada satisfactoriamente.

---

## 18. Estado

**SEC-01 completada.**

WorkflowService cuenta ahora con protección JWT para sus operaciones HTTP y aislamiento por tenant basado en la identidad autenticada.

La implementación mantiene separados los boundaries HTTP y RabbitMQ, conserva el patrón Outbox existente y dispone de Integration Tests que verifican tanto el acceso válido como el rechazo de accesos cross-tenant.