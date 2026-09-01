# PLAN-01 — Paso 11. Seleccionar actividades de apoyo a la docencia

Una vez que se han realizado las asignaciones de materias y se han determinado las horas frente a grupo, `SchedulingService` deberá permitir seleccionar las **actividades de apoyo a la docencia** que serán asignadas al docente dentro del periodo académico activo.

Las actividades de apoyo a la docencia representan actividades que no corresponden a horas frente a grupo y que forman parte de la jornada laboral del docente.

Estas actividades son obligatorias dentro de la planeación de la jornada docente y permiten complementar las horas correspondientes a su jornada laboral.

Entre las principales actividades se encuentran:

- Residencia profesional.
- Tutorías.
- Asesorías académicas.
- Otras actividades de apoyo a la docencia.

La selección, asignación y determinación de horas de estas actividades deberá formar parte de la planeación académica administrada por `SchedulingService`.


## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` será responsable de:

- Registrar las actividades de apoyo a la docencia asignadas al docente.
- Registrar las horas destinadas a cada actividad.
- Asociar las actividades con el docente correspondiente.
- Considerar únicamente las actividades pertenecientes al periodo académico activo para determinar la carga actual.
- Controlar que las horas asignadas no superen las horas disponibles del docente.
- Integrar posteriormente estas horas con las horas frente a grupo para conformar la carga académica.

`AcademicStaffService` continúa siendo propietario de la información contractual del docente y de las horas correspondientes a su jornada laboral.

Actividades de apoyo a la docencia

Las actividades de apoyo a la docencia son aquellas **que no representan** impartición directa de una materia frente a un grupo.

Conceptualmente:

```
Carga académica del docente
        │
        ├── Horas frente a grupo
        │
        └── Horas de apoyo a la docencia
                │
                ├── Residencia profesional
                ├── Tutorías
                ├── Asesorías académicas
                └── Otras
```
Ambos tipos de horas forman parte de la carga académica del docente.

Sin embargo, las horas de apoyo no necesariamente tienen que existir en todas las cargas.

Un docente podrá tener:
``
Solo horas frente a grupo
``
o:
``
Solo horas de apoyo
``
o:
``
Horas frente a grupo
+
Horas de apoyo
``
Esto permite representar situaciones como docentes pertenecientes a otras áreas o carreras que únicamente apoyan a determinado programa educativo mediante una materia o mediante una actividad específica.


## Residencia profesional

**Residencia profesional** es una actividad de apoyo a la docencia que puede ser asignada a un docente.

A diferencia de una materia ordinaria, los créditos asociados a Residencia Profesional **no deberán convertirse automáticamente en horas frente a grupo**.

La cantidad de horas que se asignará al docente deberá ser determinada por el `DivisionHead`.

La asignación podrá depender, entre otros factores, del número de residentes que sean asesorados por el docente.

Conceptualmente:

```
Residencia Profesional
        │
        ├── Docente José
        │       └── X residentes
        │
        └── Docente María
                └── Y residentes
```
El `DivisionHead` determinará las horas que correspondan a cada docente considerando las necesidades de la planeación y la cantidad de residentes asignados.

Por lo tanto:

```
Créditos de Residencia Profesional
                ≠
Horas frente a grupo
```
La cantidad de horas **deberá registrarse explícitamente como horas de apoyo a la docencia.**


## Tutorías

Las tutorías son actividades de apoyo **asociadas a los grupos aperturados** dentro de la planeación académica.

Cada grupo podrá tener un docente designado como `tutor`.

El `tutor` podrá pertenecer preferentemente al área académica correspondiente, aunque esta condición no deberá considerarse una restricción absoluta.

Regularmente las tutorías contemplan dos horas, pero la cantidad de horas deberá poder ser determinada por el `DivisionHead` de acuerdo con las necesidades de la planeación.

Conceptualmente:

```
Grupo 1A
    │
    └── Tutor → Docente José
                  └── 2 horas
```
La cantidad de horas deberá registrarse como horas de apoyo a la docencia.


## Asesorías académicas

Las asesorías académicas representan actividades mediante las cuales un docente proporciona orientación académica a estudiantes.

La asignación podrá realizarse preferentemente a docentes del área correspondiente o de Ciencias Básicas, aunque esta preferencia no constituye una regla obligatoria.

El `DivisionHead` determinará el docente y las horas que correspondan a esta actividad de acuerdo con las necesidades de la carrera.

Conceptualmente:

```
Asesoría académica
        │
        └── Docente asignado
                └── Horas determinadas por DivisionHead
```
Estas horas forman parte de las horas de apoyo a la docencia.


## Otras actividades

El `DivisionHead` podrá determinar otras actividades de apoyo a la docencia de acuerdo con las necesidades de la carrera y de la institución.

Estas actividades podrán utilizarse para complementar la jornada laboral del docente cuando sea necesario.

El `DivisionHead` deberá determinar:

- La actividad.
- El docente responsable.
- Las horas asignadas.

La cantidad de horas deberá respetar la disponibilidad del docente dentro del periodo académico activo.


## Determinación de horas

Las horas de apoyo a la docencia serán determinadas por el `DivisionHead`.

No todas las actividades tendrán necesariamente una cantidad fija de horas.

Por ejemplo:

```
Residencia profesional
→ Horas determinadas según la asignación de residentes.

Tutoría
→ Regularmente 2 horas, pero puede ser determinado otro valor.

Asesoría académica
→ Horas determinadas según las necesidades de la carrera.

Otra actividad
→ Horas determinadas por el DivisionHead.

```
Por lo tanto, el modelo deberá permitir almacenar explícitamente las horas asignadas a cada actividad.


## Disponibilidad del docente

Las horas de apoyo a la docencia deberán consumir las horas disponibles del docente de la misma manera que las horas frente a grupo.

Por ejemplo:

```
Docente José

Horas totales:                  40

Horas frente a grupo:           25
Horas de apoyo:                   5
──────────────────────────────────
Horas utilizadas:                30

Horas disponibles:               10

```
Si se intenta asignar una actividad de apoyo de 12 horas: 30 + 12 = 42

La operación deberá rechazarse porque excede las 40 horas disponibles.

## Disponibilidad global del docente

Para determinar las horas disponibles del docente deberán considerarse todas las asignaciones pertenecientes al periodo académico activo.

Esto incluye:

```
Horas frente a grupo
+
Horas de apoyo a la docencia

```
Por ejemplo:

```
Docente José

Carrera A
    Materia → 10 horas

Carrera B
    Materia → 5 horas

Carrera A
    Tutoría → 2 horas

Carrera B
    Asesoría académica → 3 horas

Total utilizado = 20 horas

```
Si el docente cuenta con 40 horas: 40 - 20 = 20 horas disponibles

Las asignaciones realizadas en otras carreras también deberán considerarse.


## Periodo académico activo

Solamente las actividades de apoyo pertenecientes al periodo académico activo deberán utilizarse para determinar la disponibilidad actual del docente.

Las actividades correspondientes a periodos anteriores forman parte del histórico y no deberán reducir la disponibilidad del periodo actual.

Por ejemplo:

```
Periodo 2025-2
Tutoría → 4 horas

Periodo 2026-1
Tutoría → 2 horas

```
Para la planeación de 2026-1 solamente deberán considerarse: 2 horas

## Relación con las horas frente a grupo

Las horas frente a grupo y las horas de apoyo son conceptos diferentes, pero ambas forman parte de la carga académica del docente.

Conceptualmente:

```
Carga académica
        │
        ├── Horas frente a grupo
        │
        └── Horas de apoyo
```

La disponibilidad total deberá considerar ambas.

Por ejemplo:
```
Horas totales:              40

Frente a grupo:             24
Apoyo a la docencia:         8
──────────────────────────────
Total utilizado:            32

Disponibles:                 8

```

## Carga académica parcial

La carga académica de un docente no deberá asumir que siempre contiene horas frente a grupo y horas de apoyo simultáneamente.

Son válidas las siguientes situaciones:

```
Solo horas frente a grupo
Docente
    │
    └── Horas frente a grupo: 20
Solo horas de apoyo
Docente
    │
    └── Horas de apoyo: 10
Ambos tipos de horas
Docente
    │
    ├── Frente a grupo: 20
    └── Apoyo:           10
```
Esto resulta particularmente importante para docentes que pertenecen a otra carrera y únicamente participan en el programa mediante una materia o mediante una actividad de apoyo.


## `DivisionHead`

El `DivisionHead` será quien determine las actividades de apoyo que serán asignadas dentro de la planeación de su programa educativo.

Podrá determinar:

- Qué actividad se requiere.
- Qué docente será responsable.
- Cuántas horas serán asignadas.
- La distribución de las actividades conforme a las necesidades de la carrera.

Sin embargo, la regla de disponibilidad deberá ser aplicada por `       `.

El `DivisionHead` decide la asignación dentro de las reglas permitidas; `SchedulingService` garantiza que la operación no genere una carga superior a las horas disponibles.


## Persistencia

Las actividades de apoyo deberán formar parte del modelo propietario de `SchedulingService`.

Conceptualmente:

```
TeachingSupportActivity
        │
        ├── `TeacherId`
        ├── `AcademicPeriodId`
        ├── `ActivityType`
        ├── `GroupId` (cuando corresponda)
        └── `AssignedHours`
```
Los nombres definitivos dependerán del modelo físico que se determine para `SchedulingService`.

No deberán almacenarse estas horas dentro de `AcademicStaffService`.

`AcademicStaffService`

`AcademicStaffService` proporciona la información contractual necesaria para conocer las horas que puede tener asignadas un docente.

Conceptualmente:

```
AcademicStaffService
        │
        └── Horas contractuales
```
Mientras que:

```
SchedulingService
        │
        ├── Horas frente a grupo
        └── Horas de apoyo
```
Esta separación permite distinguir entre: Capacidad contractual y Carga académica asignada.

## AcademicService

`AcademicService` podrá proporcionar información relacionada con la estructura académica cuando sea necesaria para contextualizar la actividad.

Por ejemplo, podrá utilizarse información relacionada con:

- `AcademicPeriod`.
- `EducationalProgram`.
- `StudyPlan`.
- `Subject`.

`SchedulingService` no deberá modificar directamente entidades propiedad de `AcademicService`.

La comunicación deberá realizarse mediante contratos definidos entre los servicios.

## `AdminBff`

`AdminBff` podrá componer y presentar la información necesaria para que el `DivisionHead` pueda seleccionar y asignar actividades.

Por ejemplo:

```
Docente: José

Horas totales:        40
Frente a grupo:       24
Horas de apoyo:        8
Disponibles:            8

```
Actividades:

```
☐ Residencia profesional
☐ Tutoría
☐ Asesoría académica
☐ Otra

```
`AdminBff` no deberá determinar por sí mismo si una asignación excede las horas disponibles.

La validación deberá permanecer en `SchedulingService`.


## `TenantId`

Las actividades de apoyo deberán estar asociadas al `TenantId` correspondiente.

La disponibilidad del docente deberá calcularse exclusivamente con información perteneciente al mismo contexto institucional.

No deberán mezclarse actividades o cargas académicas de diferentes tenants.


## Dependencias técnicas

El paso 11 depende principalmente de:

- Periodo académico activo.
- Docente disponible.
- Contexto del programa educativo seleccionado.
- Información de horas contractuales del docente.
- Horas frente a grupo previamente asignadas.
- Grupos existentes cuando la actividad esté asociada a un grupo.
- Reglas académicas necesarias para contextualizar la actividad.

Las dependencias entre servicios deberán resolverse mediante contratos.

No deberá requerirse acceso directo a las bases de datos de otros servicios.


## Reglas funcionales

- Las actividades de apoyo a la docencia forman parte de la carga académica del docente.
- Las actividades de apoyo no representan horas frente a grupo.
- Las actividades de apoyo son complementarias para justificar la jornada laboral del docente.
- Las horas de apoyo deberán registrarse explícitamente.
- El `DivisionHead` determina las actividades y las horas correspondientes.
- Residencia profesional no deberá convertir automáticamente sus créditos en horas frente a grupo.
- Las horas de Residencia Profesional serán determinadas por el `DivisionHead`.
- Las tutorías estarán **asociadas a los grupos correspondientes**.
- Las tutorías podrán tener una duración determinada por el `DivisionHead`; regularmente se consideran **dos horas**.
- Las asesorías académicas podrán asignarse a docentes del área o de Ciencias Básicas, sin que esto constituya una restricción obligatoria.
- El `DivisionHead` podrá definir otras actividades de apoyo según las necesidades de la carrera y de la institución.
- Las horas de apoyo deberán consumir las horas disponibles del docente.
- Las horas frente a grupo y las horas de apoyo deberán considerarse conjuntamente para determinar la carga utilizada.
- Solamente las asignaciones del periodo académico activo afectan la disponibilidad actual del docente.
- Las asignaciones de periodos anteriores se conservan como histórico y no afectan la disponibilidad actual.
- Las asignaciones realizadas en otras carreras deberán considerarse para determinar la disponibilidad global del docente.
- Un docente puede tener únicamente horas frente a grupo.
- Un docente puede tener únicamente horas de apoyo.
- Un docente puede tener horas frente a grupo y horas de apoyo.
- Las horas asignadas no deberán superar las horas disponibles del docente.
- `SchedulingService` es propietario de las horas de apoyo y de la carga académica.
- `AcademicStaffService` es propietario de la información contractual del docente.
- `AdminBff` puede presentar y componer información, pero no debe ejecutar reglas de negocio de disponibilidad.
- Todas las operaciones deberán respetar `TenantId`.
- Ningún servicio deberá acceder directamente a la base de datos de otro servicio.


## Fuera de alcance

Este paso no contempla:

- Registro de la propuesta completa de carga académica.
- Aprobación de la carga académica.
- Publicación de la carga académica.
- Horarios.
- Aulas.
- Inscripción de estudiantes.
- Validación de estudiantes residentes.
- Gestión operativa de Residencia Profesional.
- Gestión operativa de Tutorías.
- Gestión operativa de Asesorías Académicas.
- Historial académico del estudiante.

Estas actividades se consideran aquí únicamente como actividades de apoyo asignables al docente dentro de la planeación académica.

## Resultado esperado

Al finalizar el paso 11, `SchedulingService` deberá contar con las actividades de apoyo a la docencia seleccionadas para cada docente y con las horas asignadas a cada una.

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
              ├── Tutoría - 1A
              │       └── 2 horas de apoyo
              │
              └── Asesoría académica
                      └── 4 horas de apoyo

Total frente a grupo:  8 horas
Total de apoyo:        6 horas
Total utilizado:      14 horas

```
Estas horas deberán quedar disponibles para que el paso 13 — Registrar propuesta de carga académica en SIA pueda consolidar la carga completa del docente.