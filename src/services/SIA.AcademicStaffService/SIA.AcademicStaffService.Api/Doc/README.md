# SIA.AcademicStaffService

## Responsabilidad

`AcademicStaffService` es el servicio propietario de la información de negocio correspondiente al personal académico de la institución.

Su responsabilidad actual es administrar las personas que forman parte del personal académico y los perfiles de negocio que representan sus responsabilidades institucionales como docente, coordinador o responsable de división.

Las entidades administradas por este servicio representan perfiles de negocio y no sustituyen las cuentas, roles o permisos administrados por `IdentityService`.

## Lo que sí hace

Actualmente `AcademicStaffService`:

* Crea, consulta, actualiza, activa y desactiva personas del personal académico.
* Administra la información institucional y de contacto asociada a `Person`.
* Administra perfiles de docentes mediante `Teacher`.
* Administra perfiles de coordinadores mediante `Coordinator`.
* Administra perfiles de responsables de división mediante `DivisionHead`.
* Conserva información académica propia del perfil, como grado académico.
* Conserva información profesional y contractual del docente cuando corresponde.
* Mantiene la relación de un responsable de división con un programa educativo mediante su identificador externo.
* Genera eventos de integración cuando las entidades administradas son creadas, actualizadas, activadas o desactivadas.
* Expone contratos públicos para que otros servicios puedan reaccionar a cambios del personal académico sin utilizar sus entidades internas.

## Lo que no hace

`AcademicStaffService` no es responsable de:

* Crear cuentas de usuario.
* Administrar contraseñas, autenticación, tokens, roles o permisos.
* Crear o administrar programas educativos, planes de estudio, materias o periodos académicos.
* Crear grupos.
* Crear o publicar oferta académica.
* Asignar docentes a grupos.
* Construir horarios.
* Administrar aulas.
* Generar cargas académicas.
* Determinar las horas finales asignadas a un docente dentro de una carga académica.
* Validar la planeación académica completa.
* Consultar directamente bases de datos pertenecientes a otros servicios.

Estas responsabilidades pertenecen a sus respectivos dominios.

## Base de datos propietaria

La base de datos propietaria del servicio es:

`SIA_AcademicStaffDb`

Solo `AcademicStaffService` puede leer y escribir directamente sobre esta base.

Otros servicios no deben acceder directamente a sus tablas.

## Entidades administradas actualmente

### Person

Representa la información base de una persona perteneciente al personal académico.

Actualmente conserva información como:

* Número de trabajador.
* Nombre.
* Apellidos.
* Correo.
* Teléfono.
* Grado académico.
* Estado.
* Fechas técnicas de creación y actualización.

`Person` funciona como base para los perfiles de negocio específicos administrados por este servicio.

### Teacher

Representa el perfil docente de una persona.

Actualmente conserva información como:

* `PersonId`.
* Perfil profesional.
* Tipo de contratación.
* Horas de contrato.
* Estado.
* Fechas técnicas de creación y actualización.

La existencia de un `Teacher` representa el perfil de negocio docente. No equivale al rol de acceso `Teacher` administrado por `IdentityService`.

### Coordinator

Representa el perfil de coordinador asociado a una persona.

Actualmente conserva información como:

* `PersonId`.
* Estado.
* Fechas técnicas de creación y actualización.

### DivisionHead

Representa el perfil de responsable de división asociado a una persona.

Actualmente conserva información como:

* `PersonId`.
* `EducationalProgramId`.
* Estado.
* Fechas técnicas de creación y actualización.

`EducationalProgramId` es una referencia a información cuyo propietario es `AcademicService`. No representa propiedad sobre el programa educativo.

## Límites con otros servicios

### IdentityService

`IdentityService` es responsable de:

* Cuentas de usuario.
* Autenticación.
* Contraseñas.
* Tokens.
* Roles.
* Permisos.
* Claims y autorización.

`AcademicStaffService` administra los perfiles de negocio del personal académico.

Por lo tanto:

* Un rol de Identity no sustituye una entidad de negocio de AcademicStaff.
* Retirar o modificar un rol de acceso no implica eliminar automáticamente el perfil académico correspondiente.
* `AcademicStaffService` no administra credenciales ni permisos.

### AcademicService

`AcademicService` es propietario de la estructura académica, incluyendo conceptos como programas educativos, planes de estudio, materias y periodos académicos.

`AcademicStaffService` no crea ni modifica estas entidades.

Cuando necesita relacionar información propia con un concepto académico externo, conserva únicamente el identificador necesario, como `EducationalProgramId`.

No existen llaves foráneas ni consultas SQL hacia `SIA_AcademicDb`.

### SchedulingService

`SchedulingService` es propietario de la planeación académica.

Entre sus responsabilidades se encuentran:

* Grupos.
* Oferta académica.
* Asignación de docentes.
* Horarios.
* Aulas.
* Cargas académicas.
* Horas asignadas dentro de la planeación.

`AcademicStaffService` administra quién es el docente y cuáles son los datos propios de su perfil; `SchedulingService` decide cómo ese docente participa dentro de la planeación académica.

Por lo tanto, `AcademicStaffService` no debe asumir responsabilidades de asignación, horarios o carga académica.

## Eventos que publica

Los contratos de integración actuales del servicio se encuentran versionados como `v1`.

### Person

* `PersonCreatedIntegrationEvent.v1`
* `PersonUpdatedIntegrationEvent.v1`
* `PersonActivatedIntegrationEvent.v1`
* `PersonDeactivatedIntegrationEvent.v1`

### Teacher

* `TeacherCreatedIntegrationEvent.v1`
* `TeacherUpdatedIntegrationEvent.v1`
* `TeacherActivatedIntegrationEvent.v1`
* `TeacherDeactivatedIntegrationEvent.v1`

### Coordinator

* `CoordinatorCreatedIntegrationEvent.v1`
* `CoordinatorUpdatedIntegrationEvent.v1`
* `CoordinatorActivatedIntegrationEvent.v1`
* `CoordinatorDeactivatedIntegrationEvent.v1`

### DivisionHead

* `DivisionHeadCreatedIntegrationEvent.v1`
* `DivisionHeadUpdatedIntegrationEvent.v1`
* `DivisionHeadActivatedIntegrationEvent.v1`
* `DivisionHeadDeactivatedIntegrationEvent.v1`

Los eventos permiten que otros servicios mantengan referencias locales o reaccionen a cambios sin acceder directamente a la base de datos de `AcademicStaffService`.

## Eventos que consume

Actualmente `AcademicStaffService` no consume eventos de integración de otros servicios.

Cuando sea necesario incorporar información proveniente de otro dominio, deberá hacerse mediante contratos públicos, eventos de integración o referencias locales autorizadas, nunca mediante acceso directo a una base de datos ajena.

## Reglas críticas

* `AcademicStaffService` es el único propietario de `SIA_AcademicStaffDb`.
* Ningún otro servicio puede acceder directamente a esta base de datos.
* `AcademicStaffService` no puede acceder directamente a bases de datos pertenecientes a otros servicios.
* Las entidades de dominio `Person`, `Teacher`, `Coordinator` y `DivisionHead` son internas del servicio y no deben compartirse con otros dominios.
* La comunicación externa se realiza mediante contratos o eventos de integración.
* Los identificadores pertenecientes a otros dominios se consideran referencias externas y no generan ownership sobre esas entidades.
* No se permiten llaves foráneas entre bases de datos de servicios diferentes.
* La existencia de un perfil de negocio no debe confundirse con la existencia de un rol de acceso en `IdentityService`.
* Las decisiones de planeación académica permanecen bajo responsabilidad de `SchedulingService`.
* La estructura académica permanece bajo responsabilidad de `AcademicService`.
