# PLAN-01 — Paso 1: Ingresar al SIA

## Actividad BPMN

1. Ingresar al SIA

Representa el inicio de la interacción del usuario con el sistema para acceder a la aplicación.

Servicio propietario `IdentityService`

El acceso al SIA es responsabilidad de `IdentityService`, ya que este servicio es el encargado de:

- Autenticación.
- Cuentas de usuario.
- Credenciales.
- Tokens.
- Roles y permisos.

`AdminBff` no autentica directamente al usuario; funciona como punto de entrada para las solicitudes de la aplicación y delega la autenticación al mecanismo correspondiente.

---

## Tipo de interacción

Comando / solicitud de autenticación

```
Conceptualmente:

Usuario
   │
   │ credenciales
   ▼
IdentityService
   │
   │ autenticación
   ▼
Token / contexto autenticado

```
---

## Información requerida

En este paso, `IdentityService` requiere la información necesaria para autenticar la cuenta, por ejemplo:

- Identificador de usuario.
- Credencial correspondiente.
- Información necesaria para el mecanismo de autenticación configurado.

No se debe solicitar todavía TenantId al usuario como mecanismo para seleccionar la institución, ya que TEN-02 estableció que el TenantId no debe ser controlado directamente por el frontend.

---

## TenantId

En este paso no debe confiarse en un `TenantId` proporcionado por el frontend.

El contexto de tenant deberá establecerse mediante el mecanismo institucional definido para SIA y validarse posteriormente contra TenancyService.

Conceptualmente:
```
Usuario
   │
   ▼
IdentityService
   │
   ▼
Contexto autenticado
   │
   └── TenantId validado


La resolución concreta de `InstituteCode` → `TenantId` corresponde a `TenancyService`, no a `IdentityService` ni a `AdminBff`.
```

--- 

## AdminBff

`AdminBff` puede actuar como punto de entrada de la aplicación administrativa, pero no debe convertirse en propietario de la autenticación.

Puede:

- Recibir la solicitud del cliente.
- Propagar el contexto de autenticación.
- Propagar `TenantId` y `CorrelationId` cuando ya hayan sido establecidos y validados.
- Componer información proveniente de diferentes servicios cuando corresponda.

No debe:

- Mantener usuarios.
- Validar contraseñas.
- Crear tokens.
- Mantener un catálogo propio de tenants.
- Consultar directamente bases de datos de otros servicios.

---

## Dependencias

Este paso depende técnicamente de:

`IdentityService` para autenticación.
`TenancyService` para la resolución/validación del contexto institucional cuando corresponda.

No depende directamente de:

- AcademicService
- AcademicStaffService
- SchedulingService
- WorkflowService

---

