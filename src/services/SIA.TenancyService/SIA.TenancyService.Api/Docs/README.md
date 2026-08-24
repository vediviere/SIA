# SIA.TenancyService

## Responsabilidad

`TenancyService` es el servicio propietario de las instituciones que utilizan SIA y de la información necesaria para identificar el contexto institucional de una operación.

Administra el `TenantId`, el código institucional, el dominio de correo permitido y el estado de cada institución.

`TenancyService` es la autoridad sobre:

- `Tenant`.
- Instituciones.
- Identificadores institucionales.
- Configuración básica propia del tenant.
- Estado de los tenants.
- Información necesaria para identificar el contexto institucional.

Los demás servicios deberán utilizar el identificador del tenant como referencia de contexto y no deberán asumir la propiedad de la información administrada por `TenancyService`.

---

## Lo que sí hace

Actualmente `TenancyService`:

- Administra la identidad técnica de una institución mediante `TenantId`.
- Mantiene un `InstituteCode` público y único por institución.
- Mantiene el nombre de la institución.
- Mantiene el dominio de correo institucional permitido.
- Mantiene el estado activo o inactivo del tenant.
- Resuelve un `InstituteCode` válido a su `TenantId`.
- Valida que el correo corresponda al dominio configurado.
- Impide resolver instituciones inexistentes o inactivas.

## Lo que no hace

`TenancyService` no es responsable de:

- Crear cuentas de usuario.
- Administrar contraseñas, sesiones o tokens.
- Administrar roles o permisos.
- Crear perfiles docentes o administrativos.
- Crear estudiantes.
- Administrar matrículas.
- Validar la situación escolar de un alumno.
- Administrar programas educativos o planes de estudio.
- Administrar grupos, horarios o calificaciones.
- Asignar automáticamente el rol `Student`.
- Acceder directamente a bases de datos de otros servicios.

Estas responsabilidades pertenecen a sus respectivos servicios.

---

## Base de datos

La base de datos propietaria es:

`SIA_TenancyDb`

Solo `TenancyService` puede leer y escribir directamente sobre esta base.

## Entidad principal

### Tenant

`Tenant` representa el contexto institucional dentro del cual se almacenan y procesan los datos correspondientes a una institución.

Cada tenant deberá contar con un identificador único.

El `TenantId` será utilizado por los demás servicios para mantener el aislamiento de la información correspondiente a cada institución.

Mantiene:

- `TenantId`.
- `InstituteCode`.
- Nombre.
- Dominio de correo.
- Estado activo o inactivo.
- Fecha de creación.
- Fecha de actualización.

`InstituteCode` se normaliza a mayúsculas y el dominio de correo se normaliza a minúsculas.

## Resolución institucional

El endpoint actual es:

`POST /api/tenants/resolve`

Recibe:

- `InstituteCode`.
- Correo institucional.

Cuando los datos son válidos, responde:

- `TenantId`.
- `InstituteCode` normalizado.

Este endpoint representa una consulta síncrona controlada. No modifica el tenant ni inicia un proceso distribuido.

## Relación con IdentityService

El frontend puede proporcionar `InstituteCode`, pero nunca controla directamente `TenantId`.

El flujo acordado es:

1. IdentityService recibe el autorregistro.
2. IdentityService consulta a TenancyService.
3. TenancyService valida el código, el estado y el dominio del correo.
4. TenancyService devuelve el `TenantId`.
5. IdentityService crea la cuenta utilizando el `TenantId` validado.
6. La cuenta se crea sin roles.
7. SchoolControl validará posteriormente la matrícula antes de asignar `Student`.

IdentityService no consulta directamente `SIA_TenancyDb`.

## Eventos que publica

Actualmente no publica eventos.

Cuando se implemente la creación, actualización, activación o desactivación de instituciones, esos cambios deberán publicar eventos versionados mediante Outbox.

## Eventos que consume

Actualmente no consume eventos.

## Pendiente para la continuación de TenancyService

- Alta administrativa de instituciones.
- Actualización de instituciones.
- Activación y desactivación.
- Administración de campus o planteles.
- Configuraciones adicionales por tenant.
- Múltiples dominios institucionales cuando exista el requerimiento.
- Auditoría.
- Outbox y eventos de integración.
- Autorización de operaciones administrativas.

## Reglas críticas

- `TenantId` nunca queda bajo control libre del frontend.
- `InstituteCode` debe ser único.
- Solo una institución activa puede resolverse.
- El correo debe pertenecer al dominio configurado.
- Ningún servicio accede directamente a `SIA_TenancyDb`.
- TenancyService no comparte su entidad `Tenant`.
- La comunicación externa se realiza mediante contratos públicos.
- PublicGateway no contiene la lógica de resolución institucional.
- IdentityService podrá utilizar información institucional mediante contratos de integración permitidos.