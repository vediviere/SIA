# PLAN-01 — Paso 13. Registrar propuesta de carga académica en SIA

Una vez definidas las materias asignadas a los docentes y las horas de apoyo a la docencia, `SchedulingService` deberá permitir registrar la propuesta de carga académica correspondiente al `DivisionHead`.

El registro no representa únicamente la carga académica de un docente.

El resultado de este paso será el **conjunto completo de cargas académicas** propuestas por un `DivisionHead` para el `StudyPlan` o programa educativo que se encuentra administrando durante el periodo académico activo.

Conceptualmente:

```
DivisionHead
     │
     └── Programa educativo
             │
             └── Periodo académico
                     │
                     └── Propuesta de carga académica
                              │
                              ├── Docente 1
                              │      ├── Materias
                              │      └── Horas de apoyo
                              │
                              ├── Docente 2
                              │      ├── Materias
                              │      └── Horas de apoyo
                              │
                              └── Docente N
                                     ├── Materias
                                     └── Horas de apoyo
```

## Servicio propietario

El propietario de esta operación es: `SchedulingService`

`SchedulingService` será responsable de:

- Registrar la propuesta de carga académica.
- Asociar la propuesta con el periodo académico correspondiente.
- Asociar la propuesta con el programa educativo correspondiente.
- Asociar la propuesta con el DivisionHead que la registra.
- Consolidar las asignaciones de los docentes.
- Consolidar las horas frente a grupo.
- Consolidar las horas de apoyo a la docencia.
- Validar que las asignaciones que forman parte de la propuesta sean consistentes.
- Mantener el estado de la propuesta.
- Permitir que la propuesta posteriormente sea enviada a coordinación académica para validación.

## Alcance de la propuesta

La propuesta deberá representar el conjunto de cargas académicas correspondientes al ámbito administrado por el `DivisionHead`.

Por ejemplo:
```
`DivisionHead`: Juan Pérez
Programa educativo: Ingeniería en Sistemas Computacionales
Periodo: 2026-1

La propuesta podrá contener:

Docente José
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


Docente María
    ├── Programación - 1A
    │       └── 5 horas frente a grupo
    │
    └── Asesoría académica
            └── 3 horas de apoyo
```

La propuesta deberá conservar todas estas asignaciones como parte de un mismo registro de planeación.

No se registra una propuesta por docente

El registro de la propuesta no deberá generar una propuesta independiente por cada docente.

La unidad funcional será la propuesta correspondiente al ámbito de planeación del `DivisionHead`.

Por ejemplo:

```
Propuesta 2026-1
Ingeniería en Sistemas Computacionales
DivisionHead: Juan Pérez

    ├── Docente José
    ├── Docente María
    ├── Docente Carlos
    └── Docente Ana
```

De esta manera, el paso 13 representa el conjunto de cargas que el responsable de división propone para su programa educativo.

## Responsabilidad del DivisionHead

El `DivisionHead` será responsable de registrar la propuesta correspondiente a su ámbito de responsabilidad.

Cuando un `DivisionHead` tenga más de un programa educativo asociado, cada propuesta deberá conservar el contexto del programa seleccionado.

Por ejemplo:
```
`DivisionHead` Juan Pérez
        │
        ├── Ingeniería en Sistemas Computacionales
        │       └── Propuesta 2026-1
        │
        └── Ingeniería en Gestión Empresarial
                └── Propuesta 2026-1
```
No deberán mezclarse en una misma propuesta las cargas correspondientes a diferentes programas educativos administrados por el mismo `DivisionHead`.

## Contexto de la propuesta

La propuesta deberá quedar asociada como mínimo con:

- `TenantId`
- `AcademicPeriodId`
- `EducationalProgramId`
- `DivisionHeadId`

Estos elementos permiten determinar:

- A qué institución pertenece la propuesta.
- Para qué periodo se está realizando.
- Qué programa educativo se está planeando.
- Qué responsable de división realizó la propuesta.
- Relación con el periodo académico

La propuesta deberá registrarse dentro del periodo académico activo consultado previamente.

Conceptualmente:

```
AcademicPeriod
      │
      └── Propuesta de carga académica
```
No deberá registrarse una nueva propuesta de planeación para un periodo que no se encuentre habilitado para la planeación académica.

Los periodos anteriores deberán conservarse como histórico y no deberán mezclarse con las cargas activas del periodo actual.


## Consolidación de horas frente a grupo

La propuesta deberá consolidar las horas frente a grupo previamente asignadas.

Por ejemplo:

```
Docente José

Cálculo Integral - 1A → 4 horas
Cálculo Integral - 1B → 4 horas
Programación - 2A     → 5 horas

Total:

Horas frente a grupo = 13
```
Estas asignaciones deberán formar parte de la propuesta del docente.


## Consolidación de horas de apoyo

La propuesta deberá incluir también las horas de apoyo a la docencia.

Por ejemplo:
```
Docente José

Horas frente a grupo:
13

Horas de apoyo:
    Residencia Profesional → 6
    Tutoría                → 2
    Asesoría académica     → 3

Total:

Horas frente a grupo = 13
Horas de apoyo        = 11
───────────────────────────
Carga propuesta       = 24 horas

```

## Docentes con únicamente horas frente a grupo

La propuesta deberá permitir que un docente tenga únicamente horas frente a grupo.

Ejemplo:

```
Docente José

Frente a grupo: 20 horas

Apoyo: 0 horas

```
La ausencia de horas de apoyo no deberá impedir el registro de la propuesta.

## Docentes con únicamente horas de apoyo

La propuesta también deberá permitir que un docente tenga únicamente horas de apoyo.

Esto puede ocurrir, por ejemplo, cuando un docente de otra área participa en actividades de apoyo para el programa educativo.

Ejemplo:

```
Docente María

Frente a grupo: 0 horas
 
Apoyo: 4 horas
```
La propuesta deberá poder registrar esta situación.


## Docentes provenientes de otros programas educativos

Un docente puede participar en la planeación de diferentes programas educativos.

Por lo tanto, la carga académica propuesta para un docente no deberá considerarse únicamente desde la perspectiva del programa que está registrando el `DivisionHead`.

Las horas asignadas al docente en otros programas educativos deberán continuar siendo consideradas para determinar su disponibilidad.

Ejemplo:

```
Docente José

Programa A
    Frente a grupo → 15 horas

Programa B
    Frente a grupo → 10 horas

Programa B
    Apoyo          → 5 horas

Total:
30 horas

```
La propuesta del programa B deberá respetar las horas que el docente ya tenga comprometidas en otros ámbitos de planeación.

## Validación antes del registro

Antes de registrar la propuesta, `SchedulingService` deberá verificar que las asignaciones que la conforman sean consistentes con las reglas establecidas durante los pasos anteriores.

Entre otras validaciones:

- El periodo corresponde al contexto de planeación.
- El programa educativo corresponde al ámbito del DivisionHead.
- Los docentes existen como referencias válidas.
- Las materias pertenecen al contexto académico correspondiente.
- Los grupos utilizados pertenecen a la planeación correspondiente.
- Las horas frente a grupo son válidas.
- Las horas de apoyo son válidas.
- La carga de cada docente no excede sus horas disponibles.
- No existen asignaciones duplicadas.
- Las asignaciones pertenecen al periodo correspondiente.

## Estado de la propuesta

La propuesta deberá contar con un estado que permita distinguir entre una propuesta que todavía está siendo preparada y una que ya puede enviarse a validación.

Conceptualmente:

```
Draft
  │
  │ Registrar propuesta
  ▼
Propuesta registrada
  │
  │ Enviar a coordinación
  ▼
Submitted

```
El nombre definitivo de los estados deberá establecerse durante el diseño físico y de contratos.

Lo importante para PLAN-01 es que exista una distinción entre: Propuesta en preparación y Propuesta enviada a validación


## Registro de la propuesta

El registro deberá representar el momento en el que el conjunto de asignaciones queda formalmente almacenado como propuesta.

Conceptualmente:
```

`DivisionHead`
     │
     │ Registrar propuesta
     ▼
`SchedulingService`
     │
     ├── Periodo
     ├── Programa educativo
     ├── Docentes
     ├── Materias
     ├── Grupos
     ├── Horas frente a grupo
     └── Horas de apoyo
```
El registro deberá ser realizado por `.

## AdminBff

`AdminBff` podrá ser responsable de presentar y componer la información necesaria para que el `DivisionHead` revise la propuesta antes de registrarla.

Por ejemplo:

```
Programa:
Ingeniería en Sistemas Computacionales

Periodo:
2026-1

Propuesta:

Docente José
    Frente a grupo: 13
    Apoyo: 11
    Total: 24

Docente María
    Frente a grupo: 10
    Apoyo: 3
    Total: 13
```
`AdminBff` podrá combinar información proveniente de diferentes servicios para construir esta vista.

Sin embargo, `AdminBff` no deberá convertirse en propietario de la carga académica ni almacenar una copia autoritativa de la propuesta.

La decisión de aceptar el registro deberá pertenecer a `SchedulingService`.

## Consultas necesarias

Para construir y validar la propuesta podrán requerirse consultas relacionadas con: 

`AcademicService` información como:

- Periodo académico.
- Programa educativo.
- Plan de estudios.
- Materias.
- Participación de materias en el plan.

`AcademicStaffService` información como:

- Docente.
- Estado del docente.
- Información contractual necesaria para determinar sus horas totales.

`SchedulingService` información como:

- Grupos.
- Oferta académica.
- Asignaciones de docentes.
- Horas frente a grupo.
- Actividades de apoyo.
- Horas de apoyo.
- Propuestas existentes.
- Estado de la planeación.

Estas consultas deberán realizarse mediante contratos o mecanismos de comunicación entre servicios.

Nunca mediante acceso directo a las bases de datos.

## Comando

Conceptualmente, la operación podrá representarse como: `RegisterAcademicLoadProposal`

El comando deberá identificar el contexto de la propuesta y las asignaciones que serán registradas.

Como mínimo deberá contemplarse información equivalente a:

- `TenantId`
- `AcademicPeriodId`
- `EducationalProgramId`
- `DivisionHeadId`
- `CorrelationId`

y la información correspondiente al conjunto de cargas que forman parte de la propuesta.

Los nombres definitivos podrán ajustarse durante el diseño de los contratos.

## Evento de integración

Una vez registrada exitosamente la propuesta, `SchedulingService` podrá publicar un evento de integración para comunicar que existe una nueva propuesta disponible para validación.

Conceptualmente: `AcademicLoadProposalRegisteredIntegrationEvent.v1`

El evento deberá contener únicamente información necesaria para que los consumidores puedan reaccionar al registro.

No deberá exponer entidades internas de `SchedulingService`.

```
Paso 14

El resultado principal de este paso habilita el siguiente:

Paso 13
Registrar propuesta
        │
        ▼
Propuesta registrada
        │
        ▼
Paso 14
Enviar propuesta a coordinación académica

```
El paso 14 deberá operar sobre una propuesta previamente registrada.

Por lo tanto, no deberá ser posible enviar a validación una propuesta que todavía no haya sido registrada formalmente.

## TenantId

Toda la propuesta deberá mantenerse dentro del contexto de su `TenantId`.

El `TenantId` deberá conservarse durante las comunicaciones entre los servicios involucrados.

Conceptualmente:

```
AdminBff
    │
    │ TenantId
    ▼
SchedulingService
    │
    ├── AcademicService
    │
    └── AcademicStaffService
```
No deberá ser posible combinar información de diferentes tenants dentro de una misma propuesta.


## Dependencias técnicas

El paso 13 depende funcionalmente de:

- Paso 4 — Consultar periodo escolar activo.
- Paso 5 — Cargar plan de estudios.
- Paso 6 — Definir grupos.
- Paso 7 — Seleccionar docente.
- Paso 8 — Verificar cumplimiento del docente.
- Paso 9 — Asignar materia al docente.
- Paso 10 — Asignar horas frente a grupo.
- Paso 11 — Seleccionar actividades de apoyo.
- Paso 12 — Asignar horas de apoyo.

Estas operaciones proporcionan la información que posteriormente será consolidada en la propuesta.

## Dependencias entre servicios

La dependencia técnica principal será:

```
AcademicService
        │
        ├── Periodo
        ├── Programa educativo
        ├── Plan de estudios
        └── Materias
                │
                ▼
SchedulingService
        │
        ├── Grupos
        ├── Oferta
        ├── Docentes asignados
        ├── Horas frente a grupo
        ├── Horas de apoyo
        └── Propuesta de carga
                │
                ▼
          AdminBff
                │
                ▼
        Vista para DivisionHead
```
`AcademicStaffService` participa proporcionando información propia del docente, particularmente aquella necesaria para determinar su capacidad contractual.

Ninguno de estos servicios deberá acceder directamente a la base de datos de otro servicio.

## Reglas funcionales
- El registro de la propuesta pertenece a `SchedulingService`.
- La propuesta representa el conjunto de cargas académicas del ámbito administrado por un `DivisionHead`.
- Una propuesta no deberá representar únicamente la carga de un docente.
- Una propuesta deberá contener las asignaciones de los docentes que forman parte de la planeación correspondiente.
- La propuesta deberá asociarse a un `TenantId`.
- La propuesta deberá asociarse a un periodo académico.
- La propuesta deberá asociarse a un programa educativo.
- La propuesta deberá identificar al `DivisionHead` responsable de registrarla.
- Una propuesta deberá integrar horas frente a grupo y horas de apoyo.
- Un docente podrá tener únicamente horas frente a grupo.
- Un docente podrá tener únicamente horas de apoyo.
- Un docente podrá tener ambas.
- Las horas de periodos anteriores no deberán afectar la disponibilidad del periodo activo.
- Las horas asignadas al docente en otros programas deberán considerarse para validar su disponibilidad global.
- La carga total de un docente no deberá exceder sus horas disponibles.
- No deberán registrarse asignaciones duplicadas.
- El programa educativo de la propuesta deberá pertenecer al contexto administrado por el `DivisionHead`.
- No deberán mezclarse en una misma propuesta las cargas de diferentes programas educativos.
- Una propuesta deberá encontrarse registrada antes de poder enviarse a coordinación académica.
- La propuesta deberá conservar un estado que permita distinguir una propuesta en preparación de una propuesta enviada a validación.
- `AdminBff` podrá componer la información necesaria para presentar la propuesta, pero no será propietario de ella.
- `SchedulingService` será el propietario de la propuesta de carga académica.
- `AcademicService` continuará siendo propietario de la estructura académica.
- `AcademicStaffService` continuará siendo propietario de la información del personal académico.
- Ningún servicio deberá acceder directamente a la base de datos de otro servicio.
- Las operaciones deberán respetar `TenantId`.

## Fuera de alcance

Este paso no contempla:

- Envío de la propuesta a coordinación académica.
- Validación de la propuesta por coordinación académica.
- Aprobación de la propuesta.
- Rechazo de la propuesta.
- Modificación de la propuesta después de ser enviada.
- Construcción de horarios.
- Asignación de aulas.
- Inscripción de estudiantes.
- Validación de estudiantes.
- Cálculo de calificaciones.

Estas actividades corresponden a pasos posteriores del flujo PR-001.

## Resultado esperado

Al finalizar el paso 13 deberá existir una propuesta de carga académica registrada para el periodo y programa educativo correspondientes.

La propuesta deberá contener el conjunto de cargas definido por el `DivisionHead`.

Ejemplo:

```
Propuesta de carga académica
Periodo: 2026-1
Programa: Ingeniería en Sistemas Computacionales
Responsable: DivisionHead Juan Pérez

    ├── Docente José
    │     ├── Cálculo Integral - 1A
    │     │      └── 4 horas frente a grupo
    │     │
    │     ├── Cálculo Integral - 1B
    │     │      └── 4 horas frente a grupo
    │     │
    │     ├── Residencia Profesional
    │     │      └── 6 horas de apoyo
    │     │
    │     └── Tutoría - 1A
    │            └── 2 horas de apoyo
    │
    ├── Docente María
    │     ├── Programación - 1A
    │     │      └── 5 horas frente a grupo
    │     │
    │     └── Asesoría académica
    │            └── 3 horas de apoyo
    │
    └── Docente Carlos
          └── ...
```
La propuesta quedará registrada bajo el contexto del `DivisionHead`, programa educativo, periodo y `TenantId`, con un estado que permita posteriormente ejecutar el:

Paso 14 — Enviar propuesta a coordinación académica para su validación.