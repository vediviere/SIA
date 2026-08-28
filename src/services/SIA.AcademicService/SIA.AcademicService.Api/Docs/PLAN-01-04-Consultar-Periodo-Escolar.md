# 4. Consultar periodo escolar activo

Servicio propietario: AcademicService

Responsabilidad:

`AcademicService` es propietario de `AcademicPeriod`, por lo que es responsable de determinar qué periodo escolar se encuentra activo para el contexto del `TenantId` y consultar la información necesaria para continuar con la Planeación Académica.

En este paso no se crea ni modifica el periodo escolar. Solamente se consulta su estado y las fechas configuradas para determinar si el proceso de Planeación Académica puede continuar.

---

## Consulta necesaria

`AcademicService` deberá proporcionar el periodo escolar correspondiente al `TenantId` recibido en el contexto de la operación.

La consulta deberá considerar:

- `TenantId`.
- Estado del `AcademicPeriod`.
- Fechas configuradas para el proceso de carga académica del docente.

De acuerdo con el modelo actual de `AcademicPeriod`, se dispone de:
```
AcademicPeriod
├── AcademicPeriodId
├── TenantId
├── Code
├── Name
├── StartDate
├── EndDate
├── AcademicLoadProcessStartDate
├── ...
└── Status
```

---

## Resultado de la consulta

La respuesta deberá permitir identificar:

- AcademicPeriodId
- Code
- Name
- Status
- AcademicLoadProcessStartDate
- AcademicLoadProcessEndDate

El resultado será utilizado posteriormente por `SchedulingService` para determinar si puede iniciarse o registrarse una nueva carga académica.

---

## Regla funcional

La existencia de un periodo escolar no significa automáticamente que se pueda crear una nueva carga académica.

Para iniciar una nueva carga deberán considerarse las fechas definidas específicamente para el proceso:
```
AcademicLoadProcessStartDate
        ↓
Periodo permitido para iniciar carga
        ↓
AcademicLoadProcessEndDate

```

Por lo tanto, `AcademicService` proporciona la información oficial del periodo y sus fechas, mientras que la operación de creación de la carga permanecerá bajo responsabilidad de `SchedulingService`.

---

## TenantId

La consulta deberá realizarse siempre dentro del contexto del `TenantId`.

`AcademicService` no deberá devolver un periodo perteneciente a otro tenant.

El `TenantId` no deberá ser seleccionado ni modificado por el frontend para obtener información de otro contexto institucional.

---

## AdminBff

AdminBff podrá solicitar la información a `AcademicService` y componerla con la información necesaria para presentar al usuario el estado del periodo.

AdminBff no deberá:

- Consultar directamente SIA_AcademicDb.
- Determinar por sí mismo cuál es el periodo escolar.
- Modificar el TenantId.
- Crear o modificar AcademicPeriod.
- Implementar la lógica propietaria del periodo.

---

## Comando

En este paso no existe un comando de negocio, debido a que no se modifica información.

La operación corresponde a una:

``` Consulta → AcademicService ```

---

## Evento de integración

No se requiere un evento de integración para este paso.

El periodo ya existe y únicamente se está consultando su información.

---

## Dependencias técnicas

Este paso depende de:

- AcademicPeriod administrado por AcademicService.
- TenantId establecido en el contexto de la operación.
- Contexto de EducationalProgram seleccionado previamente.

El resultado de esta consulta será una dependencia para el paso posterior de la Planeación Académica, particularmente para determinar si es posible continuar con la creación de una nueva carga académica.

---

## Regla de comunicación

Ningún servicio deberá consultar directamente la base de datos de `AcademicService`.

La comunicación deberá realizarse mediante el contrato o mecanismo de comunicación definido para `AcademicService`.

```
AdminBff
   │
   │ Consulta AcademicPeriod
   │ TenantId + CorrelationId
   ▼
AcademicService
   │
   ▼
AcademicPeriod
   │
   ├── AcademicPeriodId
   ├── Status
   ├── AcademicLoadProcessStartDate
   └── AcademicLoadProcessEndDate
   │
   ▼
AdminBff / SchedulingService

```

---

## Resultado esperado del paso

Al finalizar este paso, el flujo deberá contar con la información del periodo escolar que corresponde al `TenantId` actual y, específicamente, con las fechas que permitirán determinar si la creación de una nueva carga académica puede continuar.