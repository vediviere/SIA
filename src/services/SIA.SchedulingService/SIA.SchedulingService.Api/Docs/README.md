# SIA.SchedulingService

## Responsabilidad

`SchedulingService` es el servicio propietario de la planeación académica operativa de una institución.

Su responsabilidad es administrar la información necesaria para construir la oferta académica de un periodo, organizar grupos, asignar docentes dentro de cargas académicas, administrar espacios físicos y establecer horarios de clase y actividades de apoyo.

`SchedulingService` utiliza identificadores de otros dominios cuando necesita relacionar la planeación con docentes, programas educativos, materias, periodos académicos o divisiones, pero esos identificadores funcionan únicamente como referencias externas y no transfieren la propiedad de esas entidades al servicio.

## Lo que sí hace

Actualmente `SchedulingService` administra conceptos relacionados con:

* Edificios.
* Tipos de aula o laboratorio.
* Aulas y laboratorios.
* Grupos.
* Oferta académica.
* Carga académica docente.
* Horarios de clase.
* Actividades de apoyo.
* Horas de apoyo docente.
* Horarios asociados a actividades de apoyo.
* Asignación de espacios físicos dentro de la planeación.
* Horas de clase y horas de apoyo asociadas a una carga académica.
* Referencias a periodos académicos, programas educativos, materias, docentes y divisiones necesarias para construir la planeación.
* Publicación de eventos de integración derivados de cambios realizados sobre información propia del servicio.

## Lo que no hace

`SchedulingService` no es responsable de:

* Crear cuentas de usuario.
* Administrar autenticación, contraseñas, tokens, roles o permisos.
* Crear o administrar personas del personal académico.
* Administrar el perfil profesional o contractual oficial de un docente.
* Crear programas educativos.
* Crear planes de estudio.
* Crear materias.
* Administrar la definición oficial de periodos académicos.
* Administrar inscripciones o reinscripciones de alumnos.
* Administrar el historial escolar o kardex.
* Capturar o administrar calificaciones.
* Administrar perfiles de estudiantes.
* Sustituir los procesos institucionales de aprobación que correspondan a `WorkflowService`.
* Consultar directamente bases de datos pertenecientes a otros servicios.

Estas responsabilidades permanecen en los dominios propietarios correspondientes.

## Base de datos propietaria

La base de datos propietaria del servicio es:

`SIA_SchedulingDb`

Solo `SchedulingService` puede leer y escribir directamente sobre esta base.

Otros servicios no deben acceder directamente a sus tablas.

Del mismo modo, `SchedulingService` no puede acceder directamente a las bases de datos de otros servicios.

## Entidades administradas

### Building

Representa un edificio disponible para la planeación de espacios académicos.

Conserva información propia del edificio, como código, nombre, descripción y estado.

### ClassroomType

Representa la clasificación de un espacio utilizado durante la planeación, por ejemplo un tipo de aula o laboratorio.

Permite clasificar los espacios físicos administrados por `SchedulingService`.

### ClassroomLab

Representa un aula o laboratorio concreto disponible para asignaciones de horario.

Mantiene su relación interna con un edificio y un tipo de aula, además de información como código, nombre, capacidad, descripción y estado.

### Group

Representa un grupo utilizado durante la planeación académica.

Mantiene información como:

* `EducationalProgramId`.
* Nombre del grupo.
* Turno.
* Capacidad.
* Estado.

`EducationalProgramId` es una referencia externa al programa educativo cuyo propietario es `AcademicService`.

### AcademicLoad

Representa la carga académica asignada a un docente dentro de un periodo.

Mantiene información como:

* `TeacherId`.
* `DivisionId`.
* `AcademicPeriodId`.
* Número de oficio.
* Fecha de propuesta.
* Horas de clase.
* Horas de apoyo.
* Fecha de asignación.
* Estado.

`TeacherId`, `DivisionId` y `AcademicPeriodId` son referencias necesarias para la planeación y no convierten a `SchedulingService` en propietario del docente, la división o el periodo académico.

### AcademicOffering

Representa una asignación dentro de la oferta académica.

Relaciona elementos necesarios para determinar qué materia será impartida, a qué grupo pertenece y bajo qué carga académica se realiza.

Actualmente utiliza referencias como:

* `GroupId`.
* `SubjectId`.
* `AcademicLoadId`.

`SubjectId` identifica una materia cuyo propietario continúa siendo `AcademicService`.

### ClassSchedule

Representa el horario asignado a una oferta académica.

Relaciona la oferta con un aula o laboratorio y un periodo académico, además de conservar información de día, hora de inicio, hora de término y estado.

### SupportActivity

Representa una actividad de apoyo que puede formar parte de la carga académica de un docente.

Mantiene la definición de la actividad y la información necesaria para utilizarla dentro de la planeación.

### TeachingSupportHour

Representa la cantidad de horas asignadas a una actividad de apoyo dentro de una carga académica.

Relaciona una actividad de apoyo con una carga académica y conserva las horas correspondientes.

### SupportSchedule

Representa el horario asignado a una actividad de apoyo.

Relaciona las horas o actividades de apoyo con un espacio físico y un periodo académico, conservando información de día, hora de inicio y hora de término.

## Referencias a otros dominios

`SchedulingService` utiliza identificadores pertenecientes a otros dominios para construir la planeación académica.

Entre ellos se encuentran actualmente:

* `TeacherId`.
* `DivisionId`.
* `AcademicPeriodId`.
* `EducationalProgramId`.
* `SubjectId`.

Estos identificadores son referencias externas.

No deben implementarse como llaves foráneas entre bases de datos ni utilizarse para consultar directamente las tablas del servicio propietario.

Cuando `SchedulingService` necesite información adicional proveniente de otro dominio, deberá obtenerla mediante contratos públicos, eventos de integración o referencias locales autorizadas.

## Límites con otros servicios

### IdentityService

`IdentityService` administra:

* Cuentas de usuario.
* Autenticación.
* Contraseñas.
* Tokens.
* Roles.
* Permisos.
* Claims y autorización.

`SchedulingService` no administra identidad ni determina los privilegios de acceso de un docente, coordinador, responsable de división u otro usuario.

### AcademicStaffService

`AcademicStaffService` es propietario de los perfiles de negocio correspondientes al personal académico.

Administra conceptos como personas, docentes, coordinadores y responsables de división, junto con la información que pertenece a esos perfiles.

`SchedulingService` utiliza identificadores como `TeacherId` y `DivisionId` cuando necesita incorporar al personal dentro de una planeación.

Por lo tanto:

* `AcademicStaffService` determina quién es el docente y conserva su perfil.
* `SchedulingService` determina cómo participa ese docente dentro de una carga y una oferta académica.
* `SchedulingService` no modifica directamente el perfil del docente.
* `AcademicStaffService` no debe crear horarios, grupos, ofertas ni cargas académicas.

### AcademicService

`AcademicService` es propietario de la estructura académica institucional.

Entre sus responsabilidades se encuentran conceptos como:

* Programas educativos.
* Planes de estudio.
* Materias.
* Periodos académicos.

`SchedulingService` utiliza sus identificadores cuando son necesarios para construir grupos, ofertas, cargas y horarios.

Por lo tanto:

* `AcademicService` define la estructura académica.
* `SchedulingService` utiliza esa estructura para realizar la planeación operativa.
* Una referencia como `SubjectId`, `EducationalProgramId` o `AcademicPeriodId` no transfiere ownership a `SchedulingService`.

### SchoolControlService

`SchoolControlService` es propietario de la información escolar del alumno.

Entre sus responsabilidades se encuentran inscripción, reinscripción, situación escolar, historial académico y relación del estudiante con sus materias.

`SchedulingService` construye la oferta y la planeación que posteriormente puede ser utilizada por procesos escolares, pero no administra la inscripción del alumno ni su historial.

### EvaluationService

`EvaluationService` es propietario de los procesos de evaluación y calificaciones.

`SchedulingService` puede proporcionar información relacionada con grupos, docentes u oferta académica mediante sus contratos o eventos, pero no captura ni modifica calificaciones.

### WorkflowService

`WorkflowService` es responsable de coordinar procesos institucionales que requieren etapas, responsables, aprobaciones, rechazos o devoluciones.

`SchedulingService` conserva y modifica el estado propio de sus entidades, pero no debe convertirse en un motor general de workflow institucional.

## Eventos que publica actualmente

Los eventos de integración actuales utilizan versión `v1`.

### Building

* `BuildingCreatedIntegrationEvent.v1`
* `BuildingUpdatedIntegrationEvent.v1`
* `BuildingActivatedIntegrationEvent.v1`
* `BuildingDeactivatedIntegrationEvent.v1`

### Group

* `GroupCreatedIntegrationEvent.v1`
* `GroupUpdatedIntegrationEvent.v1`
* `GroupActivateIntegrationEvent.v1`
* `GroupDeactivatedIntegrationEvent.v1`

### AcademicLoad

* `AcademicLoadCreatedIntegrationEvent.v1`
* `AcademicLoadUpdatedIntegrationEvent.v1`
* `AcademicLoadActivatedIntegrationEvent.v1`
* `AcademicLoadDeactivatedIntegrationEvent.v1`

### AcademicOffering

* `AcademicOfferingCreatedIntegrationEvet.v1`
* `AcademicOfferingUpdatedIntegrationEvent.v1`
* `AcademicOfferingActivatedIntegrationEvent.v1`
* `AcademicOfferingDeactivatedIntegrationEvent.v1`

> Nota: el contrato actual de creación de oferta está implementado como `AcademicOfferingCreatedIntegrationEvet`. El nombre se conserva aquí para reflejar exactamente la implementación existente.

### ClassroomType

* `ClassroomTypeCreatedIntegrationEvent.v1`
* `ClassroomTypeUpdatedIntegrationEvent.v1`
* `ClassroomTypeDeletedIntegrationEvent.v1`
* `ClassroomTypeRestoredIntegrationEvent.v1`

### ClassroomLab

* `ClassroomLabCreatedIntegrationEvent.v1`
* `ClassroomLabUpdatedIntegrationEvent.v1`
* `ClassroomLabDeletedIntegrationEvent.v1`
* `ClassroomLabRestoredIntegrationEvent.v1`

### ClassSchedule

* `ClassScheduleCreatedIntegrationEvent.v1`
* `ClassScheduleUpdatedIntegrationEvent.v1`
* `ClassScheduleDeletedIntegrationEvent.v1`
* `ClassScheduleRestoredIntegrationEvent.v1`

### SupportSchedule

* `SupportScheduleCreatedIntegrationEvent.v1`
* `SupportScheduleUpdatedIntegrationEvent.v1`
* `SupportScheduleDeletedIntegrationEvent.v1`
* `SupportScheduleRestoredIntegrationEvent.v1`

### SupportActivity

* `SupportActivityCreatedIntegrationEvent.v1`
* `SupportActivityUpdatedIntegrationEvent.v1`
* `SupportActivityDeletedIntegrationEvent.v1`
* `SupportActivityRestoredIntegrationEvent.v1`

## TeachingSupportHours

Actualmente existen los siguientes contratos y se generan mensajes Outbox para ellos:

* `TeachingSupportHoursCreatedIntegrationEvent.v1`
* `TeachingSupportHoursUpdatedIntegrationEvent.v1`
* `TeachingSupportHoursActivatedIntegrationEvent.v1`
* `TeachingSupportHoursDeactivatedIntegrationEvent.v1`

Sin embargo, estos tipos todavía no están registrados en el publicador Outbox actual del servicio. Por esa razón no se consideran eventos completamente soportados por el flujo de publicación hasta que se complete esa integración.

## Eventos que consume

Actualmente `SchedulingService` no tiene consumidores de eventos de integración registrados.

Cuando necesite recibir información proveniente de `AcademicService`, `AcademicStaffService` u otro dominio, deberá hacerlo mediante consumidores explícitos y mantener únicamente las referencias o proyecciones locales necesarias.

No debe sustituirse ese mecanismo por consultas directas a bases de datos externas.

## Reglas críticas

* `SchedulingService` es el único propietario de `SIA_SchedulingDb`.
* Ningún otro servicio puede acceder directamente a esta base de datos.
* `SchedulingService` no puede acceder directamente a bases de datos pertenecientes a otros servicios.
* Las entidades internas del dominio no deben compartirse con otros servicios.
* La comunicación externa debe realizarse mediante contratos, eventos de integración o modelos de lectura autorizados.
* Las referencias a entidades externas no transfieren ownership.
* No deben existir llaves foráneas entre `SIA_SchedulingDb` y bases de otros servicios.
* `AcademicService` conserva el ownership de la estructura académica.
* `AcademicStaffService` conserva el ownership del perfil del personal académico.
* `SchoolControlService` conserva el ownership de la información escolar del alumno.
* `EvaluationService` conserva el ownership de las evaluaciones y calificaciones.
* `IdentityService` conserva el ownership de cuentas, roles y permisos.
* `SchedulingService` conserva el ownership de la planeación académica operativa.
