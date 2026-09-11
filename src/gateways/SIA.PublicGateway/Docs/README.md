

# SIA.PublicGateway

`SIA.PublicGateway` es el punto de entrada público para las solicitudes que deben acceder a los BFF y servicios internos de la plataforma SIA.

Está implementado con ASP.NET Core y utiliza YARP como Reverse Proxy.

## Responsabilidad

PublicGateway es responsable de:

- Exponer las rutas públicas autorizadas de la plataforma.
- Enrutar solicitudes hacia los BFF correspondientes.
- Ocultar al cliente las direcciones internas de los servicios.
- Preservar headers necesarios para el contexto de la solicitud, como `Authorization` y `X-Correlation-Id`.
- Traducir fallos de conectividad del proxy a respuestas HTTP de Gateway mediante YARP.

PublicGateway no contiene reglas de negocio.

Las reglas y casos de uso pertenecen a los servicios de dominio correspondientes.

## Arquitectura

El primer flujo configurado es:


Frontend / Cliente
        |
        v
SIA.PublicGateway
        |
        | YARP
        v
SIA.AdminBff
        |
        +---------------------+
        |                     |
        v                     v
AcademicService        SchedulingService