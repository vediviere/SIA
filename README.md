# SIA Platform

SIA es una plataforma académica institucional basada en una arquitectura SOA moderna con microservicios por dominio, DDD y comunicación orientada a eventos.

## Principios base

- Cada servicio es dueño de sus datos.
- Ningún servicio accede directamente a la base de datos de otro servicio.
- Los servicios no comparten entidades de dominio.
- La comunicación entre servicios se realiza mediante contratos y eventos.
- Los BFF adaptan la información para cada tipo de portal.
- Los gateways controlan la entrada externa al sistema.
- Las reglas de negocio viven en los servicios de dominio.
