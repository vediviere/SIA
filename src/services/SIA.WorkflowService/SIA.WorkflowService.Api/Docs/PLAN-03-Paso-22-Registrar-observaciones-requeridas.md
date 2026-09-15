# PR-001 — Paso 22. Registrar observaciones y correcciones requeridas

## Objetivo

Registrar formalmente las observaciones y correcciones requeridas identificadas durante la revisión de una propuesta de carga académica cuando el Coordinador Académico determina que existen elementos que deben ser modificados por la Jefatura de División.

Este paso pertenece al flujo de retrabajo de la propuesta y tiene como finalidad dejar persistidas las indicaciones que deberán atenderse antes de que la Jefatura pueda reenviar la propuesta para una nueva revisión.

Las reglas de revisión, los criterios utilizados para identificar observaciones y la granularidad de los elementos observados se encuentran definidos en el Paso 21.

---

## Condición de entrada

El Paso 22 únicamente se ejecuta cuando, como resultado del Paso 21, existen observaciones o correcciones requeridas.

Si la propuesta no presenta observaciones y puede continuar directamente con el flujo de aprobación, este paso no se ejecuta y el proceso continúa hacia el Paso 24.

Conceptualmente:

```text
Paso 21
Revisar propuesta
        │
        ├── Sin observaciones ───────────→ Paso 24
        │
        └── Con observaciones
                    │
                    ▼
                 Paso 22
       Registrar observaciones
                    │
                    ▼
                 Paso 23
       Enviar propuesta para ajustes
```

---

## Responsabilidad de WorkflowService

`WorkflowService` es responsable de conservar el registro formal de las observaciones generadas durante el proceso de revisión.

Su responsabilidad en este paso comprende:

* Registrar las observaciones asociadas al proceso de revisión.
* Mantener la relación entre cada observación y la propuesta correspondiente.
* Identificar el elemento de la propuesta que requiere corrección mediante referencias externas.
* Conservar quién registró la observación.
* Conservar la fecha y hora en que fue registrada.
* Mantener las observaciones como parte del historial del proceso de revisión.
* Proporcionar la información necesaria para que la Jefatura conozca las correcciones requeridas.
* Mantener el contexto de `TenantId`.
* Mantener el contexto de `CorrelationId`.

`WorkflowService` no modifica directamente la información propietaria de `SchedulingService`.

---

## Información registrada

Cada observación deberá conservar como mínimo la información necesaria para identificarla y relacionarla con el proceso de revisión correspondiente.

Conceptualmente:

```text
ReviewProcess
    │
    └── ReviewObservation
            ├── ObservationId
            ├── AcademicLoadId
            ├── OfferingId
            ├── Description
            ├── CreatedBy
            ├── CreatedUtc
            ├── TenantId
            └── CorrelationId
```

Los identificadores `AcademicLoadId` y `OfferingId` representan referencias externas a información cuyo propietario corresponde a `SchedulingService`.

`WorkflowService` no debe duplicar el contenido completo de la carga académica ni de la oferta académica.

---

## Asociación de la observación

Cada observación deberá quedar asociada al proceso de revisión y al elemento específico de la propuesta que requiere atención.

Conceptualmente:

```text
Proceso de revisión
        │
        ├── Observación 1
        │       └── Offering A
        │
        ├── Observación 2
        │       └── Offering B
        │
        └── Observación 3
                └── Offering C
```

Una misma propuesta puede contener múltiples observaciones.

Las observaciones deberán conservar referencias suficientes para que el sistema pueda identificar posteriormente qué elemento de la propuesta debe ser corregido.

La definición de qué elementos pueden ser observados y la granularidad funcional de dichas observaciones corresponde al Paso 21.

---

## Contenido de la observación

La observación deberá contener una descripción clara de la corrección requerida.

El contenido debe permitir que la Jefatura de División comprenda qué debe revisar o modificar antes de reenviar la propuesta.

La observación no deberá almacenar una copia de los datos académicos originales.

Ejemplo conceptual:

```text
OfferingId: 12345

Observación:
"Revisar la asignación del docente debido a que
la distribución de horas requiere modificación."
```

El contenido específico de la observación es responsabilidad del Coordinador Académico durante la revisión.

---

## Autor de la observación

Cada observación deberá conservar la referencia al usuario que la registró.

La identidad del usuario pertenece al contexto de `IdentityService`.

`WorkflowService` no administra las cuentas ni las credenciales del usuario.

Por lo tanto, deberá conservar únicamente el identificador necesario para relacionar la observación con el usuario que realizó la acción.

Conceptualmente:

```text
IdentityService
      │
      │ UserId
      ▼
WorkflowService
      │
      └── ReviewObservation.CreatedBy
```

---

## Fecha de registro

Cada observación deberá conservar la fecha y hora en que fue registrada.

Se utilizará `CreatedUtc` como referencia técnica para mantener consistencia temporal entre servicios.

La fecha de registro permite conservar el historial de las observaciones realizadas durante el proceso de revisión.

---

## Historial de observaciones

Las observaciones forman parte del historial del proceso de revisión.

Una observación registrada no deberá eliminarse únicamente porque posteriormente la Jefatura realice una corrección.

El historial permite conservar evidencia de las observaciones realizadas durante cada etapa de revisión.

Las correcciones posteriores deberán quedar asociadas a una nueva interacción o revisión del proceso, sin alterar retrospectivamente la observación original.

---

## Estado de las observaciones

El registro de una observación representa una corrección pendiente de atención dentro del proceso de retrabajo.

Conceptualmente:

```text
Observación registrada
        │
        ▼
Corrección pendiente
        │
        ▼
Jefatura realiza ajustes
        │
        ▼
Nueva revisión
```

El cierre o resolución de una observación deberá determinarse como parte del flujo posterior de retrabajo y nueva revisión.

Este paso no modifica por sí mismo la información académica de la propuesta.

---

## Relación con AcademicLoad

`AcademicLoad` pertenece a `SchedulingService`.

`WorkflowService` únicamente conserva la referencia necesaria para relacionar las observaciones con la propuesta sometida a revisión.

Por lo tanto:

```text
WorkflowService
    │
    └── AcademicLoadId
              │
              │ referencia externa
              ▼
SchedulingService
    └── AcademicLoad
```

No deberá existir:

* Acceso directo a la base de datos de `SchedulingService`.
* Llave foránea entre las bases de datos.
* Copia completa de `AcademicLoad`.
* Copia completa de los datos de las materias, docentes, grupos u horas.

---

## Relación con Offering

Cuando una observación corresponda a una oferta académica específica, `WorkflowService` deberá conservar la referencia necesaria para identificarla.

Conceptualmente:

```text
ReviewObservation
        │
        └── OfferingId
                │
                ▼
        SchedulingService
                │
                └── AcademicOffering
```

La información detallada del `AcademicOffering` continúa siendo propiedad de `SchedulingService`.

---

## TenantId

Cada observación deberá conservar `TenantId`.

El `TenantId` identifica el contexto institucional al que pertenece el proceso de revisión.

La observación únicamente podrá relacionarse con una propuesta y sus elementos pertenecientes al mismo contexto institucional.

No deberán generarse observaciones que relacionen información perteneciente a diferentes tenants.

---

## CorrelationId

La operación de registro deberá conservar `CorrelationId`.

`CorrelationId` permitirá relacionar técnicamente la creación de las observaciones con la ejecución correspondiente del flujo PR-001.

Conceptualmente:

```text
Solicitud de revisión
        │
        └── CorrelationId
                │
                ├── ReviewProcess
                └── ReviewObservation
```

Esto permitirá facilitar trazabilidad, diagnóstico y seguimiento entre los servicios participantes.

---

## Comunicación con otros servicios

El registro de observaciones pertenece a `WorkflowService`.

Cuando posteriormente sea necesario comunicar a `SchedulingService` que existen correcciones requeridas, la comunicación deberá realizarse mediante contratos o eventos de integración.

No se permitirá el acceso directo a la base de datos de `SchedulingService`.

La modificación efectiva de la propuesta continuará siendo responsabilidad de `SchedulingService`.

---

## Preparación para el Paso 23

El resultado de este paso es un conjunto de observaciones formalmente registradas y asociadas al proceso de revisión.

Estas observaciones serán utilizadas por el Paso 23 para comunicar a la Jefatura que la propuesta requiere ajustes.

Conceptualmente:

```text
Paso 21
Revisión
   │
   ▼
Paso 22
Observaciones registradas
   │
   │
   ▼
Paso 23
Enviar propuesta para ajustes
```

El Paso 22 no habilita por sí mismo el reenvío de la propuesta.

La condición y mecanismo mediante los cuales la Jefatura podrá recibir la propuesta para ajustes y posteriormente reenviarla serán definidos en los pasos correspondientes del flujo.

---

## Reglas funcionales

1. El Paso 22 únicamente se ejecuta cuando existen observaciones derivadas del Paso 21.
2. Si no existen observaciones, el flujo no pasa por este paso.
3. Una propuesta puede tener múltiples observaciones.
4. Cada observación deberá estar asociada al proceso de revisión correspondiente.
5. Cada observación deberá identificar el elemento de la propuesta al que corresponde mediante referencias externas.
6. Cada observación deberá conservar una descripción de la corrección requerida.
7. Cada observación deberá conservar el identificador del usuario que la registró.
8. Cada observación deberá conservar su fecha y hora de registro.
9. Las observaciones forman parte del historial del proceso de revisión.
10. El registro de una observación no modifica directamente `AcademicLoad`.
11. `WorkflowService` no es propietario de `AcademicLoad` ni de `AcademicOffering`.
12. No se deberán copiar datos completos de `SchedulingService` para registrar una observación.
13. Las referencias a información de `SchedulingService` deberán manejarse mediante identificadores externos.
14. Cada observación deberá conservar `TenantId`.
15. Cada operación deberá conservar `CorrelationId`.
16. No deberán existir relaciones de base de datos entre `WorkflowService` y `SchedulingService`.
17. La corrección de la propuesta corresponde a los servicios propietarios de la información modificada.
18. El registro de observaciones prepara la propuesta para el flujo de retrabajo posterior.
19. El Paso 22 no habilita por sí mismo el reenvío de la propuesta.
20. Las observaciones originales no deberán modificarse retrospectivamente para ocultar el historial de revisión.

---

## Fuera de alcance

Este paso no contempla:

* Modificar `AcademicLoad`.
* Modificar `AcademicOffering`.
* Asignar docentes.
* Modificar horas frente a grupo.
* Modificar horas de apoyo.
* Modificar grupos.
* Modificar horarios.
* Aprobar la propuesta.
* Realizar una nueva revisión.
* Determinar si la corrección realizada por la Jefatura es válida.
* Enviar nuevamente la propuesta a revisión.
* Administrar cuentas de usuario.
* Administrar permisos.
* Acceder directamente a la base de datos de otro servicio.

Estas responsabilidades corresponden a otros pasos y servicios del flujo PR-001.

---

## Componentes involucrados

### WorkflowService

Responsable de:

* Proceso de revisión.
* Registro de observaciones.
* Historial de revisión.
* `TenantId`.
* `CorrelationId`.
* Referencias externas hacia la propuesta y sus elementos.

### SchedulingService

Responsable de:

* `AcademicLoad`.
* `AcademicOffering`.
* Materias.
* Docentes asignados.
* Grupos.
* Horas frente a grupo.
* Horas de apoyo.
* Modificación de la información de la propuesta.

### IdentityService

Responsable de:

* Identidad del usuario.
* Cuentas.
* Autenticación.
* Roles.
* Permisos.

`WorkflowService` únicamente conserva las referencias de identidad necesarias para la trazabilidad de las acciones.

---

## Resultado del paso

Al finalizar el Paso 22:

```text
Proceso de revisión
        │
        ├── Propuesta referenciada
        │
        └── Observaciones registradas
                │
                ├── Elemento afectado
                ├── Corrección requerida
                ├── Usuario que observa
                ├── Fecha de registro
                ├── TenantId
                └── CorrelationId
```

La propuesta queda documentada con las correcciones que deberán atenderse durante el flujo de retrabajo.

El siguiente paso del proceso será el **Paso 23 — Enviar propuesta para ajustes**.
