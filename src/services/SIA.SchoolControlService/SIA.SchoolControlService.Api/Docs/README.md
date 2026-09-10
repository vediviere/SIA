
# Descripción
El servicio deberá administrar la información necesaria para identificar académicamente a un estudiante y consultar su situación escolar, sin asumir responsabilidades propias de autenticación, identidad digital o estructura académica.

## Responsabilidad

SchoolControlService es el servicio propietario de la información de negocio correspondiente a los estudiantes y al control escolar de la institución.

Su responsabilidad principal es administrar la información académica y escolar asociada al estudiante, incluyendo su matrícula institucional, su relación con un programa educativo y la información necesaria para representar su trayectoria escolar.

Las entidades administradas por este servicio representan información de control escolar y no sustituyen las cuentas, credenciales, roles o permisos administrados por IdentityService.

---

## Lo que sí hace

Actualmente SchoolControlService:

- Crea, consulta, actualiza, activa y desactiva estudiantes.
- Administra la matrícula institucional del estudiante.
- Administra la información de identificación escolar necesaria para el estudiante.
- Mantiene la relación del estudiante con un programa educativo mediante su identificador correspondiente.
- Administra la información necesaria para representar la trayectoria escolar del estudiante.
- Administra el historial académico del estudiante.
- Conserva información relacionada con materias cursadas por el estudiante.
- Conserva información relacionada con resultados académicos cuando corresponda.
- Permite consultar información de control escolar necesaria para otros procesos.
- Mantiene el contexto TenantId de la información escolar.
- Expone contratos públicos para que otros servicios puedan consultar o reaccionar a información de control escolar.
- Genera eventos de integración cuando las entidades administradas por el servicio requieran notificar cambios a otros dominios.

---

## Lo que no hace

SchoolControlService no es responsable de:

- Crear cuentas de usuario.
- Administrar contraseñas.
- Administrar autenticación.
- Administrar tokens.
- Administrar roles.
- Administrar permisos.
- Administrar claims.
- Resolver directamente el TenantId.
- Crear o administrar instituciones.
- Crear o administrar programas educativos.
- Crear o administrar planes de estudio.
- Crear o administrar materias.
- Definir prerrequisitos académicos entre materias.
- Crear o administrar periodos académicos.
- Crear grupos.
- Crear o publicar oferta académica.
- Asignar docentes.
- Construir horarios.
- Administrar aulas.
- Administrar cargas académicas.
- Definir la estructura académica de los planes de estudio.
- Acceder directamente a bases de datos pertenecientes a otros servicios.

Estas responsabilidades pertenecen a sus respectivos dominios.s

---		

## Límites con IdentityService

`IdentityService` es responsable de la identidad digital de los usuarios.

Entre sus responsabilidades se encuentran:

- Cuentas de usuario.
- Autenticación.
- Contraseñas.
- Tokens.
- Roles.
- Permisos.
- Claims.
- Autorización.

`SchoolControlService` administra la identidad escolar del estudiante.

Por lo tanto:
```
IdentityService
    │
    └── Cuenta / identidad digital

SchoolControlService
    │
    └── Student / identidad escolar
```
La existencia de un estudiante no implica automáticamente la existencia de una cuenta de usuario.

De igual manera, la existencia de una cuenta de usuario no sustituye el registro académico del estudiante.