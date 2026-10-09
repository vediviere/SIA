# Definición funcional: Uso de etiquetas de competencias para determinar la afinidad docente–materia

## 1. Objetivo

Definir cómo se utilizarán las etiquetas de competencias asociadas a docentes y materias para apoyar la determinación de afinidad entre ambos, así como su influencia en la lista de docentes candidatos durante la planeación académica.

Esta definición establece el comportamiento funcional que deberá respetar una implementación posterior en `SchedulingService`. No contempla implementar la regla ni establecer una fórmula matemática de puntuación.

## 2. Alcance

La definición contempla:

* La relación funcional entre las competencias de un docente y las asociadas a una materia.
* El comportamiento ante una coincidencia total, parcial o inexistente.
* El tratamiento de docentes sin competencias registradas.
* La prioridad del perfil académico y la carrera frente a la afinidad por etiquetas.
* La clasificación de la afinidad docente–materia.
* La relevancia de las etiquetas asociadas a las materias.
* El comportamiento de las materias de especialidad y de Ciencias Básicas.
* El tratamiento de etiquetas inactivas.
* El orden funcional de los docentes candidatos.
* La libertad del Jefe de División para seleccionar al docente.

## 3. Responsabilidad de los servicios

| Servicio               | Responsabilidad                                                                                                                                                           |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `AcademicService`      | Mantener el catálogo `CompetencyTag` y las asociaciones de competencias con materias mediante `SubjectCompetencies`, incluyendo la relevancia asignada a cada asociación. |
| `AcademicStaffService` | Mantener las competencias asociadas a los docentes mediante `TeacherCompetencies`, incluyendo el nivel de competencia correspondiente.                                    |
| `SchedulingService`    | Consultar la información disponible, aplicar los criterios funcionales de afinidad y presentar la lista de docentes candidatos durante la asignación académica.           |

Las relaciones entre servicios se realizarán mediante los contratos correspondientes. `SchedulingService` no deberá consultar directamente las bases de datos de los otros servicios.

## 4. Relación funcional etiqueta–docente–materia

La afinidad se determina mediante la comparación de las competencias asociadas a una materia con las competencias registradas para cada docente candidato.

La relación funcional comprende tres elementos:

1. **Materia:** tiene asociadas una o varias etiquetas de competencias mediante `SubjectCompetencies`. Cada asociación cuenta con un nivel de relevancia para esa materia.
2. **Docente:** tiene registradas sus competencias mediante `TeacherCompetencies`, incluyendo el nivel de competencia definido en el diseño funcional.
3. **Comparación:** permite identificar las etiquetas coincidentes, las no coincidentes y la relevancia de las competencias coincidentes, como información de apoyo para la selección del docente.

El catálogo de etiquetas es compartido. Las etiquetas se asocian a la entidad `Subject`, no a `StudyPlanSubject`. Por ello, una misma materia reutilizada en distintos planes de estudio mantiene las mismas asociaciones de competencias.

La relación no implica una asignación automática de docentes ni sustituye las validaciones propias de la planeación académica.

## 5. Reglas funcionales

### RF-01. Una materia puede tener varias etiquetas

Una materia puede tener asociadas varias etiquetas de competencias. Estas representan las competencias que se consideran relevantes para impartirla.

La presencia de varias etiquetas permite comparar diferentes competencias del docente con las asociadas a la materia.

### RF-02. Las etiquetas son un criterio orientativo

Las etiquetas sirven como referencia para apoyar la decisión del Jefe de División. No constituyen, por sí mismas, un requisito obligatorio que impida asignar a un docente.

La ausencia de coincidencias no deberá bloquear automáticamente la selección.

### RF-03. Prioridad del perfil académico y la carrera

La afinidad del perfil académico y la carrera del docente respecto de la materia tiene mayor relevancia que la coincidencia por etiquetas.

Ejemplo: para una materia de Ingeniería en Sistemas Computacionales, un docente cuya carrera y perfil académico correspondan a esa área tendrá mayor afinidad inicial que un docente de Ingeniería Industrial, suponiendo que ambos cumplen los demás requisitos aplicables.

Un docente de Ingeniería Industrial que tenga etiquetas coincidentes con la materia puede seguir siendo candidato. Sin embargo, la coincidencia por etiquetas no deberá desplazar automáticamente la mayor afinidad de carrera y perfil académico.

Por tanto, la evaluación funcional deberá considerar primero la afinidad del perfil académico y la carrera y, posteriormente, utilizar las etiquetas para complementar la comparación entre candidatos.

### RF-04. Coincidencia parcial de etiquetas

La coincidencia puede ser parcial. Si una materia tiene asociadas varias etiquetas y el docente coincide con una o más de ellas, se mostrarán las coincidencias identificadas.

La coincidencia parcial permite reconocer competencias relevantes sin exigir que el docente cuente con todas las etiquetas asociadas a la materia.

La comparación deberá considerar la relevancia de las etiquetas coincidentes para la materia y respetar la prioridad del perfil académico y la carrera.

### RF-05. Ausencia de coincidencia

Cuando un docente no coincida con ninguna de las etiquetas activas asociadas a una materia:

* No se considerará automáticamente incompatible.
* No será excluido de la lista de candidatos únicamente por esa razón.
* El Jefe de División podrá evaluar su perfil académico, carrera y los demás criterios aplicables.
* La ausencia de coincidencias no sustituye ni invalida las demás validaciones de asignación.

### RF-06. Docentes sin etiquetas registradas

Cuando un docente no tenga competencias registradas, el sistema deberá distinguir esta situación de aquella en la que sí tiene competencias, pero no coincide con las asociadas a la materia.

El docente se mostrará como **sin información de competencias registrada** y podrá permanecer en la lista de candidatos.

No deberá interpretarse la ausencia de información como evidencia de falta de competencia.

### RF-07. Materias de especialidad

Las materias de especialidad utilizarán la misma lógica general de comparación por etiquetas que las demás materias.

Las etiquetas permiten identificar competencias específicas que pueden ser relevantes para impartir la materia. No se establece una restricción adicional ni una coincidencia mínima obligatoria por el hecho de que la materia sea de especialidad.

La afinidad del perfil académico y la carrera mantiene su prioridad frente a la coincidencia por etiquetas.

### RF-08. Materias de Ciencias Básicas

Las materias de Ciencias Básicas podrán considerarse dentro de la afinidad relacionada cuando exista una relación académica pertinente entre el perfil del docente y el contenido de la materia.

Las competencias asociadas a la materia permiten complementar esta valoración e identificar conocimientos o habilidades que el docente puede aportar, incluso cuando su carrera de origen no coincida directamente con la carrera en la que se imparte la materia.

La coincidencia de etiquetas no sustituye la valoración del perfil académico y profesional ni genera una restricción automática de asignación.

### RF-09. Selección por parte del Jefe de División

El Jefe de División conserva la decisión final de selección del docente.

Podrá seleccionar a un docente aunque:

* No tenga etiquetas registradas.
* No coincida con las etiquetas activas asociadas a la materia.
* Presente solamente una coincidencia parcial.
* Su afinidad con la materia sea relacionada o complementaria, en lugar de directa.

Esta decisión deberá respetar las demás reglas, validaciones y restricciones aplicables a la asignación académica.

La afinidad por etiquetas no realizará asignaciones automáticas ni bloqueará por sí sola una selección.

### RF-10. Etiquetas inactivas

Las etiquetas inactivas no podrán utilizarse para crear nuevas asociaciones.

Cuando una etiqueta se inactive en el catálogo general, las asociaciones existentes con docentes y materias se conservarán de acuerdo con las reglas funcionales de sus respectivos documentos.

Sin embargo, las etiquetas inactivas **no participarán en las comparaciones actuales de afinidad docente–materia**.

Su conservación responde a la necesidad de mantener las asociaciones y referencias históricas, pero no significa que sigan siendo relevantes para una valoración vigente.

### RF-11. Relevancia de las etiquetas asociadas a materias

Cada etiqueta asociada a una materia cuenta con un nivel de relevancia que indica qué tan importante es esa competencia para impartirla.

Los niveles son:

| Valor | Nivel de relevancia | Interpretación                                                    |
| ----: | ------------------- | ----------------------------------------------------------------- |
|     1 | Baja                | La competencia es complementaria para la materia.                 |
|     2 | Media               | La competencia tiene una relevancia considerable para la materia. |
|     3 | Alta                | La competencia es especialmente relevante para la materia.        |

La relevancia pertenece a la asociación entre la materia y la etiqueta. Por tanto, una misma etiqueta puede tener diferente relevancia en distintas materias.

Al comparar docentes candidatos, el sistema deberá considerar la relevancia de las etiquetas coincidentes. La cantidad de coincidencias no deberá interpretarse de manera aislada de la importancia de las competencias para la materia.

Estos valores no representan el nivel de dominio del docente. El nivel de dominio se registra de manera independiente en la asociación entre el docente y su competencia.

### RF-12. Orden funcional de los docentes candidatos

La presentación de los docentes candidatos deberá respetar el siguiente orden jerárquico:

1. **Afinidad directa:** docentes cuyo perfil académico y carrera presentan una relación directa con la materia.
2. **Afinidad relacionada:** docentes cuyo perfil académico y carrera presentan una relación pertinente, aunque no sea directa, con la materia.
3. **Afinidad complementaria:** docentes cuyo perfil presenta una relación complementaria que puede resultar útil para impartir la materia.

Dentro de cada nivel de afinidad, se considerará la coincidencia entre las competencias vigentes del docente y las etiquetas activas asociadas a la materia, tomando en cuenta la relevancia de las etiquetas para dicha materia.

Cuando los candidatos mantengan condiciones comparables de afinidad y coincidencia, se dará prioridad al docente que tenga un nivel de competencia más alto en las competencias coincidentes relevantes para la materia.

La falta de competencias registradas o de coincidencias no deberá utilizarse como motivo de exclusión automática. El sistema deberá distinguir esos casos y permitir que el Jefe de División evalúe la información disponible.

Este orden funcional no establece una fórmula matemática ni una puntuación numérica de afinidad.

## 6. Influencia en la lista de docentes candidatos

La lista de docentes candidatos deberá proporcionar información útil para que el Jefe de División tome una decisión fundamentada.

Para ello, deberá considerar los criterios en el siguiente orden funcional:

1. **Afinidad del perfil académico y la carrera:** determina el nivel principal de afinidad y tiene prioridad sobre la coincidencia por etiquetas.
2. **Coincidencia de competencias:** permite identificar qué competencias del docente coinciden con las etiquetas activas de la materia.
3. **Relevancia de las etiquetas:** permite distinguir las coincidencias de acuerdo con la importancia que cada competencia tiene para la materia.
4. **Nivel de competencia del docente:** permite diferenciar candidatos cuando los criterios anteriores sean comparables.
5. **Información no disponible o ausencia de coincidencia:** debe distinguirse de una incompatibilidad obligatoria.

La lista podrá mostrar si el docente presenta coincidencias totales, parciales, ninguna coincidencia o si carece de competencias registradas.

Estas situaciones son informativas y no deberán convertirse automáticamente en restricciones de asignación.

La presentación de candidatos deberá respetar los niveles de afinidad definidos en RF-12. Dentro de cada nivel, las coincidencias de competencias y su relevancia servirán para apoyar la comparación; cuando persista una condición comparable, se considerará el nivel de competencia del docente.

Esta definición establece la prioridad funcional de los criterios, pero no prescribe una fórmula numérica ni un algoritmo matemático específico.

## 7. Validaciones y restricciones

* La comparación deberá utilizar las asociaciones de competencias registradas para la materia y para el docente.
* Solo se considerarán las etiquetas activas del catálogo general.
* Las asociaciones existentes con etiquetas inactivas se conservarán, pero no participarán en las comparaciones actuales.
* La coincidencia parcial deberá reconocerse como información relevante.
* La relevancia de cada etiqueta para la materia deberá considerarse al comparar coincidencias.
* La falta de etiquetas no deberá interpretarse como falta de competencia.
* La falta de coincidencia no deberá excluir automáticamente a un docente.
* La coincidencia por etiquetas no deberá tener mayor prioridad que la afinidad del perfil académico y la carrera.
* El nivel de competencia del docente deberá considerarse para diferenciar candidatos cuando los criterios anteriores sean comparables.
* La clasificación de una materia como de especialidad no establecerá por sí misma una restricción adicional.
* Las materias de Ciencias Básicas podrán considerarse dentro de la afinidad relacionada cuando corresponda.
* El Jefe de División deberá conservar la decisión final, sujeta a las demás reglas de asignación académica.
* La valoración no deberá realizar asignaciones automáticas ni bloquear una selección por la sola ausencia de coincidencias.

## 8. Criterios de aceptación

* Está documentada la relación funcional entre las competencias del docente y las asociadas a la materia.
* Se permite que una materia tenga varias etiquetas.
* Se definen los niveles de relevancia de las etiquetas de la materia: 1 (Baja), 2 (Media) y 3 (Alta).
* Se establece que la relevancia de una etiqueta corresponde a su asociación con una materia y puede variar entre materias.
* Se define la coincidencia parcial como información útil para comparar candidatos.
* Se establece que las etiquetas son orientativas y no constituyen una restricción automática.
* Se define el comportamiento de docentes sin etiquetas y de docentes sin coincidencias.
* Se establece que el perfil académico y la carrera tienen mayor relevancia que la afinidad por etiquetas.
* Se definen los niveles de afinidad directa, relacionada y complementaria.
* Se contempla la afinidad relacionada para materias de Ciencias Básicas cuando corresponda.
* Se establece que las materias de especialidad utilizan la misma lógica general de comparación.
* Se define el tratamiento de etiquetas inactivas: se conservan las asociaciones existentes, pero no se consideran en las comparaciones actuales.
* Se establece el orden funcional de los candidatos, considerando primero la afinidad académica y profesional, después las coincidencias y su relevancia, y posteriormente el nivel de competencia cuando los criterios anteriores sean comparables.
* Se establece que el Jefe de División conserva la decisión final de selección.
* Se especifican las responsabilidades de `AcademicService`, `AcademicStaffService` y `SchedulingService`.
* Se evita introducir una fórmula matemática, puntuación o asignación automática no aprobada.

## 9. Dependencias

**PLAN-05:** esta definición depende de las decisiones y entregables correspondientes a dicha tarea.

La implementación posterior deberá utilizar las reglas funcionales aquí documentadas y respetar las responsabilidades establecidas para cada microservicio.

## 10. Fuera de alcance

Esta tarea no contempla:

* Implementar el algoritmo de afinidad.
* Modificar la asignación académica de docentes.
* Crear asignaciones automáticas.
* Definir porcentajes, puntuaciones o fórmulas matemáticas.
* Modificar las reglas de disponibilidad horaria, perfil profesional u otros requisitos de asignación.
* Modificar la estructura de las bases de datos o los contratos de los microservicios.

El objetivo es cerrar el comportamiento funcional antes de implementar la regla.
