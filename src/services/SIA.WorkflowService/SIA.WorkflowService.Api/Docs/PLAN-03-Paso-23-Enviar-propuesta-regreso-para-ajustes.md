# PR-001 — Paso 23. Enviar propuesta de regreso para ajustes

## Objetivo

Formalizar la devolución de la propuesta de carga académica a la Jefatura de División cuando el Coordinador Académico determina que requiere ajustes.

La acción confirma que la propuesta no puede continuar con el flujo de aprobación y debe regresar a la Jefatura para atender las observaciones registradas.

## Condición de entrada

Este paso se ejecuta cuando existen observaciones que requieren corrección por parte de la Jefatura de División.

Las observaciones ya fueron registradas previamente y no se modifican ni se vuelven a registrar en este paso.

## Acción

El Coordinador Académico selecciona la acción para enviar la propuesta de regreso para ajustes.

Al confirmar la acción:

* La propuesta queda marcada como **Rechazada** para efectos del flujo de revisión.
* La propuesta queda pendiente de atención por parte de la Jefatura de División.
* Las observaciones previamente registradas quedan disponibles para su consulta.
* La propuesta no continúa hacia aprobación.

El estado `Rechazada` indica que la propuesta requiere correcciones; no implica eliminarla.

## Notificación

Al confirmarse la devolución, la Jefatura de División deberá recibir una notificación indicando que su propuesta fue devuelta para ajustes.

La notificación deberá permitirle identificar que:

* La propuesta requiere correcciones.
* Existen observaciones pendientes de atención.
* Debe ingresar al SIA para consultar las observaciones y realizar los ajustes correspondientes.

El contenido detallado de las observaciones no deberá duplicarse innecesariamente dentro de la notificación.

## Estado del proceso

La transición funcional del paso es:

```text
Propuesta en revisión
        │
        ▼
Rechazada para ajustes
        │
        ▼
Pendiente de atención por Jefatura
```

La propuesta permanece dentro del mismo proceso de revisión.

No se crea un nuevo proceso de revisión únicamente por devolver la propuesta.

## Responsabilidad técnica

`WorkflowService` registra y formaliza la devolución de la propuesta dentro del proceso de revisión.

`NotificationsService` es responsable de entregar la notificación correspondiente a la Jefatura.

La información académica de la propuesta continúa siendo propiedad de `SchedulingService`.

La comunicación entre servicios deberá realizarse mediante los contratos de integración definidos para el flujo.

## Datos de trazabilidad

La operación conserva:

* `TenantId`
* `CorrelationId`
* Identificador del proceso de revisión.
* Identificador de la propuesta.
* Usuario que ejecutó la devolución.
* Fecha y hora de la devolución.

## Reglas funcionales

1. El Paso 23 solo se ejecuta cuando la propuesta requiere ajustes.
2. La devolución cambia la situación de la propuesta a **Rechazada** dentro del flujo de revisión.
3. `Rechazada` significa que requiere correcciones y no implica eliminación.
4. Las observaciones deben existir antes de ejecutar esta acción.
5. Las observaciones no se crean nuevamente en este paso.
6. La propuesta queda pendiente de atención por parte de la Jefatura.
7. La Jefatura debe ser notificada de la devolución.
8. La propuesta no puede continuar hacia aprobación mientras requiera ajustes.
9. La devolución conserva la trazabilidad del proceso mediante `TenantId` y `CorrelationId`.
10. La devolución no modifica directamente el contenido académico de la propuesta.
11. La devolución no representa el reenvío de una propuesta corregida.

## Fuera de alcance

Este paso no contempla:

* Registrar observaciones.
* Modificar la propuesta.
* Atender las observaciones.
* Aprobar la propuesta.
* Reenviar la propuesta corregida.
* Realizar una nueva revisión.
