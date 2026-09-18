# PR-001 — Paso 25. Generar carga académica

## Objetivo

Consolidar la propuesta autorizada como **carga académica aprobada** en el SIA.

## Condición de entrada

Este paso únicamente se ejecuta cuando la propuesta cuenta con autorización del Coordinador Académico.

## Acción

`SchedulingService` toma la propuesta autorizada y la establece como carga académica oficial.

Como resultado:

* La carga académica queda asentada en el SIA.
* Las cargas correspondientes a los profesores quedan listas.
* La información aprobada se convierte en la base para continuar con el proceso de generación de horarios.

## Responsabilidad

`SchedulingService` es responsable de generar y consolidar la carga académica aprobada.

La información generada deberá conservar la relación con la propuesta que le dio origen y mantener:

* `TenantId`.
* `CorrelationId`.
* Identificador de la propuesta.
* Periodo académico correspondiente.

## Reglas

1. Solo puede generarse una carga a partir de una propuesta autorizada.
2. La carga generada debe corresponder a lo autorizado.
3. La carga académica queda asentada como aprobada en el SIA.
4. Las cargas de los profesores quedan disponibles para el proceso de horarios.
5. No se deben modificar los elementos autorizados durante esta generación.
6. La generación debe conservar la trazabilidad con la propuesta aprobada.
