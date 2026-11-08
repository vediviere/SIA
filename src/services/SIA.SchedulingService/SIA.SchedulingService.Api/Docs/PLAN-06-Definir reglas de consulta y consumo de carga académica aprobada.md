# PLAN-06 — Definir reglas de consulta y consumo de carga académica aprobada

## 1. Objetivo

Establecer las reglas funcionales para consultar y consumir la carga académica una vez que ha sido oficialmente aprobada y materializada en el sistema.

La finalidad es definir qué perfiles pueden consultar la información, qué periodos estarán disponibles, qué procesos utilizarán la carga académica y cómo se realizará la comunicación entre los microservicios involucrados.

Esta definición busca garantizar que la información de la carga académica sea consistente, esté centralizada en el servicio responsable y pueda ser utilizada por los distintos procesos institucionales sin duplicar innecesariamente su administración.

## 2. Alcance

Esta definición contempla:

- Consulta de cargas académicas oficialmente aprobadas.
- Consulta de cargas correspondientes al periodo vigente y a periodos anteriores.
- Perfiles autorizados y alcance de sus consultas.
- Procesos institucionales que consumen información de la carga académica.
- Responsabilidad de SchedulingService como fuente oficial.
- Comunicación entre microservicios mediante consultas y eventos de integración.
- Conservación de la información histórica en los procesos consumidores.

### 2.1 Fuera de alcance

No se contempla en esta definición:

- Creación y planeación de la carga académica.
- Revisión y autorización de la carga académica.
- Oficialización y materialización de la carga académica.
- Definición detallada del procedimiento para modificar una carga académica después de su aprobación.
- Reglas particulares de operación de los módulos consumidores.

Los procesos de planeación, revisión, autorización y oficialización corresponden a PR-001 — Planeación Académica.

El procedimiento para gestionar modificaciones extraordinarias de una carga académica se documentará de manera independiente. Dicho documento establecerá las reglas para solicitar, autorizar, registrar y aplicar los cambios, así como su impacto en los procesos consumidores.

## 3. Responsabilidades por microservicio

### 3.1 SchedulingService

Es el responsable de administrar y conservar la información oficial de la carga académica.

Sus responsabilidades son:

- Mantener la información de las cargas académicas oficialmente aprobadas.
- Permitir la consulta de cargas académicas de acuerdo con los permisos y el ámbito de responsabilidad del usuario solicitante.
- Proporcionar información a los microservicios que la requieran mediante los contratos de comunicación establecidos.
- Publicar eventos de integración cuando existan cambios autorizados que afecten la información oficial.
- Conservar la información histórica de las cargas académicas.

SchedulingService será la fuente oficial de consulta para los datos relacionados con la carga académica.

### 3.2 WorkflowService

Es responsable de administrar los procesos de revisión, autorización y seguimiento de los flujos de trabajo.

No será responsable de almacenar ni administrar el contenido completo de la carga académica.

### 3.3 Microservicios consumidores

Los microservicios que utilicen información de la carga académica serán responsables de consumir únicamente los datos necesarios para sus propios procesos.

Cuando requieran conservar información para su operación, deberán administrar sus propios datos sin asumir la propiedad de la carga académica.

Asimismo, deberán atender los eventos de integración que correspondan a los cambios que afecten sus procesos.

### 3.4 AdminBff

Será responsable de atender las consultas provenientes del frontend administrativo y componer la información necesaria para presentar las vistas correspondientes.

No será responsable de administrar la carga académica ni de implementar reglas de negocio que correspondan a SchedulingService o a los microservicios consumidores.

### 3.5 NotificationsService

Será responsable de gestionar la entrega de notificaciones cuando los procesos correspondientes requieran informar a los usuarios sobre acontecimientos relacionados con la carga académica.

No será responsable de determinar ni autorizar cambios en la carga.

## 4. Perfiles autorizados para consultar la carga académica

La consulta de información deberá respetar el rol del usuario y su ámbito de responsabilidad institucional.

| Perfil | Alcance de consulta |
|---|---|
| Jefe de División | Podrá consultar las cargas académicas de los docentes pertenecientes a las carreras o programas educativos que tenga asociados. |
| Coordinador académico | Podrá consultar las cargas académicas de las carreras o programas educativos que estén bajo su ámbito de responsabilidad. |
| Docente | Podrá consultar únicamente su propia carga académica. |
| Administrador | Podrá consultar las cargas académicas dentro de su ámbito de administración, de acuerdo con los permisos asignados. |
| Otros departamentos | Podrán consultar únicamente la información necesaria para los procesos y funciones que tengan autorizados. |

### 4.1 Reglas de acceso

**RF-01.** Todo usuario deberá estar autenticado para consultar información de cargas académicas.

**RF-02.** El sistema deberá validar que el usuario cuente con los permisos necesarios para realizar la consulta solicitada.

**RF-03.** El acceso a la información deberá limitarse al ámbito de responsabilidad del usuario.

**RF-04.** Un docente únicamente podrá consultar su propia carga académica.

**RF-05.** Un Jefe de División únicamente podrá consultar información de las carreras o programas educativos que tenga asociados.

**RF-06.** Los usuarios de otros departamentos no tendrán acceso indiscriminado a la carga académica completa. Su consulta estará limitada a la información requerida para sus funciones autorizadas.

**RF-07.** Los permisos de consulta deberán respetar el contexto institucional correspondiente, incluyendo el TenantId.

## 5. Consulta de cargas académicas por periodo

Los usuarios autorizados podrán consultar tanto las cargas académicas del periodo vigente como las correspondientes a periodos anteriores.

### 5.1 Reglas de consulta histórica

**RF-08.** El sistema deberá permitir consultar cargas académicas de periodos anteriores, siempre que el usuario cuente con los permisos correspondientes.

**RF-09.** La consulta histórica deberá conservar la información que estuvo oficialmente registrada para el periodo seleccionado.

**RF-10.** Las cargas académicas de periodos anteriores deberán permanecer disponibles para consulta, aunque el periodo ya haya concluido.

**RF-11.** Los cambios realizados en una carga académica durante un periodo posterior no deberán alterar los registros históricos de periodos anteriores.

**RF-12.** La consulta de información histórica deberá respetar las mismas restricciones de acceso y ámbito de responsabilidad aplicables a la consulta de información vigente.

## 6. Procesos institucionales que consumen la carga académica

La carga académica oficialmente aprobada será utilizada como fuente de información para distintos procesos institucionales.

Los microservicios responsables de dichos procesos podrán consumir la información necesaria para realizar sus operaciones.

### 6.1 Horarios

El proceso de horarios utilizará la información de la carga académica para identificar las asignaciones oficiales de los docentes.

Entre los datos que pueden requerirse se encuentran:

- Docente asignado.
- Materia.
- Grupo.
- Periodo académico.
- Horas asignadas.
- Información relacionada con la disponibilidad del docente, cuando corresponda.

### 6.2 Asistencias

El proceso de asistencia utilizará la información de la carga académica para identificar las relaciones oficiales entre docentes, materias, grupos y periodos.

Esto permitirá asociar los registros de asistencia con los grupos y las asignaturas correspondientes.

### 6.3 Calificaciones

El proceso de calificaciones utilizará la carga académica para identificar las asignaciones docentes y académicas que correspondan al periodo.

La información permitirá determinar las relaciones entre docentes, materias, grupos y periodos para la operación del registro y consulta de calificaciones.

### 6.4 Tutorías

El proceso de tutorías utilizará la información correspondiente a la asignación de tutores académicos a los grupos.

Entre los datos requeridos se encuentran:

- Docente tutor.
- Grupo asignado.
- Periodo académico.
- Información de la asignación de tutoría.

### 6.5 Evaluación docente

El proceso de evaluación docente utilizará la información de las asignaciones académicas para identificar las materias y grupos asociados a cada docente durante el periodo correspondiente.

### 6.6 Indicadores

El proceso de indicadores utilizará la información de la carga académica para realizar los cálculos y análisis institucionales que requieran datos relacionados con las asignaciones docentes y académicas.

### 6.7 Reportes

Los reportes institucionales podrán consumir información de la carga académica para generar consultas y documentos relacionados con la distribución de materias, grupos, docentes y actividades asignadas.

La información presentada deberá respetar los permisos y el ámbito de consulta del usuario solicitante.

## 7. Reglas generales de consumo de información

**RF-13.** SchedulingService será la fuente oficial de información de las cargas académicas aprobadas.

**RF-14.** Los microservicios consumidores deberán obtener la información mediante los mecanismos de comunicación establecidos entre microservicios.

**RF-15.** Los microservicios consumidores deberán utilizar únicamente la información necesaria para ejecutar sus propios procesos.

**RF-16.** Ningún microservicio consumidor deberá asumir la propiedad o administración de la carga académica oficial.

**RF-17.** Los microservicios consumidores podrán conservar información derivada o referencias necesarias para su operación, siempre que correspondan a datos de su propia responsabilidad.

**RF-18.** Los datos consumidos no deberán utilizarse para modificar directamente la información oficial almacenada en SchedulingService.

**RF-19.** Cuando un proceso requiera información actualizada de la carga académica, podrá solicitarla directamente a SchedulingService mediante los contratos de comunicación establecidos.

**RF-20.** Las consultas realizadas desde el frontend administrativo deberán gestionarse a través del BFF correspondiente, el cual podrá obtener y componer la información de los microservicios responsables.

## 8. Comunicación y actualización entre microservicios

Para mantener la consistencia de la información, se contemplan dos mecanismos de comunicación:

### 8.1 Consultas entre microservicios

Cuando un microservicio necesite información de una carga académica para ejecutar una operación, podrá solicitarla directamente a SchedulingService mediante los contratos de comunicación establecidos.

Este mecanismo permitirá obtener información específica sin necesidad de duplicar la carga académica completa en cada servicio.

### 8.2 Eventos de integración

Cuando exista un cambio autorizado que afecte una carga académica oficialmente registrada, SchedulingService será responsable de publicar los eventos de integración correspondientes.

Los microservicios consumidores deberán identificar si el cambio afecta sus procesos y actuar únicamente sobre la información que sea de su responsabilidad.

Los eventos permitirán comunicar los cambios sin establecer dependencias directas entre las bases de datos de los microservicios.

### 8.3 Reglas de actualización

**RF-21.** SchedulingService será responsable de comunicar mediante eventos los cambios autorizados que afecten la información oficial de la carga académica.

**RF-22.** Los microservicios consumidores deberán procesar únicamente los eventos que sean relevantes para sus procesos.

**RF-23.** Cada microservicio consumidor será responsable de actualizar sus propios datos cuando un evento afecte su información.

**RF-24.** Los cambios en la carga académica no deberán provocar la reconstrucción o actualización innecesaria de toda la información almacenada por los microservicios consumidores.

**RF-25.** Los procesos consumidores deberán conservar la trazabilidad histórica de sus operaciones. Una modificación posterior no deberá alterar los registros correspondientes a operaciones realizadas antes de la fecha en que el cambio tenga efecto.

**RF-26.** La comunicación entre microservicios deberá respetar los contratos establecidos y el contexto de identificación y trazabilidad de las operaciones, incluyendo TenantId y CorrelationId.

## 9. Consideraciones sobre modificaciones extraordinarias

La carga académica podrá requerir modificaciones posteriores a su oficialización debido a situaciones extraordinarias.

Sin embargo, el procedimiento para gestionar dichas modificaciones no forma parte del alcance de PLAN-06.

Para evitar inconsistencias entre los procesos institucionales, se establece lo siguiente:

- Los cambios deberán gestionarse mediante un procedimiento funcional independiente.
- SchedulingService conservará la responsabilidad sobre la información oficial.
- Los cambios autorizados deberán comunicarse a los microservicios consumidores cuando afecten sus procesos.
- Los microservicios consumidores serán responsables de aplicar las actualizaciones que correspondan a sus propios datos.
- Los registros históricos deberán conservar la información que corresponda al momento en que se realizaron las operaciones.

**Nota:** El documento independiente de modificaciones extraordinarias definirá a detalle las solicitudes, autorizaciones, versionamiento, fechas de vigencia, notificaciones, registro histórico y tratamiento de los impactos en los módulos consumidores.

## 10. Seguridad y trazabilidad

**RF-27.** Las consultas deberán validar la identidad, los permisos y el ámbito de responsabilidad del usuario.

**RF-28.** La información de una institución no deberá estar disponible para usuarios pertenecientes a otro Tenant, salvo que exista una autorización explícita contemplada por el sistema.

**RF-29.** Las solicitudes entre microservicios deberán conservar el contexto necesario para identificar la institución y dar seguimiento a la operación.

**RF-30.** Las consultas y operaciones relacionadas con la carga académica deberán respetar los mecanismos de seguridad y autorización establecidos en la arquitectura de SIA.

**RF-31.** La información histórica deberá conservarse de manera que permita identificar las asignaciones académicas correspondientes a cada periodo.

## 11. Resultado esperado

Al concluir la implementación de las reglas definidas en PLAN-06, SIA deberá permitir:

1. Consultar cargas académicas oficialmente aprobadas.
2. Consultar información de periodos vigentes y anteriores.
3. Restringir el acceso de acuerdo con los perfiles y ámbitos de responsabilidad.
4. Proporcionar información oficial a los procesos institucionales que la requieran.
5. Permitir que los microservicios consumidores obtengan información mediante contratos de comunicación.
6. Comunicar los cambios autorizados mediante eventos de integración.
7. Mantener la responsabilidad de cada microservicio sobre sus propios datos.
8. Conservar la trazabilidad histórica de los procesos que consumen la carga académica.
9. Mantener separado el procedimiento detallado para modificaciones extraordinarias.

## 12. Relación con otros procesos y documentos

| Proceso o documento | Relación con PLAN-06 |
|---|---|
| PR-001 — Planeación Académica | Define la planeación, revisión, autorización y oficialización de la carga académica. |
| PLAN-06 — Consulta y consumo de carga académica aprobada | Define las reglas de consulta, acceso y consumo posterior a la oficialización. |
| Documento de modificaciones extraordinarias de carga académica | Definirá el procedimiento para gestionar cambios posteriores a la oficialización y sus efectos en los procesos consumidores. |

---

**Fin de la definición funcional PLAN-06.**