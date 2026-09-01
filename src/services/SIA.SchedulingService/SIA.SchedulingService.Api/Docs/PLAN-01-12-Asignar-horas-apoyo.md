# PLAN-01 — Paso 12. Asignar horas de apoyo

Una vez seleccionada una actividad de apoyo a la docencia para un docente, `SchedulingService` deberá permitir asignar la cantidad de horas que corresponderán a dicha actividad dentro del periodo académico activo.

Las horas de apoyo representan horas de la jornada laboral del docente que no corresponden a horas frente a grupo.

La cantidad de horas será determinada por el `DivisionHead` de acuerdo con las necesidades de la carrera, la institución y las características de la actividad asignada.

Las horas asignadas deberán sumarse a la carga académica del docente y deberán considerarse para determinar sus horas disponibles restantes.

## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` será responsable de:

- Registrar las horas de apoyo asignadas.
- Asociar las horas con la actividad de apoyo correspondiente.
- Asociar las horas con el docente.
- Asociar la asignación con el periodo académico activo.
- Acumular las horas de apoyo del docente.
- Considerar las horas frente a grupo previamente asignadas.
- Validar que la carga total no exceda las horas disponibles del docente.

`AcademicStaffService` únicamente proporciona la información contractual necesaria para conocer las horas totales disponibles del docente.

## Relación con el paso 11

El paso 11 determina: ¿Qué actividad se asignará?

El paso 12 determina: ¿Cuántas horas tendrá esa actividad?

Por ejemplo:

```
Paso 11

Docente: José
Actividad: Tutoría
Grupo: 1A

Posteriormente:

Paso 12

Docente: José
Actividad: Tutoría
Grupo: 1A
Horas: 2
```

Por lo tanto:

```
Seleccionar actividad
        ↓
Asignar horas

```

## Responsabilidad del DivisionHead

El `DivisionHead` será responsable de determinar la cantidad de horas que se asignará a cada actividad de apoyo.

Esta cantidad no deberá ser inferida automáticamente por `AdminBff`.

Por ejemplo:

```
Residencia profesional → 6 horas
Tutoría                 → 2 horas
Asesoría académica      → 4 horas
Otra actividad          → 3 horas

```

El valor dependerá de las necesidades de la planeación académica.


## Residencia profesional

En Residencia Profesional, las horas de apoyo no deberán calcularse convirtiendo los créditos de la actividad en horas.

La cantidad de horas será determinada por el `DivisionHead`.

Por ejemplo:

```
Docente José
    │
    └── Residencia Profesional
            ├── Residente 1
            ├── Residente 2
            └── Residente 3

Horas de apoyo asignadas: 6

```

La cantidad de residentes podrá ser uno de los elementos que el `DivisionHead` considere para determinar las horas, pero la regla de asignación de horas pertenece a la planeación académica.


## Tutorías

Para una tutoría, el `DivisionHead` deberá determinar las horas correspondientes.

Aunque regularmente se consideran dos horas: Tutoría → 2 horas

el modelo no deberá asumir que siempre serán exactamente dos horas.

Por lo tanto, deberá ser posible registrar: Tutoría → 2 horas

o, cuando la planeación lo determine: Tutoría → X horas


## Acumulación de horas de apoyo

Las horas asignadas a diferentes actividades deberán acumularse.

Ejemplo:

```
Docente José

Residencia profesional      6 horas
Tutoría                     2 horas
Asesoría académica          4 horas
Otra actividad              3 horas
────────────────────────────────────
Total apoyo                15 horas

```
Estas 15 horas deberán sumarse a las horas frente a grupo.


## Cálculo de carga utilizada

La carga utilizada del docente deberá considerar ambos tipos de horas:

```
Carga utilizada
=
Horas frente a grupo
+
Horas de apoyo

```
Por ejemplo:

```
Docente José

Horas totales:              40

Frente a grupo:             18
Horas de apoyo:             15
────────────────────────────────
Carga utilizada:            33

Horas disponibles:           7

```

## Validación de disponibilidad

Antes de registrar las horas de apoyo, `SchedulingService` deberá verificar que la nueva asignación no provoque que el docente exceda sus horas disponibles.

Conceptualmente:

```
Horas frente a grupo
+
Horas de apoyo existentes
+
Nuevas horas de apoyo
≤
Horas totales disponibles

```
Ejemplo válido:

```
Horas totales:              40
Frente a grupo:             20
Apoyo existente:            15
Nueva actividad:             5
────────────────────────────────
Total:                      40

```
Ejemplo no válido:

```
Horas totales:              40
Frente a grupo:             20
Apoyo existente:            15
Nueva actividad:             6
────────────────────────────────
Total:                      41

```

La segunda asignación deberá rechazarse.


## Asignaciones provenientes de otras carreras

Las horas de apoyo también deberán considerarse dentro de la carga global del docente.

Por ejemplo:

```
Docente José

Carrera A
    Materia → 10 horas

Carrera B
    Materia → 5 horas

Carrera B
    Tutoría → 2 horas

Carrera A
    Asesoría → 3 horas

Carga utilizada:

10 + 5 + 2 + 3 = 20 horas

```
Las horas deberán acumularse independientemente del programa educativo al que corresponda cada asignación, siempre que pertenezcan al mismo contexto institucional y periodo académico.


## Periodo académico activo

Las horas de apoyo solamente deberán afectar la disponibilidad del docente cuando pertenezcan al periodo académico activo.

Por ejemplo:
```
Periodo 2025-2
Tutoría → 4 horas

Periodo 2026-1
Tutoría → 2 horas

```
Para calcular la disponibilidad durante 2026-1: solo se consideran las 2 horas de 2026-1.

Las horas de periodos anteriores permanecen como histórico.


## Modificación de horas

Si el `DivisionHead` modifica las horas asignadas a una actividad, `SchedulingService` deberá actualizar el total de horas de apoyo del docente.

Ejemplo:

Antes:
```
Tutoría → 2 horas
```
Se modifica a:
```
Tutoría → 4 horas

```
El sistema deberá actualizar:
```
Horas de apoyo anteriores
        ↓
Reemplazar 2 por 4
        ↓
Recalcular carga utilizada
        ↓
Recalcular horas disponibles

```
La modificación deberá volver a validar el límite de horas del docente.

## Eliminación de horas

Cuando una actividad de apoyo deje de formar parte de la planeación, las horas correspondientes deberán dejar de contabilizarse en la carga activa del docente.

Ejemplo:

Antes:
```
Frente a grupo → 20
Tutoría         → 2
Asesoría        → 4

Total           → 26

```
Después de eliminar la tutoría:

```
Frente a grupo → 20
Asesoría       → 4

Total           → 24

```
Las horas disponibles deberán incrementarse consecuentemente.


## AcademicStaffService

`AcademicStaffService` es propietario de la información contractual del docente.

Por ejemplo:

```
Teacher
    │
    └── Horas contractuales: 40
```

`SchedulingService` utilizará esta información para validar la capacidad del docente.

No deberá registrar las horas de apoyo dentro de `AcademicStaffService`.

La separación es:

```

AcademicStaffService
        ↓
Capacidad contractual

SchedulingService
        ↓
Carga académica asignada
        ├── Frente a grupo
        └── Apoyo

```

## AcademicService

`AcademicService` podrá proporcionar información académica necesaria para contextualizar la actividad o el periodo.

`SchedulingService` no deberá modificar directamente las entidades de `AcademicService`.

La comunicación deberá realizarse mediante contratos.


## AdminBff

`AdminBff` podrá presentar al `DivisionHead` la información necesaria para asignar las horas.

Por ejemplo:

```
Docente: José

Horas totales:        40
Frente a grupo:       20
Horas de apoyo:        15
Disponibles:            5

Actividad:
Tutoría

Horas a asignar: [ 2 ]

Disponibles después: 3

```

`AdminBff` no deberá determinar por sí mismo si la asignación es válida.

La validación deberá realizarse en `SchedulingService`.


## Consulta necesaria

Antes de asignar nuevas horas de apoyo, `SchedulingService` deberá disponer de la información necesaria para conocer:

- Horas contractuales del docente.
- Horas frente a grupo asignadas en el periodo activo.
- Horas de apoyo ya asignadas en el periodo activo.
- Nueva cantidad de horas que se pretende asignar.

La información que pertenezca a otro servicio deberá obtenerse mediante contratos autorizados.


## Comando

Conceptualmente, la operación podrá representarse como: `AssignTeachingSupportHours`

El comando deberá identificar como mínimo:

`TeacherId`
`AcademicPeriodId`
`TeachingSupportActivityId`
`AssignedHours`
`TenantId`
`CorrelationId`

Los nombres definitivos podrán ajustarse al modelo físico y de contratos de `SchedulingService`.

## Eventos de integración

Cuando la asignación o modificación de horas de apoyo represente un cambio relevante para otros servicios, `SchedulingService` podrá publicar un evento de integración.

Conceptualmente:

`TeachingSupportHoursAssignedIntegrationEvent.v1`

El evento deberá contener únicamente la información necesaria para que otros servicios reaccionen al cambio.

No deberá exponer entidades internas de SchedulingService.


## TenantId

Las horas de apoyo deberán respetar el `TenantId`.

El cálculo de disponibilidad deberá realizarse únicamente considerando asignaciones pertenecientes al mismo `tenant`.

No deberán mezclarse las horas de docentes entre diferentes instituciones.


## Dependencias técnicas

El paso 12 depende de:

- Paso 4: periodo académico activo.
- Paso 6: grupos y oferta por grupos cuando corresponda.
- Paso 7: docente seleccionado.
- Paso 9: materia asignada al docente, cuando corresponda.
- Paso 10: horas frente a grupo previamente asignadas.
- Paso 11: actividad de apoyo seleccionada.
- Información contractual del docente.
- Asignaciones existentes del docente dentro del periodo activo.

Ninguna dependencia deberá requerir acceso directo a la base de datos de otro servicio.

## Relación con el paso 13

El paso 12 no registra todavía la propuesta completa de carga académica.

Aquí se determina:

```
Actividad de apoyo
        +
Cantidad de horas

```
El resultado alimentará posteriormente la propuesta:

```
Paso 10
Horas frente a grupo
        │
        ├─────────────┐
                      ▼
Paso 12          Propuesta de carga
Horas de apoyo ────────┘
                      │
                      ▼
                 Paso 13
```
Por lo tanto, el paso 13 será el encargado de consolidar la información que se ha construido durante los pasos anteriores.

## Reglas funcionales

- Las horas de apoyo pertenecen a la carga académica administrada por `SchedulingService`.
- Las horas de apoyo no representan horas frente a grupo.
- Las horas de apoyo son complementarias para justificar la jornada laboral del docente.
- La cantidad de horas de cada actividad deberá registrarse explícitamente.
- El `DivisionHead` determina la cantidad de horas de apoyo.
- Las horas de Residencia Profesional no deberán calcularse convirtiendo sus créditos en horas frente a grupo.
- Las horas de Residencia Profesional serán determinadas por el `DivisionHead`.
- Las tutorías podrán tener una cantidad de horas determinada por el `DivisionHead`, aunque regularmente sean dos horas.
- Las asesorías académicas deberán contar con una cantidad de horas asignada.
- Las actividades adicionales deberán contar con las horas determinadas por el `DivisionHead`.
- Las horas de apoyo deberán acumularse por docente y periodo académico.
- Las horas de apoyo deberán sumarse a las horas frente a grupo para determinar la carga utilizada.
- Las asignaciones de otras carreras deberán considerarse para determinar la disponibilidad global del docente.
- Solamente las asignaciones del periodo académico activo afectan la disponibilidad actual.
- Las horas de periodos anteriores se conservan como histórico.
- Una modificación de horas deberá recalcular la carga utilizada y la disponibilidad.
- La eliminación de una actividad deberá liberar las horas correspondientes.
- La carga total del docente no deberá superar sus horas disponibles.
- `SchedulingService` es propietario de las horas de apoyo.
- `AcademicStaffService` es propietario de la información contractual del docente.
- `AdminBff` puede presentar información, pero no ejecutar las reglas de validación de disponibilidad.
- Todas las operaciones deberán respetar `TenantId`.
- Ningún servicio deberá consultar directamente la base de datos de otro servicio.

## Fuera de alcance

Este paso no contempla:

- Registro de la propuesta completa de carga académica.
- Aprobación de la carga académica.
- Publicación de la carga académica.
- Horarios.
- Aulas.
- Inscripción de estudiantes.
- Operación de Residencia Profesional.
- Operación de Tutorías.
- Operación de Asesorías Académicas.
- Gestión del historial académico del estudiante.

Estas actividades se consideran únicamente desde el punto de vista de asignación de horas de apoyo al docente.

## Resultado esperado

Al finalizar el paso 12, cada actividad de apoyo seleccionada para un docente deberá contar con la cantidad de horas asignadas y dichas horas deberán reflejarse en la carga académica activa del docente.

Ejemplo:

```
Periodo académico activo
        │
        └── Docente José
              │
              ├── Cálculo Integral - 1A
              │       └── 4 horas frente a grupo
              │
              ├── Cálculo Integral - 1B
              │       └── 4 horas frente a grupo
              │
              ├── Residencia Profesional
              │       └── 6 horas de apoyo
              │
              └── Tutoría - 1A
                      └── 2 horas de apoyo

Total frente a grupo:   8 horas
Total de apoyo:         8 horas
Total utilizado:       16 horas

```
Estas horas quedarán disponibles para ser consolidadas posteriormente en el paso 13 — Registrar propuesta de carga académica en SIA.