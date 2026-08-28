# 6. Definir grupos (oferta por grupos)

Servicio propietario: `SchedulingService`

Responsabilidad:

`SchedulingService` es propietario de la planeación académica y, por lo tanto, es responsable de definir los grupos que serán utilizados para la oferta académica del periodo correspondiente.

Este paso utiliza como referencia el `StudyPlan` previamente cargado desde `AcademicService`, pero las decisiones sobre qué materias se aperturan, a qué grupo pertenecen y qué turno tendrán forman parte de la planeación académica.

El proceso contempla:

- Apertura de semestres.
- Selección de materias que serán ofertadas.
- Asignación de materias a grupos.
- Definición del turno de los grupos.
- Consideración del periodo académico correspondiente.

---

## Consulta del plan de estudios

`SchedulingService` necesitará consultar la estructura académica definida en `AcademicService` para conocer las materias que pertenecen al StudyPlan seleccionado.

La consulta deberá permitir obtener información como:
```
StudyPlan
    │
    └── StudyPlanSubject
            │
            └── Subject
                 ├── Code
                 ├── Name
                 ├── Semester
                 └── Credits

```
`SchedulingService` no será propietario de estas materias ni del plan de estudios.

La información será utilizada como referencia para construir la oferta por grupos.

---

## Selección de materias a aperturar

A partir del plan de estudios cargado, el responsable de la planeación podrá seleccionar las materias que serán aperturadas durante el periodo.

Como regla general, las materias se seleccionan respetando el semestre definido en el plan de estudios.

Sin embargo, la planeación puede requerir incorporar materias pertenecientes a otros semestres.

Por ejemplo:
```
Plan de estudios

Semestre 1
    Materia A
    Materia B

Semestre 3
    Materia C
    Materia D

Semestre 5
    Materia E
```

La planeación podría considerar:

Apertura
```
Semestre 1
    Materia A
    Materia B
    Materia C
 ```
cuando las reglas académicas y la operación institucional permitan incorporar una materia de otro semestre.

Por lo tanto:

El semestre definido en `StudyPlanSubject` sirve como referencia para la planeación, pero no debe interpretarse como una restricción absoluta que impida seleccionar una materia de otro semestre.

Las reglas específicas que determinen cuándo puede realizarse este movimiento deberán conservarse como reglas de negocio de la planeación académica.

---

## Apertura de semestres

El responsable de la planeación, es decir, `DivisionHead`, deberá definir los semestres que serán aperturados para el periodo.

La apertura se realizará de acuerdo con el comportamiento del periodo académico.

Por ejemplo, en un periodo determinado pueden aperturarse los semestres impares:

1.º semestre
3.º semestre
5.º semestre
7.º semestre
9.º semestre

En otro periodo podrán corresponder los semestres pares.

La determinación de qué semestres corresponden al periodo deberá formar parte de las reglas de negocio de `SchedulingService`.

No deberá inferirse únicamente a partir del número almacenado en Semester.

---
## Definición de grupos

Una vez determinadas las materias que serán aperturadas, estas deberán organizarse dentro de grupos.

Conceptualmente:
```
Semestre
    │
    ├── Grupo A
    │      ├── Materia 1
    │      ├── Materia 2
    │      └── Materia 3
    │
    └── Grupo B
           ├── Materia 1
           ├── Materia 2
           └── Materia 3

```
La asignación de una materia a un grupo forma parte de la planeación académica.

Generalmente los grupos corresponden al semestre al que pertenece la materia dentro de la estructura académica; sin embargo, deberán contemplarse situaciones extraordinarias en las que una materia pueda incorporarse a un grupo diferente.

---

## Definición de turno

Al definir los grupos también se deberá determinar el turno correspondiente.

Los turnos contemplados actualmente son:

Matutino
Vespertino

Como criterio general de operación:
```
Semestres de menor rango
        ↓
Turno Matutino

Semestres de mayor rango
        ↓
Turno Vespertino
```

Esta distribución representa una regla operativa habitual y no deberá utilizarse para inferir automáticamente el turno únicamente a partir del semestre.

El turno deberá quedar definido explícitamente dentro de la planeación del grupo.

---

## Relación entre semestre, grupo y turno

Conceptualmente, el resultado de este paso será:
```
Periodo académico
        │
        ▼
Semestres aperturados
        │
        ├── Semestre 1
        │      ├── Grupo 1A
        │      │      └── Matutino
        │      │
        │      └── Grupo 1B
        │             └── Matutino
        │
        ├── Semestre 3
        │      └── Grupo 3A
        │             └── Matutino
        │
        ├── Semestre 7
        │      └── Grupo 7A
        │             └── Vespertino
        │
        └── Semestre 9
               └── Grupo 9A
                      └── Vespertino
```
La estructura exacta de los grupos y la asignación de materias deberá ser administrada por `SchedulingService`.

---

## Materias de otros semestres

El modelo deberá permitir que una materia seleccionada para la oferta pertenezca a un semestre diferente al semestre principal del grupo.

Esto es necesario debido a que en la operación real pueden existir situaciones en las que se incorporen materias de otros semestres para evitar retrasos en la trayectoria académica de los estudiantes.

Por ejemplo:
```
Grupo 3A

Semestre principal:
3

Materias:
├── Materia correspondiente a semestre 3
├── Materia correspondiente a semestre 3
└── Materia correspondiente a semestre 5
```
La posibilidad de realizar esta asignación no modifica el Semester definido en `AcademicService`.

`AcademicService` conserva la estructura del plan de estudios; `SchedulingService` determina cómo esa estructura será utilizada dentro de una oferta académica concreta.

---

## Consulta

Para este paso se requiere información de:

`AcademicService`

- StudyPlan.
- StudyPlanSubject.
- Subject.
- Semestre de la materia.
- Identificadores correspondientes.

`SchedulingService`

- Periodo académico de la planeación.
- Grupos existentes o nuevos grupos a definir.
- Oferta académica que se está construyendo.

---

## Comando

Este paso sí requiere comandos de negocio, porque se está construyendo/modificando la planeación académica.

Conceptualmente se requieren operaciones para:

- Aperturar grupos.
- Definir el turno del grupo.
- Asociar materias seleccionadas a la oferta del grupo.

Los nombres definitivos de los comandos y contratos deberán definirse durante la implementación y no forman parte de esta tarea.

---

## Eventos de integración

Para este paso no es necesario asumir eventos de integración entre servicios únicamente para consultar materias o planes de estudio.

Si posteriormente otros servicios necesitan reaccionar ante la publicación o modificación de la oferta académica, `SchedulingService` podrá publicar eventos de integración correspondientes.

La definición de esos eventos deberá estar asociada al momento en que la oferta académica cambie de estado, no a la simple consulta del plan de estudios.

---

## TenantId

Todas las operaciones de este paso deberán ejecutarse dentro del `TenantId` correspondiente.

El `TenantId` deberá mantenerse desde el contexto inicial del flujo y utilizarse para:

- Consultar el `StudyPlan` correspondiente.
- Consultar las materias.
- Crear o modificar grupos.
- Construir la oferta académica.

No deberá ser posible utilizar materias, planes o grupos pertenecientes a otro `tenant`.

---

## AdminBff

`AdminBff` podrá componer la información necesaria para presentar una interfaz de planeación como:

```
Periodo
    ↓
Semestres a aperturar
    ↓
Materias disponibles
    ↓
Grupo
    ↓
Turno
    ↓
Materias asignadas al grupo

```

También podrá combinar información proveniente de diferentes servicios para construir la vista.

Sin embargo, `AdminBff` no deberá:

- Ser propietario de grupos.
- Ser propietario de la oferta académica.
- Modificar `StudyPlan`.
- Modificar `Subject`.
- Consultar directamente las bases de datos.
- Implementar las reglas de negocio de la planeación.
- Decidir por sí mismo qué materias pueden ofertarse.

Las decisiones deberán ejecutarse mediante operaciones de `SchedulingService`.

---

## Dependencias técnicas

Este paso depende de:

- `AcademicService` para consultar el `StudyPlan` y las materias.
- `SchedulingService` para construir la oferta académica.
- `AcademicPeriodId` obtenido en el paso 4.
- `StudyPlanId` obtenido en el paso 5.
- `EducationalProgramId` seleccionado previamente.
- `TenantId`.

El resultado de este paso será utilizado por los pasos posteriores relacionados con la construcción de la carga académica y la planeación docente.

---

## Resultado esperado

Al finalizar este paso deberá existir una definición de los grupos que participarán en la oferta académica del periodo, incluyendo:
```
Periodo
    │
    ├── Semestres aperturados
    │
    ├── Grupos
    │      ├── Semestre
    │      ├── Turno
    │      └── Materias ofertadas
    │
    └── Materias incorporadas desde otros semestres
```
`SchedulingService` será responsable de esta estructura de planeación.

`AcademicService` continuará siendo propietario del `StudyPlan`, `StudyPlanSubject` y `Subject`; este paso solamente utiliza dicha información para construir la oferta académica.