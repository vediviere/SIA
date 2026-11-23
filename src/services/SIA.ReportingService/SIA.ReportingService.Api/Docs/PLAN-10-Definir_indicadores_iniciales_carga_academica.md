# PLAN-10 — Definir indicadores iniciales de carga académica

## 1. Objetivo

Definir el conjunto inicial de indicadores académicos que ReportingService deberá obtener a partir de las cargas académicas oficializadas, estableciendo sus reglas de cálculo, alcance de consulta, tratamiento por periodo académico y mecanismos de integración con SchedulingService.

Esta definición busca establecer una base funcional consistente para la primera versión de ReportingService, evitando interpretaciones diferentes durante su implementación y permitiendo que REPORT-02 se desarrolle sin decisiones funcionales pendientes.

## 2. Alcance

Esta definición contempla:

- Determinación de las cargas académicas que deben contabilizarse.
- Definición de indicadores iniciales de horas y docentes.
- Consulta por periodo académico.
- Consulta institucional y por carrera/programa.
- Disponibilidad de información histórica.
- Aislamiento de datos por Tenant.
- Información necesaria para alimentar el modelo de consulta de ReportingService.
- Sincronización inicial de cargas históricas y actualización posterior mediante eventos.

### Fuera de alcance

- Definición de indicadores adicionales a los establecidos en este documento.
- Comparativas estadísticas entre periodos.
- Indicadores de desempeño docente.
- Indicadores de aprovechamiento estudiantil.
- Evaluación de productividad docente.
- Diseño físico de tablas, índices o bases de datos.
- Definición de endpoints, DTOs o contratos técnicos definitivos.
- Diseño visual del dashboard.
- Modificaciones al proceso de planeación, asignación o aprobación de cargas académicas.

---

## 3. Fuente oficial de información

SchedulingService es el servicio responsable de administrar las cargas académicas y sus asignaciones, por lo que constituye la fuente oficial de información para los indicadores definidos en PLAN-10.

ReportingService deberá consumir exclusivamente información correspondiente a cargas académicas oficializadas.

### 3.1 Definición de carga aprobada

Para efectos de Reporting, una carga académica aprobada es aquella que ha completado el proceso de revisión y aprobación correspondiente y que se encuentra oficializada en SchedulingService.

Se consideran:

- Cargas académicas aprobadas y oficializadas.
- Cargas que contengan únicamente materias.
- Cargas que contengan únicamente actividades de apoyo.
- Cargas que contengan materias y actividades de apoyo.

Se excluyen:

- Propuestas de carga académica.
- Cargas pendientes de revisión.
- Cargas rechazadas.
- Cargas que aún no han sido oficializadas.

Los indicadores deberán calcularse únicamente a partir de las cargas que cumplen con esta definición.

---

## 4. Indicadores iniciales

La primera versión de ReportingService deberá contemplar los siguientes cuatro indicadores.

### RF-01 — Total de horas clase

Representa la suma de los créditos de las materias incluidas en las cargas académicas oficializadas, dentro del periodo y alcance de consulta seleccionados.

**Regla de cálculo:**

Total de horas clase = suma de Credits de las materias asignadas en cargas oficializadas.

El campo Credits se utilizará directamente como valor para este indicador, considerando que representa la suma de horas teóricas y prácticas de la materia conforme al modelo académico de SIA.

No será necesario volver a sumar las horas teóricas y prácticas para obtener este indicador.

Se contabilizarán únicamente las materias correspondientes a cargas oficializadas.

### RF-02 — Total de horas de apoyo

Representa la suma de las horas asignadas a actividades de apoyo incluidas en las cargas académicas oficializadas.

Se contabilizarán las horas explícitamente asignadas a cada actividad de apoyo.

Se incluyen, entre otras:

- Tutorías.
- Asesorías.
- Residencia Profesional.
- Otras actividades de apoyo contempladas en la carga académica.

Las horas de apoyo se contabilizarán independientemente de las horas clase.

#### Tratamiento de Residencia Profesional

Residencia Profesional deberá contabilizarse como actividad de apoyo para efectos de los indicadores de Reporting.

Las horas contabilizadas corresponderán a las horas explícitamente asignadas al docente en SchedulingService.

No se deberán utilizar los créditos de Residencia Profesional para calcular las horas asignadas al docente, ya que su asignación depende de las horas de atención que determine el Jefe de División.

### RF-03 — Total de horas asignadas

Representa el total de horas clase y horas de apoyo asignadas dentro de las cargas académicas oficializadas.

**Regla de cálculo:**

Total de horas asignadas = Total de horas clase + Total de horas de apoyo.

No se aplicarán ponderaciones, ajustes ni cálculos adicionales.

Este indicador deberá utilizar los resultados de RF-01 y RF-02, evitando duplicar reglas de cálculo.

### RF-04 — Docentes con carga aprobada

Representa el número de docentes únicos que cuentan con al menos una carga académica oficializada dentro del periodo y alcance de consulta seleccionados.

Reglas:

- Un docente con varias materias contará como un docente.
- Un docente con materias y actividades de apoyo contará como un docente.
- Un docente con cargas en varias carreras o programas contará una sola vez en la consulta institucional.
- Un docente con carga exclusivamente de actividades de apoyo también contará.
- Un docente con cargas pendientes o rechazadas, sin cargas oficializadas, no será contabilizado.
- Al consultar una carrera o programa específico, el docente contará si tiene al menos una carga oficializada dentro de ese programa.

El conteo deberá realizarse utilizando la identificación del docente, evitando duplicados.

---

## 5. Ejemplo de cálculo de indicadores

Considerando las siguientes cargas oficializadas:

| Concepto | Valor |
|----------|------:|
| Materia A — Credits | 5 |
| Materia B — Credits | 6 |
| Materia C — Credits | 5 |
| Tutoría | 2 h |
| Asesoría | 3 h |
| Residencia Profesional | 4 h |

Resultado:

| Indicador | Resultado |
|-----------|----------:|
| Total de horas clase | 16 |
| Total de horas de apoyo | 9 |
| Total de horas asignadas | 25 |

Los créditos de las materias se utilizan directamente como horas clase contabilizadas. Las horas de tutoría, asesoría y Residencia Profesional se obtienen de sus asignaciones explícitas de apoyo.

---

## 6. Tratamiento por periodo académico

### RF-05 — Consulta por periodo

Los indicadores deberán calcularse exclusivamente sobre las cargas académicas oficializadas pertenecientes al periodo académico seleccionado.

La consulta inicial deberá contemplar un solo periodo académico a la vez.

No se deberán mezclar cargas de diferentes periodos en una misma consulta.

### RF-06 — Consulta histórica

ReportingService deberá permitir consultar indicadores de periodos académicos anteriores, utilizando la información histórica de cargas oficializadas correspondiente a cada periodo.

La información de periodos anteriores deberá conservarse y estar disponible para consulta conforme a los permisos del usuario.

Las cargas históricas no deberán ser modificadas por la consulta de indicadores.

---

## 7. Alcance institucional y por carrera/programa

### RF-07 — Consulta institucional

ReportingService deberá permitir consultar los indicadores a nivel institucional, considerando todas las cargas académicas oficializadas correspondientes al Tenant y periodo seleccionados.

En este alcance:

- Las horas clase se sumarán considerando todas las cargas del periodo.
- Las horas de apoyo se sumarán considerando todas las actividades de apoyo del periodo.
- Las horas asignadas serán la suma de horas clase y apoyo.
- Los docentes se contabilizarán de manera única, aunque participen en varias carreras o programas.

### RF-08 — Filtro por carrera/programa

ReportingService deberá permitir filtrar los indicadores por carrera o programa educativo.

Cuando se seleccione una carrera/programa:

- Las horas clase corresponderán únicamente a las materias de las cargas dentro de dicho programa.
- Las horas de apoyo corresponderán únicamente a las actividades de apoyo dentro de dicho programa.
- Las horas asignadas corresponderán a la suma de ambos indicadores dentro del programa.
- Los docentes se contabilizarán de manera única dentro del programa seleccionado.

Un docente que tenga carga oficializada en más de una carrera podrá aparecer en la consulta individual de cada carrera, pero deberá contarse una sola vez en la consulta institucional.

---

## 8. Aislamiento por Tenant

### RF-09 — TenantId obligatorio

Todos los indicadores deberán calcularse y consultarse respetando el Tenant al que pertenece la información.

ReportingService deberá garantizar que:

- Las cargas académicas de un Tenant no se mezclen con las de otro.
- La consulta institucional se limite al Tenant correspondiente.
- Los filtros por carrera/programa se apliquen dentro del Tenant autorizado.
- La sincronización inicial y el consumo de eventos respeten el TenantId.
- No exista exposición de información académica entre instituciones.

TenantId deberá formar parte del contexto de integración y del modelo de consulta de ReportingService.

---

## 9. Modelo de consulta de ReportingService

### RF-10 — Modelo de consulta propio

ReportingService deberá mantener su propio modelo de consulta alimentado con información proveniente de SchedulingService.

Este modelo permitirá calcular y consultar los indicadores sin depender de consultas en tiempo real a SchedulingService para cada solicitud del dashboard.

El modelo de Reporting será responsabilidad de ReportingService y podrá organizar la información de acuerdo con las necesidades de consulta y cálculo de indicadores.

### RF-11 — Información mínima requerida

ReportingService deberá disponer de información que permita identificar y calcular los indicadores definidos.

Como mínimo, deberá obtener:

| Información | Propósito |
|-------------|-----------|
| TenantId | Aislamiento institucional |
| AcademicLoadId | Identificación de la carga académica |
| AcademicPeriodId | Identificación del periodo |
| TeacherId | Conteo de docentes únicos |
| EducationalProgramId | Filtro por carrera/programa |
| Identificación de materias | Identificar materias asignadas |
| Credits | Cálculo de horas clase |
| Identificación de actividades de apoyo | Identificar actividades asignadas |
| AssignedHours | Cálculo de horas de apoyo |
| Estado de oficialización | Determinar cargas contabilizables |
| Identificador de versión o actualización | Controlar actualizaciones de información |

La información deberá ser suficiente para reconstruir o actualizar los indicadores correspondientes a una carga académica sin consultar directamente la base de datos de otro servicio.

---

## 10. Integración entre SchedulingService y ReportingService

### RF-12 — Integración mediante eventos

SchedulingService deberá proporcionar información a ReportingService mediante mecanismos de integración autorizados.

La actualización del modelo de consulta de ReportingService se realizará principalmente mediante eventos publicados por SchedulingService cuando existan cambios relevantes en las cargas académicas oficializadas.

ReportingService será responsable de consumir dichos eventos y mantener su modelo de consulta actualizado.

No se permitirá el acceso directo a la base de datos de SchedulingService.

### RF-13 — Evento con representación completa de la carga

Para alimentar el modelo de Reporting, SchedulingService deberá publicar una representación completa y actualizada de la carga académica oficializada.

La información recibida deberá permitir a ReportingService reemplazar la versión anterior de la carga por la nueva, evitando la acumulación de registros o la duplicación de horas, materias, actividades o docentes.

La representación deberá incluir los datos necesarios para identificar:

- Tenant.
- Carga académica.
- Periodo.
- Docente.
- Carrera/programa.
- Materias y sus Credits.
- Actividades de apoyo y AssignedHours.
- Estado de la carga.
- Identificador de versión o actualización.

La estructura técnica definitiva del evento se establecerá durante la implementación de REPORT-02 y las tareas técnicas correspondientes.

### RF-14 — Oficialización de una carga

Cuando una carga académica sea oficializada en SchedulingService, se deberá publicar la información correspondiente para que ReportingService incorpore la carga a su modelo de consulta.

La carga deberá comenzar a contabilizarse en los indicadores una vez que corresponda a su estado oficializado.

### RF-15 — Actualización extraordinaria de cargas

Cuando una carga oficializada sea modificada mediante un procedimiento extraordinario autorizado, SchedulingService deberá publicar la nueva representación completa de la carga.

ReportingService deberá actualizar la información previamente registrada para esa carga, de modo que los indicadores reflejen la información vigente.

La actualización no deberá provocar duplicación de horas ni de docentes.

El procedimiento de modificación extraordinaria, sus autorizaciones y su control de versiones se rigen por la definición funcional correspondiente a cambios extraordinarios de carga académica.

### RF-16 — Cambios de estado que afectan la contabilización

Cuando una carga oficializada deje de ser contabilizable como resultado de un procedimiento autorizado, SchedulingService deberá comunicar el cambio a ReportingService.

ReportingService deberá actualizar su modelo de consulta y dejar de contabilizar dicha carga conforme al estado oficial recibido.

El tratamiento deberá preservar la integridad de los indicadores y la trazabilidad de las actualizaciones.

---

## 11. Sincronización inicial de cargas históricas

### RF-17 — Carga inicial de información

Al iniciar la operación de ReportingService, deberá contemplarse un mecanismo de sincronización inicial para obtener las cargas académicas oficializadas existentes en SchedulingService, incluyendo cargas de periodos anteriores.

Esta sincronización permitirá construir el modelo de consulta de Reporting antes de depender exclusivamente de los eventos generados posteriormente.

La sincronización:

- Incluirá cargas oficializadas existentes.
- Respetará el Tenant.
- Considerará periodos académicos históricos.
- Excluirá propuestas y cargas no oficializadas.
- Obtendrá la información mediante un contrato de integración autorizado.
- No implicará acceso directo a la base de datos de SchedulingService.

### RF-18 — Actualización posterior a la sincronización

Una vez construido el modelo inicial, ReportingService mantendrá la información actualizada mediante el consumo de eventos de SchedulingService.

La sincronización inicial y el consumo de eventos deberán coexistir sin duplicar cargas ni provocar inconsistencias en los indicadores.

Los mecanismos técnicos para garantizar consistencia, idempotencia, control de versiones, recuperación y reprocesamiento deberán definirse en la implementación técnica de REPORT-02.

---

## 12. Seguridad y permisos de consulta

### RF-19 — Acceso administrativo

Los indicadores definidos en PLAN-10 están destinados a consultas administrativas de carga académica.

El acceso deberá respetar el ámbito institucional y las autorizaciones del usuario.

Los usuarios con permisos administrativos podrán consultar indicadores institucionales o por carrera/programa, de acuerdo con su alcance autorizado.

La consulta de históricos también deberá respetar las autorizaciones correspondientes.

### RF-20 — Protección de información entre servicios

ReportingService consumirá información mediante los contratos de integración definidos.

No deberá:

- Consultar directamente bases de datos de otros microservicios.
- Modificar cargas académicas propiedad de SchedulingService.
- Oficializar cargas académicas.
- Administrar el proceso de aprobación.
- Alterar la asignación de docentes, materias o actividades.

SchedulingService conservará la responsabilidad sobre las cargas académicas oficiales.

---

## 13. Validaciones y excepciones

### RF-21 — Cargas no oficializadas

Las cargas pendientes, propuestas, rechazadas o no oficializadas no deberán contribuir a ningún indicador.

### RF-22 — Cargas sin materias

Una carga que únicamente contenga actividades de apoyo deberá contabilizarse en horas de apoyo y horas asignadas. Su docente deberá contabilizarse en docentes con carga aprobada.

### RF-23 — Cargas sin actividades de apoyo

Una carga que únicamente contenga materias deberá contabilizarse en horas clase y horas asignadas. Su docente deberá contabilizarse en docentes con carga aprobada.

### RF-24 — Cargas con materias y apoyo

Las cargas que contengan materias y actividades de apoyo deberán contabilizar ambos conceptos, evitando duplicar las horas clase o apoyo.

### RF-25 — Docentes con múltiples cargas

Un docente con varias cargas oficializadas dentro del mismo periodo deberá contabilizarse una sola vez en el indicador de docentes, dentro del alcance de consulta seleccionado.

### RF-26 — Docentes en varias carreras/programas

En la consulta institucional, un docente que tenga cargas en varias carreras deberá contarse una sola vez.

En la consulta filtrada por carrera/programa, deberá contarse si tiene carga oficializada dentro de ese programa.

### RF-27 — Residencia Profesional

Residencia Profesional deberá contabilizarse mediante las horas explícitamente asignadas como actividad de apoyo.

No se deberán utilizar sus créditos para el cálculo de horas de apoyo ni sumar sus créditos como horas clase por el hecho de estar relacionada con una materia del plan académico.

### RF-28 — Información histórica

La consulta de un periodo histórico deberá utilizar las cargas oficializadas correspondientes a ese periodo, sin incorporar cargas de otros periodos.

### RF-29 — Aislamiento institucional

Toda consulta, sincronización o actualización deberá respetar TenantId.

### RF-30 — Actualización de cargas en Reporting

La recepción de una actualización de carga deberá reemplazar la información correspondiente a la versión anterior, evitando acumulaciones o duplicaciones en los indicadores.

### RF-31 — Consistencia del modelo de consulta

ReportingService deberá mantener un modelo de consulta consistente con la información oficial recibida de SchedulingService.

Los mecanismos técnicos para recuperación ante eventos perdidos, reintentos, duplicados, reprocesamiento y sincronización deberán definirse durante la implementación técnica.

---

## 14. Información consumida

ReportingService consumirá información de SchedulingService correspondiente a cargas académicas oficializadas.

La información incluirá los elementos necesarios para identificar cargas, periodos, docentes, carreras/programas, materias, créditos, actividades de apoyo, horas asignadas y estado de oficialización.

La información histórica será obtenida mediante la sincronización inicial y los mecanismos de integración autorizados.

---

## 15. Información generada

ReportingService generará y expondrá los siguientes indicadores:

- Total de horas clase.
- Total de horas de apoyo.
- Total de horas asignadas.
- Docentes con carga aprobada.

Estos indicadores podrán consultarse por:

- Tenant.
- Periodo académico.
- Carrera/programa educativo, cuando se aplique el filtro.

ReportingService mantendrá su propio modelo de consulta para ofrecer estos resultados sin acceder directamente a las bases de datos de otros servicios.

---

## 16. Criterios de aceptación

PLAN-10 se considera funcionalmente definido cuando:

- Se establece que únicamente las cargas aprobadas y oficializadas son contabilizables.
- Se define el total de horas clase mediante la suma de Credits de las materias.
- Se define el total de horas de apoyo mediante AssignedHours de las actividades de apoyo.
- Residencia Profesional se contempla como actividad de apoyo y se contabiliza por las horas explícitamente asignadas.
- Se define el total de horas asignadas como la suma de horas clase y horas de apoyo.
- Se define el conteo de docentes únicos con carga oficializada.
- Se contempla la consulta de un periodo académico a la vez.
- Se contempla la consulta de periodos históricos.
- Se contempla la consulta institucional y por carrera/programa.
- Se establece el aislamiento obligatorio por Tenant.
- ReportingService contará con su propio modelo de consulta.
- Se define la integración mediante eventos de SchedulingService con representación completa y actualizada de la carga.
- Se contempla la actualización de cargas oficializadas mediante procedimientos extraordinarios autorizados.
- Se contempla la sincronización inicial de cargas históricas.
- Se establece que ReportingService no tendrá acceso directo a bases de datos de otros servicios.
- La definición permite implementar REPORT-02 sin decisiones funcionales pendientes sobre los indicadores iniciales.

---

## 17. Relación con tareas previas y posteriores

| Tarea | Relación |
|-------|----------|
| PR-001 — Planeación Académica | Define el proceso de planeación, revisión y oficialización de carga. |
| PLAN-06 — Definir reglas de consulta y consumo de carga aprobada | Establece las reglas generales de consumo de cargas oficializadas por otros procesos y servicios. |
| PLAN-09 — Definir documento oficial de carga académica aprobada | Establece los documentos institucionales generados a partir de cargas oficializadas. |
| PLAN-10 — Definir indicadores iniciales de carga académica | Define los indicadores y la información necesaria para ReportingService. |
| REPORT-02 — Implementar indicadores iniciales de carga académica | Implementará el modelo de consulta y los indicadores definidos en PLAN-10. |

---

## 18. Resultado esperado

Al concluir PLAN-10, SIA contará con una definición funcional de los indicadores iniciales de carga académica que permitirá a ReportingService consultar información institucional confiable, aislada por Tenant, filtrable por periodo y carrera/programa, y alimentada a partir de cargas académicas oficializadas de SchedulingService.

Esta definición establece la base funcional para implementar REPORT-02, manteniendo la separación de responsabilidades entre microservicios y evitando accesos directos a bases de datos ajenas.