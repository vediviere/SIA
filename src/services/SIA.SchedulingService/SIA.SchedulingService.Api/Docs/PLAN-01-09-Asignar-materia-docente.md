# PLAN-01 — Paso 9. Asignar materia al docente

Una vez seleccionado el docente y verificado que cumple con las condiciones requeridas para participar en la planeación académica, `SchedulingService` deberá permitir asociar el docente con la materia y grupo correspondiente.

Esta operación representa la asignación académica de un docente a una materia ofertada dentro de un grupo y un periodo académico activo.

La asignación pertenece a `SchedulingService`, debido a que forma parte de la planeación académica.

`AcademicStaffService` continúa siendo propietario de la información del docente, pero no de la asignación académica.


## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` es responsable de registrar la relación entre:

```
Docente
   ↓
Materia
   ↓
Grupo
   ↓
Periodo académico

```

- La información del docente utilizada durante la operación pertenece a `AcademicStaffService`.
- La información de la materia y del plan de estudios pertenece a `AcademicService`.
- La información del grupo y de la asignación pertenece a `SchedulingService`.

## Contexto de la asignación

La asignación deberá realizarse dentro del contexto académico correspondiente.

Como mínimo deberá identificarse:

- `TenantId`.
- `AcademicPeriodId`.
- Programa educativo.
- Plan de estudios.
- Grupo.
- Materia.
- Docente (`TeacherId`).

Conceptualmente:

```
AcademicPeriod
      │
      └── EducationalProgram
              │
              └── StudyPlan
                    │
                    └── Subject
                          │
                          └── Group
                                │
                                └── Teacher
```

La relación representa que un docente impartirá una determinada materia en un grupo específico durante el periodo académico correspondiente.

## Relación docente–materia

La asignación no deberá modificar la entidad `Teacher` ni la entidad `Subject`.

La operación deberá registrar la participación del docente dentro de la planeación académica.

Conceptualmente:

```
Teacher
   │
   │ TeacherId
   ▼
AcademicAssignment
   │
   ├── AcademicOffering / Group
   └── Subject

```
La asignación deberá utilizar identificadores de las entidades propietarias de otros servicios, sin apropiarse de dichas entidades.

## Asignación dentro del grupo

La materia deberá asignarse al docente dentro de un grupo específico.

Por ejemplo:

```
Periodo activo
     │
     └── Ingeniería en Sistemas Computacionales
             │
             └── Grupo 1A
                    │
                    └── Cálculo Integral
                           │
                           └── Docente José
```

La asignación a **1A** no implica automáticamente que el mismo docente imparta la materia en otro grupo.

Por ejemplo:

```
Cálculo Integral - 1A → José
Cálculo Integral - 1B → María

```
son asignaciones diferentes.

Si posteriormente se asigna también 1B a José:

```
Cálculo Integral - 1A → José
Cálculo Integral - 1B → José

```
se deberán considerar ambas asignaciones para el cálculo de sus horas frente a grupo.

## Periodo académico

La asignación deberá pertenecer a un periodo académico.

Solamente las asignaciones correspondientes al periodo académico activo deberán afectar la planeación y disponibilidad actual del docente.

Las asignaciones pertenecientes a periodos anteriores forman parte del histórico y no deberán utilizarse para disminuir la disponibilidad del periodo actual.

Por ejemplo:

```
Periodo 2026-1
José → 20 horas

Periodo 2025-2
José → 30 horas

```
Para la planeación de 2026-1, las 30 horas de 2025-2 no deberán sumarse a las 20 horas actuales.


## Validaciones previas

Antes de registrar la asignación, `SchedulingService` deberá asegurarse de que se hayan cumplido las validaciones definidas en el paso 8.

Entre ellas:

- El docente existe y se encuentra disponible para participar en la planeación.
- El docente pertenece al mismo `TenantId`.
- El docente cumple con el perfil requerido para la materia.
- El docente cumple con las condiciones de adscripción o compatibilidad definidas.
- El docente cuenta con disponibilidad de horas.
- La asignación corresponde al periodo académico activo.
- La materia pertenece al contexto académico correspondiente.
- El grupo pertenece al contexto de planeación correspondiente.

El paso 9 no deberá omitir estas condiciones simplemente porque el docente haya sido seleccionado previamente en la interfaz.

Las validaciones deberán ejecutarse nuevamente en el momento de registrar la operación para evitar inconsistencias entre la selección y la asignación definitiva.

## Comando

La operación deberá exponerse como un comando de `SchedulingService`.

Conceptualmente: `AssignTeacherToSubject`

El comando deberá identificar la información necesaria para realizar la asignación, por ejemplo:

- `TenantId`
- `AcademicPeriodId`
- `GroupId`
- `SubjectId`
- `TeacherId`

Los nombres definitivos del contrato podrán determinarse durante el diseño técnico.

El comando deberá ser procesado por `SchedulingService`.

## Persistencia

La asignación deberá almacenarse utilizando el modelo propietario de `SchedulingService`.

No deberá crearse una copia de la información completa del docente ni de la materia.

Se deberán conservar únicamente las referencias necesarias, por ejemplo:

- `TeacherId`
- `SubjectId`
- `GroupId`
- `AcademicPeriodId`

Los identificadores pertenecientes a otros servicios se consideran referencias externas.

No deberán establecerse llaves foráneas hacia bases de datos pertenecientes a otros servicios.

## AcademicStaffService

`AcademicStaffService` proporciona la información correspondiente al docente.

Por ejemplo:

- `TeacherId`
- Perfil profesional
- Programa educativo / adscripción
- Estado
- Horas contractuales

`SchedulingService` podrá utilizar esta información para validar la asignación.

Sin embargo:

`#` no registra la asignación del docente a la materia.

La asignación pertenece exclusivamente a `SchedulingService`.

## AcademicService

`AcademicService` es propietario de la información académica utilizada para identificar la materia, plan de estudios y programa educativo.

`SchedulingService` utilizará los identificadores correspondientes.

No deberá modificar directamente:

- `Subject`.
- `StudyPlan`.
- `StudyPlanSubject`.
- `EducationalProgram`.
- `AcademicPeriod`.

La comunicación con `AcademicService` deberá realizarse mediante los contratos definidos entre servicios.

## SchedulingService

`SchedulingService` es propietario de:

- La oferta académica.
- Los grupos.
- La planeación académica.
- La asignación del docente.
- La carga académica.
- Las horas frente a grupo.

Por lo tanto, es el servicio responsable de registrar la asignación:

```
Teacher
     ↓
Subject + Group

```
dentro del periodo académico.

## AdminBff

`AdminBff` podrá enviar la solicitud de asignación desde la interfaz administrativa hacia `SchedulingService`.

Conceptualmente:

```
AdminBff
   │
   │ AssignTeacherToSubject
   ▼
SchedulingService
   │
   ├── valida contexto académico
   ├── valida docente
   └── registra asignación

```

`AdminBff` no deberá registrar directamente la asignación ni acceder a la base de datos de `SchedulingService`.

Su responsabilidad se limita a orquestar la comunicación y presentar el resultado al cliente.


## TenantId

La asignación deberá conservar y validar el `TenantId`.

El docente, grupo, materia y contexto de planeación deberán pertenecer al mismo `tenant`.

No deberá permitirse una asignación como:

```
Tenant A
   Docente José
        ↓
Tenant B
   Grupo 1A
   
```

El `TenantId` deberá formar parte del contexto de la operación y no deberá poder ser utilizado por el cliente para seleccionar arbitrariamente otra institución.

## Evento de integración

Una vez registrada correctamente la asignación, `SchedulingService` podrá publicar un evento de integración para comunicar el cambio a otros servicios que necesiten reaccionar ante una asignación docente.

Conceptualmente:

`TeacherAssignedToSubjectIntegrationEvent.v1`

El nombre definitivo deberá establecerse durante el diseño de los contratos de integración.

El evento deberá contener únicamente la información necesaria para que los consumidores reaccionen al cambio y no deberá exponer entidades internas completas de `SchedulingService`.

## Horas frente a grupo

El registro de la asignación deberá permitir posteriormente determinar las horas frente a grupo correspondientes al docente.

Por ejemplo, si:

```
Cálculo Integral
Créditos: 4

```
se asigna a:

```
José → Grupo 1A

```
la asignación representa:

```
José
└── Cálculo Integral - 1A
    └── 4 horas frente a grupo

```

Si también se registra:

```
José → Cálculo Integral - 1B

```
la carga resultante será:

```
1A → 4 horas
1B → 4 horas
─────────────
     8 horas frente a grupo

```

El cálculo y acumulación de las horas frente a grupo deberá quedar bajo responsabilidad de `SchedulingService`.

La definición detallada del paso 10 establecerá cómo se determina y registra este valor.

## Relaciones duplicadas

No deberá permitirse registrar dos veces la misma asignación dentro del mismo contexto académico.

Por ejemplo, no deberá existir:

```
José
   ↓
Cálculo Integral
   ↓
Grupo 1A

```
más de una vez para el mismo periodo y tenant.

La combinación que identifique una asignación deberá mantenerse única conforme al modelo de `SchedulingService`.

## Dependencias técnicas

El paso 9 depende de:

- La existencia del periodo académico activo.
- La existencia del plan de estudios correspondiente.
- La existencia de la materia.
- La existencia del grupo.
- La selección del docente.
- Las validaciones realizadas en el paso 8.
- La información de AcademicStaffService necesaria para validar al docente.
- La información de AcademicService necesaria para validar el contexto académico.

Todas las dependencias deberán resolverse mediante contratos de servicio.

No se permitirá acceso directo a las bases de datos de otros servicios.

## Fuera de alcance

Este paso no contempla:

- Creación del docente.
- Modificación del perfil profesional del docente.
- Creación de materias.
- Modificación del plan de estudios.
- Creación de grupos.
- Cálculo definitivo de horas frente a grupo.
- Generación completa de la carga académica.
- Horarios.
- Aulas.
- Asignación de horarios al docente.
- Validación de estudiantes.
- Inscripciones.

Estas responsabilidades pertenecen a otros pasos o dominios del flujo PR-001.

## Resultado esperado

Al finalizar el paso 9 deberá existir una asignación académica que relacione un docente con una materia dentro de un grupo y un periodo académico activo.

Conceptualmente:

```
Periodo académico activo
        │
        └── Grupo 1A
              │
              └── Cálculo Integral
                    │
                    └── Docente José
```
La asignación será propiedad de `SchedulingService`, respetará `TenantId`, conservará Correlation durante la operación y utilizará únicamente contratos para comunicarse con `AcademicService` y `AcademicStaffService`.

La asignación registrada constituirá la información necesaria para que el paso 10 pueda determinar las horas frente a grupo correspondientes al docente.