# BFF-01 — Definir e implementar estándar base de AdminBff

## Descripción

Actualmente `AdminBff` se encuentra como un proyecto base compuesto principalmente por `Program.cs`, configuración y archivo de proyecto, sin una estructura funcional implementada.

Esta tarea establece la estructura, responsabilidades y reglas técnicas que deberá seguir `AdminBff` antes de comenzar la implementación de clientes internos y endpoints.

La tarea **no implementa todavía clientes ni endpoints funcionales**. Su objetivo es establecer el estándar y definir los contratos necesarios para el primer tramo del flujo PR-001 — Planeación Académica.

---

## Objetivo

Establecer una arquitectura base para `SIA.AdminBff` que permita:

* Exponer contratos orientados a las necesidades del frontend.
* Organizar los endpoints del BFF.
* Organizar los clientes utilizados para comunicarse con servicios internos.
* Definir la configuración de las URLs internas.
* Definir los contratos `Request` y `Response`.
* Establecer reglas para la composición de información.
* Propagar correctamente la autenticación.
* Propagar `TenantId`.
* Propagar `CorrelationId`.
* Estandarizar el manejo de errores provenientes de servicios internos.
* Mantener las reglas de negocio dentro de los servicios propietarios.
* Definir las operaciones necesarias para el primer tramo de PR-001.

---

# Principios del AdminBff

`AdminBff` funciona como una capa de adaptación entre el frontend y los servicios internos de SIA.

Su responsabilidad principal es adaptar las necesidades del frontend a los contratos de los servicios internos y, cuando sea necesario, componer información proveniente de más de un servicio.

```text id="bffprinciples"
┌──────────────────────┐
│      Angular         │
└──────────┬───────────┘
           │
           │ HTTP
           ▼
┌──────────────────────┐
│      AdminBff        │
│                      │
│ Adaptación           │
│ Composición          │
│ Contratos            │
│ Seguridad técnica    │
│ Manejo de errores    │
└──────────┬───────────┘
           │
           │ HTTP / contratos internos
           ▼
┌──────────────────────────────────────┐
│          Servicios SIA               │
│                                      │
│ AcademicService                      │
│ AcademicStaffService                 │
│ SchedulingService                    │
│ WorkflowService                      │
└──────────────────────────────────────┘
```

`AdminBff` **no sustituye a los servicios de dominio**.

Los servicios internos continúan siendo propietarios de sus datos y reglas de negocio.

---

# Responsabilidades

## Responsabilidades de AdminBff

`AdminBff` puede:

* Exponer endpoints orientados al frontend.
* Adaptar `Request` del frontend a los contratos internos.
* Adaptar respuestas internas a contratos `Response` del frontend.
* Componer información proveniente de varios servicios.
* Propagar información de contexto.
* Propagar autenticación.
* Propagar `TenantId`.
* Normalizar errores técnicos provenientes de servicios internos.
* Ocultar al frontend la estructura interna de los microservicios.
* Coordinar llamadas necesarias para construir una respuesta.

---

## Responsabilidades prohibidas

`AdminBff` no debe:

* Implementar reglas académicas.
* Determinar si un docente tiene horas disponibles.
* Determinar qué docente debe impartir una materia.
* Validar perfiles académicos.
* Determinar la carga académica permitida.
* Crear o modificar grupos directamente.
* Crear o modificar materias directamente.
* Crear o modificar planes de estudio directamente.
* Determinar reglas de horarios.
* Determinar disponibilidad de aulas.
* Aprobar o rechazar cargas académicas.
* Acceder directamente a la base de datos de otro servicio.
* Mantener copias de las bases de datos de los servicios.
* Convertirse en un servicio de dominio.
* Duplicar reglas que ya pertenecen a `AcademicService`, `AcademicStaffService`, `SchedulingService` o `WorkflowService`.

---

# Estructura propuesta

La estructura inicial de `AdminBff` deberá separar endpoints, contratos, clientes y componentes transversales.

```text id="bffstructure"
SIA.AdminBff
│
├── Endpoints/
│   ├── Academic/
│   ├── AcademicStaff/
│   ├── Scheduling/
│   └── Workflow/
│
├── Contracts/
│   ├── Academic/
│   │   ├── Requests/
│   │   └── Responses/
│   │
│   ├── AcademicStaff/
│   │   ├── Requests/
│   │   └── Responses/
│   │
│   ├── Scheduling/
│   │   ├── Requests/
│   │   └── Responses/
│   │
│   └── Workflow/
│       ├── Requests/
│       └── Responses/
│
├── Clients/
│   ├── Academic/
│   ├── AcademicStaff/
│   ├── Scheduling/
│   └── Workflow/
│
├── Infrastructure/
│   ├── Authentication/
│   ├── Tenancy/
│   ├── Errors/
│   └── Http/
│
├── Configuration/
│
├── Extensions/
│
└── Program.cs
```

La estructura podrá evolucionar conforme crezca el BFF, pero las responsabilidades deberán mantenerse separadas.

---

# Endpoints

Los endpoints de `AdminBff` estarán organizados de acuerdo con el contexto funcional que consumirá el frontend.

Para PR-001, el BFF deberá exponer operaciones relacionadas con Planeación Académica sin trasladar al BFF la responsabilidad de ejecutar las reglas del proceso.

Ejemplo conceptual:

```text id="bffendpoints"
AdminBff
│
├── /api/academic
│
├── /api/academic-staff
│
├── /api/scheduling
│
└── /api/workflow
```

La organización final de las rutas podrá ajustarse durante la implementación, pero deberá conservar una separación clara entre los contextos.

---

# Clientes internos

Cada servicio interno consumido por `AdminBff` deberá contar con un cliente dedicado.

```text id="bffclients"
Clients/
│
├── Academic/
│   └── IAcademicClient
│
├── AcademicStaff/
│   └── IAcademicStaffClient
│
├── Scheduling/
│   └── ISchedulingClient
│
└── Workflow/
    └── IWorkflowClient
```

Los clientes serán responsables de:

* Construir las solicitudes HTTP.
* Consumir los endpoints internos.
* Enviar headers de contexto.
* Deserializar respuestas.
* Identificar errores HTTP.
* Exponer una interfaz consumible por los endpoints del BFF.

Los clientes **no deben implementar reglas de negocio**.

---

# Registro de clientes internos

Los clientes deberán registrarse mediante `HttpClient` utilizando `IHttpClientFactory`.

Ejemplo conceptual:

```csharp
services.AddHttpClient<IAcademicClient, AcademicClient>();
services.AddHttpClient<IAcademicStaffClient, AcademicStaffClient>();
services.AddHttpClient<ISchedulingClient, SchedulingClient>();
services.AddHttpClient<IWorkflowClient, WorkflowClient>();
```

La implementación concreta deberá respetar la configuración de URLs internas definida para cada servicio.

El registro de clientes deberá centralizarse para evitar la creación manual de `HttpClient` dentro de los endpoints.

---

# Configuración de URLs internas

Las URLs de los servicios internos no deberán estar codificadas directamente dentro de los clientes.

La configuración deberá mantenerse fuera del código.

Ejemplo conceptual:

```json
{
  "Services": {
    "AcademicService": {
      "BaseUrl": "..."
    },
    "AcademicStaffService": {
      "BaseUrl": "..."
    },
    "SchedulingService": {
      "BaseUrl": "..."
    },
    "WorkflowService": {
      "BaseUrl": "..."
    }
  }
}
```

La configuración deberá poder cambiar dependiendo del ambiente:

```text
Development
      │
      ▼
URLs desarrollo

Testing
      │
      ▼
URLs pruebas

Production
      │
      ▼
URLs producción
```

No se deberá requerir modificar el código de los clientes para cambiar las URLs de los servicios.

---

# Contratos Request / Response

`AdminBff` deberá definir contratos propios orientados al frontend.

Los contratos del BFF no deben exponer directamente los modelos internos de dominio de los servicios.

```text id="bffcontracts"
Angular
   │
   │ Request
   ▼
AdminBff Contract
   │
   ▼
Internal Service Contract
```

Y para las respuestas:

```text id="bffresponse"
Internal Service
   │
   ▼
Internal Response
   │
   ▼
AdminBff Mapping
   │
   ▼
BFF Response
   │
   ▼
Angular
```

Esto permite que los servicios internos puedan evolucionar sin obligar al frontend a conocer su estructura interna.

---

# Composición de respuestas

Una de las responsabilidades principales del BFF es componer información cuando el frontend necesita datos pertenecientes a diferentes servicios.

Ejemplo para Planeación Académica:

```text id="composition"
                    AdminBff
                       │
          ┌────────────┼────────────┐
          │            │            │
          ▼            ▼            ▼
     Academic     AcademicStaff  Scheduling
     Service        Service       Service
          │            │            │
          └────────────┼────────────┘
                       │
                       ▼
              Respuesta compuesta
                       │
                       ▼
                    Angular
```

Por ejemplo, una pantalla podría necesitar:

* Periodo académico desde `AcademicService`.
* Programa educativo desde `AcademicService`.
* Información del Jefe de División desde `AcademicStaffService`.
* Carga académica desde `SchedulingService`.

`AdminBff` puede realizar las llamadas necesarias y construir una respuesta orientada a la pantalla.

Sin embargo, la composición **no significa que el BFF pueda modificar o interpretar las reglas de esos dominios**.

---

# Propiedad de la información

Cada servicio continúa siendo propietario de su información.

| Información         | Servicio propietario   |
| ------------------- | ---------------------- |
| Periodo académico   | `AcademicService`      |
| Programa educativo  | `AcademicService`      |
| Plan de estudios    | `AcademicService`      |
| Materias            | `AcademicService`      |
| Docente             | `AcademicStaffService` |
| Jefe de División    | `AcademicStaffService` |
| Grupos              | `SchedulingService`    |
| Oferta académica    | `SchedulingService`    |
| Asignación docente  | `SchedulingService`    |
| Horario             | `SchedulingService`    |
| Carga académica     | `SchedulingService`    |
| Proceso de revisión | `WorkflowService`      |

`AdminBff` solamente consume esta información mediante contratos.

---

# Autenticación

La autenticación será realizada por el mecanismo de identidad correspondiente de SIA.

`AdminBff` deberá recibir la credencial o contexto de autenticación proveniente del frontend y propagarlo hacia los servicios internos cuando el flujo lo requiera.

```text id="auth"
Angular
   │
   │ Authorization
   ▼
AdminBff
   │
   │ Authorization
   ▼
Servicio interno
```

`AdminBff` no debe:

* Crear identidades.
* Administrar usuarios.
* Administrar contraseñas.
* Crear roles.
* Implementar autorización académica.
* Sustituir a `IdentityService`.

La autenticación y autorización deberán respetar las responsabilidades establecidas para los servicios correspondientes.

---

# TenantId

`TenantId` es obligatorio para mantener el aislamiento entre instituciones.

El BFF deberá conservar y propagar el `TenantId` durante las llamadas a servicios internos.

```text id="tenant"
Frontend
   │
   │ Tenant context
   ▼
AdminBff
   │
   ├───────────────► AcademicService
   │
   ├───────────────► AcademicStaffService
   │
   ├───────────────► SchedulingService
   │
   └───────────────► WorkflowService
```

Todas las operaciones deberán ejecutarse dentro del contexto institucional correspondiente.

`AdminBff` no debe permitir que una solicitud utilice información perteneciente a otro `TenantId`.

El BFF tampoco debe utilizar un `TenantId` arbitrario recibido del frontend para cambiar el contexto institucional sin validación.

---

# Manejo de errores

Los errores provenientes de servicios internos deberán ser tratados de forma consistente antes de ser enviados al frontend.

```text id="errors"
Servicio interno
       │
       │ HTTP error
       ▼
AdminBff
       │
       ├── Identificar error
       ├── Registrar contexto
       └── Mapear respuesta
       │
       ▼
Frontend
```

El BFF deberá diferenciar, como mínimo:

* Errores de autenticación.
* Errores de autorización.
* Recursos no encontrados.
* Solicitudes inválidas.
* Conflictos.
* Errores de comunicación con servicios internos.
* Errores inesperados.

La respuesta hacia Angular deberá utilizar un formato consistente.

Ejemplo conceptual:

```json
{
  "code": "RESOURCE_NOT_FOUND",
  "message": "No se encontró el recurso solicitado.",
  "correlationId": "..."
}
```

El mensaje expuesto al frontend no deberá revelar información interna innecesaria.

---

# Comunicación entre servicios

`AdminBff` se comunicará con los servicios internos mediante sus contratos expuestos.

```text
AdminBff
   │
   ├── HTTP ──► AcademicService
   │
   ├── HTTP ──► AcademicStaffService
   │
   ├── HTTP ──► SchedulingService
   │
   └── HTTP ──► WorkflowService
```

No se permitirá:

```text
AdminBff ───────────────► Base de datos de otro servicio
```

Cada servicio debe mantener la propiedad de su propia base de datos.

---

# Operaciones necesarias para PR-001

El primer tramo de PR-001 requiere que `AdminBff` pueda componer la información necesaria para las operaciones de Planeación Académica.

Las operaciones se derivan de las responsabilidades establecidas en `PLAN-01`.

## Contexto del usuario y programa

El BFF deberá poder obtener la información necesaria para establecer el contexto con el que el usuario trabajará en Planeación Académica.

Información involucrada:

* Usuario autenticado.
* Jefe de División.
* Programas educativos asociados.
* Programa educativo seleccionado.
* TenantId.

La información del Jefe de División y sus asociaciones deberá provenir de `AcademicStaffService`.

La información del programa educativo deberá provenir de `AcademicService`.

---

## Consultar periodo académico

El BFF deberá permitir obtener el periodo académico correspondiente al contexto de Planeación Académica.

El propietario de esta información es:

`AcademicService`

El BFF solamente adaptará la información necesaria para el frontend.

---

## Consultar plan de estudios

El BFF deberá poder obtener el plan de estudios correspondiente al programa seleccionado.

Información involucrada:

* Programa educativo.
* Plan de estudios.
* Materias.
* Semestre.
* Información necesaria para la planeación.

El propietario de esta información es:

`AcademicService`

---

## Consultar docentes

El BFF deberá permitir obtener la información necesaria para mostrar docentes disponibles para el proceso de Planeación Académica.

La información del docente y su perfil pertenece a:

`AcademicStaffService`

La disponibilidad y asignación de horas pertenece a:

`SchedulingService`

Por lo tanto, si una pantalla requiere ambas informaciones, `AdminBff` podrá componerlas sin implementar la regla de disponibilidad.

```text id="teachercomposition"
                 AdminBff
                    │
          ┌─────────┴─────────┐
          │                   │
          ▼                   ▼
AcademicStaffService    SchedulingService
          │                   │
          │ Perfil            │ Disponibilidad
          │ Docente           │ Horas asignadas
          │                   │
          └─────────┬─────────┘
                    │
                    ▼
             Respuesta BFF
```

La decisión de si un docente cumple con los requisitos para una asignación continúa siendo responsabilidad del servicio propietario correspondiente.

---

## Consultar información de planeación

El BFF deberá poder obtener la información de Planeación Académica que sea necesaria para las pantallas del frontend.

Esta información puede involucrar:

* Periodo.
* Programa educativo.
* Plan de estudios.
* Grupos.
* Oferta académica.
* Docentes.
* Asignaciones.
* Carga académica.

La composición deberá respetar siempre la propiedad de cada servicio.

---

# Operaciones fuera del alcance de BFF-01

Aunque son necesarias posteriormente para PR-001, las siguientes operaciones no se implementan en esta tarea:

* Clientes funcionales.
* Endpoints funcionales.
* Persistencia propia relacionada con dominio académico.
* Implementación de reglas de negocio.
* Implementación de asignación docente.
* Implementación de cálculo de horas.
* Implementación de validación de disponibilidad.
* Implementación de creación de grupos.
* Implementación de creación de horarios.
* Implementación de envío de propuestas.
* Implementación del proceso de revisión.

Estas operaciones se implementarán en tareas posteriores.

---

# Flujo general

La interacción esperada entre frontend, BFF y servicios puede representarse de la siguiente manera:

```text id="generalflow"
┌──────────────┐
│   Angular    │
└──────┬───────┘
       │
       │ Request
       │ Authentication
       │ TenantId
       ▼
┌──────────────────────┐
│      AdminBff        │
│                      │
│ Endpoint             │
│ Contract             │
│ Client               │
│ Composition          │
│ Error handling       │
└──────┬───────────────┘
       │
       ├──────────────► AcademicService
       │
       ├──────────────► AcademicStaffService
       │
       ├──────────────► SchedulingService
       │
       └──────────────► WorkflowService
                         │
                         ▼
                    Service Response
                         │
                         ▼
                    AdminBff Response
                         │
                         ▼
                       Angular
```

---

# Dependencias con PLAN-01

Esta tarea depende de `PLAN-01`, ya que la definición de los endpoints, clientes y contratos del BFF debe respetar el ownership establecido para PR-001.

```text id="dependency"
PLAN-01
Definición de ownership
        │
        ▼
BFF-01
Estándar de AdminBff
        │
        ▼
BFF-02
Implementación de operaciones
```

`BFF-01` no debe redefinir las responsabilidades establecidas en `PLAN-01`.

---

# Reglas arquitectónicas

- `AdminBff` es una capa de adaptación y composición.
- `AdminBff` no es propietario de información académica.
- Cada servicio mantiene la propiedad de su dominio y base de datos.
- No se permite acceso directo a bases de datos de otros servicios.
- Los clientes internos deben estar encapsulados dentro de `AdminBff`.
- Los contratos del BFF deben estar orientados al frontend.
- El BFF puede componer información de varios servicios.
- La composición no debe contener reglas de negocio.
- `TenantId` debe propagarse y respetarse en todas las operaciones.
- La autenticación debe propagarse de acuerdo con el mecanismo de seguridad de SIA.
- Los errores internos deben transformarse a un contrato consistente para el frontend.
- Las URLs de servicios internos deben configurarse externamente.
- Los clientes internos deben utilizar `HttpClientFactory`.
- Las reglas académicas pertenecen a los servicios propietarios.
- Las reglas de Scheduling pertenecen a `SchedulingService`.
- `AdminBff` no debe duplicar modelos de dominio.
- `AdminBff` no debe almacenar información de dominio únicamente para evitar consultar a su servicio propietario.

---

# Resultado esperado

Al finalizar `BFF-01`, `SIA.AdminBff` contará con un estándar técnico definido y aprobado que establecerá:

* Estructura interna del proyecto.
* Organización de endpoints.
* Organización de clientes.
* Organización de contratos.
* Configuración de servicios internos.
* Propagación de autenticación.
* Propagación de `TenantId`.
* Manejo estandarizado de errores.
* Reglas de composición.
* Límites de responsabilidad del BFF.
* Operaciones requeridas para PR-001.
* Dependencias con `AcademicService`, `AcademicStaffService`, `SchedulingService` y `WorkflowService`.

El estándar definido en esta tarea deberá utilizarse como referencia para la implementación posterior de `AdminBff` y para futuros BFF de SIA.

No se implementan todavía clientes ni endpoints funcionales.
