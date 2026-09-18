# PR-001 — Paso 26. Actualizar estatus de la propuesta aprobada

## Objetivo

Actualizar el estatus de la propuesta una vez que la carga académica aprobada ha sido generada y dejar informado a la Jefatura de División que puede iniciar el proceso de generación de horarios.

## Condición de entrada

Este paso se ejecuta después de que la carga académica aprobada ha sido generada y asentada en el SIA.

## Acción

Se actualiza el estatus de la propuesta para reflejar que su proceso de aprobación ha concluido y que la carga académica ya es oficial.

## Notificación

Una vez actualizado el estatus, se notifica a la Jefatura de División que:

* La carga académica fue aprobada.
* Las cargas de los profesores ya están disponibles.
* Puede iniciar el proceso de generación de horarios.

## Responsabilidad

`SchedulingService` actualiza el estatus relacionado con la propuesta y comunica que la carga académica ya se encuentra oficialmente disponible para continuar con el proceso.

`NotificationsService` es responsable de entregar la notificación a la Jefatura de División.

## Reglas

1. El estatus únicamente se actualiza después de generar la carga académica aprobada.
2. La propuesta queda identificada como aprobada/oficial para continuar con el proceso.
3. La Jefatura de División debe ser notificada.
4. La notificación debe indicar que puede iniciar la generación de horarios.
5. La actualización debe conservar `TenantId` y `CorrelationId`.
6. La notificación no modifica la carga académica.
