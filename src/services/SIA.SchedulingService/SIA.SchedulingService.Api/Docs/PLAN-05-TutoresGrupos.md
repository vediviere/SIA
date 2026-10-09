# PLAN-05 — Asignación de tutores académicos a grupos aperturados

## 1. Objetivo

Definir las reglas funcionales para asignar, sustituir y retirar tutores académicos de los grupos aperturados en SIA, garantizando la relación de cada asignación con la carga académica individual del docente y la conservación del histórico de tutorías.

La asignación de tutor académico se considera una actividad de apoyo con un comportamiento específico. Por ello, su registro se administra mediante una asignación de tutoría vinculada al grupo y a la carga académica correspondiente, sin generar un registro adicional de actividad complementaria para representar la misma tutoría.

## 2. Alcance

Este documento contempla:

* Asignación de un docente como tutor académico de un grupo aperturado.
* Relación de la asignación con la carga académica individual del docente tutor.
* Restricción de un máximo de un tutor vigente por grupo durante un mismo periodo.
* Asignación de un mismo docente como tutor de varios grupos.
* Sustitución de un tutor durante el mismo periodo.
* Retiro lógico de una asignación de tutoría.
* Conservación del histórico de tutores asignados en periodos anteriores.
* Definición de las horas asociadas a la tutoría.

### Fuera del alcance

Este documento no define:

* La creación y configuración general de los grupos.
* La apertura de materias ni la planeación académica completa.
* La distribución general de materias y actividades de apoyo entre docentes.
* La asignación de tutores mediante criterios de afinidad o competencias.
* El seguimiento de las actividades realizadas por el tutor.
* La evaluación del desempeño del tutor.
* Los procedimientos de baja o cambio de adscripción de docentes.
* El diseño técnico de endpoints, DTO, comandos, eventos o contratos entre microservicios.

Estos procesos y aspectos deberán documentarse en sus respectivos documentos funcionales o técnicos.

## 3. Responsabilidades

### 3.1. Jefe de División

Es el responsable de asignar tutores académicos a los grupos aperturados comprendidos en su ámbito de autorización.

Sus responsabilidades son:

* Seleccionar al docente que desempeñará la función de tutor académico.
* Realizar la asignación durante la creación de la carga académica.
* Sustituir al tutor cuando sea necesario.
* Retirar una asignación de tutoría cuando corresponda.
* Garantizar que las operaciones respeten las reglas de negocio establecidas.

Todas las operaciones deben estar sujetas a la autorización correspondiente.

### 3.2. Docente tutor

Es el docente al que se asigna la responsabilidad de tutoría académica de uno o varios grupos aperturados.

Para ser elegible, basta con contar con el perfil de docente. No se establece como requisito adicional tener una materia asignada al grupo que tutorizará.

Un docente puede ser tutor de varios grupos, siempre que cada asignación cumpla las reglas de negocio y se relacione con su carga académica individual correspondiente.

## 4. Conceptos funcionales

### 4.1. Carga académica individual

Representa la carga laboral de un docente para una carrera y un periodo académico determinados.

Se identifica mediante la combinación de:

* Docente.
* Carrera o programa educativo.
* Periodo académico.

Durante la planeación académica se distribuyen las materias y las actividades de apoyo entre los docentes. Una vez definidas las asignaciones, cada docente cuenta con su propia carga académica para la carrera y el periodo correspondientes.

Una carga académica puede contener materias, actividades de apoyo o ambas. No es obligatorio que todas las cargas académicas incluyan materias.

### 4.2. Grupo aperturado

Es el grupo habilitado como resultado del proceso académico correspondiente y asociado a las materias que se impartirán durante un periodo.

Solo los grupos aperturados para materias son elegibles para recibir una asignación de tutor académico.

### 4.3. Asignación de tutoría

Es el registro que establece la relación entre un grupo aperturado, el docente que lo atiende como tutor y la carga académica individual correspondiente.

La asignación se representa mediante la entidad `GroupTutors`.

Este registro constituye la representación de la tutoría para efectos de su administración. No debe generarse otro registro de actividad complementaria para representar la misma asignación de tutoría.

### 4.4. Asignación vigente

Es una asignación de tutoría que permanece activa y se considera válida para el grupo y periodo correspondientes.

Una asignación retirada deja de considerarse vigente, pero su información se conserva para mantener el histórico.

### 4.5. Histórico de tutorías

Es el conjunto de registros de asignaciones anteriores que permite conocer qué docentes fueron tutores de los grupos en periodos pasados y conservar la trazabilidad de las sustituciones realizadas.

Las asignaciones anteriores no deben eliminarse físicamente ni reutilizarse para representar una nueva asignación.

## 5. Reglas de negocio

### RN-01. Responsable de la asignación

El Jefe de División autorizado es el responsable de asignar, sustituir y retirar tutores académicos de los grupos comprendidos en su ámbito de responsabilidad.

### RN-02. Momento de asignación

La asignación del tutor académico se realiza durante la creación de la carga académica.

La asignación debe relacionarse con la carga académica individual del docente que desempeñará la tutoría.

### RN-03. Elegibilidad del docente

Para ser asignado como tutor académico, basta con que la persona cuente con el perfil de docente.

No se requiere que tenga una materia asignada al grupo que atenderá como tutor.

### RN-04. Elegibilidad del grupo

Únicamente se pueden asignar tutores a grupos aperturados para las materias del periodo académico correspondiente.

No se permite crear una asignación de tutoría para un grupo que no cumpla esta condición.

### RN-05. Máximo de un tutor vigente por grupo y periodo

Cada grupo puede tener como máximo un tutor académico con asignación vigente durante un mismo periodo académico.

Si es necesario cambiar al tutor, debe retirarse la asignación anterior antes de registrar la nueva.

### RN-06. Un docente puede tutorizar varios grupos

Un mismo docente puede ser asignado como tutor académico de varios grupos.

Cada asignación debe registrarse de manera independiente y conservar su relación con la carga académica individual del docente correspondiente.

### RN-07. Relación obligatoria con la carga académica

Toda asignación de tutoría debe estar relacionada con la carga académica individual del docente que desempeña la función de tutor.

La carga académica debe corresponder al docente, la carrera y el periodo de la asignación.

No se permite utilizar la carga académica de otro docente para representar la tutoría.

### RN-08. Sustitución del tutor

Cuando se sustituya al tutor durante el mismo periodo académico, se debe retirar lógicamente la asignación anterior y crear un nuevo registro para el nuevo tutor.

La nueva asignación debe utilizar la carga académica individual del docente que asumirá la tutoría.

No se permite modificar el registro anterior para convertirlo en la asignación del nuevo tutor.

### RN-09. Retiro de una asignación

El Jefe de División autorizado puede retirar una asignación de tutoría cuando ya no deba mantenerse vigente.

El retiro se realiza mediante borrado lógico, sin eliminar físicamente el registro.

Si el retiro no implica una sustitución, el grupo queda sin una asignación de tutoría vigente hasta que se registre otra, si corresponde.

### RN-10. Conservación del histórico

Las asignaciones retiradas deben conservarse para mantener el histórico de tutorías.

El histórico debe permitir identificar las asignaciones anteriores y los docentes que desempeñaron la función de tutor en periodos pasados.

La creación de una nueva asignación no debe sobrescribir ni eliminar los registros anteriores.

### RN-11. Independencia de las asignaciones

Cada relación entre un tutor y un grupo constituye una asignación independiente.

Retirar o sustituir al tutor de un grupo no debe modificar las asignaciones vigentes de ese mismo docente en otros grupos.

### RN-12. Horas de tutoría

La asignación de tutoría contempla actualmente dos horas.

Se conserva el atributo funcional de horas asociado a la asignación para permitir flexibilidad futura. Su valor inicial y las modificaciones que pudieran requerirse deberán respetar las reglas vigentes de la institución.

### RN-13. Representación de la tutoría

La tutoría se administra directamente mediante la asignación correspondiente al grupo.

No debe registrarse una segunda actividad complementaria para representar la misma tutoría, aunque las actividades de apoyo en general formen parte de la carga académica.

### RN-14. Independencia de las materias en la carga académica

Una carga académica puede contener únicamente actividades de apoyo, únicamente materias o ambas.

La ausencia de materias asignadas directamente al docente tutor no impide que este pueda desempeñar la tutoría de un grupo elegible, siempre que cumpla las demás reglas de negocio.

## 6. Flujos funcionales

### 6.1. Asignar un tutor académico

**Actor:** Jefe de División autorizado.

**Precondiciones:**

* El Jefe de División cuenta con autorización para administrar la asignación.
* El grupo está aperturado para una materia.
* El docente seleccionado cuenta con el perfil de docente.
* Existe una carga académica individual correspondiente al docente, la carrera y el periodo.
* El grupo no tiene otro tutor con asignación vigente para ese periodo.

**Flujo principal:**

1. El Jefe de División inicia la creación de la carga académica.
2. Selecciona al docente que desempeñará la tutoría.
3. Selecciona el grupo aperturado que recibirá la asignación.
4. El sistema verifica que el grupo sea elegible y que no tenga otro tutor vigente durante el periodo.
5. El sistema verifica que el docente tenga el perfil requerido y que exista una carga académica correspondiente.
6. El sistema registra la asignación de tutoría vinculada al grupo y a la carga académica del docente.
7. El sistema establece las horas correspondientes a la asignación conforme a las reglas vigentes.
8. El sistema confirma que la asignación se registró correctamente.

**Excepciones:**

* Si el grupo no está aperturado para una materia, el sistema impide la asignación.
* Si el grupo ya tiene un tutor vigente durante el mismo periodo, el sistema impide registrar otro.
* Si el docente no cuenta con el perfil requerido, el sistema impide la asignación.
* Si no existe una carga académica correspondiente al docente, la carrera y el periodo, el sistema impide registrar la asignación.
* Si el usuario no tiene autorización, el sistema rechaza la operación.

### 6.2. Sustituir al tutor académico

**Actor:** Jefe de División autorizado.

**Precondiciones:**

* Existe una asignación de tutoría vigente para el grupo.
* El Jefe de División está autorizado para administrar la asignación.
* El nuevo docente cumple el requisito de elegibilidad.
* Existe una carga académica individual del nuevo docente para la carrera y el periodo correspondientes.

**Flujo principal:**

1. El Jefe de División identifica la asignación de tutoría que debe sustituirse.
2. Selecciona al nuevo docente tutor.
3. El sistema valida que el nuevo docente sea elegible y que exista su carga académica correspondiente.
4. El sistema retira lógicamente la asignación anterior.
5. El sistema crea un nuevo registro de asignación para el nuevo docente.
6. El nuevo registro queda relacionado con la carga académica individual del nuevo tutor.
7. El sistema conserva el registro anterior para el histórico.
8. El sistema confirma la sustitución.

**Excepciones:**

* Si el nuevo docente no cumple las condiciones de elegibilidad, el sistema impide la sustitución.
* Si no existe una carga académica correspondiente al nuevo docente, la carrera y el periodo, el sistema impide crear la nueva asignación.
* Si el usuario no tiene autorización, el sistema rechaza la operación.

### 6.3. Retirar una asignación de tutoría

**Actor:** Jefe de División autorizado.

**Precondiciones:**

* Existe una asignación de tutoría vigente.
* El Jefe de División cuenta con autorización para administrarla.

**Flujo principal:**

1. El Jefe de División consulta la asignación vigente.
2. Selecciona la asignación que desea retirar.
3. Solicita el retiro.
4. El sistema verifica que la asignación exista y esté vigente.
5. El sistema realiza el retiro lógico de la asignación.
6. El sistema conserva el registro para el histórico.
7. El sistema confirma el retiro.

**Excepciones:**

* Si la asignación no existe o ya fue retirada, el sistema impide realizar nuevamente la operación.
* Si el usuario no tiene autorización, el sistema rechaza la operación.

## 7. Validaciones y restricciones

| Elemento            | Validación                                                                                 |
| ------------------- | ------------------------------------------------------------------------------------------ |
| Autorización        | Solo el Jefe de División autorizado puede administrar las asignaciones.                    |
| Docente             | Debe contar con el perfil de docente.                                                      |
| Grupo               | Debe estar aperturado para una materia.                                                    |
| Carga académica     | Debe corresponder al docente, la carrera y el periodo de la asignación.                    |
| Unicidad de tutoría | Un grupo no puede tener más de un tutor vigente durante el mismo periodo.                  |
| Varios grupos       | Un docente puede tutorizar varios grupos con asignaciones independientes.                  |
| Sustitución         | Debe retirarse lógicamente la asignación anterior y crearse un registro nuevo.             |
| Retiro              | La asignación deja de estar vigente, pero el registro se conserva.                         |
| Histórico           | Los registros anteriores no deben sobrescribirse ni eliminarse físicamente.                |
| Horas               | La asignación contempla actualmente dos horas.                                             |
| Representación      | No debe generarse un registro adicional de actividad complementaria para la misma tutoría. |

## 8. Criterios de aceptación

### CA-01. Asignación de tutor

**Dado** que el Jefe de División está autorizado, el grupo está aperturado y el docente cuenta con una carga académica correspondiente,

**cuando** se registra la asignación de tutoría,

**entonces** el sistema crea una asignación vigente relacionada con el grupo y con la carga académica individual del docente.

### CA-02. Docente sin materia asignada al grupo

**Dado** que un docente cuenta con el perfil requerido y con una carga académica correspondiente,

**cuando** el Jefe de División lo asigna como tutor de un grupo elegible en el que no imparte materias,

**entonces** el sistema permite la asignación, siempre que se cumplan las demás reglas de negocio.

### CA-03. Grupo no elegible

**Dado** que un grupo no está aperturado para una materia,

**cuando** el Jefe de División intenta asignarle un tutor,

**entonces** el sistema impide la operación.

### CA-04. Un solo tutor por grupo y periodo

**Dado** que un grupo ya tiene una asignación de tutoría vigente durante un periodo,

**cuando** se intenta registrar otro tutor para el mismo grupo y periodo sin retirar la asignación anterior,

**entonces** el sistema impide crear una segunda asignación vigente.

### CA-05. Un docente en varios grupos

**Dado** que un docente tiene una carga académica correspondiente y cumple las condiciones requeridas,

**cuando** se le asigna como tutor de varios grupos elegibles,

**entonces** el sistema permite registrar asignaciones independientes para cada grupo, conservando la relación con su carga académica.

### CA-06. Sustitución de tutor

**Dado** que existe una asignación de tutoría vigente y se requiere sustituir al docente,

**cuando** el Jefe de División realiza la sustitución,

**entonces** el sistema retira lógicamente la asignación anterior y crea un nuevo registro relacionado con la carga académica del nuevo tutor.

### CA-07. Conservación del histórico

**Dado** que un grupo ha tenido distintos tutores,

**cuando** se realizan sustituciones durante uno o varios periodos,

**entonces** los registros anteriores se conservan y permiten conocer las asignaciones históricas.

### CA-08. Retiro sin sustitución

**Dado** que existe una asignación de tutoría vigente,

**cuando** el Jefe de División solicita retirarla sin asignar otro tutor,

**entonces** el sistema deja de considerarla vigente y conserva el registro histórico.

### CA-09. Independencia entre grupos

**Dado** que un docente es tutor de varios grupos,

**cuando** se retira o sustituye su asignación en uno de ellos,

**entonces** las asignaciones correspondientes a los demás grupos permanecen sin cambios.

### CA-10. Control de autorización

**Dado** que un usuario no está autorizado para administrar la asignación de tutoría,

**cuando** intenta crear, sustituir o retirar una asignación,

**entonces** el sistema rechaza la operación.

### CA-11. Representación de la tutoría

**Dado** que se registra una asignación de tutoría,

**cuando** el sistema guarda la información,

**entonces** la tutoría queda representada por la asignación correspondiente y no se genera un segundo registro de actividad complementaria para la misma función.

## 9. Dependencias funcionales

La funcionalidad depende de:

* **Gestión de docentes:** permite identificar al docente que desempeñará la tutoría.
* **Planeación académica y carga académica:** proporciona la carga individual correspondiente al docente, la carrera y el periodo.
* **Gestión de grupos:** proporciona los grupos aperturados para las materias.
* **Control de acceso:** permite validar que el Jefe de División esté autorizado para administrar la asignación.

La asignación de tutorías debe integrarse con estos procesos respetando las responsabilidades funcionales de cada componente de SIA.

## 10. Fuera de alcance y consideraciones pendientes

Este documento no define el seguimiento de las tutorías, el registro de sesiones, las actividades realizadas por el tutor, los resultados de atención a estudiantes ni la evaluación del desempeño del docente.

Tampoco establece procedimientos para consultar reportes históricos específicos, ni define mecanismos técnicos de auditoría o integración entre microservicios más allá de las reglas funcionales descritas.

Cualquier ampliación relacionada con estos temas deberá definirse por separado antes de incorporarse al alcance de esta funcionalidad.
