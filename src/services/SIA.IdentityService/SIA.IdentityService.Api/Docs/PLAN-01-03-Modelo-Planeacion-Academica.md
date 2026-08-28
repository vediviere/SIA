# 1. Ingresar al SIA

Servicio propietario: `IdentityService`

Responsabilidad:

- Autenticar al usuario.
- Validar sus credenciales.
- Determinar la identidad del usuario autenticado.
- Generar el contexto de autenticación necesario para que los demás servicios puedan identificar al usuario.

Información requerida:

- Credenciales del usuario.
- Información de autenticación administrada por IdentityService.

Información resultante:

- `UserId`.
- Claims/contexto de identidad necesarios para autorización.
- `TenantId`, cuando forme parte del contexto de identidad establecido por la arquitectura.

Tipo: Consulta/autenticación.

---

## `AdminBff`:

- Puede orquestar la interacción con el frontend.
- No autentica directamente al usuario.
- No debe generar ni seleccionar manualmente un `TenantId`.

Eventos: No se requiere un evento de integración para continuar este paso del flujo.

