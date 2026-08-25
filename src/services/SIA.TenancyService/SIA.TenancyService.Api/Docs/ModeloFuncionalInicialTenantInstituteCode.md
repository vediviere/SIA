# TEN-02 — Definir modelo funcional inicial de Tenant e InstituteCode

## Descripción

`TenancyService` es el servicio responsable de administrar las instituciones que utilizan SIA y su contexto de tenant.

Una institución representa una organización que utiliza el sistema bajo un contexto aislado.

Cada institución deberá contar con un identificador interno: `TenantId`

y con un código institucional utilizado para su identificación administrativa y resolución inicial: `InstituteCode`

La relación conceptual será:
```
InstituteCode
      ↓
Institution / Tenant
      ↓
TenantId
```
---

### Tenant / Institution

Un Tenant representa el contexto institucional de una organización dentro de SIA.

La institución tendrá información mínima para permitir su identificación y determinar si puede utilizar los servicios del sistema.

Conceptualmente:
```
Tenant
 │
 ├── TenantId
 ├── InstituteCode
 ├── Name
 └── IsActive

 ```
---

### TenantId

`TenantId` es el identificador interno y único del tenant dentro de SIA.

Características:

- Identifica de manera única a una institución dentro de SIA.
- Es administrado por TenancyService.
- No debe ser elegido por el frontend.
- No debe derivarse directamente de InstituteCode.
- Es el identificador utilizado para establecer el contexto institucional en los servicios que soporten multi-tenancy.

El TenantId deberá permanecer independiente del código administrativo de la institución.

---

## InstituteCode

`InstituteCode` es un código definido y administrado por SIA para identificar una institución de manera práctica.

No se considera un identificador oficial del TecNM, aunque puede coincidir con códigos utilizados institucionalmente.

Por ejemplo:

Institución: Instituto Tecnológico de Venustiano Carranza

InstituteCode: ITSVC

El hecho de que ITSVC pueda coincidir con un código utilizado por el TecNM no implica que SIA dependa de un catálogo oficial externo para su generación o administración.

InstituteCode pertenece al contexto institucional administrado por TenancyService.

---

## Naturaleza de InstituteCode

`InstituteCode` será definido por SIA y podrá ser asignado administrativamente al registrar una institución.

No deberá utilizarse como sustituto de `TenantId`.

La relación será:
```
InstituteCode = ITSVC
        ↓
Instituto Tecnológico de Venustiano Carranza
        ↓
TenantId = {identificador interno}
```

El código sirve para localizar el contexto institucional.

El TenantId identifica internamente dicho contexto.

---

## Modificación de InstituteCode

`InstituteCode` podrá modificarse administrativamente.

La modificación no deberá crear un nuevo tenant ni cambiar el TenantId de la institución.

Ejemplo:

Antes:

InstituteCode = ITVC
TenantId = T1

Después:

InstituteCode = ITSVC
TenantId = T1

El tenant continúa siendo el mismo.

La modificación deberá mantener las reglas de unicidad establecidas para `InstituteCode`.

No deberá existir más de una institución activa o registrada con el mismo InstituteCode dentro del ámbito definido para SIA.

---

## Unicidad de InstituteCode

`InstituteCode` deberá ser único dentro de SIA.

No será válido registrar:

Tenant A
InstituteCode = ITSVC

Tenant B
InstituteCode = ITSVC

La unicidad deberá mantenerse tanto durante el alta como durante cualquier modificación administrativa del código.

Antes de modificar un `InstituteCode`, TenancyService deberá verificar que el nuevo valor no se encuentre asociado a otra institución.

--- 

## Normalización de `InstituteCode`

El código deberá normalizarse antes de almacenarse y antes de utilizarse para realizar una búsqueda.

La normalización deberá contemplar como mínimo:

Eliminación de espacios al inicio y al final.
Conversión a una representación consistente para evitar diferencias artificiales.

La comparación deberá ser independiente de mayúsculas y minúsculas.

Por ejemplo, los siguientes valores deberán representar el mismo código:

- ITSVC
- itsvc
- Itsvc
- ITSVC
- ITSVC 

La representación almacenada deberá utilizar una forma normalizada y consistente.

Conceptualmente:
```
Entrada: " itsvc "

        ↓
Normalización: "ITSVC"

        ↓
Búsqueda en TenancyService
```
---

## Estado de la institución

La institución deberá contar con un estado que permita determinar si puede continuar utilizando SIA.

Conceptualmente:
```
Institution
    │
    ├── Active
    └── Inactive
```
---

## Institución activa
Una institución activa puede utilizar los servicios correspondientes a su tenant, de acuerdo con las reglas de cada servicio.

## Institución inactiva

Una institución inactiva representa una institución que ya no tiene habilitado el servicio de SIA.

Esto puede ocurrir, por ejemplo, cuando la institución ya no renueva el servicio contratado.

La inactividad no implica necesariamente eliminar el tenant ni su información histórica.

La institución deberá conservar su identidad y datos administrativos para mantener la integridad de la información existente.

---

## Alta institucional

En el contexto de esta tarea, el alta institucional representa el registro inicial de una institución en SIA y la habilitación de su contexto institucional.

También se considera que una institución que dejó de renovar el servicio puede quedar inactiva.

Por lo tanto:
```
Institución activa
        ↓
Puede utilizar SIA

Institución inactiva
        ↓
No puede continuar nuevos procesos institucionales
```

Una institución inactiva no deberá poder iniciar nuevamente procesos como si se tratara de una institución activa.

La reactivación deberá ser una acción administrativa explícita y no deberá ocurrir automáticamente como consecuencia de recibir un `InstituteCode`.

---

## Relación con IdentityService

`IdentityService` requiere conocer el contexto institucional para realizar operaciones relacionadas con usuarios y autenticación.

Sin embargo, `IdentityService` no será propietario de la información de instituciones.

La autoridad sobre:

TenantId.
InstituteCode.
Nombre institucional.
Estado de la institución.

pertenece a `TenancyService`.

`IdentityService` deberá utilizar mecanismos de integración autorizados para obtener o resolver esta información.

No deberá mantener un catálogo independiente y autoritativo de instituciones.

Conceptualmente:
```
IdentityService
       │
       │ solicita / consume información institucional
       ▼
TenancyService
       │
       ├── InstituteCode
       ├── TenantId
       ├── Name
       └── Status
```
La existencia de una referencia a `TenantId` dentro de IdentityService no implica propiedad sobre el tenant.

---

## Fuera de alcance

Esta tarea NO contempla:

- Autenticación.
- Contraseñas.
- Usuarios.
- Roles.
- Permisos.
- Claims.
- Administración de identidad.
- Catálogos académicos.
- Programas educativos.
- Planes de estudio.
- Materias.
- Periodos académicos.
- Inscripciones.
- Oferta académica.
- Horarios.
- Indicadores.
- Procesos operativos de otros servicios.

Estos conceptos pertenecen a otros dominios.

---

## Reglas funcionales
- `TenancyService` es la autoridad sobre las instituciones y tenants de SIA.
- Cada institución deberá tener un `TenantId` propio.
- `TenantId` será administrado por TenancyService.
- `InstituteCode` será un código definido y administrado por SIA.
- `InstituteCode` no será considerado un código oficial del TecNM aunque pueda coincidir con uno.
- `InstituteCode` deberá ser único dentro de SIA.
- `InstituteCode` podrá modificarse administrativamente.
- La modificación de `InstituteCode` no deberá modificar el `TenantId`.
- La unicidad de `InstituteCode` deberá mantenerse después de cualquier modificación.
- `InstituteCode` deberá normalizarse antes de almacenarse y buscarse.
- La comparación de `InstituteCode` deberá ser independiente de mayúsculas y minúsculas.
- Un `InstituteCode` inexistente no deberá producir un `TenantId`.
- Una institución inactiva no deberá poder continuar nuevos procesos institucionales que requieran un tenant activo.
- La reactivación de una institución deberá ser una acción administrativa explícita.
- El frontend no deberá controlar directamente el `TenantId`.
- La resolución de `InstituteCode` a `TenantId` deberá realizarse mediante `TenancyService`.
- `TenancyService` será propietario de `SIA_TenancyDb`.
- Ningún otro servicio deberá acceder directamente a `SIA_TenancyDb`.
- IdentityService no deberá mantener un catálogo autoritativo independiente de instituciones.
- Las referencias a `TenantId` en otros servicios no representan ownership sobre el tenant.
- La información institucional deberá integrarse entre servicios mediante contratos o mecanismos de integración autorizados.
- El modelo V1 deberá contener como mínimo `TenantId`, `InstituteCode`, `Name`, `EmailDomain` y `IsActive`.