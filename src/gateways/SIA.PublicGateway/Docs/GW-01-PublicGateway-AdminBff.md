# GW-01 — Configurar ruta inicial PublicGateway → AdminBff para PR-001

## Objetivo

Configurar la primera ruta funcional entre `SIA.PublicGateway` y `SIA.AdminBff`, permitiendo que los clientes administrativos accedan al flujo de planificación académica a través del punto de entrada oficial de SIA, sin conocer directamente la dirección interna del BFF.

La implementación mantiene al Gateway como un componente exclusivamente de infraestructura, sin incorporar reglas de negocio.


## Arquitectura involucrada

El flujo configurado es:


Cliente / Frontend
       |
       | HTTPS
       | Authorization: Bearer <JWT>
       | X-Correlation-Id: <Guid>
       v
SIA.PublicGateway
https://localhost:7011
       |
       | YARP Reverse Proxy
       v
SIA.AdminBff
https://localhost:7101
       |
       +----------------------+
       |                      |
       v                      v
SIA.AcademicService    SIA.SchedulingService