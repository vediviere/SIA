
## Descripción

SchoolControlService deberá ser quien pueda confirmar que existe un estudiante válido antes de otorgar o habilitar el acceso correspondiente.

El modelo debe respetar la separación acordada:

```
UserId
    ↓
Cuenta de acceso en IdentityService

StudentId
    ↓
Alumno dentro de SchoolControlService

Matrícula
    ↓
Identificador escolar/institucional del alumno

```
---

## Student

`Student` representa al alumno dentro del contexto de control escolar de una institución.

La existencia de un `Student` significa que la institución reconoce a una persona como alumno dentro de su contexto académico y escolar.

Student no representa una cuenta de acceso, una identidad digital ni un rol de usuario.

Conceptualmente:
```
Student
    │
    ├── StudentId
    ├── TenantId
    ├── Matrícula
    ├── Estado
    └── Información mínima de identificación escolar

```
`SchoolControlService` es responsable de administrar esta entidad.

---

## StudentId

`StudentId` es el identificador propio del alumno dentro de `SchoolControlService`.

Debe ser independiente de cualquier identificador utilizado por IdentityService.

Por lo tanto:

StudentId  ≠  UserId

`StudentId` identifica al alumno dentro del dominio de control escolar.

`UserId` identifica la cuenta de acceso dentro de IdentityService.

El valor de `StudentId` no deberá depender de la matrícula.

La matrícula puede utilizarse para localizar al alumno, pero no sustituye al identificador interno de la entidad Student.

---

## TenantId

Cada Student pertenece a un único tenant.

Conceptualmente:
```
Tenant
   │
   └── TenantId
          │
          └── Student

```
`TenantId` identifica la institución a la que pertenece el alumno dentro de SIA.

SchoolControlService deberá conservar el `TenantId` necesario para mantener el aislamiento institucional de sus datos.

`TenantId` es administrado por `TenancyService` y no es propiedad de `SchoolControlService`.

`SchoolControlService` no deberá generar ni modificar la identidad institucional representada por `TenantId`.

---

## Matrícula

La matrícula es el identificador escolar o institucional utilizado por una institución para reconocer a un alumno dentro de su contexto académico.

Por ejemplo:
```
Student
    │
    ├── StudentId
    │       = identificador interno de SIA
    │
    └── Matrícula
            = identificador escolar del alumno
```
La `matrícula` es un dato de negocio de `SchoolControlService`.

No representa una cuenta de acceso y no sustituye a `UserId`.

Diferencia entre `StudentId`, `UserId` y `matrícula`

Los tres conceptos cumplen funciones diferentes:
```
UserId
    ↓
Identifica una cuenta de acceso
IdentityService


StudentId
    ↓
Identifica un alumno
SchoolControlService


Matrícula
    ↓
Identifica al alumno dentro del contexto escolar
SchoolControlService

```
Los tres identificadores pueden relacionarse conceptualmente, pero no representan la misma entidad.

---

## Información mínima de identificación escolar

Para V1, `Student` deberá contar con la información mínima necesaria para identificarlo dentro de la institución.

Como mínimo deberá contemplarse:
```
Student
    │
    ├── StudentId
    ├── TenantId
    ├── Matrícula
    ├── Nombre
    ├── Apellidos
    └── Status
```
La información exacta de identificación podrá evolucionar posteriormente, pero V1 deberá permitir reconocer de manera suficiente al alumno dentro del contexto institucional.

La información de autenticación, contraseñas, tokens y demás datos de identidad digital no pertenecen a `Student`.

---

## Unicidad de matrícula

La matrícula deberá ser única dentro del contexto institucional correspondiente.

La misma matrícula podrá existir en diferentes tenants si las instituciones utilizan esquemas independientes de identificación.

Por ejemplo:
```
Tenant A
    Matrícula = 123456


Tenant B
    Matrícula = 123456
```
Esto no representa una duplicidad, debido a que pertenecen a instituciones diferentes.

Por lo tanto, la combinación funcional deberá considerarse única mediante:

```
TenantId + Matrícula

No deberá existir:

Tenant A
    Student 1
    Matrícula = 123456

Tenant A
    Student 2
    Matrícula = 123456
```
La matrícula no deberá asignarse simultáneamente a dos estudiantes activos o históricos que representen al mismo alumno dentro del mismo `tenant`.

---

## Estados de Student

Pendiente cuando se termine de definir el flujo de vida del alumno dentro del contexto escolar.