# PLAN-01 — Paso 10. Asignar horas frente a grupo

Una vez que un docente ha sido asociado a una materia y a un grupo dentro de la planeación académica, `SchedulingService` deberá determinar y registrar las horas frente a grupo correspondientes a dicha asignación.

Las horas frente a grupo representan las horas que el docente tiene comprometidas por impartir una materia dentro de un grupo durante un periodo académico activo.

La información deberá utilizarse para determinar la carga académica acumulada del docente y sus horas disponibles restantes.

Esta operación pertenece a `SchedulingService`, debido a que las horas frente a grupo forman parte de la carga y planeación académica.


## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` es responsable de:

- Registrar las horas frente a grupo.
- Acumular las horas asignadas al docente.
- Considerar las asignaciones realizadas en diferentes grupos y programas educativos.
- Determinar las horas disponibles restantes del docente dentro del periodo académico activo.
- Impedir que la carga académica exceda las horas disponibles del docente.

`AcademicStaffService` únicamente es propietario de la información contractual o laboral que establece las horas que tiene disponibles el docente.

Origen de las horas frente a grupo

Las horas frente a grupo se determinan a partir de la asignación de una materia a un docente dentro de un grupo.

Por ejemplo:

```
Materia: Cálculo Integral
Grupo: 1A
Créditos: 4
Docente: José

La asignación genera:

José
   │
   └── Cálculo Integral - 1A
          └── 4 horas frente a grupo
```
Las horas frente a grupo deberán corresponder a las horas que la materia representa dentro de la planeación académica.

La regla exacta de conversión entre los datos académicos de la materia y las horas frente a grupo deberá mantenerse conforme al modelo académico aprobado.


## Acumulación de horas

Las horas frente a grupo deberán acumularse por docente dentro del periodo académico activo.

Por ejemplo:

```
Docente José

Cálculo Integral - 1A     4 horas
Cálculo Integral - 1B     4 horas
Física - 2A               5 horas
----------------------------------
Total                     13 horas

```
El resultado representa: Horas frente a grupo = 13

Estas horas deberán considerarse para determinar las horas disponibles restantes del docente.


## Asignaciones en diferentes grupos

Un mismo docente podrá impartir la misma materia en diferentes grupos.

Por ejemplo:

```
Cálculo Integral

Grupo 1A → José → 4 horas
Grupo 1B → José → 4 horas

```
En este caso: Horas frente a grupo de José = 8

Cada asignación deberá contabilizarse individualmente.

No deberá asumirse que impartir una misma materia en varios grupos constituye una sola asignación.

Asignaciones en diferentes programas educativos

Las horas frente a grupo deberán acumularse independientemente del programa educativo al que pertenezca la materia.

Por ejemplo:

```
Docente José
Horas disponibles: 40

Ingeniería en Sistemas Computacionales
    Materia A → 5 horas

Ingeniería en Gestión Empresarial
    Materia B → 4 horas

Ciencias Básicas
    Materia C → 5 horas

Total horas frente a grupo = 14

Por lo tanto:

40 horas totales
-14 horas asignadas
-------------------
26 horas disponibles

```
Una asignación realizada en otra carrera continúa consumiendo horas disponibles del mismo docente dentro del periodo académico.


## Periodo académico activo

Solamente las asignaciones correspondientes al periodo académico activo deberán utilizarse para determinar la disponibilidad actual del docente.

Por ejemplo:

```
Periodo 2026-1
José → 20 horas

Periodo 2025-2
José → 30 horas

```
Para la planeación de 2026-1: Horas asignadas = 20


Las 30 horas del periodo anterior forman parte del histórico y no deberán reducir la disponibilidad del periodo actual.


## Horas totales y horas disponibles

El cálculo deberá considerar las horas totales disponibles del docente y las horas frente a grupo ya asignadas durante el periodo activo.

Conceptualmente:

```
Horas disponibles
=
Horas totales del docente
-
Horas frente a grupo asignadas en el periodo activo

```
Por ejemplo:

```
Docente José

Horas totales:              40
Horas frente a grupo:       28
────────────────────────────────
Horas disponibles:          12

```
Si posteriormente se registra una asignación de 4 horas:

```
Horas totales:              40
Horas frente a grupo:       32
Horas disponibles:           8

```
No deberá permitirse una asignación que provoque que las horas frente a grupo superen las horas totales disponibles.


## Validación del límite de horas

Antes de registrar una nueva asignación de horas, `SchedulingService` deberá verificar que:

```
Horas frente a grupo actuales
+
Horas de la nueva asignación
≤
Horas totales disponibles

Ejemplo válido:

Horas totales:               40
Horas asignadas:             36
Nueva asignación:              4
────────────────────────────────
Resultado:                    40

```
Ejemplo no válido:

```

Horas totales:               40
Horas asignadas:             38
Nueva asignación:              4
────────────────────────────────
Resultado:                    42

```
La segunda operación deberá rechazarse porque excede la capacidad de horas del docente.


## Actualización de disponibilidad

Cada vez que una asignación sea registrada correctamente, las horas frente a grupo acumuladas deberán reflejarse en la disponibilidad del docente.

Conceptualmente:

```
Asignación
     ↓
Horas frente a grupo
     ↓
Acumulación de carga
     ↓
Horas disponibles restantes

```
La disponibilidad deberá recalcularse considerando todas las asignaciones vigentes del docente dentro del periodo activo.


## Modificación o eliminación de una asignación

Si posteriormente una asignación docente es modificada o eliminada, las **horas frente a grupo** correspondientes deberán dejar de contabilizarse o actualizarse conforme al nuevo valor.

Por ejemplo:

```
Antes:

José
├── Materia A → 4 horas
├── Materia B → 5 horas
└── Materia C → 4 horas

Total = 13 horas

```
Si se elimina la asignación de Materia B:

```
José
├── Materia A → 4 horas
└── Materia C → 4 horas

Total = 8 horas

```
Las horas disponibles deberán actualizarse consecuentemente.


## Comando

La operación deberá ser responsabilidad de `SchedulingService`.

Conceptualmente: `AssignTeachingHours`

Sin embargo, si las **horas frente a grupo** son una consecuencia directa de `AssignTeacherToSubject`, podrá determinarse durante la implementación que no sea necesario exponer un comando independiente y que las horas se registren como parte de la misma operación.

La decisión final deberá mantener una única fuente de verdad para la asignación y evitar inconsistencias entre:

Docente asignado y Horas frente a grupo


## Persistencia

Las horas frente a grupo deberán almacenarse dentro del modelo propietario de `SchedulingService`.

La información deberá quedar asociada al contexto de la asignación correspondiente.

Conceptualmente:

```
AcademicAssignment
    │
    ├── TeacherId
    ├── SubjectId
    ├── GroupId
    ├── AcademicPeriodId
    └── TeachingHours
```
Los nombres definitivos dependerán del modelo físico existente en `SchedulingService`.

No deberá crearse una copia del perfil del docente ni de la materia.


## AcademicStaffService

`AcademicStaffService` proporciona la información propia del docente necesaria para determinar su capacidad contractual.

Por ejemplo:

```
Teacher
   │
   └── Horas contractuales
```

`AcademicStaffService` no deberá almacenar las horas frente a grupo generadas por la planeación.

La distinción es:

```
AcademicStaffService
        ↓
¿Cuántas horas puede tener el docente?

SchedulingService
        ↓
¿Cuántas horas tiene asignadas actualmente?

```

## AcademicService

`AcademicService` proporciona la información académica correspondiente a la materia.

`SchedulingService` podrá utilizar dicha información para determinar las horas asociadas a la materia conforme a las reglas académicas establecidas.

No deberá modificar directamente:

- Subject.
- StudyPlan.
- StudyPlanSubject.
- EducationalProgram.
- AcademicPeriod.

La comunicación deberá realizarse mediante los contratos correspondientes.


## AdminBff

`AdminBff` podrá mostrar al usuario la información resultante de la asignación y disponibilidad.

Por ejemplo:

```
Docente: José

Horas totales:       40
Horas asignadas:     28
Horas disponibles:   12

```

`AdminBff` podrá componer información proveniente de los servicios correspondientes, pero no deberá calcular ni modificar directamente la carga académica.

La regla de negocio deberá permanecer en `SchedulingService`.


## Eventos de integración

Cuando el registro o modificación de horas frente a grupo represente un cambio relevante para otros servicios, `SchedulingService` podrá publicar un evento de integración.

Conceptualmente:

`TeachingHoursAssignedIntegrationEvent.v1`

El evento deberá contener únicamente la información necesaria para que otros servicios reaccionen al cambio.

No deberá exponer directamente las entidades internas de `SchedulingService`.

Si la asignación y las horas frente a grupo forman parte de una única operación transaccional, deberá evaluarse durante la implementación si resulta más adecuado publicar un único evento de asignación docente que incluya las horas correspondientes.


## TenantId

La asignación de horas deberá respetar el `TenantId` del contexto institucional.

Las asignaciones consideradas para calcular las horas frente a grupo deberán pertenecer al mismo tenant del docente y de la planeación correspondiente.

No deberán mezclarse cargas académicas pertenecientes a diferentes instituciones.

Conceptualmente:

```
Tenant A
    José → 20 horas

Tenant B
    José → 10 horas

```
La disponibilidad de José dentro de `Tenant` A no deberá calcularse utilizando las horas pertenecientes a `Tenant B`.


## Dependencias técnicas

El paso 10 depende de:

- La existencia de un periodo académico activo.
- La existencia del grupo.
- La existencia de la materia.
- La existencia del docente.
- La asignación realizada en el paso 9.
- La información de horas disponibles del docente.
- Las asignaciones existentes del docente dentro del periodo activo.
- La información académica necesaria para determinar las horas de la materia.

Las dependencias entre servicios deberán resolverse mediante contratos.

Ninguna operación deberá requerir acceso directo a la base de datos de otro servicio.

## Reglas funcionales

- Las horas frente a grupo pertenecen a la planeación académica administrada por `SchedulingService`.
- Las horas frente a grupo se generan a partir de las materias asignadas al docente dentro de grupos.
- Las horas de todas las asignaciones del docente deberán acumularse dentro del periodo académico activo.
- Las asignaciones de otras carreras también consumen horas disponibles del docente.
- Las asignaciones de periodos anteriores no afectan la disponibilidad del periodo actual.
- Un docente puede impartir la misma materia en diferentes grupos.
- Cada asignación en un grupo deberá contabilizarse individualmente.
- Las horas frente a grupo no deberán superar las horas totales disponibles del docente.
- La disponibilidad deberá actualizarse conforme se agreguen, modifiquen o eliminen asignaciones.
- `AcademicStaffService` es propietario de la información contractual del docente, no de sus horas frente a grupo.
- `SchedulingService` es propietario de la carga académica y las horas frente a grupo.
- El cálculo deberá respetar `TenantId`.
- Las comunicaciones entre servicios deberán conservar `Correlation`.
- Ningún servicio deberá consultar directamente la base de datos de otro servicio.


## Fuera de alcance

Este paso no contempla:

- Creación o modificación del perfil del docente.
- Modificación de horas contractuales.
- Creación de materias.
- Creación de grupos.
- Creación de horarios.
- Asignación de aulas.
- Inscripción de estudiantes.
- Validación de avance académico.
- Cálculo de créditos obtenidos por estudiantes.
- Historial académico.
- Resultado esperado

Al finalizar el paso 10, la asignación docente deberá contar con las horas frente a grupo correspondientes y estas deberán formar parte de la carga académica del docente dentro del periodo académico activo.

Por ejemplo:

```
Periodo académico activo
        │
        └── Docente José
              │
              ├── Materia A - Grupo 1A → 4 horas
              ├── Materia B - Grupo 2A → 5 horas
              └── Materia C - Grupo 1B → 4 horas
                                      ─────────
                                      13 horas

El sistema deberá poder determinar:

Horas totales:             40
Horas frente a grupo:      13
Horas disponibles:         27

```
La información quedará bajo propiedad de SchedulingService y será utilizada por los siguientes pasos del flujo PR-001 para continuar con la construcción de la carga académica y la planeación.