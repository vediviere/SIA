# Reglas de trabajo para juniors, seniors y líder técnico

## Objetivo

Este documento define las reglas iniciales para asignar trabajo técnico dentro de SIA según el nivel de experiencia y el riesgo de cada tarea.

El objetivo no es impedir que los desarrolladores con menor experiencia participen en partes importantes del sistema, sino evitar que cambios de alto impacto se realicen sin la supervisión necesaria.

Las reglas buscan:

* Proteger la arquitectura de SIA.
* Reducir errores derivados de decisiones técnicas sin contexto suficiente.
* Permitir que los juniors aporten valor desde los primeros sprints.
* Facilitar el aprendizaje progresivo.
* Distribuir correctamente las responsabilidades.
* Evitar que todo el trabajo dependa del líder técnico.
* Utilizar Jira como apoyo para identificar el nivel de riesgo de cada tarea.

## Principio general

La asignación de una tarea debe considerar dos factores:

1. La dificultad de implementación.
2. El impacto que un error podría tener sobre la plataforma.

Una tarea técnicamente pequeña puede seguir siendo crítica si modifica una decisión transversal.

Por ejemplo:

Modificar un DTO interno de un servicio puede ser una tarea sencilla.

Modificar un contrato utilizado por varios servicios puede tener pocas líneas de código, pero producir un impacto transversal importante.

Por esta razón, el nivel de una tarea no se determina únicamente por cuánto código requiere.

---

# 1. Juniors

Los desarrolladores junior participan desde el inicio del proyecto y deben recibir tareas reales que les permitan aprender la arquitectura y contribuir al producto.

Sin embargo, sus primeras tareas deben realizarse dentro de caminos técnicos previamente definidos por seniors o por el líder técnico.

## 1.1 Tareas que puede tomar un junior

Un junior puede ser responsable de tareas como:

* Documentación técnica guiada.
* Actualización de README.
* DTOs simples.
* Requests y Responses simples.
* Validadores.
* Catálogos.
* Mapeos sencillos.
* Pruebas unitarias.
* Casos de prueba previamente definidos.
* Pantallas simples.
* Formularios.
* Componentes visuales dentro de una arquitectura frontend existente.
* Endpoints pequeños dentro de servicios ya estructurados.
* Consultas simples dentro del servicio propietario de los datos.
* Correcciones pequeñas y claramente delimitadas.
* Ajustes de mensajes o validaciones.
* Tareas explícitamente marcadas como `junior-friendly`.

Estas tareas deben contar con:

* Descripción clara.
* Criterios de aceptación.
* Servicio o componente identificado.
* Alcance limitado.
* Patrón de implementación existente cuando corresponda.

## 1.2 Qué significa junior-friendly

La etiqueta:

`junior-friendly`

indica que la tarea puede ser tomada por un desarrollador junior como responsable principal de implementación.

No significa:

* Que la tarea no necesita revisión.
* Que el junior puede cambiar arquitectura libremente.
* Que puede ampliar el alcance de la tarea sin consultar.
* Que puede modificar contratos o componentes transversales relacionados sin autorización.

Una tarea `junior-friendly` sigue utilizando el flujo normal:

Tarea
→ implementación
→ pruebas
→ Pull Request
→ revisión
→ integración

## 1.3 Tareas que un junior no debe modificar sin supervisión

Un junior no debe asumir de manera independiente cambios relacionados con:

* Arquitectura base.
* Límites entre servicios.
* Creación de nuevos servicios.
* Comunicación entre servicios.
* Contratos utilizados por varios servicios.
* Eventos de integración.
* Eventos globales o transversales.
* Message Bus.
* RabbitMQ.
* Outbox e Inbox cuando implique modificar el patrón compartido.
* Seguridad.
* Autenticación.
* Autorización.
* Tokens.
* Multi-tenancy.
* Aislamiento entre tenants.
* Gateways.
* Enrutamiento central.
* BFFs críticos.
* Building Blocks compartidos.
* Migraciones destructivas o de alto impacto.
* Cambios estructurales importantes de base de datos.
* Workflow central.
* Configuración crítica de infraestructura.
* Pipelines de despliegue.
* Reglas arquitectónicas.
* Pruebas de arquitectura.
* Decisiones que afecten más de un dominio.

Un junior puede participar en estas tareas con fines de aprendizaje o implementación parcial, pero debe trabajar bajo supervisión de un senior o del líder técnico.

## 1.4 Qué debe hacer un junior si encuentra un problema arquitectónico

Si durante una tarea un junior detecta que necesita modificar:

* Otro servicio.
* Un contrato compartido.
* Un evento.
* Un Building Block.
* Una base de datos ajena.
* Una regla de seguridad.
* Una decisión de arquitectura.

debe detener esa parte del cambio y escalarla.

No debe resolver el problema creando una dependencia, duplicando información o agregando comunicación improvisada.

---

# 2. Seniors

Los desarrolladores senior son responsables de las áreas donde una decisión incorrecta puede producir acoplamiento, deuda técnica o impacto sobre varios componentes de SIA.

Su responsabilidad no consiste únicamente en implementar tareas difíciles.

También deben crear caminos técnicos que otros desarrolladores puedan seguir.

## 2.1 Responsabilidades de los seniors

Los seniors deben encargarse principalmente de:

* Diseño interno de servicios.
* Definición de límites de dominio junto con el líder técnico.
* Diseño de agregados y entidades importantes.
* Contratos entre servicios.
* Eventos de integración.
* Comunicación asíncrona.
* Outbox e Inbox.
* Integraciones mediante RabbitMQ.
* Persistencia de alto impacto.
* Migraciones importantes.
* Seguridad.
* Autenticación y autorización.
* Multi-tenancy.
* Infraestructura.
* Gateways.
* BFFs críticos.
* Building Blocks.
* Pruebas de arquitectura.
* Pruebas de integración críticas.
* Estrategias de resiliencia.
* Observabilidad distribuida.
* Revisión de Pull Requests de alto impacto.
* Investigación y resolución de problemas técnicos complejos.

## 2.2 Responsabilidad de crear patrones repetibles

Cuando un senior implementa por primera vez un patrón que posteriormente utilizarán otros desarrolladores, debe procurar que la implementación resulte:

* Clara.
* Repetible.
* Documentada cuando sea necesario.
* Fácil de comparar con implementaciones futuras.

Ejemplos:

* Primer consumidor de un tipo de evento.
* Implementación base de Outbox.
* Estructura estándar de un servicio.
* Estrategia de validación.
* Manejo común de errores.
* Autorización.
* Persistencia.
* Integración con otro dominio.

El objetivo es evitar que cada desarrollador resuelva el mismo problema de una manera diferente.

## 2.3 Revisión de trabajo junior

Los seniors deben revisar tareas junior cuando el cambio afecte su área de responsabilidad.

La revisión no debe limitarse a verificar que el código compile.

También debe comprobar:

* Que el cambio esté dentro del servicio correcto.
* Que no exista acceso a bases de datos ajenas.
* Que no se compartan entidades internas.
* Que no se duplique una responsabilidad existente.
* Que no se introduzcan dependencias innecesarias.
* Que se respeten los patrones existentes.
* Que los criterios de aceptación estén cumplidos.

---

# 3. Líder técnico

El líder técnico es responsable de proteger la coherencia general de SIA.

Su función principal no es implementar todas las partes críticas personalmente, sino garantizar que las decisiones tomadas por diferentes desarrolladores continúen formando una arquitectura consistente.

## 3.1 Responsabilidades del líder técnico

El líder técnico debe:

* Proteger la arquitectura general.
* Mantener los límites entre dominios.
* Revisar decisiones transversales.
* Resolver conflictos de responsabilidad entre servicios.
* Validar cambios que puedan introducir deuda técnica estructural.
* Aprobar cambios arquitectónicos importantes.
* Definir reglas de crecimiento del sistema.
* Determinar cuándo una nueva responsabilidad necesita un servicio propio.
* Evitar dependencias indebidas entre servicios.
* Validar decisiones relacionadas con Building Blocks.
* Revisar cambios críticos de infraestructura.
* Participar en decisiones de seguridad y multi-tenancy.
* Mantener las reglas de gobierno técnico.
* Coordinar con los seniors las decisiones de mayor impacto.
* Evitar que soluciones temporales se conviertan accidentalmente en arquitectura permanente.

## 3.2 El líder técnico no debe convertirse en cuello de botella

No todos los Pull Requests necesitan aprobación directa del líder técnico.

Los seniors pueden revisar y aprobar cambios dentro de los patrones y límites ya establecidos.

El líder debe intervenir principalmente cuando exista:

* Una decisión nueva de arquitectura.
* Un cambio transversal.
* Un conflicto entre dominios.
* Una modificación de contratos importantes.
* Una modificación de Building Blocks.
* Un cambio significativo de seguridad.
* Un cambio de estrategia de integración.
* Una nueva dependencia tecnológica crítica.
* Una modificación importante de infraestructura.
* Una excepción a una regla arquitectónica existente.

---

# 4. Tareas críticas

Una tarea se considera crítica cuando un error puede afectar la arquitectura, seguridad, aislamiento de datos o funcionamiento de varios componentes.

Ejemplos:

* Cambiar contratos entre servicios.
* Crear o modificar eventos de integración.
* Modificar seguridad.
* Modificar autenticación o autorización.
* Modificar multi-tenancy.
* Cambiar el ownership de información.
* Crear comunicación entre dos servicios.
* Crear un nuevo servicio.
* Modificar un Building Block.
* Modificar Gateway o infraestructura transversal.
* Realizar una migración destructiva.
* Modificar el patrón Outbox o Inbox compartido.
* Cambiar políticas de resiliencia.
* Cambiar mecanismos de despliegue.
* Cambiar reglas arquitectónicas.

Estas tareas requieren revisión de un senior o del líder técnico según su alcance.

---

# 5. Etiqueta senior-only

La etiqueta:

`senior-only`

indica que la tarea contiene decisiones o cambios cuyo responsable principal debe ser un senior o el líder técnico.

Puede utilizarse en tareas relacionadas con:

* Arquitectura.
* Seguridad.
* Multi-tenancy.
* Eventos.
* Contratos.
* Infraestructura.
* Building Blocks.
* Integraciones críticas.
* Migraciones importantes.
* Gateways.
* Workflow central.
* Decisiones transversales.

`senior-only` no significa que un junior tenga prohibido participar.

Un junior puede:

* Acompañar la implementación.
* Implementar una parte delimitada.
* Crear pruebas.
* Documentar.
* Revisar el flujo.
* Participar en pair programming.
* Utilizar la tarea como aprendizaje.

Sin embargo, no debe asumir de manera independiente:

* El diseño.
* Las decisiones arquitectónicas.
* La aprobación técnica final.
* El ownership completo del cambio.

---

# 6. Tareas que requieren revisión senior

Una tarea puede no ser `senior-only`, pero requerir revisión senior si durante su implementación toca una parte crítica.

Ejemplo:

Una tarea inicialmente consiste en agregar un endpoint simple.

Durante el desarrollo se descubre que necesita publicar un nuevo evento de integración.

El endpoint puede continuar siendo implementado por el junior, pero el diseño y contrato del nuevo evento debe revisarse con un senior.

La aparición de una necesidad técnica nueva puede cambiar el nivel de riesgo de una tarea.

El desarrollador no debe ampliar automáticamente su alcance.

---

# 7. Uso de estas reglas en Jira

Las reglas de este documento deben utilizarse durante refinamiento, planificación y asignación de tareas.

## 7.1 Tarea junior-friendly

Ejemplo:

`Crear validador para registro de aula`

Etiquetas:

`backend`
`junior-friendly`

Puede asignarse a un junior.

## 7.2 Tarea senior-only

Ejemplo:

`Definir contrato de integración entre SchedulingService y AcademicStaffService`

Etiquetas:

`backend`
`event-driven`
`senior-only`

Debe tener como responsable principal un senior o al líder técnico.

## 7.3 Tarea normal con revisión senior

Ejemplo:

`Agregar endpoint para actualizar disponibilidad docente`

La implementación puede ser realizada por un desarrollador que conozca el servicio.

Si la tarea modifica un contrato público o genera un nuevo evento, esa parte debe recibir revisión senior.

---

# 8. Asignación de tareas

Antes de asignar una tarea deben responderse estas preguntas:

1. ¿El alcance está claro?
2. ¿Afecta un solo servicio o varios?
3. ¿Existe un patrón previo para implementarla?
4. ¿Modifica contratos?
5. ¿Modifica eventos?
6. ¿Modifica seguridad?
7. ¿Modifica multi-tenancy?
8. ¿Modifica infraestructura?
9. ¿Modifica ownership de datos?
10. ¿Puede provocar impacto transversal?

Si la tarea tiene bajo riesgo y existe un camino técnico claro, puede marcarse como:

`junior-friendly`

Si implica decisiones técnicas críticas o impacto transversal, debe marcarse como:

`senior-only`

Si la implementación puede realizarse normalmente pero contiene una parte sensible, debe indicarse que:

`Requiere revisión senior`

---

# 9. Matriz rápida de asignación

| Tipo de cambio                | Junior                                          | Senior                  | Líder técnico                      |
| ----------------------------- | ----------------------------------------------- | ----------------------- | ---------------------------------- |
| Documentación guiada          | Responsable                                     | Revisión cuando aplique | No requerido normalmente           |
| DTO simple                    | Responsable                                     | Revisión normal         | No requerido                       |
| Validador                     | Responsable                                     | Revisión normal         | No requerido                       |
| Prueba unitaria               | Responsable                                     | Revisión normal         | No requerido                       |
| Endpoint pequeño              | Responsable si el servicio ya está estructurado | Revisión                | No requerido normalmente           |
| Pantalla simple               | Responsable                                     | Revisión frontend       | No requerido                       |
| Contrato entre servicios      | Participa                                       | Responsable             | Revisión si es transversal         |
| Evento de integración         | Participa                                       | Responsable             | Revisión cuando sea crítico        |
| RabbitMQ / mensajería         | Participa                                       | Responsable             | Revisión de cambios estructurales  |
| Seguridad                     | Participa                                       | Responsable             | Revisión                           |
| Multi-tenancy                 | Participa                                       | Responsable             | Revisión                           |
| Migración importante          | Apoyo                                           | Responsable             | Revisión cuando tenga alto impacto |
| Gateway                       | Apoyo                                           | Responsable             | Revisión                           |
| BFF crítico                   | Apoyo                                           | Responsable             | Revisión cuando sea transversal    |
| Building Blocks               | Apoyo                                           | Responsable             | Revisión                           |
| Pruebas de arquitectura       | Apoyo                                           | Responsable             | Revisión                           |
| Nueva decisión arquitectónica | Consultado                                      | Participa               | Responsable                        |
| Conflicto entre dominios      | Informado                                       | Participa               | Responsable                        |

---

# 10. Crecimiento de los juniors

Las restricciones definidas en este documento no son permanentes para una persona.

Un desarrollador puede asumir progresivamente tareas de mayor impacto conforme:

* Comprenda los límites de los servicios.
* Conozca las reglas arquitectónicas.
* Demuestre dominio de los patrones existentes.
* Complete correctamente tareas anteriores.
* Participe en revisiones de código.
* Trabaje junto con seniors en tareas críticas.
* Comprenda las consecuencias de cambios distribuidos.

El objetivo es ampliar progresivamente la autonomía técnica.

No se pretende mantener a un desarrollador permanentemente limitado a tareas simples.

---

# 11. Regla de escalamiento

Cualquier desarrollador, independientemente de su experiencia, debe escalar una decisión cuando:

* No está claro qué servicio es propietario de una responsabilidad.
* Dos dominios parecen poder implementar la misma funcionalidad.
* Es necesario romper una regla arquitectónica para avanzar.
* Un cambio aparentemente local comienza a afectar varios servicios.
* No existe un patrón previo para resolver el problema.
* Una migración puede provocar pérdida de información.
* Un contrato existente necesita romper compatibilidad.
* Una decisión puede afectar seguridad o aislamiento entre tenants.

Detenerse para revisar una decisión de alto impacto no debe considerarse bloqueo improductivo.

Es parte del proceso de protección arquitectónica.

---

# 12. Regla final

El nivel de experiencia determina quién debe asumir la responsabilidad de una decisión, pero no quién puede aprender sobre ella.

Los juniors deben participar y crecer.

Los seniors deben establecer patrones, revisar y transmitir conocimiento.

El líder técnico debe proteger la coherencia del sistema y permitir que el equipo avance sin convertir todas las decisiones en dependencias personales.

La meta es que SIA pueda crecer tanto en funcionalidades como en capacidad técnica del equipo sin perder sus límites arquitectónicos.
