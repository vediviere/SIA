# PR-001 — Paso 24. Aprobar carga académica

## Objetivo

Autorizar definitivamente la propuesta de carga académica después de que la Jefatura de División haya atendido las observaciones y realizado las modificaciones requeridas.

## Condición de entrada

Este paso únicamente puede ejecutarse cuando la propuesta ha pasado por el proceso de revisión y, en caso de haber requerido ajustes, la Jefatura de División ha realizado las modificaciones correspondientes.

La propuesta puede haber pasado por uno o múltiples ciclos de corrección y revisión.

## Acción

El Coordinador Académico revisa el resultado final y autoriza la carga académica.

Al autorizar:

* La propuesta queda aprobada.
* La carga académica deja de estar sujeta a modificaciones ordinarias.
* Lo establecido en la propuesta queda formalmente autorizado.
* La propuesta puede continuar al proceso de generación de la carga académica oficial.

## Responsabilidad

`WorkflowService` registra la decisión de aprobación del Coordinador Académico y deja constancia de que la propuesta fue autorizada.

La aprobación deberá conservar:

* Identificador de la propuesta.
* Usuario que autoriza.
* Fecha y hora de autorización.
* `TenantId`.
* `CorrelationId`.

La información académica de la carga continúa siendo propiedad de `SchedulingService`.

## Reglas

1. Solo el Coordinador Académico puede autorizar la carga.
2. La autorización únicamente procede después de atender las correcciones requeridas.
3. Puede existir uno o múltiples ciclos de corrección antes de la autorización.
4. Una carga autorizada no puede modificarse mediante el flujo ordinario de planeación.
5. La autorización debe quedar registrada para efectos de trazabilidad.
6. La propuesta aprobada continúa al Paso 25.
