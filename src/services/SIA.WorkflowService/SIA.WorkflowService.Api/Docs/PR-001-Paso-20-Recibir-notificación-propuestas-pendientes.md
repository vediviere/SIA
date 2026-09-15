# PR-001 — Paso 20. Recibir notificación de propuestas pendientes

## Descripción

Una vez que una propuesta de carga académica ha sido enviada por una Jefatura de División para su revisión, el Coordinador Académico deberá recibir una notificación indicando que existen propuestas pendientes de revisar.

La notificación no representa la revisión de la propuesta ni modifica por sí misma su contenido académico.

Su objetivo es informar al Coordinador Académico que existe una o varias propuestas en estado pendiente de revisión y permitirle ingresar al contexto correspondiente dentro de SIA.

Este paso se relaciona con la recepción inicial del proceso implementada conceptualmente en `WorkflowService` y con la generación de notificaciones hacia el usuario responsable de realizar la revisión.

---

## Objetivo

Permitir que el Coordinador Académico conozca que existen propuestas de carga académica pendientes de revisión y pueda acceder al listado o contexto desde el cual realizará el Paso 21 — Revisar propuesta de carga académica.

---

## Responsable funcional

La acción de revisión corresponde al perfil de negocio:

`Coordinator`

administrado por `AcademicStaffService`.

La cuenta, autenticación, roles y permisos de acceso corresponden a `IdentityService`.

La interfaz consumida por el Coordinador Académico será expuesta mediante:

`SIA.AdminBff`

El proceso de revisión pendiente será administrado por:

`WorkflowService`

La notificación será responsabilidad de:

`NotificationsService`

La propuesta de carga académica continuará siendo propiedad de:

`SchedulingService`

---

## Perfiles y permisos

Solamente un usuario autenticado y autorizado para actuar dentro del contexto institucional correspondiente como Coordinador Académico podrá acceder a las propuestas pendientes de revisión.

Conceptualmente:

```text
Usuario autenticado
        │
        ▼
IdentityService
        │
        │ autorización
        ▼
AdminBff
        │
        │ contexto institucional
        ▼
Verificar acceso como Coordinator
        │
        ▼
Consultar propuestas pendientes
```

`AdminBff` no determina por sí mismo si una persona es Coordinador.

El BFF deberá utilizar el contexto de autenticación y los contratos autorizados para verificar que el usuario puede acceder a las operaciones correspondientes.

La existencia de una entidad `Coordinator` en `AcademicStaffService` representa el perfil de negocio.

La autorización de acceso no deberá inferirse únicamente a partir de un identificador recibido desde el frontend.

---

## Información mostrada al Coordinador

La pantalla o pool de procesos podrá mostrar, como mínimo, la información necesaria para identificar una propuesta pendiente.

Por ejemplo:

* Identificador de la propuesta.
* Jefatura de División que la envió.
* Programa educativo correspondiente.
* Periodo académico.
* Fecha de envío.
* Estado actual de revisión.
* Indicación de que existen observaciones, cuando corresponda.
* Número o versión de revisión, cuando exista una nueva versión derivada de un retrabajo.

La información mostrada puede ser compuesta por `AdminBff`, pero cada servicio mantiene la propiedad de sus datos.

---

## Propuestas provenientes de una o varias Jefaturas

El Coordinador Académico puede recibir propuestas provenientes de diferentes Jefaturas de División.

Cada propuesta deberá conservar su propia identidad y proceso de revisión.

Conceptualmente:

```text
Coordinador Académico
        │
        ├── Propuesta A
        │      Jefatura A
        │
        ├── Propuesta B
        │      Jefatura B
        │
        └── Propuesta C
               Jefatura C
```

La revisión de una propuesta no deberá modificar automáticamente el estado de las demás.

Cada propuesta tendrá su propio ciclo de revisión.

---

## Estado pendiente de revisión

Una propuesta enviada correctamente desde `SchedulingService` deberá encontrarse disponible para revisión mediante el proceso correspondiente en `WorkflowService`.

El estado funcional inicial del proceso será:

`En revisión`

Mientras una propuesta se encuentre en revisión:

* Puede ser consultada por el Coordinador autorizado.
* No debe considerarse aprobada.
* No debe estar disponible para una nueva modificación ordinaria por la Jefatura.
* Debe conservar la referencia a la propuesta original.
* Debe conservar `TenantId`.
* Debe conservar `CorrelationId`.

---

## Notificación

Cuando exista una propuesta pendiente de revisión, `NotificationsService` podrá generar una notificación dirigida al Coordinador correspondiente.

La notificación deberá identificar suficientemente el contexto para permitir que el usuario conozca:

* Que existe una propuesta pendiente.
* De qué Jefatura proviene.
* A qué programa educativo corresponde.
* A qué periodo corresponde.

La notificación no deberá contener una copia completa de la carga académica.

La información detallada deberá obtenerse mediante los contratos correspondientes cuando el Coordinador ingrese al proceso de revisión.

---

## Reingreso después de retrabajo

Si una propuesta fue regresada a la Jefatura para realizar correcciones y posteriormente es enviada nuevamente, deberá volver a estar disponible para revisión.

El sistema deberá conservar la relación entre:

```text
Propuesta
    │
    ├── Revisión inicial
    │
    ├── Observaciones
    │
    └── Nueva versión enviada
```

El Coordinador deberá poder identificar que la propuesta fue enviada nuevamente después de un retrabajo.

No es necesario mostrar obligatoriamente un contador visual de modificaciones, pero el proceso deberá conservar internamente la trazabilidad suficiente para distinguir:

* Primera revisión.
* Propuesta regresada.
* Propuesta corregida y reenviada.
* Revisión posterior.

---

## TenantId

Todas las propuestas y procesos mostrados al Coordinador deberán pertenecer al mismo `TenantId` del contexto institucional autorizado.

No deberá ser posible consultar mediante el pool de procesos:

```text
Tenant A
    │
    └── Propuesta perteneciente a Tenant B
```

`TenantId` deberá conservarse durante:

* La consulta de propuestas.
* La creación del proceso.
* La generación de notificaciones.
* La apertura de la propuesta.
* Las decisiones posteriores de aprobación o retrabajo.

---

## CorrelationId

El `CorrelationId` deberá utilizarse para mantener trazabilidad entre los componentes involucrados.

Conceptualmente:

```text
SchedulingService
        │
        │ CorrelationId
        ▼
WorkflowService
        │
        ├── proceso de revisión
        │
        ▼
NotificationsService
        │
        ▼
AdminBff
```

La generación de una notificación o consulta posterior no deberá modificar el `CorrelationId` correspondiente a la operación original cuando forme parte de la misma cadena de procesamiento.

---

## Flujo de datos

```text
SchedulingService
        │
        │ Propuesta enviada a revisión
        ▼
WorkflowService
        │
        │ Crea / mantiene proceso
        │ Estado: En revisión
        ▼
NotificationsService
        │
        │ Notificación
        ▼
Coordinador Académico
        │
        │ Accede a SIA
        ▼
AdminBff
        │
        │ Consulta procesos pendientes
        ▼
WorkflowService
```

Cuando sea necesario obtener información detallada de la propuesta:

```text
AdminBff
    │
    ├── WorkflowService
    │       Estado del proceso
    │
    ├── SchedulingService
    │       Propuesta y carga académica
    │
    ├── AcademicService
    │       Contexto académico
    │
    └── AcademicStaffService
            Información institucional
```

---

## Límites de responsabilidad

### NotificationsService

Es responsable de comunicar que existe una acción o propuesta pendiente.

No decide:

* Aprobar.
* Rechazar.
* Modificar.
* Validar reglas académicas.

---

### WorkflowService

Es responsable de administrar el proceso y su estado de revisión.

No es propietario del contenido completo de la carga académica.

Debe conservar referencias y contexto necesarios para controlar el proceso.

---

### SchedulingService

Es propietario de la propuesta de carga académica y de los cambios de estado que correspondan a su ciclo de vida.

No debe delegar a `WorkflowService` la propiedad del contenido académico.

---

### AdminBff

Puede:

* Consultar procesos pendientes.
* Componer información para el frontend.
* Exponer contratos orientados a la interfaz.
* Propagar autenticación, `TenantId` y `CorrelationId`.

No puede:

* Aprobar directamente sin utilizar el flujo correspondiente.
* Decidir reglas académicas.
* Modificar la carga académica por cuenta propia.

---

## Resultado del paso

Al finalizar este paso, el Coordinador Académico conoce que existen propuestas pendientes y puede seleccionar una propuesta para iniciar su revisión.

La propuesta continúa en estado:

`En revisión`

El siguiente paso corresponde a:

`Paso 21 — Revisar propuesta de carga académica`.
