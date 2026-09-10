# 3. Módulo de Planeación Académica

Servicio propietario: SchedulingService

Una vez establecido:
```
Usuario autenticado
        +
DivisionHead identificado
        +
EducationalProgram seleccionado
        ↓
Módulo de Planeación Académica

```
el usuario puede acceder al módulo de Planeación Académica.

`SchedulingService` es responsable de la planeación académica y, por lo tanto, de la información relacionada con:

- Cargas académicas.
- Planeación docente.
- Grupos.
- Oferta académica.
- Horarios.
- Asignaciones correspondientes a la planeación.

En este paso **todavía no** se crea una nueva carga académica.

El usuario puede visualizar las cargas existentes, incluyendo:

- Cargas actuales.
- Cargas anteriores.

La creación de una nueva carga queda condicionada por el paso **4: Consultar periodo escolar activo.**

---

## Consulta necesaria:

`SchedulingService` deberá proporcionar las cargas académicas correspondientes al contexto seleccionado:

`TenantId`
`EducationalProgramId`

y, cuando corresponda, al periodo académico.

Información resultante:

`TenantId`
`EducationalProgramId`
`DivisionHeadId`
`AcademicPeriodId`

El `AcademicPeriodId` será relevante para determinar si puede iniciarse una nueva planeación, pero su consulta corresponde al paso siguiente.

---

## `AdminBff`:

Puede componer una vista como:

Planeación Académica

Programa:
Ingeniería en Sistemas Computacionales
```
Cargas académicas
├── Periodo actual
├── Periodos anteriores
└── Nueva carga académica
        │
        └── Requiere validar periodo activo

```
No deberá determinar por sí mismo si existe un periodo activo.

---

## Contexto técnico acumulado de los pasos 1–3

Hasta este punto, el flujo puede representarse así:
```
1. Ingresar al SIA
        │
        ▼
IdentityService
        │
        │ UserId / contexto de autenticación
        ▼
2. Seleccionar programa educativo
        │
        ▼
AcademicStaffService
        │
        │ DivisionHead → EducationalProgram
        ▼
Contexto del programa
        │
        ▼
3. Módulo de Planeación Académica
        │
        ▼
SchedulingService
        │
        ├── Consultar cargas actuales
        ├── Consultar cargas anteriores
        │
        └── Nueva carga
                │
                ▼
        4. Consultar periodo escolar activo
```
---

### Contexto que deberá conservarse

A medida que avance el flujo, tendremos que conservar como mínimo:

- TenantId
- UserId
- DivisionHeadId
- EducationalProgramId

Y posteriormente:

- AcademicPeriodId

cuando sea determinado por el paso 4.