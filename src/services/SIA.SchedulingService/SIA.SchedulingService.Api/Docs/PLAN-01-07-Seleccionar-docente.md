# 7. Seleccionar docente

Una vez definidos los grupos y las materias que serán ofertadas dentro del periodo académico, `SchedulingService` deberá permitir seleccionar un docente para participar en la planeación académica.

La selección deberá realizarse sobre docentes candidatos que puedan impartir la materia correspondiente.

El objetivo de este paso es identificar al docente que podrá ser asociado posteriormente a la materia ofertada, sin realizar todavía la asignación definitiva de horas frente a grupo.

La selección del docente pertenece al contexto de la planeación académica y, por lo tanto, la operación es responsabilidad de `SchedulingService`.

`AcademicStaffService` continúa siendo propietario de la información propia del docente.

---

## Servicio propietario

El servicio propietario de este paso es:

`SchedulingService`

`SchedulingService` es responsable de determinar qué docente será seleccionado dentro de la planeación académica.

`AcademicStaffService` es responsable de proporcionar la información propia del docente necesaria para determinar los candidatos.

Por lo tanto:

```
AcademicStaffService
        │
        │ información del docente
        ▼
SchedulingService
        │
        │ selección dentro de la planeación
        ▼
Docente seleccionado
```

La selección no deberá implicar modificaciones directas sobre las entidades internas de AcademicStaffService.


## Información necesaria

Para seleccionar un docente, `SchedulingService` necesita conocer información suficiente para identificar candidatos compatibles con la materia y el programa educativo.

Entre la información necesaria se encuentra:

- `TeacherId`.
- `Programa educativo` al que pertenece el docente.
- `Perfil profesional` del docente.
- `Estado` del docente.
- `Información necesaria` para determinar si puede participar en la asignación académica.

La información correspondiente al perfil del docente pertenece a `AcademicStaffService`.

La información correspondiente a la materia, grupo, periodo y planeación pertenece a `SchedulingService`.


## Criterios para mostrar docentes candidatos

Los docentes candidatos deberán priorizarse considerando su relación con el programa educativo y su perfil profesional.

Como criterio general, deberán considerarse:

- Docentes pertenecientes al programa educativo asociado al plan de estudios.
- Docentes de Ciencias Básicas.
- Docentes cuyo perfil profesional sea afín a la materia aunque no pertenezcan directamente al programa educativo.

Por ejemplo, para una materia de un programa de Ingeniería en Sistemas Computacionales, podrán considerarse docentes cuyo perfil sea compatible con materias como:

- Cálculo.
- Química.
- Física.
- Administración.

Un docente de otra área, como un contador, podría ser candidato para impartir una materia de administración cuando su perfil profesional sea considerado compatible.

La prioridad de presentación de candidatos deberá permitir que los docentes pertenecientes al programa educativo aparezcan antes que otros candidatos compatibles.


## Consulta de docentes candidatos

La consulta de candidatos será responsabilidad de `SchedulingService`.

Para obtener la información necesaria, `SchedulingService` podrá solicitar información a `AcademicStaffService` mediante los mecanismos de comunicación definidos entre servicios.

No deberá realizar consultas directas a la base de datos de `AcademicStaffService`.

Conceptualmente:

```
SchedulingService
        │
        │ consulta de docentes candidatos
        ▼
AcademicStaffService
        │
        ▼
Información de docentes

```
La respuesta deberá contener únicamente la información necesaria para que `SchedulingService` pueda construir la selección dentro del contexto de la planeación.


## Relación con la materia y el grupo

La selección del docente se realiza dentro del contexto de una materia que será ofertada para un grupo.

Por lo tanto, el contexto mínimo de la selección incluye:

```
AcademicPeriod
      │
      └── StudyPlan / programa educativo
                  │
                  └── Grupo
                        │
                        └── Materia
                              │
                              └── Docente candidato
```

`SchedulingService` deberá conservar el contexto necesario para identificar posteriormente a qué oferta académica corresponde el docente seleccionado.


## Horas disponibles

La disponibilidad de horas del docente es un criterio necesario para la selección, pero su asignación definitiva corresponde a los pasos posteriores del flujo.

El docente podrá tener asignaciones dentro del mismo periodo académico en diferentes programas educativos.

Por ejemplo:
``` 
Docente José
Horas totales: 40

Carrera A → 10 horas
Carrera B → 15 horas

Horas utilizadas: 25
Horas disponibles: 15

```

Por lo tanto, para determinar la disponibilidad del docente deberán considerarse sus asignaciones académicas correspondientes al periodo académico activo, independientemente del programa educativo en el que se encuentren.

Las cargas correspondientes a periodos anteriores no deberán disminuir la disponibilidad del periodo actual.

La validación definitiva de que el docente no exceda sus horas disponibles se realizará en el paso correspondiente a la verificación de condiciones y asignación.


## Selección no equivale a asignación

El hecho de que un docente aparezca como candidato o sea seleccionado en la interfaz no significa todavía que se haya realizado la asignación académica definitiva.

La diferencia conceptual es:

```
Seleccionar docente
        ↓
Identificar candidato
        ↓
Verificar condiciones
        ↓
Asignar docente

```
Por lo tanto, el paso 7 no deberá modificar todavía las horas frente a grupo del docente ni completar la carga académica.


## Comando

La selección definitiva del docente dentro de la planeación será una operación de `SchedulingService`.

Conceptualmente: `SelectTeacher`

o el contrato equivalente que se determine durante el diseño de la API.

El comando deberá contener únicamente la información necesaria para identificar:

- El contexto de la planeación.
- La oferta o materia correspondiente.
- El docente seleccionado.

No deberá incluir información perteneciente a otros dominios que `SchedulingService` no sea propietario de administrar.


## Eventos de integración

La selección del docente, por sí misma, no requiere necesariamente un evento de integración con otros servicios si todavía no representa una asignación académica persistida.

Los eventos de integración deberán emitirse cuando exista un cambio de estado que otros servicios necesiten conocer.

Por lo tanto, no se define un evento obligatorio para la simple consulta o selección visual de candidatos en este paso.

Los eventos relacionados con la asignación efectiva del docente deberán definirse en los pasos posteriores cuando se determine el momento exacto en que la relación docente–oferta académica queda formalmente registrada.


## AdminBff

`AdminBff` podrá componer la información necesaria para presentar la pantalla de selección del docente.

Por ejemplo:

```
AdminBff
   │
   ├── SchedulingService
   │       └── Materia / Grupo / Oferta
   │
   └── AcademicStaffService
           └── Docentes candidatos
```

`AdminBff` podrá combinar las respuestas para presentar al usuario:

- Materia.
- Grupo.
- Programa educativo.
- Docentes candidatos.
- Información relevante del perfil.
- Información necesaria para la selección.

Sin embargo, `AdminBff` no deberá convertirse en propietario de ninguna de estas entidades.

La selección y las reglas de negocio deberán permanecer bajo responsabilidad del servicio propietario correspondiente.


## TenantId

Todas las operaciones deberán conservar el `TenantId` correspondiente al contexto institucional.

El docente candidato deberá pertenecer al mismo `TenantId` que el contexto de la planeación académica.

No deberá ser posible seleccionar un docente perteneciente a otra institución.

Conceptualmente:
```
TenantId
   │
   ├── AcademicService
   ├── AcademicStaffService
   └── SchedulingService
```
Los servicios deberán utilizar el mismo contexto institucional para resolver y validar la operación.

`TenantId` no deberá ser proporcionado libremente por el usuario como mecanismo para seleccionar otra institución.


## Dependencias técnicas

El paso 7 depende principalmente de:

- `AcademicStaffService` para obtener información de docentes.
- `SchedulingService` para el contexto de la planeación.
- `AcademicService` para la información académica de la materia, plan y programa educativo cuando sea necesaria.

La comunicación deberá realizarse mediante contratos definidos entre servicios.

No deberá existir acceso directo a:

- `SIA_AcademicStaffDb`
- `SIA_AcademicDb`

desde `SchedulingService`.

## Límites de responsabilidad

`AcademicStaffService`

Es propietario de:

- Identidad académica del docente.
- Perfil profesional.
- Adscripción o relación institucional correspondiente.
- Información propia del docente.

No es responsable de:

- Seleccionar docentes para una oferta.
- Asignar materias.
- Administrar grupos.
- Administrar cargas académicas.
- Determinar la planeación académica.

`AcademicService`

Es propietario de:

- Programas educativos.
- Planes de estudio.
- Materias.
- Periodos académicos.

No es responsable de seleccionar docentes ni administrar su carga académica.

`SchedulingService`

Es propietario de:

- Planeación académica.
- Grupos.
- Oferta académica.
- Carga académica.
- Asignación de docentes dentro de la planeación.

Por lo tanto, `SchedulingService` es el propietario del paso 7.


## `AdminBff`

Es responsable únicamente de:

- Orquestar las solicitudes necesarias para la interfaz administrativa.
- Componer información proveniente de servicios autorizados.
- Presentar la información al cliente.

No es propietario de docentes, materias, grupos ni cargas académicas.


## Resultado esperado

Al finalizar el paso 7, el usuario deberá poder visualizar y seleccionar un docente candidato para una materia y grupo determinados.

La selección deberá considerar la prioridad de docentes pertenecientes al programa educativo, docentes de Ciencias Básicas y docentes con perfiles profesionales compatibles.

La selección visual del docente no deberá considerarse todavía como la asignación definitiva ni deberá modificar por sí misma las horas frente a grupo.

La asignación, la validación definitiva de disponibilidad y el cálculo de horas frente a grupo corresponden a los pasos posteriores del flujo PR-001.