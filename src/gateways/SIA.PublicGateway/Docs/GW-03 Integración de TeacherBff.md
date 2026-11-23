# GW-03 — Integración de TeacherBff en PublicGateway

## 1. Objetivo

Integrar `SIA.TeacherBff` al sistema de enrutamiento de `SIA.PublicGateway`, permitiendo que las solicitudes del módulo docente ingresen por el Gateway y sean dirigidas al BFF correspondiente.

La integración debe conservar las responsabilidades de seguridad del Gateway, incluyendo autenticación JWT, propagación del contexto de solicitud y protección de los servicios internos detrás de sus respectivos BFF.

**Tarea relacionada:** `SIA-GW-03 — Integrar TeacherBff en PublicGateway`.

## 2. Arquitectura del recorrido

El recorrido implementado es:

```text
Frontend Docente
       |
       v
SIA.PublicGateway
       |
       |  Validación JWT
       |  CorrelationId
       |  Autorización
       |
       v
    YARP Proxy
       |
       |  /api/teacher/{**catch-all}
       |
       v
SIA.TeacherBff
       |
       v
Endpoints del BFF
```

El Gateway funciona como punto de entrada público. TeacherBff permanece detrás de él y no se agregan rutas directas desde PublicGateway hacia los microservicios internos.

## 3. Configuración de YARP

La configuración se agregó en:

`appsettings.Development.json`

### 3.1. Route

Se agregó la ruta `teacher-bff`:

```json
"teacher-bff": {
  "ClusterId": "teacher-bff",
  "AuthorizationPolicy": "Authenticated",
  "Match": {
    "Path": "/api/teacher/{**catch-all}"
  },
  "Transforms": [
    {
      "PathRemovePrefix": "/api/teacher"
    }
  ]
}
```

Características:

- `ClusterId`: identifica el cluster de destino.
- `AuthorizationPolicy`: requiere un usuario autenticado.
- `Match.Path`: define el prefijo público para las solicitudes docentes.
- `PathRemovePrefix`: elimina `/api/teacher` antes de reenviar la solicitud al BFF.

### 3.2. Cluster

Se agregó el cluster correspondiente:

```json
"teacher-bff": {
  "Destinations": {
    "teacher-bff-primary": {
      "Address": "https://localhost:7111/"
    }
  }
}
```

La dirección corresponde al perfil HTTPS de ejecución local de TeacherBff.

### 3.3. Transformación de rutas

La transformación permite conservar un prefijo público uniforme sin modificar las rutas internas del BFF.

Ejemplo:

| Solicitud pública | Solicitud reenviada |
|---|---|
| `/api/teacher/health` | `/health` |
| `/api/teacher/profile` | `/profile` |

La configuración actual corresponde al ambiente `Development`.

Para otros ambientes deberán proporcionarse las rutas y direcciones de destino mediante la configuración correspondiente a cada ambiente. No se debe utilizar la dirección localhost de desarrollo en ambientes desplegados.

## 4. Autenticación y propagación de encabezados

La ruta utiliza la política `Authenticated`.

El Gateway mantiene su configuración existente de autenticación JWT, que valida los parámetros de firma, emisor, audiencia y vigencia, además del claim `tenant_id`.

No fue necesario agregar transforms específicos para los encabezados:

- `Authorization`
- `X-Correlation-Id`

YARP conserva estos encabezados en el reenvío de solicitudes con la configuración utilizada.

TeacherBff también ejecuta su propio pipeline de autenticación y correlación.

## 5. Pruebas de integración

Se modificó:

`SIA.PublicGateway.Tests/Integration/GatewayRouteTests.cs`

Se agregó un backend TeacherBff simulado mediante una aplicación HTTP local con puerto dinámico.

El backend simulado permite capturar la solicitud recibida y verificar el comportamiento del Gateway sin depender de una instancia real de TeacherBff durante las pruebas automatizadas.

### 5.1. Escenarios cubiertos

| Escenario | Resultado esperado |
|---|---|
| Solicitud sin token JWT | `401 Unauthorized` |
| Solicitud con JWT válido | Reenvío hacia TeacherBff |
| Propagación de Authorization | Se conserva el encabezado |
| Propagación de CorrelationId | Se conserva el identificador |
| Transformación de ruta | `/api/teacher/profile` se reenvía como `/profile` |
| Ruta desconocida | `404 Not Found`, sin reenvío a los BFF simulados |

### 5.2. Configuración de Rate Limiting Tests

Se modificó:

`SIA.PublicGateway.Tests/Integration/RateLimitingTests.cs`

Se agregó la configuración JWT requerida por `WebApplicationFactory` para iniciar el Gateway durante las pruebas.

Se reutilizan las constantes y la clave de firma definidas en la clase de pruebas.

## 6. Validación automatizada

Proyecto ejecutado:

```powershell
dotnet test src/gateways/SIA.PublicGateway.Tests/SIA.PublicGateway.Tests.csproj
```

Resultado:

```text
Resumen de pruebas:
Total:     10
Correctas: 10
Errores:   0
Omitidas:  0
```

También se ejecutó:

```powershell
git diff --check
```

Sin errores de formato o espacios en blanco.

## 7. Validación manual

Se ejecutaron `SIA.PublicGateway` y `SIA.TeacherBff` simultáneamente desde Visual Studio.

Se configuraron los secretos locales necesarios para la validación JWT en ambos proyectos, utilizando los mismos parámetros de firma que IdentityService.

Las claves de firma se mantienen fuera de los archivos versionados.

### 7.1. Health directo de TeacherBff

Solicitud:

```http
GET https://localhost:7111/health
```

Respuesta observada:

```json
{
  "service": "SIA.TeacherBff",
  "status": "Healthy"
}
```

### 7.2. Solicitud a través de PublicGateway sin token

Solicitud:

```http
GET https://localhost:7001/api/teacher/health
```

Resultado:

```text
401 Unauthorized
```

Esto confirma que la ruta protegida rechaza solicitudes sin autenticación.

### 7.3. Solicitud a través de PublicGateway con JWT válido

Solicitud:

```http
GET https://localhost:7001/api/teacher/health
Authorization: Bearer <JWT>
```

Respuesta observada:

```json
{
  "service": "SIA.TeacherBff",
  "status": "Healthy"
}
```

También se verificó la presencia de `X-Correlation-Id` en la respuesta.

**Resultado:** se comprobó manualmente el recorrido real:

```text
PublicGateway → TeacherBff → Health
```

## 8. Archivos modificados

| Archivo | Cambio |
|---|---|
| `SIA.PublicGateway/appsettings.Development.json` | Ruta y cluster de TeacherBff |
| `SIA.PublicGateway.Tests/Integration/GatewayRouteTests.cs` | Pruebas de integración de la ruta TeacherBff |
| `SIA.PublicGateway.Tests/Integration/RateLimitingTests.cs` | Configuración JWT para ejecución de pruebas |

## 9. Alcance y consideraciones

Esta tarea incorpora la integración de enrutamiento entre PublicGateway y TeacherBff.

No incluye:

- Implementación de nuevos endpoints funcionales de negocio en TeacherBff.
- Modificaciones en los microservicios internos.
- Exposición directa de microservicios a través del Gateway.
- Configuración de despliegue para ambientes distintos de Development.

La configuración de otros ambientes deberá definirse de acuerdo con la infraestructura y el mecanismo de configuración correspondiente.

## 10. Resultado

**GW-03 completada e integrada en `develop`.**

Se verificó:

- Enrutamiento de solicitudes hacia TeacherBff.
- Protección de la ruta mediante JWT.
- Propagación de Authorization y CorrelationId.
- Transformación de rutas mediante YARP.
- Rechazo de rutas desconocidas.
- Pruebas automatizadas: 10/10 correctas.
- Recorrido HTTP real entre PublicGateway y TeacherBff.

La integración deja preparado el punto de entrada del módulo docente a través de PublicGateway.