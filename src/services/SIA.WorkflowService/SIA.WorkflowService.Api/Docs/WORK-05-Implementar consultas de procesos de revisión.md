# WORK-05 — Consultas de procesos de revisión

## Objetivo

Implementar en `WorkflowService` las consultas necesarias para que Coordinación Académica pueda:

- Consultar los procesos de revisión pendientes (`InReview`).
- Consultar el detalle de un proceso de revisión por `Id`.
- Mantener aislamiento estricto por `TenantId`.
- Consumir contratos propios de `WorkflowService`.
- Exponer las consultas mediante endpoints HTTP protegidos.
- Contar con pruebas automatizadas para comportamiento funcional y aislamiento multi-tenant.

---

## Alcance implementado

Se implementó el Read Side de procesos de revisión siguiendo el enfoque de **CQRS parcial/pragmático** utilizado en SIA.

La solución mantiene separadas las responsabilidades de escritura y lectura:

```text
WRITE SIDE
──────────
POST approve / return
       │
       ▼
    UseCase
       │
       ▼
 IReviewStore
       │
       ▼
  ReviewStore
       │
       ▼
ReviewProcess Aggregate
       │
       ▼
     Outbox


READ SIDE
─────────
GET review-processes
GET review-processes/{id}
       │
       ▼
IReviewProcessQueries
       │
       ▼
ReviewProcessQueries
       │
       ▼
EF Core / AsNoTracking
       │
       ▼
WorkflowService.Contracts
```

No se modificó `IReviewStore` para agregar operaciones de consulta destinadas a UI.

---

# 1. Contracts

Se agregaron Responses propios de `WorkflowService`:

```text
SIA.WorkflowService.Contracts
└── Responses
    └── ReviewProcesses
        ├── ReviewProcessListItemResponse.cs
        ├── ReviewProcessResponse.cs
        └── ReviewObservationResponse.cs
```

## ReviewProcessListItemResponse

Representa un proceso dentro del listado de procesos pendientes.

Incluye información como:

- `Id`
- `AcademicLoadProposalId`
- `DivisionHeadId`
- `Version`
- `SubmittedAtUtc`
- `CreatedAtUtc`

`TenantId` no se expone como parte del Response.

El tenant funciona como criterio de aislamiento y autorización, no como información que el cliente deba proporcionar para seleccionar el contexto.

## ReviewProcessResponse

Representa el detalle de un proceso de revisión.

Incluye:

- Identificación del proceso.
- Propuesta académica asociada.
- Jefe de división.
- Versión.
- Estado.
- Fechas relevantes.
- CorrelationId.
- Observaciones asociadas al proceso.

## ReviewObservationResponse

Representa las observaciones asociadas al proceso.

El contrato utiliza tipos pertenecientes a `WorkflowService.Contracts`, evitando exponer directamente tipos de `Domain`.

---

# 2. Application — Query Contract

Se agregó:

```text
SIA.WorkflowService.Application
└── Interfaces
    └── Queries
        └── IReviewProcessQueries.cs
```

El contrato contiene las operaciones:

```csharp
Task<IReadOnlyList<ReviewProcessListItemResponse>> GetInReviewAsync(
    Guid tenantId,
    CancellationToken cancellationToken);

Task<ReviewProcessResponse?> GetByIdAsync(
    Guid tenantId,
    Guid processId,
    CancellationToken cancellationToken);
```

## Decisión de multi-tenancy

`TenantId` forma parte explícita de ambas operaciones.

No existe una operación equivalente a:

```csharp
GetByIdAsync(Guid processId, ...)
```

Esto obliga a que la implementación de las consultas conozca el tenant bajo el cual se está realizando la operación.

---

# 3. Infrastructure — ReviewProcessQueries

Se agregó:

```text
SIA.WorkflowService.Infrastructure
└── Persistence
    └── Queries
        └── ReviewProcessQueries.cs
```

`ReviewProcessQueries` implementa `IReviewProcessQueries` utilizando `WorkflowDbContext`.

Las consultas utilizan:

```csharp
AsNoTracking()
```

debido a que corresponden exclusivamente a operaciones de lectura.

## Consulta de procesos pendientes

El listado aplica simultáneamente los filtros:

```csharp
process.TenantId == tenantId &&
process.Status == ReviewStatus.InReview
```

Por lo tanto:

- Sólo se recuperan procesos del tenant autenticado.
- Sólo se recuperan procesos cuyo estado es `InReview`.

El filtrado ocurre directamente en la consulta ejecutada por EF Core.

## Consulta por Id

La búsqueda utiliza simultáneamente:

```csharp
process.TenantId == tenantId &&
process.Id == processId
```

No se recupera primero el proceso para comprobar posteriormente su `TenantId`.

Esto evita recuperar información perteneciente a otro tenant.

Desde la perspectiva del Tenant A, un `ProcessId` perteneciente al Tenant B se comporta igual que un recurso inexistente.

---

# 4. Dependency Injection

Se registró la Query en `Program.cs`:

```csharp
builder.Services.AddScoped<IReviewProcessQueries, ReviewProcessQueries>();
```

El lifetime utilizado es `Scoped`, consistente con `WorkflowDbContext`.

No fue necesario modificar el registro existente de:

```csharp
builder.Services.AddScoped<IReviewStore, ReviewStore>();
```

---

# 5. Exposición HTTP

Se extendió `ReviewProcessesController` para incorporar las operaciones de lectura.

El Controller ya se encontraba protegido mediante:

```csharp
[Authorize(Roles = nameof(RoleCode.Coordinator))]
```

y dispone de `ITenantContext`.

Se agregaron los endpoints:

```http
GET /api/review-processes

GET /api/review-processes/{processId}
```

## GET /api/review-processes

Obtiene los procesos pendientes (`InReview`) correspondientes al tenant autenticado.

El `TenantId` se obtiene mediante:

```csharp
_tenantContext.TenantId
```

y no mediante Route, Query String o Request Body.

Flujo:

```text
JWT
 │
 ├── tenant_id
 │
 ▼
ITenantContext
 │
 ▼
ReviewProcessesController
 │
 ▼
IReviewProcessQueries.GetInReviewAsync(tenantId)
 │
 ▼
WHERE TenantId = tenantId
  AND Status = InReview
```

## GET /api/review-processes/{processId}

Obtiene el detalle del proceso correspondiente al tenant autenticado.

Si el proceso:

- no existe, o
- pertenece a otro tenant,

la Query devuelve `null` y el Controller responde:

```http
404 Not Found
```

No se revela al consumidor si el identificador solicitado corresponde a un recurso existente perteneciente a otro tenant.

---

# 6. Multi-tenancy

El aislamiento quedó implementado en varias capas.

## Fuente del TenantId

El cliente no proporciona el `TenantId`.

El flujo utilizado es:

```text
JWT
 │
 └── tenant_id
       │
       ▼
 ITenantContext
       │
       ▼
 Controller
       │
       ▼
 IReviewProcessQueries
       │
       ▼
 EF Core
```

## Listado

Una consulta realizada para Tenant A no devuelve procesos `InReview` pertenecientes a Tenant B.

## Consulta por Id

Si existe:

```text
Tenant B
└── ProcessId = BBB
```

y Tenant A solicita:

```http
GET /api/review-processes/BBB
```

la consulta se ejecuta conceptualmente como:

```sql
WHERE TenantId = TenantA
  AND Id = BBB
```

El resultado es vacío y HTTP responde:

```http
404 Not Found
```

Esto evita exposición de información cross-tenant.

---

# 7. Query Tests

Se agregó:

```text
SIA.WorkflowService.Tests
└── Infrastructure
    └── Queries
        └── ReviewProcessQueriesTests.cs
```

Las pruebas utilizan:

- xUnit.
- EF Core.
- SQLite In-Memory.
- `WorkflowDbContext` real.

No se utiliza `Mock<IReviewProcessQueries>` porque el objetivo es probar la implementación real de las consultas EF Core.

Se implementaron siete pruebas.

## 1. Listar solamente procesos InReview

```text
GetInReviewAsync_WhenProcessesExist_ShouldReturnOnlyInReviewProcessesForTenant
```

Comprueba que un proceso `Approved` del mismo tenant no aparezca en la consulta de pendientes.

## 2. Aislamiento de tenant en listado

```text
GetInReviewAsync_WhenAnotherTenantHasProcesses_ShouldNotReturnThem
```

Se crean procesos `InReview` para dos tenants diferentes y se comprueba que únicamente se devuelvan los correspondientes al tenant solicitado.

## 3. Listado vacío

```text
GetInReviewAsync_WhenNoProcessesExist_ShouldReturnEmptyList
```

Comprueba que la Query devuelva una colección vacía cuando no existen procesos aplicables.

## 4. Obtener proceso por Id

```text
GetByIdAsync_WhenProcessBelongsToTenant_ShouldReturnProcess
```

Comprueba la recuperación y proyección de un proceso perteneciente al tenant solicitado.

## 5. Aislamiento cross-tenant por Id

```text
GetByIdAsync_WhenProcessBelongsToAnotherTenant_ShouldReturnNull
```

Este es uno de los tests principales de aislamiento.

El proceso existe físicamente en la base de datos, pero pertenece a otro tenant.

El resultado esperado es:

```csharp
Assert.Null(result);
```

## 6. Proceso inexistente

```text
GetByIdAsync_WhenProcessDoesNotExist_ShouldReturnNull
```

Comprueba el comportamiento ante un identificador inexistente.

## 7. Proyección de Observations

```text
GetByIdAsync_WhenProcessHasObservations_ShouldReturnObservations
```

Comprueba el mapping de las observaciones de Domain hacia `ReviewObservationResponse`.

Se validan datos como:

- `Id`
- `TargetType`
- `TargetId`
- `Description`
- `CreatedBy`
- `CreatedAtUtc`

Los siete Query Tests finalizaron correctamente.

---

# 8. Integration Tests HTTP

Se reutilizó el proyecto existente:

```text
SIA.WorkflowService.IntegrationTests
├── Infrastructure
│   ├── JwtTokenFactory.cs
│   └── WorkflowApiFactory.cs
│
└── ReviewProcesses
    └── ReviewProcessesTests.cs
```

No fue necesario crear una nueva infraestructura para WORK-05.

## WorkflowApiFactory

La Factory existente:

- utiliza `WebApplicationFactory<Program>`;
- sustituye SQL Server por SQLite In-Memory;
- mantiene una conexión SQLite para los tests;
- elimina `OutboxPublisherService`;
- elimina el Hosted Service de MassTransit;
- evita conexiones reales a RabbitMQ.

Esto permite ejecutar pruebas HTTP contra el pipeline real de ASP.NET Core sin depender de:

- SQL Server real;
- RabbitMQ real;
- Docker.

## JwtTokenFactory

Se reutilizó `JwtTokenFactory` para generar JWT de prueba con:

- `sub`
- `tenant_id`
- `role`

Esto permite probar el flujo real:

```text
JWT
 ↓
Authentication
 ↓
Authorization
 ↓
ITenantContext
 ↓
Controller
 ↓
Query
 ↓
Database
```

---

# 9. Nuevos Integration Tests

Se agregaron tres pruebas HTTP específicas para WORK-05.

## 1. Listado HTTP

```text
GetInReview_WithValidTenantToken_ShouldReturnOnlyInReviewProcessesForTenant
```

Comprueba mediante HTTP que:

- la respuesta sea `200 OK`;
- solamente se devuelvan procesos `InReview`;
- no se devuelvan procesos finalizados del mismo tenant;
- no se devuelvan procesos de otro tenant.

## 2. Detalle HTTP

```text
GetById_WithValidTenantToken_ShouldReturnProcess
```

Comprueba que:

```http
GET /api/review-processes/{processId}
```

devuelva:

```http
200 OK
```

cuando el proceso pertenece al tenant autenticado.

También comprueba que la respuesta pueda deserializarse correctamente como `ReviewProcessResponse`.

## 3. Aislamiento HTTP cross-tenant

```text
GetById_WithDifferentTenantToken_ShouldReturnNotFound
```

Se crea un proceso perteneciente al Tenant B y se realiza la petición utilizando un JWT correspondiente al Tenant A.

El resultado esperado es:

```http
404 Not Found
```

De esta manera el aislamiento queda comprobado de extremo a extremo:

```text
JWT Tenant A
     │
     ▼
ITenantContext = Tenant A
     │
     ▼
Controller
     │
     ▼
GetByIdAsync(TenantA, ProcessB)
     │
     ▼
EF Core
     │
     ▼
null
     │
     ▼
404 Not Found
```

Los tres Integration Tests finalizaron correctamente.

---

# 10. Relación con SEC-01

WORK-05 depende de SEC-01 para la exposición HTTP final.

Se reutilizó la infraestructura de seguridad ya existente en WorkflowService:

- JWT Bearer Authentication.
- Autorización mediante rol `Coordinator`.
- `ITenantContext`.
- Obtención de `TenantId` desde el contexto autenticado.
- Semántica `404 Not Found` para acceso cross-tenant.

No se duplicaron innecesariamente pruebas genéricas de autenticación que ya pertenecen a SEC-01.

WORK-05 se concentra en probar:

- comportamiento de las Queries;
- aislamiento multi-tenant de las Queries;
- exposición HTTP de las nuevas consultas;
- aislamiento cross-tenant a través de HTTP.

---

# 11. Acceptance Criteria

| Acceptance Criterion | Resultado |
|---|---|
| Listar `ReviewProcesses` `InReview` | ✅ Implementado |
| Obtener `ReviewProcess` por Id | ✅ Implementado |
| Todas las Queries filtradas por `TenantId` | ✅ Implementado |
| No devolver información de otro tenant | ✅ Implementado y probado |
| Exponer contratos propios de WorkflowService | ✅ Implementado |
| Tests de Queries | ✅ 7 tests |
| Tests de aislamiento | ✅ Query + HTTP |
| Exposición HTTP | ✅ Implementada |
| Integración con seguridad SEC-01 | ✅ Reutilizada |

---

# 12. Resultado final

WORK-05 queda implementado con separación entre Read Side y Write Side.

No fue necesario modificar:

- `ReviewProcess` para soportar las consultas.
- `IReviewStore` para incorporar operaciones de lectura destinadas a UI.
- `ReviewStore` para implementar el listado.
- Integration Events existentes.
- Outbox.
- Esquema de base de datos.
- Migraciones.

La solución mantiene el enfoque de CQRS pragmático utilizado en SIA:

```text
Commands / modificaciones
        ↓
     UseCases
        ↓
    DataStores
        ↓
      Domain


Queries / lecturas
        ↓
      Queries
        ↓
 EF Core Projection
        ↓
 Response Contracts
```

El aislamiento por `TenantId` está aplicado directamente en las consultas y protegido mediante pruebas automatizadas tanto a nivel de Infrastructure como HTTP.

**Estado final: WORK-05 completado.**

---

# Nota técnica

Actualmente `WorkflowApiFactory` mantiene una conexión SQLite In-Memory durante su ciclo de vida y utiliza `EnsureCreatedAsync()` sin realizar un reset explícito de la base de datos entre cada test.

Los tests actuales utilizan identificadores y `TenantId` generados dinámicamente, por lo que esto no produjo interferencias durante WORK-05.

Si la suite de Integration Tests continúa creciendo o comienza a utilizar fixtures con tenants/identificadores compartidos, conviene evaluar posteriormente una estrategia explícita de limpieza o reset de la base de datos entre tests.

Este punto no bloquea WORK-05 y no fue modificado como parte de su alcance.