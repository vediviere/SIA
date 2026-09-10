# 8. Verificar que el docente cumpla con

Después de seleccionar un docente candidato en el paso 7, `SchedulingService` deberá verificar que el docente cumpla las condiciones necesarias para poder ser asignado a la materia y grupo correspondientes.

Las condiciones que deberán verificarse son:

- Horas disponibles dentro del periodo académico activo.
- Perfil profesional compatible con la materia.
- Programa educativo al que pertenece el docente.
- Carga académica existente en el periodo activo, incluyendo asignaciones realizadas en otras carreras.

La validación deberá realizarse antes de formalizar la asignación de la materia al docente.

El objetivo de este paso es determinar si la asignación solicitada es válida desde el punto de vista académico y de disponibilidad del docente.


## Servicio propietario

El servicio propietario de esta operación será: `SchedulingService`

La razón es que la validación se realiza sobre el contexto de la planeación académica y necesita considerar simultáneamente:

- Periodo académico activo.
- Materia.
- Grupo.
- Programa educativo.
- `Docente seleccionado`.
- Carga académica existente.
- Horas frente a grupo acumuladas.

`AcademicStaffService` seguirá siendo propietario de la información propia del docente, pero no será responsable de decidir si el docente puede ser asignado a una materia dentro de una planeación académica.

Conceptualmente:

```
AcademicStaffService
        │
        │ información del docente
        ▼
SchedulingService
        │
        │ valida dentro de la planeación
        ▼
Docente válido / Docente no válido

```

# Información requerida

Para realizar la validación, `SchedulingService` necesita reunir información de diferentes contextos.

Información de AcademicStaffService

`SchedulingService` deberá consultar la información necesaria del docente, por ejemplo:

- `TeacherId`.
- Perfil profesional.
- Programa educativo o adscripción correspondiente.
- Horas totales disponibles/contractuales, según el modelo definido en AcademicStaffService.
- `Estado del docente`.

Esta información pertenece a `AcademicStaffService`.


## Información de AcademicService

Cuando sea necesaria para determinar la compatibilidad académica:

- Programa educativo.
- Plan de estudios.
- Materia.
- Información académica de la materia.

Esta información pertenece a `AcademicService`.


## Información de SchedulingService

`SchedulingService` ya posee o puede consultar internamente:

- Periodo académico activo.
- Grupo.
- Oferta académica.
- Asignaciones existentes.
- Carga académica del docente.
- Horas frente a grupo acumuladas en el periodo activo.


## Regla de periodo académico activo

La disponibilidad del docente deberá calcularse exclusivamente sobre las asignaciones correspondientes al periodo académico activo.

Por ejemplo:

``` 
Docente José

Periodo 2025-1
  20 horas asignadas

Periodo 2025-2
  30 horas asignadas

Periodo 2026-1
  15 horas asignadas

```
Si el periodo activo es 2026-1, únicamente las 15 horas del periodo 2026-1 deberán afectar la disponibilidad.

Las cargas de:

- 2025-1
- 2025-2

no deberán disminuir la disponibilidad del docente en 2026-1.

Esta regla es importante porque evita que `SchedulingService` interprete el historial de carga académica como carga vigente.

## Validación de horas disponibles

La primera validación consiste en determinar si el docente cuenta con horas disponibles para aceptar la nueva asignación.

Por ejemplo:
```
Docente José
Horas totales: 40

Carga actual periodo activo:

Materia A - Grupo 1A → 4 horas
Materia B - Grupo 3A → 5 horas
Materia C - Grupo 5A → 4 horas

Horas frente a grupo:
4 + 5 + 4 = 13

Horas disponibles:
40 - 13 = 27

```
Si se pretende asignarle una materia de 5 horas:

```
Horas disponibles actuales: 27
Nueva asignación:            5
──────────────────────────────
Horas posteriores:          32
```

- La asignación es válida porque: 32 <= 40
- Pero si el docente ya tuviera: 38 horas frente a grupo
- y se intentara agregar una materia de: 5 horas
- el resultado sería: 38 + 5 = 43
- Como: 43 > 40

la operación deberá rechazarse.


## Las cargas de otras carreras también cuentan

La disponibilidad no deberá calcularse únicamente con las materias que el docente tenga asignadas dentro de la carrera que está realizando la planeación.

También deberán considerarse las asignaciones que el docente tenga en otras carreras durante el mismo periodo activo.

Por ejemplo:
```
Docente José
Horas totales: 40

Ingeniería en Sistemas
  Cálculo Integral - 1A → 4 horas
  Programación - 2A     → 5 horas

Ingeniería Industrial
  Física - 1B           → 5 horas

Administración
  Matemáticas           → 4 horas

Entonces:

Horas frente a grupo = 4 + 5 + 5 + 4
                     = 18 horas

Por lo tanto:

Horas disponibles = 40 - 18
                  = 22 horas

```

La carrera que origina la asignación no modifica el cálculo de disponibilidad.


## Validación de perfil profesional

`SchedulingService` deberá verificar que el perfil profesional del docente sea compatible con la materia que se pretende asignar.

La información del perfil pertenece a AcademicStaffService.

La regla de compatibilidad deberá evaluarse dentro del contexto académico de la materia.

Por ejemplo:
```
Materia:
Administración

Docente:
Contador Público
```
El docente podría ser considerado compatible si las reglas académicas determinan que su perfil es afín a la materia.

Otro ejemplo:
```
Materia:
Física

Docente:
Perfil relacionado con Física

```

## Validación de programa educativo

También deberá verificarse la relación del docente con el programa educativo.

La prioridad definida en el paso 7 es:

- 1. Docentes pertenecientes al programa educativo
- 2. Docentes de Ciencias Básicas
- 3. Docentes de otras áreas con perfil afín

En el paso 8 esta información se utiliza como criterio de validación de la asignación, no solamente como criterio de ordenamiento de la lista.

Por ejemplo:
```
Programa:
Ingeniería en Sistemas Computacionales

Materia:
Programación

Docente:
Docente perteneciente a ISC
```
La relación es compatible.

En cambio, un docente perteneciente a otra área deberá justificarse mediante la compatibilidad de su perfil con la materia cuando aplique.

## Validación integral

Las condiciones no deberán evaluarse de manera aislada.

La operación deberá considerar conjuntamente:

```

                  Docente
                     │
       ┌─────────────┼─────────────┐
       ▼             ▼             ▼
    Perfil       Programa      Horas disponibles
       │             │             │
       └─────────────┼─────────────┘
                     ▼
              Validación integral
                     │
              ┌──────┴──────┐
              ▼             ▼
           Cumple        No cumple
              │             │
              ▼             ▼
       Paso 9 continúa    Rechazar
```
El docente únicamente podrá continuar hacia el paso 9 cuando las condiciones necesarias hayan sido satisfechas.

## Consulta necesaria

La consulta principal pertenece a `SchedulingService`.

Conceptualmente: `ValidateTeacherAssignment` o el contrato equivalente que posteriormente se defina para la API.

La consulta deberá recibir el contexto necesario para realizar la validación, por ejemplo:

- `TenantId`
- `AcademicPeriodId`
- `TeacherId`
- `AcademicOfferingId`
- `GroupId`

No es necesario que el usuario proporcione nuevamente información que `SchedulingService` ya pueda resolver a partir del contexto de la planeación.


## Consultas entre servicios

Para realizar la validación, `SchedulingService` podrá necesitar consultar: `AcademicStaffService`

Para obtener:

`TeacherId`
- Perfil profesional
- Programa/adscripción
- Horas totales
- Estado

Conceptualmente:
```
SchedulingService
       │
       │ GET / consulta de docente
       ▼
AcademicStaffService

```

### AcademicService

Cuando `SchedulingService` necesite información académica que no posea directamente:

- Programa educativo
- Plan de estudios
- Materia
- Perfil requerido/relación académica

Conceptualmente:
```
SchedulingService
       │
       │ consulta información académica
       ▼
AcademicService

```

**SchedulingService**

La carga académica existente deberá consultarse dentro de `SchedulingService`, ya que es información de su propio dominio. No debe solicitarse a otro servicio.


## No consultar bases de datos externas

Esta validación no deberá realizarse mediante acceso directo a bases de datos de otros servicios.

No se permitirá:
```
SchedulingService
       │
       ├──── X AcademicStaffDb
       │
       └──── X AcademicDb
```
La comunicación correcta será:
```
SchedulingService
       │
       ├────► AcademicStaffService
       │
       └────► AcademicService
```
Cada servicio será responsable de exponer el contrato necesario para consultar su propia información.


## Comando

Este paso es principalmente una validación, por lo que no necesariamente requiere un comando que modifique el estado del sistema.

La operación puede representarse como una consulta/validación: `ValidateTeacherEligibility`

El resultado deberá indicar si el docente cumple las condiciones necesarias y, en caso contrario, qué condición impide continuar.

Por ejemplo:
```
{
    "eligible": false,
    "reasons": [
        "INSUFFICIENT_AVAILABLE_HOURS"
    ]
}
```
O:
```
{
    "eligible": true
}
```
El contrato exacto de la respuesta deberá definirse posteriormente al diseñar la API; PLAN-01 solamente establece la responsabilidad y el intercambio requerido, no implementa el `endpoint`.


## Evento

Para este paso no considero necesario definir un evento de integración. La razón es que verificar si un docente cumple las condiciones no modifica por sí misma el estado del dominio.

La operación es:

```
Consultar información
        ↓
Evaluar reglas
        ↓
Resultado

```

No:

```
Modificar estado
        ↓
Publicar evento

```
Por lo tanto: paso 8 no requiere un evento de integración obligatorio.

El evento tendrá mayor sentido en el paso 9, cuando efectivamente se registre la asignación del docente a la materia/grupo.


## TenantId

Todas las consultas y validaciones deberán ejecutarse dentro del `TenantId` correspondiente al contexto de la planeación.

El flujo deberá conservar el mismo contexto institucional:

```
AdminBff
   │
   │ TenantId
   ▼
SchedulingService
   │
   ├────► AcademicStaffService
   │
   └────► AcademicService

```

`SchedulingService` no deberá aceptar un `TenantId` proporcionado arbitrariamente por el cliente para consultar información de otra institución.

El contexto de `tenant` deberá provenir del contexto autenticado de la solicitud y propagarse entre los servicios de acuerdo con los mecanismos definidos para `SIA`.

Además, cualquier docente consultado deberá pertenecer al mismo contexto institucional de la operación.

## Responsabilidad de `AdminBff`

`AdminBff` podrá componer la información necesaria para que el usuario comprenda por qué un docente puede o no ser asignado.

Por ejemplo, podría presentar:
```
Docente: José Pérez

Programa: Ingeniería en Sistemas Computacionales
Perfil: Ingeniería en Sistemas Computacionales

Horas totales:       40
Horas frente grupo: 24
Horas disponibles:   16

Materia:
Cálculo Integral

Resultado:
✓ Cumple condiciones

```
O:

```
Docente: José Pérez

Horas totales:       40
Horas frente grupo: 38
Horas disponibles:    2

Materia:
Cálculo Integral → 4 horas

Resultado:
✕ No cuenta con horas suficientes

```

Pero `AdminBff` no deberá calcular por sí mismo las horas disponibles ni decidir si el docente cumple.

La responsabilidad será:

```
SchedulingService
        ↓
determina resultado
        ↓
AdminBff
        ↓
compone/presenta información
8.17 Dependencias técnicas

```
El paso 8 depende directamente de los pasos anteriores y de los servicios propietarios de la información.

```
Paso 7
Seleccionar docente
        │
        ▼
Paso 8
Verificar condiciones
        │
        ├── AcademicStaffService
        │      └── Información del docente
        │
        ├── AcademicService
        │      └── Información académica
        │
        └── SchedulingService
               └── Carga del periodo activo
        │
        ▼
Paso 9
Asignar materia al docente

```
La dependencia más importante es que el paso 8 necesita conocer la carga académica vigente del docente antes de permitir la asignación del paso 9.

## Ownership de la información

- **Información**      |  **Servicio propietario**	  |    **Uso en paso 8**
- Docente	           |    AcademicStaffService	  |    Identificar docente
- Perfil profesional   |    AcademicStaffService	  |    Validar afinidad
- Programa/adscripción |                              |
  del docente	       |    AcademicStaffService	  |    Validar relación/prioridad
- Horas totales del    |                              |
  docente	           |     AcademicStaffService*	  |     Determinar límite
- Materia	           |     AcademicService	      |     Determinar contexto académico
- Programa educativo   |     AcademicService	      |     Determinar contexto
- Plan de estudios	   |     AcademicService	      |     Determinar contexto
- Periodo activo	   |   SchedulingService / según  |
                       |     ownership definido	      |     Filtrar carga vigente
- Grupo	               |      SchedulingService	      |     Contexto de asignación
- Oferta académica	   |     SchedulingService	      |     Contexto de asignación
- Carga académica	   |     SchedulingService	      |     Calcular horas utilizadas
- Horas frente a grupo |     SchedulingService	      |     Calcular disponibilidad

* Siempre que las horas totales/contractuales formen parte de la información laboral/académica que actualmente administra `AcademicStaffService`. Si el modelo actual de SIA las ubica en otra entidad, el ownership deberá seguir al modelo real y no duplicarse en `SchedulingService`.

## Resultado del paso

El resultado del paso 8 será uno de dos estados: 

Docente cumple

```
Docente
   ↓
Perfil compatible
   ↓
Programa compatible
   ↓
Horas disponibles
   ↓
✓ Puede continuar
   ↓
Paso 9
Asignar materia al docente

``` 
Docente no cumple

```
Docente
   ↓
Validación
   ↓
✕ No cumple
   ↓
No se permite continuar

```
La respuesta deberá identificar la razón de rechazo cuando corresponda, por ejemplo:

- INSUFFICIENT_AVAILABLE_HOURS
- INCOMPATIBLE_PROFESSIONAL_PROFILE
- INVALID_EDUCATIONAL_PROGRAM
- TEACHER_NOT_AVAILABLE

Los nombres anteriores son propuesta de contrato, no implementación definitiva.


## Relación con los pasos 9 y 10

- Este punto es especialmente importante para que PLAN-01 no mezcle responsabilidades.
- El paso 8: Verifica que la asignación pueda realizarse.
- El paso 9: Realiza la asignación de la materia al docente.
- El paso 10: Determina/establece las horas frente a grupo derivadas de esa asignación.

Por lo tanto:

```
7. Seleccionar docente
          │
          ▼
8. Verificar condiciones
          │
       ¿Cumple?
       /      \
     NO        SÍ
     │          │
     ▼          ▼
 Rechazar      9. Asignar materia
                    │
                    ▼
               10. Horas frente
                   a grupo
```

Y existe una regla fundamental: El paso 8 no debe consumir horas ni modificar la carga académica.

La disponibilidad se consulta/calcula para validar.

El consumo efectivo de horas ocurre cuando la asignación del paso 9 queda registrada y las horas frente a grupo del paso 10 forman parte de esa carga.

Esto evita que una simple consulta de candidatos o una validación fallida modifique la carga del docente.