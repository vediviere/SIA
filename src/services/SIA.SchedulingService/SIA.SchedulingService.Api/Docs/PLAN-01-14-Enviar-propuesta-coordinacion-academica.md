# PLAN-01 — Paso 14. Enviar propuesta a coordinación académica

Una vez que el `DivisionHead` ha registrado la propuesta completa de carga académica correspondiente a su programa educativo y periodo académico, deberá enviarla a Coordinación Académica para que sea revisada y validada.

Este paso representa el cambio de la propuesta desde un estado de preparación o registro hacia un estado en el que queda formalmente disponible para revisión por parte de coordinación.

Conceptualmente:

```
DivisionHead
     │
     │ Enviar propuesta
     ▼
SchedulingService
     │
     ▼
Propuesta enviada
     │
     ▼
Coordinación Académica
     │
     ▼
Validación

```
El envío no representa todavía la aprobación de la propuesta.

## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` será responsable de:

- Validar que exista una propuesta registrada.
- Validar que la propuesta pueda ser enviada.
- Cambiar el estado de la propuesta.
- Registrar que la propuesta fue enviada a coordinación académica.
- Identificar el DivisionHead responsable del envío.
- Mantener el contexto de `TenantId`.
- Publicar el evento correspondiente para notificar que la propuesta está disponible para validación.

## Propuesta que puede enviarse

La propuesta deberá corresponder al conjunto completo de cargas académicas registrado durante el paso 13.

Por ejemplo:

```
Propuesta 2026-1
Ingeniería en Sistemas Computacionales
DivisionHead: Juan Pérez

    ├── Docente José
    │     ├── Materias
    │     └── Horas de apoyo
    │
    ├── Docente María
    │     ├── Materias
    │     └── Horas de apoyo
    │
    └── Docente Carlos
          ├── Materias
          └── Horas de apoyo

```

El `DivisionHead` envía este conjunto como una propuesta integral.

No deberá enviarse únicamente la carga individual de un docente.

Condición previa

Para poder ejecutar el envío deberá existir una propuesta previamente registrada.

## Conceptualmente:

```
Propuesta en preparación
        │
        │ Paso 13
        ▼
Propuesta registrada
        │
        │ Paso 14
        ▼
Propuesta enviada

```
No deberá ser posible enviar una propuesta inexistente.

Tampoco deberá enviarse una propuesta que no se encuentre en un estado que permita su envío.

## Cambio de estado

El envío deberá provocar un cambio de estado de la propuesta.

Conceptualmente:

```
Draft
  │
  │ Enviar
  ▼
Submitted
```
Donde:

- `Draft` representa una propuesta que todavía se encuentra en preparación o registrada por el `DivisionHead`.
- `Submitted` representa una propuesta enviada formalmente a coordinación académica para su revisión.

Los nombres definitivos de los estados podrán ajustarse durante la implementación.

## Responsabilidad del DivisionHead

El `DivisionHead` es quien inicia la operación de envío.

Antes de enviarla, deberá poder revisar que la propuesta corresponda a:

- El programa educativo seleccionado.
- El periodo académico activo.
- Los docentes asignados.
- Las materias asignadas.
- Las horas frente a grupo.
- Las actividades de apoyo.
- Las horas de apoyo.

Una vez que la propuesta sea enviada, la revisión deberá pasar al ámbito de coordinación académica.

## Responsabilidad de Coordinación Académica

La coordinación académica será la responsable de revisar la propuesta recibida.

Este paso únicamente comunica y entrega la propuesta para validación.

La decisión de:

- Aprobar.
- Rechazar.
- Solicitar modificaciones.

corresponde a pasos posteriores del flujo.

Por lo tanto:

Enviar ≠ Aprobar

El envío únicamente significa: La propuesta está disponible para revisión.

## SchedulingService

`SchedulingService` mantiene la propiedad de la propuesta y de su estado.

Al recibir la solicitud de envío deberá:

- Identificar la propuesta.
- Verificar que pertenezca al TenantId correspondiente.
- Verificar que el usuario tenga el contexto necesario para realizar el envío.
- Verificar que la propuesta se encuentre registrada.
- Verificar que su estado permita el envío.
- Cambiar el estado a enviado.
- Registrar la operación.
- Publicar el evento correspondiente.

## AdminBff

`AdminBff` podrá proporcionar la interfaz mediante la cual el DivisionHead ejecuta la acción: Enviar propuesta a coordinación

`AdminBff` podrá presentar información como:

```
Programa:
Ingeniería en Sistemas Computacionales

Periodo:
2026-1

Estado:
Lista para enviar

Docentes:
15

Horas frente a grupo:
180

Horas de apoyo:
65

[Enviar propuesta]

```
Sin embargo, `AdminBff` no será propietario del estado de la propuesta.

La decisión de permitir o rechazar el envío deberá pertenecer a `SchedulingService`.


## Comando

Conceptualmente, la operación podrá representarse como: `SubmitAcademicLoadProposal`

El comando deberá identificar la propuesta y el contexto de la operación.

Como mínimo deberá contemplarse información equivalente a:

- `AcademicLoadProposalId`
- `TenantId`
- `DivisionHeadId`
- `CorrelationId`

Los nombres definitivos podrán ajustarse durante el diseño de contratos.

## Consulta necesaria

Para ejecutar el envío, `SchedulingService` deberá consultar su propia información para determinar:

- Que la propuesta existe.
- A qué periodo pertenece.
- A qué programa educativo pertenece.
- Quién es el responsable de división.
- Cuál es su estado actual.
- Si puede ser enviada.

Si se requiere información perteneciente a otro dominio, deberá solicitarse mediante el contrato correspondiente.

No deberá acceder directamente a las bases de datos de:

- `AcademicService`.
- `AcademicStaffService`.
- `Otros servicios`.
- `Evento de integración`

Una vez que la propuesta haya sido enviada correctamente, `SchedulingService` deberá publicar un evento de integración para comunicar el cambio de estado.

Conceptualmente: `AcademicLoadProposalSubmittedIntegrationEvent.v1`

Este evento podrá ser consumido por los componentes o servicios que necesiten reaccionar ante el envío de una propuesta.

La información del evento deberá mantenerse limitada al contrato de integración.

No deberán exponerse las entidades internas de `SchedulingService`.

## Comunicación con WorkflowService

El flujo de aprobación y validación puede requerir la participación de: `WorkflowService`

La responsabilidad de `WorkflowService` será administrar el flujo de trabajo correspondiente cuando así se haya definido arquitectónicamente.

Conceptualmente:

```

SchedulingService
        │
        │ ProposalSubmitted
        ▼
WorkflowService
        │
        ▼
Flujo de validación
        │
        ▼
Coordinación Académica

```
`WorkflowService` no deberá convertirse en propietario de la propuesta de carga académica.

El propietario de la información de la propuesta continúa siendo `SchedulingService`.

## AcademicService

`AcademicService` no será responsable de recibir ni administrar la propuesta.

Su responsabilidad continúa limitada a la estructura académica:

- `AcademicPeriod`.
- `EducationalProgram`.
- `StudyPlan`.
- `StudyPlanSubject`.
- `Subject`.

`SchedulingService` utilizará identificadores y contratos de AcademicService para mantener el contexto académico de la propuesta.

No deberá existir acceso directo a `SIA_AcademicDb`.

## AcademicStaffService

`AcademicStaffService` continuará siendo propietario de la información del personal académico.

No será responsable de recibir ni aprobar la propuesta.

La propuesta solamente deberá conservar las referencias necesarias hacia los docentes.

No deberá copiarse la entidad Teacher dentro de `SchedulingService`.

## TenantId

El envío deberá conservar el `TenantId` de la propuesta.

Conceptualmente:

```
DivisionHead
      │
      │ TenantId
      ▼
AdminBff
      │
      │ TenantId
      ▼
SchedulingService
      │
      │ TenantId
      ▼
Propuesta

```
El `TenantId` deberá mantenerse durante la operación y en los eventos de integración relacionados.

No deberá ser posible enviar una propuesta perteneciente a un tenant diferente al contexto autorizado del usuario.


## Integridad de la propuesta

El envío no deberá modificar las asignaciones que forman parte de la propuesta.

Por ejemplo: Antes de enviar:

```
Docente José
    ├── Materia A → 4 horas
    ├── Materia B → 5 horas
    └── Tutoría   → 2 horas

```
Después del envío: Propuesta enviada:

```
Docente José
    ├── Materia A → 4 horas
    ├── Materia B → 5 horas
    └── Tutoría   → 2 horas
```
El cambio principal corresponde al estado de la propuesta.

## Modificación posterior al envío

Una propuesta enviada a coordinación no deberá considerarse una propuesta todavía editable de forma ordinaria.

Si coordinación solicita modificaciones, el flujo deberá utilizar posteriormente el mecanismo definido para regresar la propuesta a un estado que permita su corrección.

Ese comportamiento específico corresponde al flujo de validación posterior y no deberá resolverse dentro de este paso.

## Dependencias técnicas

El paso 14 depende directamente de:

- Paso 4 — Consultar periodo escolar.
- Paso 5 — Cargar plan de estudios.
- Paso 6 — Definir grupos.
- Paso 7 — Seleccionar docente.
- Paso 8 — Verificar que el docente cumpla con los requisitos.
- Paso 9 — Asignar materia al docente.
- Paso 10 — Asignar horas frente a grupo.
- Paso 11 — Seleccionar actividades de apoyo.
- Paso 12 — Asignar horas de apoyo.
- Paso 13 — Registrar propuesta de carga académica.

El paso 13 constituye la dependencia inmediata:

```
Paso 13
Registrar propuesta
        │
        ▼
Propuesta registrada
        │
        ▼
Paso 14
Enviar propuesta

```

## Dependencias entre servicios

La interacción conceptual será:

```
AdminBff
    │
    │ SubmitAcademicLoadProposal
    ▼
SchedulingService
    │
    ├── Actualiza estado
    │
    └── Publica evento
              │
              ▼
       WorkflowService
              │
              ▼
     Validación de propuesta
```

`AcademicService` y `AcademicStaffService` no necesitan recibir directamente la propuesta para que pueda realizarse el envío.

Sus datos únicamente deberán utilizarse mediante los contratos correspondientes cuando sean necesarios.

## Reglas funcionales

- El envío de la propuesta pertenece a `SchedulingService`.
- Solamente podrá enviarse una propuesta previamente registrada.
- La propuesta deberá pertenecer al programa educativo correspondiente al `DivisionHead`.
- La propuesta deberá pertenecer al periodo académico correspondiente.
- La propuesta deberá conservar su `TenantId`.
- El `DivisionHead` será quien inicie el envío.
- La propuesta representa el conjunto de cargas académicas de su ámbito de planeación.
- El envío no representa la aprobación de la propuesta.
- El envío deberá cambiar el estado de la propuesta.
- Una propuesta enviada deberá quedar disponible para la etapa de validación correspondiente.
- `AdminBff` podrá iniciar la operación y presentar el resultado, pero no será propietario de la propuesta.
- `SchedulingService` continuará siendo propietario de la propuesta.
- `WorkflowService` podrá administrar el flujo de validación, pero no será propietario de la información de la carga académica.
- `AcademicService` continuará siendo propietario de la estructura académica.
- `AcademicStaffService` continuará siendo propietario de la información del personal académico.
- Ningún servicio deberá consultar directamente la base de datos de otro servicio.
- Las operaciones deberán respetar `TenantId`.
- El envío deberá generar el evento de integración correspondiente.
- La propuesta enviada no deberá modificarse como efecto directo de la operación de envío.

## Fuera de alcance

Este paso no contempla:

- Validación de la propuesta por coordinación académica.
- Aprobación.
- Rechazo.
- Solicitud de modificaciones.
- Corrección de la propuesta.
- Construcción de horarios.
- Asignación de aulas.
- Inscripción de estudiantes.
- Validación de estudiantes.
- Modificación de las reglas de carga académica.

Estas operaciones corresponden a etapas posteriores del flujo PR-001.

## Resultado esperado

Al finalizar el paso 14, la propuesta completa de carga académica del `DivisionHead` deberá encontrarse formalmente enviada a coordinación académica.

Conceptualmente:

```
DivisionHead
      │
      ▼
Propuesta registrada
      │
      │ Enviar
      ▼
SchedulingService
      │
      ▼
Propuesta enviada
      │
      ▼
WorkflowService
      │
      ▼
Coordinación Académica
      │
      ▼
Validación

```
El resultado del paso será una propuesta identificable, asociada a:

- `TenantId`
- `AcademicPeriodId`
- `EducationalProgramId`
- `DivisionHeadId`

con un estado equivalente a: `Submitted`

y preparada para continuar con el proceso de validación por Coordinación Académica.