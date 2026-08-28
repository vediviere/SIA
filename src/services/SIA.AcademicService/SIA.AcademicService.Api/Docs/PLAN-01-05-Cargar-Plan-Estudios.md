# 5. Cargar plan de estudios

Servicio propietario: `AcademicService`

Responsabilidad:

`AcademicService` es propietario de la estructura académica del programa educativo, incluyendo el `StudyPlan`, por lo que es responsable de consultar y proporcionar el plan de estudios correspondiente al `EducationalProgramId` seleccionado en el contexto de la operación.

En este paso no se modifica el plan de estudios. Únicamente se consulta la información necesaria para continuar con la Planeación Académica.

---

## Consulta necesaria

`AcademicService` deberá proporcionar el `StudyPlan` correspondiente al:

- TenantId
- EducationalProgramId

La consulta deberá considerar el estado y vigencia del plan de estudios para determinar cuál corresponde utilizar en el contexto académico actual.

La información relevante incluye:
```
StudyPlan
├── StudyPlanId
├── TenantId
├── EducationalProgramId
├── Code
├── Name
├── Version
├── EffectiveFrom
└── Status
```
---

## Resultado de la consulta

La respuesta deberá permitir identificar el plan de estudios que será utilizado durante la planeación:

- StudyPlanId
- EducationalProgramId
- Code
- Name
- Version
- EffectiveFrom
- Status

El `StudyPlanId` deberá conservarse como parte del contexto del flujo, ya que será necesario para las consultas posteriores relacionadas con las materias que conforman el plan.

---

## TenantId

La consulta deberá realizarse dentro del `TenantId` correspondiente al contexto actual.

No deberá ser posible cargar un `StudyPlan` perteneciente a otro `TenantId`.

El `TenantId` no deberá ser proporcionado libremente por el frontend para seleccionar información académica de otra institución.

---

## AdminBff

`AdminBff` podrá solicitar y presentar la información del plan de estudios al usuario.

Puede componer la información necesaria para mostrar, por ejemplo:

Programa educativo:
Ingeniería en Sistemas Computacionales

Plan de estudios:
ISIC-2010-224

Versión:
...

Sin embargo, `AdminBff` no deberá:

- Consultar directamente `SIA_AcademicDb`.
- Determinar cuál es el propietario del `StudyPlan`.
- Modificar el `StudyPlan`.
- Mantener una copia autoritativa del plan.
- Seleccionar un `StudyPlan` perteneciente a otro `TenantId`.

---

## Comando

En este paso no existe un comando de negocio, debido a que no se modifica información.

La operación corresponde a una:

`Consulta → AcademicService`

---

## Evento de integración

No se requiere un evento de integración para este paso.

La información del plan de estudios ya es propiedad de `AcademicService` y solamente se está consultando.

---

## Dependencias técnicas

Este paso depende de:

- `TenantId` establecido en el contexto.
- `EducationalProgramId` seleccionado previamente.
- `AcademicService`.
- `StudyPlan` existente y disponible para el programa educativo.

El resultado proporciona: `StudyPlanId`

que será utilizado por los pasos posteriores para consultar la estructura académica del plan.

---

## Regla de comunicación

Ningún servicio deberá consultar directamente las tablas de `AcademicService`.

La información deberá obtenerse mediante el contrato o mecanismo de comunicación definido para `AcademicService`.
```
AdminBff
   │
   │ Consulta StudyPlan
   │ TenantId + EducationalProgramId
   ▼
AcademicService
   │
   ▼
StudyPlan
   │
   ├── StudyPlanId
   ├── Code
   ├── Name
   ├── Version
   └── Status
   │
   ▼
Contexto de Planeación Académica
```
---

## Resultado esperado del paso

Al finalizar este paso, el flujo deberá contar con el `StudyPlan` correspondiente al programa educativo seleccionado y con su `StudyPlanId`, que permitirá continuar con las consultas de la estructura académica necesaria para la Planeación Académica.