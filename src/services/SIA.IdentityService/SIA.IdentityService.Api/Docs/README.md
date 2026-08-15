# SIA.IdentityService

## Responsabilidad

`IdentityService` es el servicio propietario de la identidad de acceso institucional y de la autorización general de las cuentas de usuario de SIA.

Su responsabilidad es administrar quién puede autenticarse en la plataforma, a qué institución pertenece su cuenta, cuál es el estado de esa cuenta y qué roles y permisos generales tiene asignados.

`IdentityService` administra identidad de acceso, no la identidad de negocio de una persona.

Por lo tanto, un `UserId` identifica una cuenta de acceso institucional, mientras que identificadores como `TeacherId`, `StudentId` u otros identificadores de dominio representan entidades administradas por sus respectivos servicios.

## Lo que sí hace

Actualmente `IdentityService`:

* Administra cuentas de usuario.
* Mantiene la relación de cada cuenta con un `TenantId`.
* Administra credenciales protegidas.
* Realiza autenticación de usuarios.
* Genera tokens de acceso.
* Genera y administra refresh tokens.
* Permite renovar una sesión mediante refresh token.
* Permite cerrar una sesión mediante revocación del refresh token.
* Administra el cambio de contraseña.
* Administra el establecimiento de la contraseña inicial.
* Administra roles.
* Administra permisos.
* Mantiene las relaciones entre usuarios y roles.
* Mantiene las relaciones entre roles y permisos.
* Permite asignar y revocar roles.
* Administra el estado de acceso de una cuenta.
* Permite activar, desactivar o bloquear cuentas.
* Permite crear cuentas para personal institucional mediante una operación autorizada.
* Permite provisionar el administrador inicial de una institución mediante el mecanismo técnico definido para ese propósito.
* Registra auditoría de operaciones relevantes de identidad.
* Publica eventos de integración relacionados con cambios importantes de las cuentas y sus roles.

## Lo que no hace

`IdentityService` no es responsable de:

* Administrar el expediente completo de una persona.
* Crear o administrar perfiles docentes.
* Crear o administrar perfiles de coordinadores o responsables de división.
* Administrar información laboral o académica del personal.
* Crear o administrar estudiantes como entidades de negocio.
* Administrar matrículas.
* Validar la existencia académica de un alumno.
* Administrar inscripciones o reinscripciones.
* Administrar historial escolar o kardex.
* Crear programas educativos.
* Crear planes de estudio.
* Crear materias.
* Crear grupos.
* Administrar horarios.
* Administrar cargas académicas.
* Capturar calificaciones.
* Mantener históricos de cargos o responsabilidades institucionales.
* Convertir roles de acceso en sustitutos de entidades de negocio.
* Consultar directamente bases de datos pertenecientes a otros servicios.

Estas responsabilidades permanecen bajo propiedad de sus respectivos dominios.

## Base de datos propietaria

La base de datos propietaria del servicio es:

`SIA_IdentityDb`

Solo `IdentityService` puede leer y escribir directamente sobre esta base.

Otros servicios no deben acceder directamente a sus tablas.

Del mismo modo, `IdentityService` no debe acceder directamente a bases de datos pertenecientes a otros servicios.

## Entidades de dominio administradas actualmente

### User

Representa una cuenta institucional de acceso a SIA.

Actualmente mantiene información como:

* `TenantId`.
* Correo electrónico.
* Hash de contraseña.
* Estado de la cuenta.
* Indicador de cambio obligatorio de contraseña.
* Fecha de creación.
* Fecha de actualización.

Los estados actuales de una cuenta permiten distinguir entre:

* `Active`.
* `Locked`.
* `Inactive`.

Un `User` representa una cuenta de acceso y no el expediente completo de la persona.

### Role

Representa una responsabilidad general utilizada para autorización.

Mantiene información como:

* Código.
* Descripción.
* Fecha de creación.
* Fecha de actualización.

Los roles expresan capacidades actuales de acceso.

No deben utilizarse para representar información histórica ni para reemplazar entidades de negocio.

### Permission

Representa una capacidad específica que puede utilizarse durante la autorización.

Mantiene información como:

* Código.
* Descripción.
* Fecha de creación.
* Fecha de actualización.

Los permisos pueden asociarse a roles y permiten controlar capacidades de acceso con mayor granularidad.

### UserRole

Representa la asignación de un rol a una cuenta de usuario.

Relaciona:

* `UserId`.
* `RoleId`.

La asignación puede permanecer activa o ser revocada.

Revocar un `UserRole` retira una capacidad de acceso, pero no elimina perfiles ni información histórica almacenada en otros dominios.

### RolePermission

Representa la asignación de un permiso a un rol.

Relaciona:

* `RoleId`.
* `PermissionId`.

La relación puede permanecer activa o ser revocada.

### RefreshToken

Representa una credencial utilizada para renovar una sesión sin solicitar nuevamente las credenciales principales del usuario.

Mantiene información como:

* `UserId`.
* Hash del token.
* Fecha de creación.
* Fecha de expiración.
* Fecha de revocación.

Los refresh tokens forman parte de la infraestructura de autenticación administrada por `IdentityService`.

## Información técnica persistida

Además de las entidades de dominio, `IdentityService` mantiene estructuras técnicas necesarias para seguridad, auditoría y mensajería.

### AuditLog

Registra información de auditoría asociada a operaciones relevantes.

Permite conservar datos como:

* `TenantId`.
* Acción realizada.
* Entidad afectada.
* Identificador de la entidad.
* Usuario responsable cuando aplica.
* Fecha de ocurrencia.
* Valores anteriores y nuevos cuando aplica.
* `CorrelationId`.

`AuditLog` es infraestructura de trazabilidad y no una entidad funcional de negocio equivalente a `User`, `Role` o `Permission`.

### OutboxMessages

Almacena eventos pendientes de publicación mediante el patrón Outbox.

Permite que cambios de identidad y sus eventos de integración se persistan de forma confiable antes de ser enviados al broker.

La implementación utiliza el componente compartido de `SIA.BuildingBlocks.Messaging`.

## Identidad institucional y Tenant

Cada `User` pertenece a un único `Tenant`.

`TenantId` identifica la institución a la que pertenece la cuenta, pero `IdentityService` no es propietario de la institución.

La información y ciclo de vida del tenant pertenecen a `TenancyService`.

Dentro de `IdentityService`, `TenantId` se utiliza para:

* Asociar una cuenta con su institución.
* Aislar operaciones de identidad por institución.
* Incluir el contexto institucional en autenticación y autorización.
* Evitar que una operación administrativa afecte cuentas pertenecientes a otro tenant.

Una misma persona que pertenezca a dos instituciones tendrá una cuenta institucional independiente en cada una, con un `UserId` distinto.

## Roles y perfiles de negocio

Un rol no es una entidad de negocio.

Ejemplos:

* `Role = Teacher` representa que la cuenta tiene actualmente capacidades asociadas al rol docente.
* `TeacherId` identifica al docente como entidad del dominio de personal académico.
* `Role = Student` representa capacidades actuales de estudiante.
* `StudentId` identifica al estudiante dentro del dominio escolar correspondiente.

Por lo tanto:

* Asignar un rol no crea automáticamente una entidad de negocio.
* Revocar un rol no elimina una entidad de negocio.
* Cambiar de función institucional no requiere crear otra cuenta dentro del mismo tenant.
* El histórico de una persona no debe reconstruirse utilizando únicamente sus roles actuales.

## Límites con otros servicios

### TenancyService

`TenancyService` es propietario de las instituciones y su información de tenant.

`IdentityService` conserva `TenantId` dentro de la cuenta porque necesita conocer el contexto institucional de autenticación y autorización.

Por lo tanto:

* `IdentityService` no crea ni administra la institución.
* `TenantId` funciona como referencia externa.
* La presencia de `TenantId` en `User` no transfiere ownership del tenant a `IdentityService`.
* `IdentityService` no consulta directamente la base de datos de `TenancyService`.

### AcademicStaffService

`AcademicStaffService` es propietario de los perfiles de negocio correspondientes al personal académico.

Entre ellos pueden existir docentes, coordinadores y responsables de división.

`IdentityService` puede administrar roles de acceso asociados a estas responsabilidades, pero no administra sus perfiles de negocio.

Por lo tanto:

* `Role = Teacher` no sustituye `Teacher`.
* Asignar el rol `Teacher` no crea un `TeacherId`.
* Revocar el rol `Teacher` no elimina el perfil docente.
* Los datos profesionales, laborales y académicos del personal no pertenecen a `IdentityService`.

### SchoolControlService

`SchoolControlService` es propietario de la información escolar del alumno.

Entre sus responsabilidades se encuentran la matrícula, inscripción, reinscripción, situación escolar e historial académico.

`IdentityService` puede administrar el rol de acceso `Student`, pero no es autoridad para determinar por sí mismo que una persona es un estudiante válido.

La definición funcional de SIA establece que la condición de alumno debe estar respaldada por la validación del dominio propietario antes de conceder los privilegios correspondientes.

Esta integración todavía no implica que `IdentityService` pueda consultar directamente `SIA_SchoolControlDb`.

### AcademicService

`AcademicService` es propietario de la estructura académica institucional.

`IdentityService` no administra programas educativos, planes de estudio, materias ni periodos académicos.

Los roles tampoco deben codificar combinaciones específicas de estos conceptos.

Por ejemplo, debe utilizarse un rol general como:

`Teacher`

y no roles como:

`Teacher_ISC_Morning_2026`

Los detalles de programa, materia, grupo, turno o periodo pertenecen a las reglas de los dominios correspondientes.

### SchedulingService

`SchedulingService` es propietario de la planeación académica operativa.

`IdentityService` puede determinar si una cuenta tiene autorización general para utilizar funcionalidades relacionadas con Scheduling, pero no decide:

* Qué grupo tiene asignado un docente.
* Qué carga académica tiene.
* Qué horario debe cumplir.
* Qué materia imparte.
* En qué aula participa.

Estas decisiones pertenecen a `SchedulingService`.

## Reglas de creación y evolución de cuentas

### Personal institucional

La definición funcional establece que el personal institucional obtiene su cuenta mediante una acción administrativa autorizada.

Conceptualmente:

Administrador autorizado
→ crea cuenta institucional
→ se crea o vincula el perfil de negocio correspondiente
→ se asigna el rol autorizado
→ primer acceso

`IdentityService` es responsable de la cuenta y del rol.

El servicio de negocio correspondiente continúa siendo responsable del perfil.

### Alumnos

La definición funcional establece que un alumno puede iniciar mediante registro libre, pero la existencia de una cuenta no debe otorgar automáticamente privilegios de estudiante.

Conceptualmente:

Registro de cuenta
→ vinculación con matrícula
→ validación por el dominio escolar
→ asignación del rol `Student` cuando la identidad académica sea válida

La matrícula y su validación no pertenecen a `IdentityService`.

El flujo completo de autoregistro y validación de alumno puede evolucionar independientemente de las responsabilidades ya establecidas en este documento.

## Cuenta sin roles

Una cuenta puede existir sin roles.

No es necesario crear un rol artificial como `Pending` para representar una cuenta que todavía no tiene privilegios institucionales.

Conceptualmente:

`User activo + Roles = []`

significa que existe una cuenta válida, pero no posee capacidades institucionales protegidas derivadas de roles.

Ningún usuario puede autoasignarse roles privilegiados.

## Eventos que publica actualmente

Los eventos de integración actuales utilizan versión `v1`.

### User

* `UserCreatedIntegrationEvent.v1`
* `UserRoleAssignedIntegrationEvent.v1`
* `UserRoleRevokedIntegrationEvent.v1`
* `PasswordChangedIntegrationEvent.v1`

Estos eventos son publicados mediante el patrón Outbox y permiten que otros servicios reaccionen a cambios relevantes de identidad sin acceder directamente a `SIA_IdentityDb`.

## Eventos que consume

Actualmente `IdentityService` no tiene consumidores de eventos de integración registrados.

Cuando una integración futura requiera reaccionar ante información de otro dominio, deberá realizarse mediante contratos públicos, eventos de integración o mecanismos autorizados de comunicación.

No deberá sustituirse esa integración por acceso directo a una base de datos externa.

## Reglas críticas

* `IdentityService` es el único propietario de `SIA_IdentityDb`.
* Ningún otro servicio puede acceder directamente a esta base de datos.
* `IdentityService` no puede acceder directamente a bases de datos pertenecientes a otros servicios.
* `User`, `Role`, `Permission`, `UserRole`, `RolePermission` y `RefreshToken` son entidades internas del dominio y no deben compartirse directamente con otros servicios.
* La comunicación externa se realiza mediante contratos o eventos de integración.
* Un `User` pertenece a un único tenant.
* `TenantId` no convierte a `IdentityService` en propietario del tenant.
* `UserId` identifica una cuenta de acceso y no sustituye identificadores de entidades de negocio.
* Los roles representan autorización actual y no histórico institucional.
* Asignar un rol no crea una entidad de negocio.
* Revocar un rol no elimina una entidad de negocio ni su histórico.
* Una cuenta puede existir sin roles.
* Ningún usuario puede autoasignarse privilegios institucionales.
* Los roles protegidos deben surgir de una autoridad o validación autorizada.
* El frontend no es autoridad para cambiar `TenantId`, roles o permisos.
* La información académica, laboral, escolar y administrativa permanece en sus servicios propietarios.
