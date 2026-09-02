# WORK-02 — Implementar recepción inicial de propuesta para revisión

## Descripción

Recepción inicial de una propuesta de carga académica enviada por `SchedulingService`, con base en el flujo PR-001 de Planeación Académica.

Cuando `SchedulingService` envía una propuesta para revisión, `WorkflowService` debe consumir el evento de integración correspondiente y crear el proceso de revisión asociado a dicha propuesta.

Esta funcionalidad representa el inicio formal del proceso de revisión por parte de Coordinación Académica. La propuesta permanece bajo la responsabilidad de `SchedulingService`; `WorkflowService` únicamente registra la referencia necesaria para administrar el proceso y su estado.

El proceso debe permitir identificar de qué Jefe de División proviene la solicitud, a qué propuesta corresponde, a qué institución pertenece y si se trata del primer envío o de una modificación y reenvío de una propuesta existente.

---

## Objetivo

Establecer el mecanismo mediante el cual `WorkflowService`:

* Recibe el evento generado por `SchedulingService` cuando una propuesta de carga académica es enviada a revisión.
* Crea el proceso de revisión correspondiente.
* Registra la referencia de la propuesta de carga académica.
* Establece el estado inicial del proceso como `En revisión`.
* Conserva `TenantId` para garantizar el aislamiento institucional.
* Registra la fecha de creación del proceso.
* Identifica la versión de la propuesta recibida.
* Permite identificar al Jefe de División que originó la solicitud.
* Evita la creación de procesos duplicados cuando el mismo evento sea recibido más de una vez.

---

## Alcance

### Incluye

* Consumir el evento de propuesta de carga académica enviada a revisión.
* Crear la entidad mínima necesaria para representar el proceso de revisión.
* Asociar el proceso con `AcademicLoadProposalId`.
* Registrar `TenantId`.
* Registrar la fecha de creación.
* Establecer el estado inicial `En revisión`.
* Registrar la versión de la propuesta.
* Registrar la referencia del Jefe de División que originó el envío.
* Implementar idempotencia mediante `Inbox` o mecanismo equivalente.
* Implementar pruebas del consumidor y de la creación del proceso.

### No incluye

* Modificar la propuesta de carga académica.
* Copiar la información completa de `AcademicLoad`.
* Administrar docentes, materias, grupos u horarios.
* Validar académicamente la propuesta.
* Aprobar o rechazar la propuesta.
* Modificar el estado de la propuesta dentro de `SchedulingService`.
* Consultar directamente la base de datos de `SchedulingService`.
* Implementar la lógica completa de notificaciones por correo, push u otros canales, salvo que exista un componente de notificaciones definido para ello.

---

## Flujo técnico

```text
┌─────────────────────────────┐
│     SchedulingService       │
│                             │
│ AcademicLoadProposal        │
│         │                   │
│         │ Submit            │
│         ▼                   │
│ AcademicLoadProposal        │
│ SubmittedIntegrationEvent   │
└──────────────┬──────────────┘
               │
               │ Event
               │ TenantId
               │ ProposalId
               │ Version
               │ DivisionHeadId
               ▼
┌─────────────────────────────┐
│      WorkflowService        │
│                             │
│      Event Consumer         │
│             │               │
│             ▼               │
│      Inbox / Idempotency    │
│             │               │
│             ▼               │
│   ReviewProcess             │
│   ├── AcademicLoadProposalId│
│   ├── TenantId              │
│   ├── DivisionHeadId        │
│   ├── Version               │
│   ├── Status                │
│   └── CreatedAt             │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Coordinación Académica      │
│                             │
│ Propuesta pendiente         │
│ de revisión                 │
└─────────────────────────────┘
```

---

## Evento de integración

El evento es generado por `SchedulingService` cuando el Jefe de División envía la propuesta a revisión.

Nombre conceptual: `AcademicLoadProposalSubmittedIntegrationEvent.v1`

El evento debe contener únicamente la información necesaria para que `WorkflowService` pueda iniciar el proceso de revisión.

### Datos mínimos

| Campo                    | Descripción                                                  |
| ------------------------ | ------------------------------------------------------------ |
| `EventId`                | Identificador único del evento para garantizar idempotencia. |
| `TenantId`               | Institución a la que pertenece la propuesta.                 |
| `AcademicLoadProposalId` | Identificador de la propuesta de carga académica.            |
| `DivisionHeadId`         | Identificador del Jefe de División que envía la propuesta.   |
| `Version`                | Número de versión o envío de la propuesta.                   |
| `SubmittedAt`            | Fecha y hora en que la propuesta fue enviada a revisión.     |

`WorkflowService` no debe recibir ni almacenar la carga académica completa como parte del evento.

---

## Comando interno

La recepción del evento puede desencadenar internamente un comando conceptual:

`CreateAcademicLoadReviewProcess`

Este comando representa la creación del proceso de revisión dentro de `WorkflowService`.

### Información mínima

* `AcademicLoadProposalId`
* `DivisionHeadId`
* `TenantId`
* `Version`
* `SubmittedAt`

---

## Proceso de revisión

`WorkflowService` debe crear una representación mínima del proceso asociado a la propuesta.

### Datos mínimos

```text
ReviewProcess
│
├── Id
├── TenantId
├── AcademicLoadProposalId
├── DivisionHeadId
├── Version
├── Status
├── CreatedAt
└── CorrelationId
```

### Estado inicial

El proceso debe crearse con: `En revisión`

Este estado representa que la propuesta fue recibida por el flujo de revisión y está pendiente de la intervención de Coordinación Académica.

No representa una aprobación.

---

## Identificación del origen

El proceso debe permitir identificar quién originó la solicitud de revisión.

Para ello se conserva la referencia: `DivisionHeadId`

La información descriptiva del Jefe de División, como nombre o datos personales, no debe duplicarse en `WorkflowService` si pertenece a `AcademicStaffService`.

Cuando sea necesario mostrar esta información, podrá obtenerse mediante un contrato/API apropiado o mediante composición realizada por el `AdminBff`, respetando los límites entre servicios.

---

## Control de versiones

Cada envío de la propuesta debe contar con una versión que permita distinguir entre:

* Primera propuesta enviada a revisión.
* Propuesta modificada y reenviada.
* Segunda o posteriores versiones de la misma propuesta.

Ejemplo:

```text
AcademicLoadProposal
        │
        ├── Version 1 → Enviada a revisión
        │
        ├── Version 2 → Modificada y reenviada
        │
        └── Version 3 → Modificada y reenviada
```

La versión permite a Coordinación Académica identificar qué envío está revisando y evita depender únicamente de la fecha de modificación.

`WorkflowService` no debe determinar por sí mismo el contenido de una modificación ni reconstruir las diferencias entre versiones.

---

## Idempotencia

La recepción de eventos debe ser idempotente.

Si `WorkflowService` recibe nuevamente el mismo evento debido a un reintento de mensajería, no debe crear un segundo proceso de revisión para la misma operación.

Se debe utilizar el `EventId` mediante un mecanismo `Inbox` o equivalente.

```text
Evento recibido
      │
      ▼
¿EventId procesado?
   │          │
  Sí          No
   │           │
   ▼           ▼
Ignorar     Crear proceso
            Registrar EventId
```

La restricción debe garantizar que un mismo evento no genere dos procesos.

---

## TenantId

`TenantId` debe viajar desde `SchedulingService` hasta `WorkflowService` y conservarse durante todo el procesamiento.

`WorkflowService` debe utilizar `TenantId` como parte del contexto institucional del proceso de revisión.

No se debe permitir que una propuesta perteneciente a un `TenantId` sea asociada a un proceso de otro `TenantId`.

```text
SchedulingService
      │
      │ TenantId = T1
      ▼
WorkflowService
      │
      │ TenantId = T1
      ▼
ReviewProcess
```

---

## Responsabilidades por servicio

| Servicio               | Responsabilidad                                                                           |
| ---------------------- | ----------------------------------------------------------------------------------------- |
| `SchedulingService`    | Mantener la propuesta de carga académica y generar el evento al enviarla a revisión.      |
| `WorkflowService`      | Recibir el evento y crear el proceso de revisión.                                         |
| `AcademicStaffService` | Mantener la información del Jefe de División.                                             |
| `AcademicService`      | Mantener la información académica que corresponda a su dominio.                           |
| `AdminBff`             | Componer la información necesaria para presentar la propuesta y su proceso a la interfaz. |

Ningún servicio debe consultar directamente la base de datos de otro servicio.

---

## Dependencias

```text
SCHED-11
   │
   │ AcademicLoadProposalSubmittedIntegrationEvent.v1
   ▼
WORK-02
   │
   ├── WorkflowService
   │
   └── Inbox / Idempotency
```

### Dependencias funcionales

* La propuesta de carga académica debe existir en `SchedulingService`.
* La propuesta debe haber sido enviada a revisión.
* `SchedulingService` debe publicar el evento de integración correspondiente.
* El contrato del evento debe estar definido antes de implementar el consumidor.

### Dependencias técnicas

* `WORK-01` — Base/infraestructura inicial de `WorkflowService`.
* `SCHED-11` — Envío de propuesta de carga académica y publicación del evento de integración.

---

## Consideraciones sobre la notificación

El paso 19 del BPMN se representa como:

**Notificar a Coordinación Académica.**

Desde el punto de vista técnico, `WORK-02` debe entenderse principalmente como el inicio del proceso de revisión al recibir la propuesta.

La creación del proceso en `WorkflowService` deja registrada la existencia de una propuesta pendiente de revisión y permite que posteriormente la interfaz o un mecanismo de notificaciones informe a Coordinación Académica.

La implementación específica del canal de notificación —por ejemplo, notificación dentro del SIA, correo electrónico o notificación push— queda fuera de esta tarea mientras no exista un servicio o contrato de notificaciones definido.

---

## Reglas de negocio aplicables

- Una propuesta enviada a revisión debe generar un proceso de revisión.
- El proceso inicia en estado `En revisión`.
- El proceso debe identificar la propuesta de carga académica a la que corresponde.
- El proceso debe identificar al Jefe de División que originó la solicitud.
- Debe registrarse la versión de la propuesta.
- `TenantId` debe conservarse y respetarse durante todo el procesamiento.
- Un mismo evento no debe generar procesos duplicados.
- `WorkflowService` no debe almacenar una copia completa de la carga académica.
- `WorkflowService` no debe acceder directamente a la base de datos de `SchedulingService`.
- La creación del proceso de revisión no implica aprobación de la propuesta.

---

## Pruebas requeridas

### Consumer

Verificar que:

* El consumidor recibe correctamente `AcademicLoadProposalSubmittedIntegrationEvent.v1`.
* Se obtiene correctamente `TenantId`.
* Se obtiene `AcademicLoadProposalId`.
* Se obtiene `DivisionHeadId`.
* Se obtiene `Version`.
* Se procesa correctamente `EventId`.

### Creación del proceso

Verificar que:

* Se crea un único proceso de revisión.
* El estado inicial es `En revisión`.
* Se registra `AcademicLoadProposalId`.
* Se registra `DivisionHeadId`.
* Se registra `TenantId`.
* Se registra `Version`.
* Se registra la fecha de creación.

### Idempotencia

Verificar que:

* El mismo evento procesado dos veces no genera dos procesos.
* El `EventId` queda registrado mediante `Inbox` o mecanismo equivalente.
* Un reintento del mensaje no modifica ni duplica el proceso existente.

---

## Resultado esperado

Al finalizar esta tarea, `WorkflowService` será capaz de recibir una propuesta de carga académica enviada por `SchedulingService` y crear el proceso inicial de revisión asociado.

El resultado será conceptualmente:

```text
Propuesta enviada
       │
       ▼
Evento publicado
       │
       ▼
WorkflowService recibe evento
       │
       ▼
Validación de idempotencia
       │
       ▼
Creación de ReviewProcess
       │
       ├── Propuesta
       ├── Jefe de División
       ├── Tenant
       ├── Versión
       └── En revisión
       │
       ▼
Propuesta disponible para revisión
por Coordinación Académica
```

La propuesta original continúa siendo propiedad de `SchedulingService`; `WorkflowService` únicamente administra el proceso de revisión y conserva las referencias necesarias para coordinar el flujo.
