# PLAN-03 — Definir reglas funcionales de revisión y retrabajo de propuesta de carga académica

## Descripción

Definir formalmente las reglas funcionales y los límites técnicos del proceso de revisión de una propuesta de carga académica.

Esta tarea corresponde al Paso 21 del proceso BPMN PR-001:

`Revisar propuesta de carga académica`

El Coordinador Académico revisará una propuesta enviada previamente por una Jefatura de División y podrá tomar una de las siguientes decisiones:

* Aprobar la propuesta.
* Regresar la propuesta para retrabajo.

La tarea define el comportamiento funcional de ambas decisiones, la granularidad de las observaciones, los estados de la propuesta, las condiciones para corregir y reenviar, y la interacción entre `SchedulingService`, `WorkflowService`, `AcademicService`, `AcademicStaffService`, `NotificationsService` y `AdminBff`.

Esta tarea no implementa todavía la funcionalidad correspondiente.

---

## Objetivo

Establecer un modelo funcional completo para la revisión de propuestas de carga académica de manera que las tareas posteriores puedan implementar:

* La aprobación de propuestas.
* El regreso para retrabajo.
* El registro de observaciones.
* La actualización de estados.
* La reapertura de la propuesta.
* El reenvío de una propuesta corregida.
* La continuidad del proceso de revisión.

---

# 1. Responsable de la revisión

La revisión funcional de una propuesta de carga académica corresponde al:

`Coordinador Académico`

El perfil de negocio `Coordinator` pertenece a:

`AcademicStaffService`

La autenticación y autorización de acceso corresponden al mecanismo de identidad de SIA.

El frontend consumirá las operaciones mediante:

`SIA.AdminBff`

---

## Regla de autorización

Solamente un usuario autenticado y autorizado para actuar como Coordinador Académico dentro del `TenantId` correspondiente podrá ejecutar una decisión sobre una propuesta.

Conceptualmente:

```text
Usuario autenticado
        │
        ▼
Identidad y autorización
        │
        ▼
AdminBff
        │
        ▼
Contexto institucional válido
        │
        ▼
Acceso como Coordinator
        │
        ▼
Revisar propuesta
```

El frontend no deberá enviar un identificador arbitrario de Coordinador como mecanismo de autorización.

La identidad del actor deberá obtenerse del contexto autenticado.

---

# 2. Alcance de la revisión

El Coordinador revisa una propuesta enviada por una Jefatura de División.

La propuesta puede contener cargas académicas de múltiples docentes pertenecientes al mismo conjunto de planeación enviado por esa Jefatura.

Conceptualmente:

```text
Jefatura de División
        │
        ▼
Propuesta de carga académica
        │
        ├── Docente A
        │       ├── Materias
        │       └── Actividades de apoyo
        │
        ├── Docente B
        │       ├── Materias
        │       └── Actividades de apoyo
        │
        └── Docente C
                ├── Materias
                └── Actividades de apoyo
```

La revisión se realiza sobre la propuesta completa.

Sin embargo, las observaciones pueden señalar elementos específicos dentro de ella.

---

# 3. Granularidad de las observaciones

Las observaciones no deberán limitarse únicamente a un comentario general sobre toda la propuesta.

El Coordinador deberá poder identificar visual y lógicamente el elemento que requiere modificación.

La granularidad mínima será:

```text
Propuesta
    │
    ├── Observación general
    │
    └── Elemento específico
            │
            ├── Offering / materia ofertada
            │
            ├── Grupo
            │
            ├── Docente asignado
            │
            ├── Horas frente a grupo
            │
            └── Actividad de apoyo
```

Una observación podrá asociarse:

* A toda la propuesta.
* A una oferta o materia específica.
* A una asignación de docente.
* A las horas frente a grupo.
* A una actividad de apoyo.

La interfaz deberá permitir al Coordinador identificar claramente qué elemento requiere atención.

---

# 4. Gestión de comentarios por Offering

Los comentarios específicos deberán conservar una referencia al elemento revisado.

Conceptualmente:

```text
ReviewObservation
        │
        ├── ProposalId
        ├── TargetType
        ├── TargetId
        ├── Comment
        ├── CreatedBy
        ├── CreatedAt
        ├── TenantId
        └── CorrelationId
```

`TargetType` permite identificar conceptualmente el tipo de elemento observado.

Ejemplos:

```text
Proposal
Offering
TeacherAssignment
TeachingHours
SupportActivity
```

`TargetId` identifica el elemento específico al que corresponde la observación.

La estructura exacta de persistencia será definida por la tarea técnica correspondiente.

---

## Ownership de las observaciones

Las observaciones forman parte del proceso de revisión.

Por lo tanto, su contexto y trazabilidad pertenecen a:

`WorkflowService`

`WorkflowService` no deberá copiar el contenido completo de la propuesta.

Deberá conservar únicamente las referencias necesarias.

Por ejemplo:

```text
ProposalId
OfferingId
AssignmentId
ActivityId
```

Los identificadores representan referencias hacia información cuyo propietario continúa siendo `SchedulingService`.

---

# 5. Aprobación de la propuesta

Cuando el Coordinador decide aprobar una propuesta:

1. La propuesta deja de estar pendiente de revisión.
2. El proceso de revisión registra la decisión.
3. Se conserva la identidad del actor que aprobó.
4. Se conserva la fecha de aprobación.
5. Se conserva `TenantId`.
6. Se conserva `CorrelationId`.
7. `WorkflowService` actualiza el estado del proceso.
8. `SchedulingService` recibe la información necesaria para actualizar el estado de la propuesta.

El resultado funcional será:

```text
Propuesta
En revisión
        │
        │ Aprobación
        ▼
Aprobada
```

---

## Impacto en WorkflowService

`WorkflowService` deberá registrar que el proceso terminó exitosamente.

Conceptualmente:

```text
Proceso de revisión
        │
        ├── Estado anterior: En revisión
        │
        ▼
Estado final: Aprobada
```

La tarea posterior responsable deberá definir el mecanismo concreto mediante comando, evento o contrato.

---

## Impacto en SchedulingService

`SchedulingService` deberá actualizar el estado de la propuesta aprobada.

`SchedulingService` continúa siendo propietario del ciclo de vida de la propuesta.

`WorkflowService` no modifica directamente la base de datos de `SchedulingService`.

La comunicación deberá realizarse mediante un contrato o evento de integración.

---

# 6. AcademicService ante la aprobación

`AcademicService` no es propietario de la propuesta de carga académica ni del proceso de revisión.

Por lo tanto, la aprobación de una propuesta no deberá modificar directamente:

* Programas educativos.
* Planes de estudio.
* Materias.
* Prerrequisitos.
* Estructura académica.

La participación de `AcademicService` se limita a proporcionar información académica necesaria para consultar o contextualizar la propuesta.

La aprobación no deberá generar una modificación de estado dentro de `AcademicService`.

---

# 7. Regresar para retrabajo

La acción funcional recomendada no será denominada como una eliminación de la propuesta.

Cuando el Coordinador encuentra elementos que requieren modificación, la propuesta será:

`Regresada para retrabajo`

Esto significa que:

* La propuesta no está aprobada.
* La propuesta no se elimina.
* La información existente se conserva.
* Se registran las observaciones.
* La Jefatura recupera la posibilidad de modificar la propuesta.
* La propuesta deberá ser enviada nuevamente para una nueva revisión.

El flujo será:

```text
En revisión
        │
        │ Regresar para retrabajo
        ▼
Requiere corrección
        │
        │ Corrección por Jefatura
        ▼
En preparación
        │
        │ Reenvío
        ▼
En revisión
```

---

# 8. No utilizar "rechazada" como estado final

La decisión del Coordinador en este flujo no representa necesariamente un rechazo definitivo.

El objetivo es permitir que la Jefatura corrija la propuesta.

Por esta razón, el estado funcional recomendado es:

`Requiere corrección`

y no:

`Rechazada`

La palabra "rechazar" podrá utilizarse en lenguaje de interfaz únicamente si posteriormente se define una decisión final diferente.

Para PLAN-03, regresar la propuesta significa:

`Requiere corrección`

---

# 9. Comportamiento posterior al retrabajo

Cuando una propuesta pasa a:

`Requiere corrección`

deberán ocurrir los siguientes efectos funcionales.

## WorkflowService

El proceso deberá registrar:

* Decisión de regreso.
* Fecha.
* Coordinador responsable.
* Observaciones generales.
* Observaciones específicas.
* Referencia a la propuesta.
* `TenantId`.
* `CorrelationId`.

El proceso no deberá eliminar su historial anterior.

---

## SchedulingService

La propuesta deberá dejar de estar bloqueada para modificación por revisión.

La Jefatura propietaria podrá corregirla dentro de las reglas correspondientes.

Mientras la propuesta se encuentre en:

`Requiere corrección`

no podrá considerarse aprobada.

---

## Jefatura de División

La Jefatura podrá:

* Consultar las observaciones.
* Consultar los elementos señalados.
* Modificar los elementos permitidos de la propuesta.
* Corregir materias.
* Corregir asignaciones.
* Corregir horas frente a grupo.
* Corregir actividades de apoyo.
* Realizar los ajustes necesarios.

No deberá poder declarar unilateralmente la propuesta como aprobada.

---

# 10. Ventana de modificación

La propuesta podrá modificarse únicamente mientras se encuentre en un estado que permita retrabajo.

Para este flujo:

```text
En preparación
        │
        │ modificación permitida
        ▼
Enviada / En revisión
        │
        │ modificación bloqueada
        │
        ├── Aprobada
        │       modificación ordinaria bloqueada
        │
        └── Requiere corrección
                modificación permitida
```

Por lo tanto:

* `En preparación`: la Jefatura puede modificar.
* `En revisión`: la Jefatura no puede modificar ordinariamente.
* `Requiere corrección`: la Jefatura puede modificar.
* `Aprobada`: la propuesta queda cerrada para modificación ordinaria.

Cualquier modificación posterior a una aprobación requerirá un proceso diferente que queda fuera del alcance de PLAN-03.

---

# 11. Condiciones para reenviar una propuesta

Una propuesta regresada podrá ser enviada nuevamente cuando:

1. Pertenezca al `TenantId` correspondiente.
2. Sea modificada por una Jefatura autorizada.
3. Se encuentre en estado `Requiere corrección` o en un estado editable derivado de este.
4. Las reglas de negocio de `SchedulingService` permitan su envío.
5. Las validaciones necesarias de la propuesta sean satisfactorias.
6. La Jefatura ejecute explícitamente la acción de reenviar.

El sistema no deberá reenviar automáticamente una propuesta solamente porque fue modificada.

El reenvío deberá ser una acción explícita.

---

# 12. Nueva revisión

Cuando una propuesta corregida sea enviada nuevamente:

```text
Requiere corrección
        │
        │ Jefatura corrige
        ▼
En preparación
        │
        │ Enviar nuevamente
        ▼
En revisión
```

La propuesta deberá conservar la relación con su proceso anterior.

El sistema deberá permitir distinguir entre:

* Revisión inicial.
* Regreso para corrección.
* Nueva versión enviada.
* Revisión posterior.
* Aprobación final.

No deberá perderse el historial de observaciones anteriores.

---

# 13. Versionado funcional

Cada reenvío después de un retrabajo deberá poder identificarse como una nueva versión de revisión de la misma propuesta.

Conceptualmente:

```text
ProposalId: P-001

Versión 1
    │
    ├── En revisión
    └── Requiere corrección

Versión 2
    │
    ├── En revisión
    └── Aprobada
```

El mecanismo físico exacto del versionado será definido por las tareas de implementación.

Funcionalmente, deberá conservarse la trazabilidad entre las versiones.

No se deberá tratar cada reenvío como una propuesta completamente independiente sin relación con la original.

---

# 14. Observaciones obligatorias

Cuando el Coordinador regrese una propuesta para retrabajo deberá existir al menos una observación que explique el motivo.

La observación puede ser:

* General.
* Específica sobre uno o varios elementos.

No deberá ser posible regresar una propuesta sin proporcionar información suficiente para que la Jefatura conozca qué requiere corrección.

---

# 15. Relación entre observaciones y correcciones

Las observaciones no obligan técnicamente a que exista una modificación literal sobre el mismo campo observado.

El Coordinador señala qué requiere atención.

La validación final de que la nueva propuesta es aceptable ocurre durante la nueva revisión.

Esto permite que la Jefatura pueda realizar los ajustes necesarios para resolver la observación sin limitar artificialmente la modificación a un único campo.

---

# 16. Notificación del resultado

Después de una decisión del Coordinador deberá generarse una comunicación hacia la Jefatura correspondiente.

Si la propuesta es aprobada:

```text
Propuesta aprobada
```

Si la propuesta requiere modificaciones:

```text
Propuesta requiere corrección
```

`NotificationsService` será responsable de la entrega de la notificación.

La decisión y su estado no pertenecen a `NotificationsService`.

---

# 17. Componentes técnicos involucrados

## SIA.AdminBff

Responsable de:

* Exponer operaciones al frontend administrativo.
* Obtener la identidad del usuario autenticado.
* Propagar autenticación.
* Propagar `TenantId`.
* Propagar `CorrelationId`.
* Componer la información necesaria para visualizar la propuesta.
* Enviar la decisión del Coordinador hacia el servicio correspondiente.

No puede:

* Decidir si una propuesta cumple reglas académicas.
* Modificar directamente la propuesta.
* Actualizar bases de datos de otros servicios.

---

## AcademicStaffService

Responsable de:

* Administrar el perfil de negocio `Coordinator`.
* Proporcionar la información necesaria para identificar el contexto institucional del personal académico.

No aprueba directamente la propuesta.

---

## AcademicService

Responsable de:

* Programas educativos.
* Planes de estudio.
* Materias.
* Estructura académica.

Puede proporcionar información de contexto para la revisión.

La aprobación o regreso de una propuesta no modifica la estructura académica administrada por este servicio.

---

## SchedulingService

Es propietario de:

* La propuesta de carga académica.
* Las cargas académicas.
* Las asignaciones.
* Las ofertas correspondientes.
* Los estados propios de la propuesta.

Debe controlar cuándo una propuesta puede modificarse, enviarse, reenviarse o quedar cerrada.

---

## WorkflowService

Es responsable de:

* El proceso de revisión.
* El estado del proceso.
* La decisión de revisión.
* La trazabilidad.
* Las observaciones.
* La referencia a la propuesta.
* El control de la continuidad entre revisión y retrabajo.

No debe almacenar una copia completa de `AcademicLoad`.

---

## NotificationsService

Es responsable de:

* Comunicar la existencia de propuestas pendientes.
* Comunicar que una propuesta fue aprobada.
* Comunicar que una propuesta requiere corrección.

No es propietario de los estados de la propuesta ni del proceso.

---

# 18. Flujo de aprobación

```text
Coordinador Académico
        │
        │ Revisar
        ▼
AdminBff
        │
        │ Decisión: Aprobar
        ▼
WorkflowService
        │
        ├── Registrar decisión
        ├── Registrar actor
        ├── Registrar fecha
        └── Estado: Aprobada
        │
        │ Evento / contrato
        ▼
SchedulingService
        │
        └── Actualizar propuesta: Aprobada
        │
        ▼
NotificationsService
        │
        └── Notificar resultado
```

---

# 19. Flujo de retrabajo

```text
Coordinador Académico
        │
        │ Revisar
        ▼
AdminBff
        │
        │ Regresar para corrección
        ▼
WorkflowService
        │
        ├── Registrar decisión
        ├── Registrar observaciones
        ├── Conservar referencias
        └── Estado: Requiere corrección
        │
        │ Evento / contrato
        ▼
SchedulingService
        │
        └── Habilitar modificación
        │
        ▼
NotificationsService
        │
        └── Notificar a Jefatura
        │
        ▼
Jefatura de División
        │
        │ Corregir
        ▼
SchedulingService
        │
        │ Reenviar
        ▼
WorkflowService
        │
        └── Nueva revisión
```

---

# 20. TenantId

Todas las operaciones deberán conservar el mismo contexto institucional.

`TenantId` deberá estar presente o poder determinarse de manera confiable en:

* Propuesta.
* Proceso de revisión.
* Observaciones.
* Decisión.
* Notificaciones.
* Reenvío.

No deberá ser posible aprobar o regresar una propuesta perteneciente a otro tenant.

---

# 21. CorrelationId

La operación de revisión deberá conservar un `CorrelationId` para permitir la trazabilidad entre servicios.

Ejemplo:

```text
AdminBff
CorrelationId: C-123
        │
        ▼
WorkflowService
CorrelationId: C-123
        │
        ▼
SchedulingService
CorrelationId: C-123
        │
        ▼
NotificationsService
CorrelationId: C-123
```

El mismo contexto de correlación deberá propagarse durante una misma operación distribuida.

---

# 22. Eventos y contratos necesarios

PLAN-03 define conceptualmente la necesidad de los siguientes intercambios.

## Decisión aprobada

```text
WorkflowService
        │
        └── ProposalApprovedForScheduling
                │
                ▼
          SchedulingService
```

El nombre técnico definitivo del evento podrá ajustarse durante la implementación.

---

## Propuesta requiere corrección

```text
WorkflowService
        │
        └── ProposalRequiresCorrection
                │
                ▼
          SchedulingService
```

La información deberá incluir únicamente el contexto necesario para identificar la propuesta y su nuevo estado.

---

## Resultado para notificación

```text
WorkflowService / SchedulingService
        │
        ▼
NotificationsService
```

La fuente definitiva del evento deberá respetar el ownership que se establezca durante la implementación.

`NotificationsService` no deberá ser utilizado como intermediario para decidir el estado de una propuesta.

---

# 23. Fuera de alcance

PLAN-03 no define:

* El esquema físico final de las tablas.
* Los endpoints definitivos.
* La implementación concreta de eventos.
* La tecnología específica de mensajería.
* El diseño visual definitivo de la pantalla.
* La modificación posterior de una propuesta aprobada.
* La aprobación parcial de una propuesta.
* La aprobación de elementos individuales mientras el resto permanece pendiente.
* La eliminación del historial de revisiones.
* Reglas académicas nuevas.
* Reglas de cálculo de carga.
* Reglas de horarios.

La propuesta se revisa como una unidad.

Las observaciones pueden ser granulares, pero la decisión funcional de aprobación o regreso aplica a la propuesta completa.

---

# 24. Reglas funcionales

1. Solamente un Coordinador Académico autorizado puede ejecutar una revisión.
2. La identidad del actor se obtiene del contexto autenticado.
3. La propuesta pertenece a `SchedulingService`.
4. El proceso de revisión pertenece a `WorkflowService`.
5. Las observaciones forman parte del proceso de revisión.
6. `WorkflowService` no almacena una copia completa de la propuesta.
7. Las observaciones pueden ser generales o específicas.
8. Una observación específica debe conservar una referencia al elemento observado.
9. La aprobación aplica a la propuesta completa.
10. El regreso para retrabajo aplica a la propuesta completa.
11. Regresar una propuesta no significa eliminarla.
12. El estado funcional posterior al regreso es `Requiere corrección`.
13. Una propuesta en revisión permanece bloqueada para modificación ordinaria.
14. Una propuesta que requiere corrección vuelve a estar disponible para modificación.
15. Una propuesta aprobada queda cerrada para modificación ordinaria.
16. No se puede regresar una propuesta sin proporcionar al menos una observación.
17. El reenvío después de una corrección debe ser una acción explícita.
18. Un reenvío genera una nueva instancia o versión de revisión conservando la trazabilidad con la propuesta original.
19. Las observaciones anteriores no se eliminan.
20. `AcademicService` no modifica su estructura académica como consecuencia de aprobar una propuesta.
21. `TenantId` debe conservarse en todo el flujo.
22. `CorrelationId` debe propagarse entre los servicios involucrados.
23. No se permite acceso directo a bases de datos de otros servicios.
24. La comunicación entre servicios se realiza mediante contratos o eventos.
25. `AdminBff` puede componer información, pero no implementar reglas académicas ni de Scheduling.
26. `NotificationsService` comunica resultados, pero no decide estados.
27. La revisión no implica aprobación parcial.
28. La granularidad de las observaciones no modifica la unidad funcional de decisión: la propuesta completa.

---

# Resultado esperado

Al finalizar PLAN-03 quedan definidas las reglas necesarias para implementar el ciclo completo:

```text
En preparación
        │
        ▼
En revisión
        │
        ├───────────────────┐
        │                   │
        ▼                   ▼
   Aprobada        Requiere corrección
                            │
                            ▼
                    Jefatura corrige
                            │
                            ▼
                      En preparación
                            │
                            ▼
                       En revisión
```

Con esta definición quedan establecidos los criterios funcionales necesarios para que las tareas posteriores implementen la evolución del proceso sin introducir decisiones pendientes sobre:

* Aprobación.
* Retrabajo.
* Estados.
* Observaciones.
* Granularidad.
* Bloqueo y liberación.
* Reenvío.
* Versionado funcional.
* `TenantId`.
* `CorrelationId`.
* Ownership entre servicios.
