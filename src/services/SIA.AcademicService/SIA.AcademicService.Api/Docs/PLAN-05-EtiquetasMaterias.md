# PLAN-05 — Etiquetas asociadas a materias

## 1. Objetivo

Definir las reglas funcionales para asociar etiquetas de competencia del catálogo institucional con materias, indicando la importancia de cada etiqueta para una materia determinada.

Estas asociaciones permitirán describir las competencias relevantes para impartir una materia y aportar información para valorar la compatibilidad de los docentes durante el proceso de asignación de carga académica.

## 2. Alcance funcional

Este documento contempla:

* La consulta y selección de etiquetas de competencia existentes.
* La asociación de etiquetas con materias.
* La asignación y modificación de la importancia de una etiqueta para una materia.
* La consulta de etiquetas asociadas a una materia.
* El retiro de asociaciones entre materias y etiquetas.
* El comportamiento de las asociaciones cuando una etiqueta del catálogo general se inactiva o reactiva.
* El uso de las asociaciones como información de apoyo para valorar la compatibilidad de docentes con las materias.

La administración del catálogo general de etiquetas se define en `PLAN-05-Crear-Etiquetas-TagCompetency.md`.

La asociación de etiquetas con docentes se define por separado en `PLAN-05-EtiquetasDocentes.md`.

## 3. Responsabilidad funcional

El catálogo general de etiquetas de competencia pertenece a **AcademicService**. La asociación entre materias y etiquetas también pertenece funcionalmente a este microservicio, dado que relaciona elementos del catálogo académico.

El **Jefe de División** es el actor autorizado para asociar etiquetas a materias, modificar la importancia de las asociaciones y retirarlas, de acuerdo con sus permisos.

Las asociaciones entre materias y etiquetas no crean ni modifican las etiquetas del catálogo general ni las competencias registradas en el perfil de los docentes.

## 4. Conceptos funcionales

### 4.1. Etiqueta de competencia

Elemento del catálogo general que representa una competencia, conocimiento, habilidad o área de dominio que puede ser relevante para una materia.

Cada etiqueta tiene un estado propio en el catálogo general: activa o inactiva.

### 4.2. Asociación materia-etiqueta

Relación que indica que una etiqueta es relevante para una materia determinada.

Una misma etiqueta puede asociarse con distintas materias y una materia puede tener varias etiquetas asociadas, respetando el límite establecido en este documento.

### 4.3. Importancia de la etiqueta para la materia (`Weight`)

Valor que representa la importancia de una etiqueta para una materia específica.

La importancia pertenece a la asociación materia-etiqueta. No representa una característica global de la etiqueta ni el nivel de dominio que tiene un docente sobre esa competencia.

Se definen tres niveles de importancia:

| Valor | Nivel | Significado funcional                                             |
| ----- | ----- | ----------------------------------------------------------------- |
| 1     | Baja  | La competencia es complementaria para la materia.                 |
| 2     | Media | La competencia tiene una relevancia considerable para la materia. |
| 3     | Alta  | La competencia es especialmente relevante para la materia.        |

La importancia puede variar entre materias. Por ejemplo, una etiqueta puede tener importancia alta para una materia e importancia baja para otra.

## 5. Reglas de negocio

**RN-01. Actor autorizado.** Solo el Jefe de División que cuente con los permisos correspondientes puede asociar etiquetas a materias, modificar la importancia de una asociación o retirarla.

**RN-02. Uso del catálogo general.** Las asociaciones deben utilizar etiquetas existentes en el catálogo general. No se permite crear una etiqueta desde la operación de asociación con una materia.

**RN-03. Etiquetas disponibles.** Solo se pueden crear nuevas asociaciones con etiquetas que estén activas en el catálogo general.

**RN-04. Límite por materia.** Cada materia puede tener como máximo diez asociaciones vigentes con etiquetas.

**RN-05. Alcance del límite.** El límite de diez etiquetas se aplica a la materia del catálogo, independientemente del plan de estudios en el que se utilice. No se calcula por separado para cada plan de estudios.

**RN-06. Límite alcanzado.** Cuando una materia tenga diez asociaciones vigentes, no se podrá asociar otra etiqueta hasta que se retire una asociación vigente.

**RN-07. Conteo de asociaciones.** Únicamente las asociaciones vigentes cuentan para el límite de diez etiquetas. Las asociaciones retiradas no cuentan como vigentes.

**RN-08. No duplicidad.** No se permite tener más de una asociación vigente entre una misma materia y una misma etiqueta.

**RN-09. Importancia obligatoria.** Al crear una asociación, el Jefe de División debe asignar uno de los tres niveles de importancia permitidos: Baja, Media o Alta.

**RN-10. Importancia por asociación.** La importancia se determina individualmente para cada asociación materia-etiqueta. Modificarla no afecta la importancia que tenga esa etiqueta en otras materias.

**RN-11. Modificación de importancia.** El Jefe de División puede modificar la importancia de una asociación vigente. El nuevo valor sustituye al anterior como valor actual.

**RN-12. Inactivación de una etiqueta.** Cuando una etiqueta del catálogo general se inactiva, sus asociaciones existentes con materias se conservan y no se retiran automáticamente.

**RN-13. Etiquetas inactivas.** Mientras una etiqueta permanezca inactiva, no se permite crear nuevas asociaciones con ella, aunque existan asociaciones previas que se conserven.

**RN-14. Reactivación de una etiqueta.** Cuando una etiqueta se reactive en el catálogo general, podrá utilizarse en nuevas asociaciones, siempre que se cumplan las demás reglas de negocio.

**RN-15. Retiro de una asociación.** Retirar una asociación deja de considerar vigente la relación entre la materia y la etiqueta. No elimina la etiqueta del catálogo general ni modifica su estado global.

**RN-16. Efecto limitado del retiro.** Retirar una asociación no afecta las asociaciones de la misma etiqueta con otras materias ni modifica las competencias registradas en los perfiles de docentes.

**RN-17. Independencia de estados.** El estado de la asociación materia-etiqueta es independiente del estado global de la etiqueta. Una etiqueta puede estar activa sin estar asociada a una materia; asimismo, una asociación existente puede conservarse aunque la etiqueta esté inactiva.

**RN-18. Uso como criterio de apoyo.** Las etiquetas asociadas a una materia aportan información para valorar la compatibilidad de los docentes, pero no determinan automáticamente quién debe impartirla.

**RN-19. Importancia de la etiqueta.** El nivel de importancia registrado para cada etiqueta debe considerarse al valorar su relevancia para una materia. Una etiqueta de importancia alta representa una competencia más relevante para esa materia que una de importancia media o baja.

**RN-20. Coincidencia parcial.** La coincidencia parcial entre las etiquetas de una materia y las competencias de un docente puede aportar información útil para valorar su compatibilidad. No se exige una coincidencia total para que un docente pueda ser considerado.

**RN-21. Docentes sin etiquetas.** La ausencia de competencias etiquetadas en el perfil de un docente no lo excluye automáticamente como candidato para impartir una materia.

**RN-22. Etiquetas inactivas en la valoración.** Las etiquetas inactivas no deben considerarse competencias vigentes para la valoración de coincidencias ni utilizarse para crear nuevas asociaciones.

**RN-23. Materias de especialidad.** Las etiquetas asociadas a materias de especialidad aportan información sobre las competencias relevantes, pero no constituyen por sí solas una restricción automática para considerar a un docente.

**RN-24. Decisión final de asignación.** La información de etiquetas y sus niveles de importancia no realiza una asignación automática de docentes ni sustituye la decisión del Jefe de División durante el proceso de asignación de carga académica.

**RN-25. Sin fórmula automática definida.** Los niveles de importancia son valores cualitativos de tres niveles. Este documento no establece porcentajes, fórmulas matemáticas ni una puntuación automática de compatibilidad.

## 6. Flujo principal: asociar una etiqueta a una materia

1. El Jefe de División selecciona la materia a la que desea asociar una etiqueta.
2. El sistema presenta las etiquetas activas disponibles en el catálogo general.
3. El Jefe de División selecciona la etiqueta que desea asociar.
4. El sistema valida que el actor tenga autorización para realizar la operación.
5. El sistema verifica que la etiqueta seleccionada exista y esté activa.
6. El sistema verifica que no exista una asociación vigente entre esa materia y esa etiqueta.
7. El sistema verifica que la materia tenga menos de diez asociaciones vigentes.
8. El Jefe de División asigna la importancia de la etiqueta: Baja, Media o Alta.
9. El sistema valida que la importancia seleccionada corresponda a uno de los tres niveles permitidos.
10. El sistema registra la asociación como vigente y guarda su importancia.
11. El sistema confirma que la asociación se realizó correctamente.

## 7. Flujo principal: consultar etiquetas de una materia

1. El Jefe de División selecciona una materia.
2. El sistema presenta las etiquetas asociadas a esa materia y el nivel de importancia de cada una.
3. El sistema muestra el estado global de las etiquetas asociadas para distinguir aquellas que están activas de las que fueron inactivadas en el catálogo general.
4. El sistema permite identificar las asociaciones vigentes.

La consulta de asociaciones retiradas y su presentación como historial quedan fuera del alcance de este documento, salvo que se defina posteriormente ese comportamiento.

## 8. Flujo principal: modificar la importancia de una asociación

1. El Jefe de División selecciona una asociación vigente de la materia.
2. El sistema muestra la importancia actual de la etiqueta para esa materia.
3. El Jefe de División selecciona el nuevo nivel de importancia.
4. El sistema valida que el nivel seleccionado sea Baja, Media o Alta.
5. El sistema actualiza la importancia de la asociación.
6. El sistema confirma que la modificación se realizó correctamente.

La modificación de importancia no crea una asociación adicional ni modifica el estado global de la etiqueta.

## 9. Flujo principal: retirar una asociación

1. El Jefe de División selecciona una asociación vigente de una materia.
2. El sistema solicita confirmar el retiro.
3. El sistema valida que el actor tenga autorización para realizar la operación.
4. El sistema deja de considerar vigente la asociación seleccionada.
5. El sistema confirma el retiro.

El retiro no elimina la etiqueta del catálogo general, no modifica su estado global y no afecta las asociaciones que tenga con otras materias.

## 10. Validaciones y excepciones

| Código | Situación                                                       | Resultado esperado                                                                                         |
| ------ | --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| VE-01  | Actor sin autorización                                          | Rechazar la operación.                                                                                     |
| VE-02  | Etiqueta inexistente en el catálogo general                     | Rechazar la asociación.                                                                                    |
| VE-03  | Etiqueta inactiva al intentar crear una asociación              | Rechazar la nueva asociación.                                                                              |
| VE-04  | Ya existe una asociación vigente entre la materia y la etiqueta | Rechazar la duplicidad.                                                                                    |
| VE-05  | La materia tiene diez asociaciones vigentes                     | Rechazar una nueva asociación hasta que se retire una vigente.                                             |
| VE-06  | Importancia distinta de Baja, Media o Alta                      | Rechazar el valor.                                                                                         |
| VE-07  | Se intenta modificar la importancia de una asociación retirada  | Rechazar la modificación.                                                                                  |
| VE-08  | Se intenta retirar una asociación que no está vigente           | Informar que no existe una asociación vigente que retirar.                                                 |
| VE-09  | La etiqueta se inactiva después de haber sido asociada          | Conservar la asociación existente y bloquear nuevas asociaciones mientras la etiqueta permanezca inactiva. |
| VE-10  | Se reactiva una etiqueta                                        | Permitir nuevas asociaciones si se cumplen las demás reglas.                                               |

## 11. Seguridad

* Validar los permisos del Jefe de División en cada operación de asociación, modificación de importancia y retiro.
* No depender únicamente de las opciones que se muestren u oculten en la interfaz.
* Evitar que una operación sobre una materia afecte las asociaciones de otras materias.
* Respetar el estado global de la etiqueta al crear asociaciones y al valorar coincidencias para la asignación docente.

## 12. Información consumida y generada

### 12.1. Información consumida

* Materia seleccionada.
* Etiqueta existente en el catálogo general.
* Estado global de la etiqueta.
* Asociaciones vigentes de la materia.
* Nivel de importancia seleccionado.
* Identidad y permisos del Jefe de División que realiza la operación.

### 12.2. Información generada o actualizada

* Asociación vigente entre una materia y una etiqueta.
* Nivel de importancia vigente de la asociación.
* Modificación de la importancia de una asociación existente.
* Retiro de una asociación.
* Resultado de validación o mensaje de error correspondiente.

## 13. Fuera de alcance

Este documento no define:

* La creación, modificación, activación o inactivación de etiquetas del catálogo general.
* La asociación de etiquetas con docentes ni los niveles de competencia registrados en sus perfiles.
* La asignación automática de docentes a materias.
* Una fórmula matemática o puntuación automática de compatibilidad.
* El diseño de tablas, relaciones físicas, claves foráneas, endpoints, DTO, comandos, eventos o detalles de implementación técnica.
* Un historial detallado de los cambios de importancia o de las asociaciones retiradas.
