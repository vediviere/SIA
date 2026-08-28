# PLAN-01 — Paso 2: Seleccionar programa educativo

## Actividad BPMN

2. Seleccionar programa educativo

Después de autenticarse, el sistema debe determinar los programas educativos que el usuario tiene autorizados para administrar.

Si el usuario tiene:

- Un solo programa educativo: el sistema lo selecciona automáticamente.
- Más de un programa educativo: el sistema solicita al usuario seleccionar cuál programa desea administrar.

El programa seleccionado establece el contexto de trabajo para los pasos posteriores del flujo BPMN-PR-001.

--- 

## Servicio propietario

AcademicStaffService

`AcademicStaffService` es propietario de la información que relaciona al responsable de división con los programas educativos que tiene bajo su responsabilidad.

AcademicService es propietario de los datos propios del `EducationalProgram`, pero no decide qué programas puede administrar un jefe de división.

Por lo tanto:
```
AcademicStaffService
        │
        │ programas autorizados
        ▼
    DivisionHead
        │
        ├── EducationalProgram A
        └── EducationalProgram B

```
Mientras que:

```
AcademicService
        │
        └── EducationalProgram
             ├── Code
             ├── Name
             ├── Level
             └── ...
```
---

## Tipo de interacción

Principalmente una consulta.

Conceptualmente:
```
AdminBff
    │
    │ ¿Qué programas puede administrar este usuario?
    ▼
AcademicStaffService
    │
    ▼
Programas asociados al DivisionHead
```
Posteriormente, `AdminBff` puede utilizar esa información para presentar la selección al usuario.

---

## Flujo cuando existe un solo programa

```
Usuario autenticado
        │
        ▼
AdminBff
        │
        │ consultar programas autorizados
        ▼
AcademicStaffService
        │
        ▼
1 programa
        │
        ▼
Seleccionar automáticamente
        │
        ▼
Continuar PR-001

```
El usuario no necesita realizar una selección manual.

---

## Flujo cuando existen varios programas

```
Usuario autenticado
        │
        ▼
AdminBff
        │
        ▼
AcademicStaffService
        │
        ▼
2 o más programas
        │
        ▼
AdminBff muestra opciones
        │
        ▼
Usuario selecciona programa
        │
        ▼
Programa seleccionado
        │
        ▼
Continuar PR-001

```
---

## Información requerida

Para determinar las opciones disponibles, `AcademicStaffService` necesita identificar al responsable de división autenticado.

La consulta deberá devolver únicamente los programas que ese usuario está autorizado a administrar.

Conceptualmente:

```
DivisionHead
    │
    ├── EducationalProgramId
    ├── EducationalProgramId
    └── ...
```
Los datos completos del programa educativo no tienen que ser propiedad de `AcademicStaffService`.

Si posteriormente se requiere información adicional de `EducationalProgram`, deberá obtenerse mediante el contrato correspondiente con `AcademicService`.

---

## ¿Quién obtiene la información del programa?

Aquí es importante no confundir referencia con propiedad.

`AcademicStaffService` puede conservar: `EducationalProgramId` como referencia externa. `AcademicService` es quien posee: `EducationalProgram`

Por lo tanto:
```
AcademicStaffService
        │
        │ EducationalProgramId
        ▼
AcademicService
        │
        ▼
EducationalProgram
```
No se debe crear una copia completa de EducationalProgram dentro de `AcademicStaffService` únicamente para realizar esta selección.

---

## AdminBff

En este paso `AdminBff` tiene un papel importante.

Puede:

- Solicitar a `AcademicStaffService` los programas autorizados.
- Determinar la experiencia de UI correspondiente a uno o múltiples programas.
- Mostrar el selector cuando existen múltiples opciones.
- Seleccionar automáticamente cuando solamente existe una.
- Mantener el programa seleccionado como parte del contexto de la operación.
- Enviar posteriormente el identificador del programa seleccionado a los servicios que lo necesiten.

No debe:

- Determinar por sí mismo qué programas puede administrar el usuario.
- Confiar en un `EducationalProgramId` enviado libremente por el frontend.
- Consultar directamente `SIA_AcademicStaffDb`.
- Consultar directamente `SIA_AcademicDb`.

---

## TenantId

El contexto institucional deberá conservarse durante todo el flujo.

Conceptualmente:
```
TenantId
   │
   ▼
Usuario autenticado
   │
   ▼
Programas autorizados
   │
   ▼
Programa seleccionado
   │
   ▼
PR-001

```

`AcademicStaffService` deberá validar que la relación del `DivisionHead` con el programa corresponde al mismo `TenantId`.

Esto evita que un usuario pueda intentar seleccionar un programa perteneciente a otra institución manipulando un identificador.

---

## Dependencias

Este paso depende de:
```
- `IdentityService` — usuario autenticado.
- `AcademicStaffService` — relación DivisionHead → `EducationalProgramId`.
- `AcademicService` — información del `EducationalProgram`, cuando sea necesaria.
```
Y será utilizado por los pasos posteriores de:

- AcademicService
- AcademicStaffService
- SchedulingService
- WorkflowService

dependiendo de qué operación realice cada paso.