# Puertos locales oficiales de SIA

## Objetivo

Este documento define la asignación oficial de puertos locales para los Gateways, BFFs y APIs de servicios de SIA.

El objetivo es evitar colisiones al ejecutar varios proyectos simultáneamente durante desarrollo, pruebas, integración y debuggeo.

La asignación definida aquí debe mantenerse sincronizada con los archivos:

`Properties/launchSettings.json`

de cada proyecto ejecutable.

---

# 1. Convención general

Cada proyecto ejecutable utiliza dos puertos locales:

* Primer puerto: HTTP.
* Segundo puerto: HTTPS.

Ejemplo:

```text
HTTP  = http://localhost:7240
HTTPS = https://localhost:7241
```

Los proyectos se agrupan por rangos para facilitar su identificación.

| Rango       | Tipo              |
| ----------- | ----------------- |
| 7000        | Gateways          |
| 7100        | BFFs              |
| 7200 - 7300 | APIs de servicios |

Entre proyectos se utiliza un salto de 10 puertos.

Esto deja espacio disponible entre asignaciones para futuras necesidades sin romper inmediatamente la convención existente.

---

# 2. Gateways

| Proyecto               | HTTP | HTTPS |
| ---------------------- | ---: | ----: |
| SIA.PublicGateway      | 7000 |  7001 |
| SIA.IntegrationGateway | 7010 |  7011 |

## URLs

### SIA.PublicGateway

```text
http://localhost:7000
https://localhost:7001
```

### SIA.IntegrationGateway

```text
http://localhost:7010
https://localhost:7011
```

---

# 3. BFFs

| Proyecto       | HTTP | HTTPS |
| -------------- | ---: | ----: |
| SIA.AdminBff   | 7100 |  7101 |
| SIA.TeacherBff | 7110 |  7111 |
| SIA.StudentBff | 7120 |  7121 |
| SIA.MobileBff  | 7130 |  7131 |

## URLs

### SIA.AdminBff

```text
http://localhost:7100
https://localhost:7101
```

### SIA.TeacherBff

```text
http://localhost:7110
https://localhost:7111
```

### SIA.StudentBff

```text
http://localhost:7120
https://localhost:7121
```

### SIA.MobileBff

```text
http://localhost:7130
https://localhost:7131
```

---

# 4. APIs de servicios

| Proyecto                     | HTTP | HTTPS |
| ---------------------------- | ---: | ----: |
| SIA.IdentityService.Api      | 7200 |  7201 |
| SIA.TenancyService.Api       | 7210 |  7211 |
| SIA.AcademicService.Api      | 7220 |  7221 |
| SIA.AcademicStaffService.Api | 7230 |  7231 |
| SIA.SchedulingService.Api    | 7240 |  7241 |
| SIA.SchoolControlService.Api | 7250 |  7251 |
| SIA.EvaluationService.Api    | 7260 |  7261 |
| SIA.WorkflowService.Api      | 7270 |  7271 |
| SIA.DocumentsService.Api     | 7280 |  7281 |
| SIA.NotificationsService.Api | 7290 |  7291 |
| SIA.ReportingService.Api     | 7300 |  7301 |

## URLs

### SIA.IdentityService.Api

```text
http://localhost:7200
https://localhost:7201
```

### SIA.TenancyService.Api

```text
http://localhost:7210
https://localhost:7211
```

### SIA.AcademicService.Api

```text
http://localhost:7220
https://localhost:7221
```

### SIA.AcademicStaffService.Api

```text
http://localhost:7230
https://localhost:7231
```

### SIA.SchedulingService.Api

```text
http://localhost:7240
https://localhost:7241
```

### SIA.SchoolControlService.Api

```text
http://localhost:7250
https://localhost:7251
```

### SIA.EvaluationService.Api

```text
http://localhost:7260
https://localhost:7261
```

### SIA.WorkflowService.Api

```text
http://localhost:7270
https://localhost:7271
```

### SIA.DocumentsService.Api

```text
http://localhost:7280
https://localhost:7281
```

### SIA.NotificationsService.Api

```text
http://localhost:7290
https://localhost:7291
```

### SIA.ReportingService.Api

```text
http://localhost:7300
https://localhost:7301
```

---

# 5. Resumen completo

| Tipo    | Proyecto                     | HTTP | HTTPS |
| ------- | ---------------------------- | ---: | ----: |
| Gateway | SIA.PublicGateway            | 7000 |  7001 |
| Gateway | SIA.IntegrationGateway       | 7010 |  7011 |
| BFF     | SIA.AdminBff                 | 7100 |  7101 |
| BFF     | SIA.TeacherBff               | 7110 |  7111 |
| BFF     | SIA.StudentBff               | 7120 |  7121 |
| BFF     | SIA.MobileBff                | 7130 |  7131 |
| API     | SIA.IdentityService.Api      | 7200 |  7201 |
| API     | SIA.TenancyService.Api       | 7210 |  7211 |
| API     | SIA.AcademicService.Api      | 7220 |  7221 |
| API     | SIA.AcademicStaffService.Api | 7230 |  7231 |
| API     | SIA.SchedulingService.Api    | 7240 |  7241 |
| API     | SIA.SchoolControlService.Api | 7250 |  7251 |
| API     | SIA.EvaluationService.Api    | 7260 |  7261 |
| API     | SIA.WorkflowService.Api      | 7270 |  7271 |
| API     | SIA.DocumentsService.Api     | 7280 |  7281 |
| API     | SIA.NotificationsService.Api | 7290 |  7291 |
| API     | SIA.ReportingService.Api     | 7300 |  7301 |

---

# 6. Infraestructura fuera de esta asignación

Esta convención aplica exclusivamente a los proyectos ejecutables de SIA durante desarrollo local:

* Gateways.
* BFFs.
* APIs de servicios.

No forma parte de esta asignación la infraestructura externa o remota, incluyendo:

* SQL Server.
* Azure SQL.
* RabbitMQ.
* RabbitMQ Management.
* Otros brokers.
* Servicios externos.
* Recursos de Azure.

Los puertos de infraestructura se administran según la configuración propia de cada componente y no deben mezclarse con la tabla oficial de puertos HTTP/HTTPS de las aplicaciones.

---

# 7. launchSettings.json

Cada proyecto debe reflejar la asignación correspondiente dentro de:

```text
Properties/launchSettings.json
```

El perfil HTTP debe utilizar únicamente el puerto HTTP oficial.

Ejemplo:

```json
"applicationUrl": "http://localhost:7240"
```

El perfil HTTPS debe contener primero HTTPS y posteriormente HTTP.

Ejemplo:

```json
"applicationUrl": "https://localhost:7241;http://localhost:7240"
```

No deben asignarse manualmente otros puertos a un proyecto sin actualizar también este documento.

---

# 8. Ejecución simultánea

La asignación evita colisiones entre los proyectos ejecutables actuales.

Esto permite ejecutar simultáneamente combinaciones como:

```text
SIA.PublicGateway
SIA.AdminBff
SIA.SchedulingService.Api
```

o varios servicios al mismo tiempo durante pruebas de integración.

Cada aplicación debe escuchar exclusivamente en sus puertos asignados.

La existencia de puertos únicos no implica que todos los proyectos deban ejecutarse simultáneamente de forma habitual.

La estrategia recomendada continúa siendo ejecutar únicamente los componentes necesarios para el escenario que se está desarrollando o depurando.

---

# 9. Incorporación de nuevos proyectos

Cuando se agregue un nuevo Gateway, BFF o API:

1. Revisar primero esta tabla.
2. Elegir un puerto libre dentro del rango correspondiente.
3. Mantener el salto de 10 cuando sea posible.
4. Asignar un puerto HTTP.
5. Asignar el puerto siguiente para HTTPS.
6. Actualizar `launchSettings.json`.
7. Actualizar este documento.
8. Verificar que no exista una asignación duplicada.

No se debe aceptar un nuevo proyecto ejecutable con puertos generados aleatoriamente si forma parte permanente de la solución SIA.

---

# 10. Regla oficial

La tabla contenida en este documento representa la asignación oficial de puertos locales de SIA.

Los archivos `launchSettings.json` deben mantenerse alineados con esta tabla.

Si existe una diferencia entre la documentación y la configuración de un proyecto, debe corregirse la inconsistencia antes de integrar el cambio a `develop`.
