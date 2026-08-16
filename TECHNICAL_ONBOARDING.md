# SIA Platform — Onboarding técnico inicial

## 1. Propósito

Este documento es la guía inicial para cualquier desarrollador que se incorpore al proyecto **SIA Platform**.

Su objetivo es permitir que un nuevo integrante entienda rápidamente:

* Qué es SIA.
* Cómo está organizada la solución.
* Cuáles son las reglas arquitectónicas que no debe romper.
* Cómo preparar y compilar el proyecto.
* Cómo ejecutar pruebas.
* Cómo trabajar con Git, Jira y Pull Requests.
* Cómo ejecutar y depurar los servicios.
* Dónde consultar la documentación técnica oficial.

Este documento no reemplaza la documentación especializada. Funciona como **punto de entrada** y dirige al desarrollador hacia los documentos que debe consultar según la tarea que vaya a realizar.

---

# 2. ¿Qué es SIA?

**SIA — Sistema Integral Académico** es una plataforma institucional diseñada para administrar procesos académicos, escolares y administrativos mediante servicios independientes por dominio.

La plataforma utiliza como base:

* SOA moderna.
* Microservicios por dominio.
* Domain-Driven Design (DDD).
* Comunicación orientada a eventos.
* Contratos públicos entre servicios.
* Base de datos propia por servicio.
* Multi-tenancy.
* Outbox/Inbox para comunicación asíncrona cuando corresponde.

La regla principal es:

> Cada servicio es dueño de su dominio y de sus datos.

Un servicio nunca debe acceder directamente a la base de datos de otro servicio.

---

# 3. Arquitectura general

La comunicación conceptual de la plataforma sigue esta estructura:

```text
Cliente
   ↓
Gateway
   ↓
BFF
   ↓
API del servicio propietario
   ↓
Application
   ↓
Domain
   ↓
Infrastructure
   ↓
Base de datos propia
```

Cuando un cambio relevante debe ser conocido por otros dominios:

```text
Servicio propietario
   ↓
Outbox
   ↓
RabbitMQ
   ↓
Consumidor
   ↓
Inbox / procesamiento local
   ↓
Modelo o información local
```

Los servicios no comparten entidades de dominio ni realizan consultas cruzadas entre sus bases.

---

# 4. Estructura principal del repositorio

La solución se organiza aproximadamente de la siguiente manera:

```text
SIA/
│
├── src/
│   ├── gateways/
│   ├── bff/
│   ├── services/
│   └── building-blocks/
│
├── tests/
│
├── docs/
│   ├── architecture/
│   ├── decisions/
│   ├── events/
│   ├── onboarding/
│   └── services/
│
├── deploy/
│
├── SIA.Platform.slnx
├── README.md
├── LOCAL_PORTS.md
└── TEAM_WORK_RULES.md
```

## 4.1 Gateways

Ubicación:

```text
src/gateways/
```

Los Gateways representan puntos de entrada a la plataforma.

Actualmente la solución contempla:

* `SIA.PublicGateway`
* `SIA.IntegrationGateway`

Los Gateways pueden encargarse de aspectos como:

* Enrutamiento.
* Autenticación en la frontera.
* Políticas de entrada.
* Propagación de información técnica.

No deben contener reglas institucionales propias de los dominios.

---

# 5. BFFs

Ubicación:

```text
src/bff/
```

BFF significa **Backend for Frontend**.

SIA mantiene BFF separados de acuerdo con los consumidores principales:

* `SIA.AdminBff`
* `SIA.TeacherBff`
* `SIA.StudentBff`
* `SIA.MobileBff`

Su responsabilidad es adaptar y componer información para las necesidades de cada cliente.

Un BFF no debe convertirse en el lugar donde se implementan reglas importantes del negocio.

---

# 6. Servicios de dominio

Ubicación:

```text
src/services/
```

Cada servicio representa una responsabilidad de negocio independiente.

Entre los servicios actuales se encuentran:

```text
SIA.IdentityService
SIA.TenancyService
SIA.AcademicService
SIA.AcademicStaffService
SIA.SchedulingService
SIA.SchoolControlService
SIA.EvaluationService
SIA.WorkflowService
SIA.DocumentsService
SIA.NotificationsService
SIA.ReportingService
```

Antes de modificar un servicio se debe leer su `README.md` para conocer:

* Su responsabilidad.
* Lo que sí hace.
* Lo que no hace.
* Su base de datos propietaria.
* Sus contratos.
* Sus eventos.
* Sus límites respecto a otros dominios.

---

# 7. Estructura interna de un servicio

Los servicios siguen una estructura común:

```text
SIA.NombreService/
│
├── SIA.NombreService.Api/
├── SIA.NombreService.Application/
├── SIA.NombreService.Contracts/
├── SIA.NombreService.Domain/
├── SIA.NombreService.Infrastructure/
└── SIA.NombreService.Tests/
```

## Api

Expone HTTP y configura el proceso ejecutable.

Aquí pueden existir:

* Controllers.
* Middleware.
* Configuración de autenticación/autorización.
* OpenAPI / Swagger.
* Registro de dependencias.

No debe contener reglas importantes de negocio.

## Application

Coordina casos de uso.

Aquí pueden existir:

* Use Cases.
* Interfaces.
* Queries.
* DTOs internos.
* Validaciones de aplicación.

Application no debe conocer detalles concretos de infraestructura.

## Domain

Contiene el corazón del negocio del servicio.

Aquí pueden existir:

* Entities.
* Value Objects.
* Enums.
* Reglas.
* Excepciones de dominio.

Domain debe mantenerse aislado de tecnologías como Entity Framework, RabbitMQ, MassTransit o proveedores externos.

## Infrastructure

Implementa detalles tecnológicos.

Aquí pueden existir:

* Entity Framework Core.
* DbContext.
* Configuraciones.
* Persistencia.
* Data Stores.
* RabbitMQ / MassTransit.
* Servicios externos.
* Implementaciones de interfaces.

## Contracts

Contiene aquello que el servicio permite compartir públicamente.

Por ejemplo:

* Requests.
* Responses.
* Integration Events.

Los demás servicios nunca deben consumir directamente las entidades de `Domain`.

## Tests

Contiene las pruebas automatizadas propias del servicio.

---

# 8. Building Blocks

Ubicación principal:

```text
src/building-blocks/
```

Los Building Blocks contienen componentes técnicos reutilizables y transversales.

Pueden incluir:

* Abstracciones técnicas.
* Mensajería.
* Observabilidad.
* Manejo común de errores.
* Elementos compartidos de infraestructura.

No deben convertirse en un dominio de negocio común.

Por ejemplo, no deben contener entidades como:

```text
Student
Teacher
Subject
Grade
Enrollment
```

Esas entidades pertenecen a sus respectivos servicios.

---

# 9. Reglas arquitectónicas básicas

Todo desarrollador debe respetar como mínimo las siguientes reglas:

1. Un servicio no accede directamente a la base de datos de otro servicio.
2. Un servicio no referencia el `Domain` de otro servicio.
3. Los servicios se comunican mediante contratos públicos o eventos.
4. Cada servicio es dueño de sus propios datos.
5. Las reglas de negocio permanecen en el servicio propietario.
6. Los Controllers no deben convertirse en contenedores de reglas de negocio.
7. Gateway y BFF no deben contener lógica institucional fuerte.
8. `Domain` no debe depender de `Infrastructure`.
9. Las tecnologías externas deben mantenerse aisladas detrás de abstracciones propias cuando corresponda.
10. No se deben introducir dependencias transversales sin revisar primero su impacto arquitectónico.
11. Los cambios relevantes deben contar con pruebas cuando corresponda.
12. Los cambios importantes de arquitectura deben quedar documentados.

Si una tarea parece requerir romper alguna de estas reglas, no se debe improvisar una excepción. Se debe solicitar revisión técnica.

---

# 10. Preparar el repositorio

## 10.1 Clonar

```bash
git clone https://github.com/vediviere/SIA.git
cd SIA
```

## 10.2 Cambiar a develop

El trabajo normal nace desde `develop`.

```bash
git checkout develop
git pull origin develop
```

No se debe comenzar una nueva tarea desde una rama local desactualizada.

---

# 11. Restaurar la solución

Desde la raíz del repositorio:

```bash
dotnet restore SIA.Platform.slnx
```

También puede realizarse desde Visual Studio al abrir la solución.

---

# 12. Compilar la solución

```bash
dotnet build SIA.Platform.slnx
```

Antes de abrir un Pull Request debe comprobarse que los proyectos afectados compilen correctamente.

Cuando el cambio sea transversal se recomienda compilar la solución completa.

---

# 13. Ejecutar pruebas

Para ejecutar las pruebas disponibles en la solución:

```bash
dotnet test SIA.Platform.slnx
```

También pueden ejecutarse proyectos de pruebas individualmente desde Visual Studio.

Ejemplo conceptual:

```bash
dotnet test ruta/al/proyecto/SIA.NombreService.Tests.csproj
```

Una tarea que incluya criterios de pruebas no debe considerarse terminada únicamente porque el proyecto compile.

---

# 14. Ejecutar una API

Cada API puede ejecutarse desde Visual Studio o mediante `dotnet run`.

Ejemplo general:

```bash
dotnet run --project src/services/SIA.NombreService/SIA.NombreService.Api/SIA.NombreService.Api.csproj
```

Los puertos oficiales no deben asignarse manualmente.

Consultar siempre:

```text
LOCAL_PORTS.md
```

Los archivos:

```text
Properties/launchSettings.json
```

deben mantenerse alineados con la tabla oficial de puertos.

---

# 15. Puertos locales

SIA utiliza rangos separados para facilitar la identificación de proyectos:

```text
7000 → Gateways
7100 → BFFs
7200–7300 → APIs de servicios
```

Ejemplos:

```text
SIA.PublicGateway       HTTP 7000 / HTTPS 7001
SIA.AdminBff            HTTP 7100 / HTTPS 7101
SIA.IdentityService.Api HTTP 7200 / HTTPS 7201
SIA.AcademicService.Api HTTP 7220 / HTTPS 7221
SIA.SchedulingService   HTTP 7240 / HTTPS 7241
```

La fuente oficial siempre es:

```text
LOCAL_PORTS.md
```

SQL Server, Azure SQL, RabbitMQ y demás infraestructura externa no forman parte de esta tabla de puertos HTTP/HTTPS.

---

# 16. Estrategia de debuggeo

No es necesario ejecutar toda SIA para trabajar en una funcionalidad.

Se utilizarán tres escenarios principales.

## 16.1 Servicio aislado

Utilizar cuando la tarea pertenece únicamente a un dominio.

Ejemplo:

```text
AcademicService.Api
```

Es el enfoque recomendado para:

* reglas de negocio;
* casos de uso;
* persistencia;
* endpoints;
* pruebas manuales del servicio.

## 16.2 Varios proyectos seleccionados

Utilizar cuando una funcionalidad necesita dos o más procesos ejecutándose simultáneamente.

Ejemplo:

```text
PublicGateway
AdminBff
IdentityService.Api
```

Visual Studio permite configurar varios proyectos de inicio cuando sea necesario.

No se deben iniciar servicios que no participan en el escenario que se está probando.

## 16.3 Vertical slice

Un **vertical slice** representa un flujo real atravesando los componentes necesarios desde la entrada hasta el resultado.

Ejemplo conceptual:

```text
Cliente
  ↓
Gateway
  ↓
BFF
  ↓
Servicio propietario
  ↓
Base de datos
```

o, si existe comunicación asíncrona:

```text
Servicio A
  ↓
Outbox
  ↓
RabbitMQ
  ↓
Servicio B
  ↓
Inbox
```

Este enfoque debe utilizarse para comprobar integraciones reales entre componentes sin necesidad de levantar toda la plataforma.

---

# 17. Git y ramas

SIA trabaja con tres ramas permanentes:

```text
develop
qa
main
```

Flujo normal:

```text
rama temporal
    ↓
develop
    ↓
qa
    ↓
main
```

## Ramas temporales

Según el tipo de tarea pueden utilizarse:

```text
feature/
bugfix/
refactor/
docs/
test/
chore/
hotfix/
```

Formato:

```text
<tipo>/SIA-<numero>-<descripcion-corta>
```

Ejemplo:

```text
feature/SIA-76-ident-16-autoregistro-alumno
```

La clave Jira debe aparecer en la rama cuando la tarea tenga una actividad asociada.

---

# 18. Reglas de Git

No está permitido:

* Trabajar directamente sobre `main`.
* Trabajar directamente sobre `qa`.
* Implementar una tarea directamente sobre `develop`.
* Hacer push directo a ramas protegidas.
* Mezclar varias funcionalidades no relacionadas dentro de una misma rama.
* Crear ramas permanentes por desarrollador.
* Fusionar cambios importantes sin revisión.

Antes de empezar una tarea:

```bash
git checkout develop
git pull origin develop
```

Después crear la rama correspondiente.

Ejemplo:

```bash
git checkout -b feature/SIA-XX-descripcion
```

---

# 19. Pull Requests

El flujo habitual de una tarea es:

```text
Jira
 ↓
rama temporal
 ↓
implementación
 ↓
pruebas
 ↓
Pull Request
 ↓
revisión
 ↓
develop
```

Antes de solicitar el merge se debe revisar como mínimo:

* ¿Respeta el dominio?
* ¿Accede a datos que no le pertenecen?
* ¿Rompe contratos existentes?
* ¿Requiere un evento?
* ¿Tiene las pruebas necesarias?
* ¿Respeta TenantId?
* ¿Incluye una migración?
* ¿Afecta un Building Block?
* ¿Necesita una decisión arquitectónica?

---

# 20. Jira y Scrum

Jira representa el trabajo oficial del equipo.

Una tarea debe tener, como mínimo cuando corresponda:

* Resumen claro.
* Descripción.
* Épica.
* Responsable.
* Criterios de aceptación.
* Dependencias.
* Estado correcto.

Los estados utilizados por el equipo representan aproximadamente:

```text
Por hacer
   ↓
En curso
   ↓
En revisión
   ↓
En pruebas
   ↓
Finalizado
```

No se debe marcar una actividad como Finalizada únicamente porque exista código.

También debe cumplir sus criterios de aceptación y la Definition of Done correspondiente.

---

# 21. Definition of Ready

Antes de comenzar una tarea se debe revisar:

```text
docs/onboarding/DEFINITION_OF_READY.md
```

La Definition of Ready define las condiciones mínimas que una actividad debe cumplir antes de considerarse suficientemente preparada para trabajar.

Si la tarea tiene ambigüedades importantes de negocio, arquitectura o dependencias, deben resolverse antes de implementar.

---

# 22. Definition of Done

Antes de marcar una actividad como Finalizada se debe revisar:

```text
docs/onboarding/DEFINITION_OF_DONE.md
```

La Definition of Done establece las condiciones que deben cumplirse para considerar terminado el trabajo.

No debe confundirse:

```text
"ya escribí el código"
```

con:

```text
"la tarea está terminada"
```

---

# 23. Reglas para juniors, semi-seniors y seniors

Consultar:

```text
TEAM_WORK_RULES.md
```

La asignación no depende solamente de la cantidad de código.

También debe considerarse:

* Complejidad técnica.
* Riesgo.
* Impacto transversal.
* Conocimiento del dominio.
* Dependencias con otros servicios.

Las tareas `junior-friendly` deben tener un camino de implementación claro y revisión correspondiente.

Los cambios de arquitectura, seguridad, contratos transversales, mensajería, multi-tenancy, Building Blocks e infraestructura crítica requieren supervisión adecuada.

---

# 24. Configuración y secretos

No deben almacenarse en Git:

* Contraseñas.
* Tokens.
* Connection strings con credenciales.
* Secretos de RabbitMQ.
* Credenciales de Azure.
* Claves privadas.

Las configuraciones sensibles del entorno de desarrollo deben mantenerse fuera del repositorio utilizando el mecanismo establecido para cada servicio.

Si una credencial aparece accidentalmente en una rama o commit, debe notificarse inmediatamente.

---

# 25. Bases de datos

Cada servicio es propietario de su propia base.

Ejemplos:

```text
IdentityService      → SIA_IdentityDb
AcademicService      → SIA_AcademicDb
AcademicStaffService → SIA_AcademicStaffDb
SchedulingService    → SIA_SchedulingDb
SchoolControlService → SIA_SchoolControlDb
```

Un servicio nunca debe:

```text
SELECT ...
FROM BaseDeOtroServicio.Tabla
```

ni crear:

* joins entre bases de microservicios;
* foreign keys entre bases;
* vistas que rompan ownership;
* acceso directo mediante otro DbContext.

Si necesita información ajena debe utilizar contratos, eventos o referencias locales autorizadas.

---

# 26. Mensajería

SIA utiliza comunicación orientada a eventos cuando un cambio de estado debe propagarse a otros dominios.

El patrón general utilizado es:

```text
Cambio de negocio
      ↓
Entidad + Outbox
      ↓
RabbitMQ
      ↓
Consumer
      ↓
Inbox / procesamiento
```

Los eventos públicos pertenecen a `Contracts`.

No deben publicarse entidades completas del Domain como contratos de integración.

Los componentes técnicos reutilizables del Outbox se mantienen en los Building Blocks correspondientes.

---

# 27. Qué hacer antes de comenzar una tarea

Antes de escribir código:

1. Leer la tarea de Jira completa.
2. Revisar criterios de aceptación.
3. Confirmar el servicio propietario.
4. Leer el README del servicio.
5. Revisar la Definition of Ready.
6. Actualizar `develop`.
7. Crear la rama correspondiente.
8. Revisar código existente antes de crear nuevas clases o abstracciones.
9. Consultar al responsable técnico si la tarea requiere modificar arquitectura, contratos o componentes transversales.

---

# 28. Qué hacer antes de abrir un Pull Request

Comprobar:

1. La solución afectada compila.
2. Las pruebas relacionadas pasan.
3. No existen secretos en los cambios.
4. No se modificaron archivos ajenos a la tarea sin motivo.
5. Se respetaron los límites del dominio.
6. No existen referencias prohibidas.
7. Las migraciones necesarias están incluidas.
8. Los contratos afectados fueron revisados.
9. La tarea Jira sigue representando el trabajo realmente realizado.

---

# 29. Documentación que debe conocer un nuevo integrante

## Arquitectura

```text
README.md
docs/architecture/ARCHITECTURE_RULES.md
docs/architecture/SERVICE_MAP.md
```

## Trabajo y calidad

```text
docs/onboarding/DEFINITION_OF_READY.md
docs/onboarding/DEFINITION_OF_DONE.md
TEAM_WORK_RULES.md
```

## Puertos

```text
LOCAL_PORTS.md
```

## Ramas y Pull Requests

Documento:

```text
SIA — Estrategia de Ramas Git y Flujo de Cambios
```

Flujo oficial:

```text
rama temporal → develop → qa → main
```

## Documentos de dominio

Consultar:

```text
docs/services/
```

y el `README.md` del servicio correspondiente.

---

# 30. Ruta recomendada para un nuevo integrante

## Primer paso

Leer:

```text
README.md
```

## Segundo paso

Leer:

```text
docs/architecture/ARCHITECTURE_RULES.md
docs/architecture/SERVICE_MAP.md
```

## Tercer paso

Leer:

```text
docs/onboarding/DEFINITION_OF_READY.md
docs/onboarding/DEFINITION_OF_DONE.md
TEAM_WORK_RULES.md
```

## Cuarto paso

Abrir:

```text
SIA.Platform.slnx
```

y compilar la solución.

## Quinto paso

Ejecutar:

```bash
dotnet test SIA.Platform.slnx
```

## Sexto paso

Leer el README del servicio donde trabajará.

## Séptimo paso

Revisar su actividad asignada en Jira.

## Octavo paso

Actualizar `develop` y crear su rama de trabajo.

A partir de este momento puede comenzar la implementación.

---

# 31. Regla final

Cuando exista duda sobre dónde implementar una funcionalidad, la primera pregunta debe ser:

> ¿Qué servicio es dueño de esta responsabilidad de negocio?

Cuando exista duda sobre una dependencia:

> ¿Estoy utilizando un contrato público o estoy acoplándome al modelo interno de otro servicio?

Cuando exista duda sobre una decisión transversal:

> ¿Este cambio afecta solamente mi servicio o puede modificar el comportamiento de toda la plataforma?

Si la respuesta no es clara, se debe solicitar revisión antes de continuar.

El objetivo del onboarding no es memorizar toda SIA el primer día.

El objetivo es que cada integrante conozca **dónde buscar información, qué reglas no debe romper y cómo incorporar sus cambios de forma segura al proyecto**.
