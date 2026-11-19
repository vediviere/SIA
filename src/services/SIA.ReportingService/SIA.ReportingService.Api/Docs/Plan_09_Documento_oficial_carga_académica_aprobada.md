# PLAN-09 — Definir documento oficial de carga académica aprobada

## 1. Objetivo

Definir los documentos oficiales que deberá generar SIA cuando una carga académica haya concluido satisfactoriamente su proceso de revisión y autorización, y haya sido oficialmente materializada por SchedulingService.

Se establecerán dos documentos:

1. **Documento general de carga académica:** concentrado de las asignaciones académicas de los docentes de una carrera o programa educativo, organizado por grupo y dirigido al Jefe de División.
2. **Documento individual de carga académica:** detalle de las asignaciones académicas de un docente, dirigido al propio docente y disponible también para el Jefe de División correspondiente.

Ambos documentos tendrán como finalidad presentar formalmente la distribución de la carga académica aprobada, proporcionando información clara sobre las asignaturas, actividades de apoyo, horas y créditos asignados.

## 2. Alcance

Esta definición contempla:

- Generación automática de los documentos oficiales después de la aprobación y oficialización de la carga académica.
- Definición del contenido del documento general e individual.
- Identificación de los destinatarios y permisos de consulta.
- Inclusión de información académica correspondiente al periodo.
- Presentación del desglose de horas y créditos.
- Identificación del responsable institucional que firma los documentos.
- Conservación de documentos históricos y generación de nuevas versiones ante cambios extraordinarios autorizados.

### 2.1 Fuera de alcance

No se contempla:

- Planeación de la carga académica.
- Asignación de docentes a materias y actividades.
- Revisión y autorización de la propuesta.
- Oficialización de la carga académica.
- Procedimiento detallado para solicitar, autorizar y aplicar modificaciones extraordinarias.
- Definición de los mecanismos técnicos de almacenamiento, generación y distribución de archivos.

Estos aspectos corresponden a PR-001, al procedimiento independiente de modificaciones extraordinarias y al diseño técnico correspondiente.

## 3. Relación con PR-001 y PLAN-06

| Documento | Responsabilidad |
|-----------|-----------------|
| PR-001 — Planeación Académica | Define el proceso de planeación, revisión, autorización y oficialización de la carga académica. |
| PLAN-06 — Consulta y consumo de carga académica aprobada | Define las reglas de consulta y consumo de la información oficial por los perfiles y procesos autorizados. |
| PLAN-09 — Documento oficial de carga académica aprobada | Define los documentos que se generan para presentar formalmente la carga académica oficial. |
| Documento de modificaciones extraordinarias | Definirá el procedimiento detallado para gestionar cambios posteriores a la oficialización y sus efectos. |

PLAN-09 no sustituye la consulta de información establecida en PLAN-06 ni modifica el proceso de aprobación definido en PR-001.

## 4. Responsabilidades por microservicio

### 4.1 SchedulingService

Es responsable de administrar la información oficial de la carga académica.

Sus responsabilidades relacionadas con PLAN-09 son:

- Identificar las cargas académicas que han sido oficialmente aprobadas y materializadas.
- Proporcionar la información necesaria para generar los documentos.
- Identificar las cargas individuales y los grupos correspondientes a cada carrera o programa educativo.
- Proporcionar los datos de asignaturas y actividades de apoyo.
- Identificar las modificaciones autorizadas que afecten los documentos oficiales.
- Mantener la relación entre la carga académica oficial y sus documentos generados.

SchedulingService será la fuente oficial de información para la generación de ambos documentos.

### 4.2 AcademicService

Será responsable de proporcionar la información académica del periodo y los datos de referencia necesarios para complementar los documentos.

Entre la información requerida se encuentra:

- Nombre y clave del periodo académico.
- Fecha límite de entrega de instrumentación didáctica.
- Fechas de entrega de reportes parciales.
- Fecha de entrega de actas finales.
- Información del programa educativo, cuando corresponda.

Las fechas se obtendrán de la tabla `AcademicPeriods`, evitando duplicar esta información en SchedulingService.

### 4.3 AcademicStaffService

Proporcionará la información correspondiente a los docentes y responsables institucionales que deba presentarse en los documentos, de acuerdo con los contratos de comunicación establecidos.

### 4.4 AdminBff

Será responsable de atender las solicitudes de consulta y descarga de los documentos desde el frontend administrativo, respetando los permisos correspondientes.

No será responsable de administrar la información oficial de la carga académica ni de establecer reglas de negocio para su generación.

## 5. Tipos de documentos oficiales

SIA deberá generar dos documentos independientes.

### 5.1 Documento general de carga académica

**Destinatario:** Jefe de División.

**Propósito:** presentar un concentrado de las cargas académicas asignadas a los docentes de una carrera o programa educativo durante un periodo académico.

El documento deberá organizar la información por grupo, permitiendo al Jefe de División identificar las asignaturas asignadas, los docentes responsables y la distribución de horas y créditos.

#### 5.1.1 Estructura del documento

**A. Encabezado institucional**

Deberá incluir:

- Nombre de la institución.
- Carrera o programa educativo.
- Periodo académico.
- Fecha de generación del documento.

**B. Asignaturas agrupadas por grupo**

Las asignaturas deberán organizarse por grupo.

Cada grupo presentará una tabla con la siguiente información:

| Campo | Descripción |
|-------|-------------|
| No. | Número consecutivo de la asignatura dentro del grupo. |
| Asignatura | Nombre de la materia asignada. |
| Clave | Clave de identificación de la asignatura. |
| Docente | Nombre del docente responsable de impartirla. |
| Horas teóricas | Horas teóricas asignadas a la materia. |
| Horas prácticas | Horas prácticas asignadas a la materia. |
| Créditos | Créditos correspondientes a la asignatura. |

Al finalizar cada grupo se deberá presentar un resumen con:

- Total de horas teóricas.
- Total de horas prácticas.
- Total de créditos.

Los créditos deberán presentarse como un dato académico independiente y no se sumarán a las horas.

**C. Actividades de apoyo por docente**

Las actividades de apoyo deberán presentarse en una sección independiente, agrupadas por docente, debido a que no necesariamente pertenecen a un grupo académico.

La tabla deberá incluir:

| Campo | Descripción |
|-------|-------------|
| Docente | Nombre del docente al que se asignó la actividad. |
| Actividad | Nombre de la actividad de apoyo. |
| Horas por semana | Número de horas semanales asignadas. |
| Observaciones | Descripción o indicaciones relacionadas con la actividad, cuando existan. |

**D. Resumen general de la carga académica**

Al finalizar el documento se deberá presentar un concentrado general con:

- Total de grupos atendidos.
- Total de asignaturas asignadas.
- Total de docentes participantes.
- Total de horas teóricas.
- Total de horas prácticas.
- Total de horas frente a grupo.
- Total de horas de apoyo.
- Total general de horas asignadas.
- Total de créditos de las asignaturas.

El total de horas frente a grupo corresponderá a la suma de horas teóricas y prácticas.

El total general de horas corresponderá a la suma de horas frente a grupo y horas de apoyo.

Los créditos no deberán incluirse en los cálculos de horas.

**E. Firma institucional**

El documento deberá incluir únicamente el espacio para la firma del Jefe de División correspondiente, siguiendo el criterio del formato institucional de referencia.

### 5.2 Documento individual de carga académica

**Destinatarios:**

- Docente propietario de la carga académica.
- Jefe de División correspondiente.

**Propósito:** presentar formalmente el detalle de las asignaturas y actividades de apoyo que integran la carga académica individual de un docente.

El documento deberá reflejar la carga académica correspondiente a la combinación de docente, carrera o programa educativo y periodo académico.

#### 5.2.1 Estructura del documento

**A. Encabezado institucional**

Deberá incluir:

- Nombre de la institución.
- Lugar y fecha de generación.
- Identificador del documento u oficio, cuando corresponda.
- Asunto: Asignación de carga académica.
- Periodo académico.
- Carrera o programa educativo.

**B. Datos del docente**

Deberá incluir:

- Nombre completo del docente.
- Carrera o programa educativo al que corresponde la carga.

**C. Asignaturas asignadas**

Se deberá presentar una tabla con el detalle de las materias asignadas.

| Campo | Descripción |
|-------|-------------|
| No. | Número consecutivo de la asignatura. |
| Asignatura | Nombre de la materia. |
| Clave | Clave de identificación de la asignatura. |
| Horas teóricas | Horas teóricas asignadas. |
| Horas prácticas | Horas prácticas asignadas. |
| Créditos | Créditos correspondientes a la asignatura. |
| Grupo | Grupo en el que se impartirá la materia. |

**D. Actividades de apoyo**

Se deberá presentar una sección independiente con las actividades de apoyo asignadas al docente.

La tabla incluirá:

| Campo | Descripción |
|-------|-------------|
| Actividad | Nombre de la actividad asignada. |
| Horas por semana | Horas semanales correspondientes a la actividad. |
| Observaciones | Indicaciones y responsabilidades asociadas a la actividad, cuando existan. |

**E. Resumen de horas**

El documento deberá mostrar la distribución de horas de la carga académica individual:


- Total de horas frente a grupo.
- Total de horas de apoyo.
- Total general de horas asignadas.

El total de horas frente a grupo corresponderá a la suma de horas teóricas y prácticas de las asignaturas.

El total general corresponderá a la suma de las horas frente a grupo y las horas de apoyo.

Los créditos deberán mostrarse de manera independiente.

**F. Fechas académicas del periodo**

El documento deberá presentar las fechas institucionales correspondientes al periodo académico:

- Fecha límite de entrega de instrumentación didáctica.
- Fecha de entrega del primer reporte parcial.
- Fecha de entrega del segundo reporte parcial.
- Fecha de entrega del tercer reporte parcial.
- Fecha de entrega de actas finales.

Estas fechas se obtendrán de los campos correspondientes de `AcademicService.AcademicPeriods`.

En particular, `PlanningSubmissionDate` representa la fecha límite de entrega de instrumentación didáctica.

No deberán capturarse nuevamente en SchedulingService.

**G. Firma institucional**

El documento deberá incluir únicamente el espacio para la firma del Jefe de División correspondiente.

No se incluirán espacios de firma para el docente ni para el Coordinador Académico.

## 6. Generación de los documentos

Ambos documentos deberán generarse automáticamente cuando la carga académica haya concluido satisfactoriamente el proceso de aprobación y SchedulingService la haya materializado como carga oficial.

### 6.1 Reglas de generación

**RF-01.** El sistema deberá generar automáticamente los documentos oficiales una vez que la carga académica haya sido aprobada y oficializada.

**RF-02.** Se deberá generar un documento individual por cada carga académica oficial de docente, carrera o programa educativo y periodo.

**RF-03.** Se deberá generar un documento general por cada carrera o programa educativo y periodo, que concentre las cargas académicas correspondientes.

**RF-04.** El documento general deberá organizar las asignaturas por grupo y las actividades de apoyo por docente.

**RF-05.** El documento individual deberá presentar exclusivamente la información correspondiente a la carga académica del docente propietario.

**RF-06.** La información presentada deberá corresponder a la versión oficial de la carga académica al momento de su generación.

**RF-07.** Los documentos deberán incluir la información académica correspondiente al periodo al que pertenece la carga.

**RF-08.** La generación de documentos no deberá modificar la información oficial de la carga académica.

## 7. Consulta y permisos de acceso

Los documentos deberán estar disponibles para consulta de acuerdo con los perfiles y ámbitos de responsabilidad definidos en PLAN-06.

### 7.1 Documento general

**RF-09.** El documento general únicamente podrá ser consultado por el Jefe de División responsable de la carrera o programa educativo correspondiente.

**RF-10.** El Jefe de División únicamente podrá consultar los documentos generales de los programas educativos que tenga asociados.

### 7.2 Documento individual

**RF-11.** El docente podrá consultar únicamente el documento individual correspondiente a su propia carga académica.

**RF-12.** El Jefe de División podrá consultar los documentos individuales de los docentes pertenecientes a las carreras o programas educativos que tenga asociados.

**RF-13.** La consulta de documentos deberá respetar los permisos, el ámbito de responsabilidad y el contexto institucional del usuario.

**RF-14.** Los documentos correspondientes a periodos anteriores deberán permanecer disponibles para consulta de los usuarios autorizados.

## 8. Comportamiento ante modificaciones posteriores

La carga académica podrá sufrir modificaciones después de su oficialización, siempre que estas se gestionen mediante el procedimiento extraordinario correspondiente y cuenten con las autorizaciones establecidas.

PLAN-09 únicamente definirá el comportamiento de los documentos ante dichos cambios.

### 8.1 Reglas de actualización

**RF-15.** Cuando una modificación extraordinaria autorizada afecte una carga académica oficial, deberá generarse una nueva versión del documento individual correspondiente.

**RF-16.** Cuando la modificación afecte el concentrado general de una carrera o programa educativo, deberá generarse una nueva versión del documento general correspondiente.

**RF-17.** Los documentos anteriores deberán conservarse como históricos y no deberán sobrescribirse.

**RF-18.** La nueva versión deberá reflejar la información actualizada de la carga académica.

**RF-19.** La nueva versión deberá identificar la fecha de vigencia del cambio correspondiente.

**RF-20.** Únicamente deberán generarse nuevamente los documentos afectados por la modificación. No será necesario regenerar todos los documentos del periodo.

**RF-21.** Los documentos históricos deberán permanecer disponibles para consulta, respetando los permisos establecidos.

El procedimiento detallado para gestionar las solicitudes, autorizaciones, versionamiento, fechas de vigencia y notificaciones de los cambios se definirá en un documento funcional independiente.

## 9. Consideraciones de presentación

Los documentos generados por SIA deberán modernizar el formato institucional de referencia, conservando su identidad institucional y facilitando la lectura de la información.

Se deberán considerar los siguientes criterios:

- Presentación formal y ordenada.
- Identificación clara de la institución, programa educativo y periodo.
- Separación visual entre asignaturas y actividades de apoyo.
- Diferenciación entre horas teóricas, horas prácticas, horas de apoyo y créditos.*
- Presentación visible de los totales.
- Identificación clara del docente o grupo al que corresponde cada sección.
- Espacio para la firma del Jefe de División.
- Conservación de la información histórica de los documentos.

El diseño visual definitivo deberá establecerse durante la etapa correspondiente de diseño de documentos y UX/UI.

## 10. Resultado esperado

Al concluir la implementación de PLAN-09, SIA deberá permitir:

1. Generar automáticamente un documento general por carrera o programa educativo y periodo.
2. Generar automáticamente un documento individual por cada carga académica oficial.
3. Organizar el documento general por grupo.
4. Presentar las actividades de apoyo agrupadas por docente.
5. Mostrar el desglose de horas teóricas, prácticas y de apoyo.
6. Presentar los créditos de manera independiente de las horas.
7. Mostrar las fechas académicas obtenidas desde AcademicService.
8. Restringir la consulta de los documentos de acuerdo con los perfiles autorizados.
9. Conservar los documentos históricos.
10. Generar nuevas versiones cuando existan modificaciones extraordinarias autorizadas que afecten los documentos.

## 11. Relación con otros documentos

| Documento | Relación |
|-----------|----------|
| PR-001 — Planeación Académica | Establece el proceso que permite obtener una carga académica aprobada y oficializada. |
| PLAN-06 — Consulta y consumo de carga académica aprobada | Establece las reglas de consulta y consumo de la información oficial. |
| PLAN-09 — Documento oficial de carga académica aprobada | Establece los documentos que representan formalmente la carga académica aprobada. |
| Documento de modificaciones extraordinarias | Establecerá el procedimiento para modificar una carga oficial y gestionar sus impactos. |


