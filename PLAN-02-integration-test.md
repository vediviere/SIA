# PLAN-02 --- Prueba de integración del primer tramo de PR-001

## 1. Objetivo

Documentar una prueba de integración **repetible** del primer tramo real
de PR-001, atravesando los componentes desarrollados durante el sprint y
verificando que una propuesta de carga académica pueda construirse y
enviarse a revisión mediante el flujo de microservicios.

El flujo objetivo de PLAN-02 es:

``` 
Cliente / Frontend
        |
        v
PublicGateway
        |
        v
AdminBff
        |
        +-----------------------+
        |                       |
        v                       v
AcademicService       AcademicStaffService
        |                       |
        +-----------+-----------+
                    |
                    v
            SchedulingService
                    |
                    v
        AcademicLoad / Proposal
                    |
                    v
                  Outbox
                    |
                    v
                 RabbitMQ
                    |
                    v
             WorkflowService
                    |
                    v
          ReviewProcess: InReview
```

La prueba no pretende validar aisladamente un único endpoint. Su
propósito es comprobar que el primer tramo funcional puede recorrer los
límites de los microservicios respetando los contratos, `TenantId`,
`CorrelationId`, Outbox/Inbox y la separación de bases de datos.

------------------------------------------------------------------------

## 2. Alcance

La prueba cubre:

-   obtención del cono académico necesario;
-   obtención y selección de un docente elegible;
-   creación de una propuesta de carga académica;
-   creación de al menos una carga académica asociada a la propuesta;
-   envío de la propuesta a revisión;
-   persistencia del Integration Event mediante Outbox;
-   publicación del evento;
-   recepción del evento por `WorkflowService`;
-   persistencia idempotente mediante Inbox;
-   creación del `ReviewProcess` en estado `InReview`;
-   conservación del `TenantId`;
-   trazabilidad mediante `CorrelationId`.

No se realiza acceso directo desde un microservicio hacia la base de
datos de otro microservicio.

------------------------------------------------------------------------

## 3. Componentes involucrados

Para ejecutar el escenario deben estar disponibles los componentes que
participan en el flujo:

-   `PublicGateway`
-   `AdminBff`
-   `AcademicService`
-   `AcademicStaffService`
-   `SchedulingService`
-   `WorkflowService`
-   SQL Server y las bases correspondientes
-   RabbitMQ
-   infraestructura compartida de mensajería/Outbox de `BuildingBlocks`

La prueba requiere autenticación. El token utilizado debe contener un
`tenant_id` válido, ya que los servicios que participan en el flujo
obtienen el tenant desde el cono autenticado.

------------------------------------------------------------------------

## 4. Datos reproducibles del escenario

Los scripts preparados para PLAN-02 utilizan identificadores conocidos
para que el escenario pueda reconstruirse.

  -----------------------------------------------------------------------------------------
  Dato                                             Valor
  ------------------------------------------------ ----------------------------------------
  `TenantId`                                       `11111111-1111-1111-1111-111111111111`

  `EducationalProgramId`                           `22222222-2222-2222-2222-222222222222`

  `AcademicPeriodId`                               `33333333-3333-3333-3333-333333333333`

  `StudyPlanId`                                    `44444444-4444-4444-4444-444444444444`

  `SubjectId`                                      `55555555-5555-5555-5555-555555555555`

  `StudyPlanSubjectId`                             `AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA`

  `TeacherPersonId`                                `66666666-6666-6666-6666-666666666666`

  `TeacherId`                                      `77777777-7777-7777-7777-777777777777`

  `DivisionHeadPersonId`                           `88888888-8888-8888-8888-888888888888`

  `DivisionId / DivisionHeadId usado en PLAN-02`   `99999999-9999-9999-9999-999999999999`
  -----------------------------------------------------------------------------------------

Datos académicos utilizados:

-   Programa: `ISC` --- Ingeniería en Sistemas Computacionales.
-   Nivel: Licenciatura.
-   Periodo: `2026-2` --- Agosto-Diciembre 2026.
-   Ventana de planeación: `2026-09-01` a `2026-09-30`.
-   Plan de estudios: `ISC-2026`, versión `1.0`.
-   Materia: `SIA-101` --- Programación Orientada a Objetos.
-   Semestre: `1`.
-   Créditos: `5`.
-   Horas de teoría: `3`.
-   Horas de práctica: `2`.
-   Docente: Ana Martínez López.
-   Perfil profesional: Desarrollo de software y programación.
-   Tipo de contrato: Tiempo Completo.
-   Horas de contrato: `40`.
-   Jefe de división de prueba: Carlos Hernández García.

> Las fechas del periodo de prueba fueron preparadas para que la
> ejecución realizada en septiembre de 2026 estuviera dentro de la
> ventana válida de planeación.

------------------------------------------------------------------------

## 5. Preparación de las bases de datos

### 5.1 AcademicService

Ejecutar el script de datos de PLAN-02 sobre `SIA_AcademicDb`.

El script es idempotente mediante validaciones `IF NOT EXISTS` y crea,
cuando no existen:

1.  Educational Program.
2.  Academic Period.
3.  Study Plan.
4.  Subject.
5.  Study Plan Subject.

Antes de continuar, comprobar que todos los registros pertenecen al
mismo:

``` 
TenantId = 11111111-1111-1111-1111-111111111111
```

### 5.2 AcademicStaffService

Ejecutar el script de datos de PLAN-02 sobre `SIA_AcademicStaffDb`.

El script crea, cuando no existen:

1.  persona del docente;
2.  docente elegible;
3.  persona del jefe de división;
4.  registro de `DivisionHeads`.

El docente debe estar activo, asociado al programa educativo de PLAN-02
y tener `ContractHours = 40`.

### 5.3 SchedulingService

Antes de iniciar la prueba, verificar que el esquema físico de la base
de datos de `SchedulingService` esté actualizado respecto del modelo EF
Core actual.

Durante la primera ejecución de PLAN-02 se detectó una **migración
pendiente en SchedulingService**. La base no estaba completamente
alineada con el esquema que esperaba el código y esto impidió continuar
normalmente con el escenario.

Por lo tanto, antes de diagnosticar un error como problema de lógica de
negocio o integración:

1.  revisar las migraciones existentes del proyecto;
2.  comprobar cuáles están aplicadas en la base;
3.  aplicar la migración pendiente correspondiente;
4.  volver a ejecutar el escenario.

Este hallazgo es relevante porque un fallo de persistencia causado por
un esquema desactualizado puede parecer inicialmente un problema del
endpoint, del UseCase o del flujo entre microservicios.

> Este documento no fija el nombre de una migración concreta porque ese
> dato no forma parte de la evidencia conservada de PLAN-02. Debe
> comprobarse contra el estado actual del repositorio antes de aplicar
> cambios.

------------------------------------------------------------------------

## 6. Consideración importante: `DivisionId` vs. `DivisionHeadId`

Durante la preparación del escenario se detectó una discrepancia de
naming que debe conocerse antes de ejecutar la prueba.

En la tabla:

``` 
SIA_AcademicStaffDb.dbo.DivisionHeads
```

la clave utilizada por el script es:

``` 
DivisionId
```

y el registro de PLAN-02 se crea con:

``` 
99999999-9999-9999-9999-999999999999
```

Sin embargo, el flujo de creación de la propuesta utiliza
conceptualmente:

``` 
DivisionHeadId
```

y `ProposalSubmittedForReviewIntegrationEvent` también transporta:

``` 
DivisionHeadId
```

Para la ejecución validada de PLAN-02 se utilizó el mismo GUID:

``` 
99999999-9999-9999-9999-999999999999
```

como valor requerido por el flujo.

Esto **no debe interpretarse como una corrección definitiva del
modelo**. Existe una discrepancia entre el naming del esquema de
`AcademicStaffService` y el naming utilizado por los contratos/código
del flujo de propuesta.

La decisión de renombrar o modificar este identificador queda fuera del
alcance de PLAN-02 y debe revisarse posteriormente considerando, como
mínimo:

-   entidad de dominio;
-   configuración de EF Core;
-   esquema SQL;
-   contratos;
-   clientes del BFF;
-   `SchedulingService`;
-   `WorkflowService`;
-   compatibilidad con datos existentes.

------------------------------------------------------------------------

## 7. Ejecución del escenario

Todas las llamadas deben realizarse a través de la entrada expuesta por
`PublicGateway`, permitiendo que el request continúe hacia `AdminBff`.
Las URLs base y puertos dependen del perfil de ejecución local, por lo
que aquí se documentan las rutas relativas.

Usar un token válido para el tenant:

``` 
11111111-1111-1111-1111-111111111111
```

Para facilitar la trazabilidad puede utilizarse un `X-Correlation-Id`
conocido. En la ejecución que confirmó el flujo completo se utilizó:

``` 
BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

### 7.1 Obtener el cono académico

Request:

``` http
GET /api/academic-planning/con/22222222-2222-2222-2222-222222222222
Authorization: Bearer <token>
X-Correlation-Id: BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

El `AdminBff` obtiene el `TenantId` de su `ITenantCon` y compone la
información procedente del cono académico y de los candidatos
docentes.

Validar en la respuesta:

-   periodo `2026-2`;
-   programa `ISC`;
-   plan `ISC-2026`;
-   materia `SIA-101`;
-   `IsWithinPlanningWindow = true`;
-   presencia del docente `77777777-7777-7777-7777-777777777777`.

Esta operación permite comprobar simultáneamente que el BFF puede
obtener el cono académico y los candidatos necesarios para iniciar
la planeación.

### 7.2 Seleccionar docente elegible

Del cono anterior seleccionar:

``` 
TeacherId = 77777777-7777-7777-7777-777777777777
```

El docente esperado pertenece al programa:

``` 
22222222-2222-2222-2222-222222222222
```

y está activo.

### 7.3 Crear propuesta

Request:

``` http
POST /api/academic-planning/proposals
Authorization: Bearer <token>
Content-Type: application/json
X-Correlation-Id: BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

Body:

``` json
{
  "educationalProgramId": "22222222-2222-2222-2222-222222222222",
  "academicPeriodId": "33333333-3333-3333-3333-333333333333",
  "divisionHeadId": "99999999-9999-9999-9999-999999999999"
}
```

La creación debe devolver `201 Created`.

`SchedulingService` crea la propuesta y genera también un
`ProposalCreatedIntegrationEvent`, persistiendo propuesta y mensaje de
Outbox mediante la misma operación de persistencia.

Para la ejecución que finalmente confirmó PLAN-02 se obtuvo:

``` 
ProposalId = 69041120-9D01-4181-8C9B-2A1624DF6075
```

En una nueva ejecución el `ProposalId` puede ser diferente.

### 7.4 Crear una carga académica

La propuesta no puede enviarse a revisión si no contiene al menos una
carga académica activa. `SubmitProposalForReviewUseCase` valida esta
condición mediante `HasAcademicLoadsAsync`.

Request:

``` http
POST /api/academic-planning/proposals/{proposalId}/loads
Authorization: Bearer <token>
Content-Type: application/json
X-Correlation-Id: BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

Body base:

``` json
{
  "teacherId": "77777777-7777-7777-7777-777777777777",
  "divisionId": "99999999-9999-9999-9999-999999999999",
  "academicPeriodId": "33333333-3333-3333-3333-333333333333",
  "officialLetterNumber": "<valor de prueba>",
  "proposedDate": "<fecha válida>",
  "assignmentDate": "<fecha válida>"
}
```

Los valores concretos del número de oficio y fechas deben cumplir las
reglas vigentes de `SchedulingService`.

En la ejecución validada se creó:

``` 
AcademicLoadId = 0C4AB27A-1DE7-4D1D-8D6E-BB43CD190B01
```

En una repetición el identificador puede ser diferente.

### 7.5 Enviar la propuesta a revisión

Request:

``` http
POST /api/academic-planning/proposals/{proposalId}/submit-for-review
Authorization: Bearer <token>
X-Correlation-Id: BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

Para el escenario confirmado:

``` http
POST /api/academic-planning/proposals/69041120-9D01-4181-8C9B-2A1624DF6075/submit-for-review
```

`SubmitProposalForReviewUseCase` realiza conceptualmente:

``` 
1. Busca la propuesta por TenantId + ProposalId.
2. Verifica que exista.
3. Verifica que la propuesta sea editable.
4. Comprueba que tenga al menos una AcademicLoad activa.
5. Ejecuta proposal.SubmitForReview().
6. Construye ProposalSubmittedForReviewIntegrationEvent.
7. Persiste la propuesta actualizada y el OutboxMessage.
8. Devuelve la respuesta.
```

El Integration Event contiene:

``` 
EventId
CorrelationId
OccurredAtUtc
TenantId
ProposalId
EducationalProgramId
AcademicPeriodId
DivisionHeadId
ProposalStatus
Status
Version = 1
```

------------------------------------------------------------------------

## 8. Verificación del Outbox

Después del `submit-for-review`, consultar `OutboxMessages` en la base
de `SchedulingService`.

Ejemplo de consulta:

``` sql
SELECT
    Id,
    EventType,
    OccurredAtUtc,
    ProcessedAtUtc,
    LastAttemptAtUtc,
    NextAttemptAtUtc,
    DeadLetteredAtUtc,
    RetryCount,
    Error,
    CorrelationId
FROM dbo.OutboxMessages
WHERE CorrelationId = 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'
ORDER BY OccurredAtUtc DESC;
```

Debe existir el mensaje correspondiente a:

``` 
ProposalSubmittedForReviewIntegrationEvent.v1
```

o al valor exacto definido actualmente por:

``` 
SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1
```

Cuando el publisher lo procesa correctamente:

-   `ProcessedAtUtc` debe contener valor;
-   `Error` debe ser `NULL`;
-   el mensaje no debe quedar dead-lettered.

------------------------------------------------------------------------

## 9. Incidencia encontrada: registro del Integration Event

Durante la primera ejecución, la existencia del mensaje en Outbox no fue
suficiente para completar el flujo.

La infraestructura compartida utiliza `OutboxEventRegistry` para
resolver el `EventType` almacenado en el Outbox y obtener el tipo CLR
que posteriormente debe deserializarse y publicarse.

Por lo tanto, `ProposalSubmittedForReviewIntegrationEvent` debe estar
registrado con el mismo `EventType` utilizado al crear el
`OutboxMessage`.

Conceptualmente:

``` 
OutboxMessage.EventType
        |
        v
OutboxEventRegistry.Resolve(eventType)
        |
        v
JsonSerializer.Deserialize(payload, CLR type)
        |
        v
MassTransit Publish
        |
        v
RabbitMQ
```

El problema encontrado consistió en que el nuevo evento de
`SchedulingService` necesitaba estar incluido en esa
configuración/registro. Sin dicha correspondencia, el mensaje podía
existir correctamente en Outbox pero el publisher no podía completar el
siguiente tramo del recorrido.

### Qué comprobar

Revisar la configuración actual de `SchedulingService` y confirmar que
exista el registro de:

``` 
ProposalSubmittedForReviewIntegrationEvent
```

contra:

``` 
SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1
```

No agregar registros duplicados ni asumir una ubicación específica: la
configuración debe revisarse contra la composición actual del servicio y
los `BuildingBlocks`.

------------------------------------------------------------------------

## 10. Verificación en WorkflowService

`WorkflowService` consume el evento publicado por `SchedulingService`.

El UseCase de creación del proceso comprueba primero la idempotencia:

``` 
WasProcessedAsync(EventId)
```

Si el evento no había sido procesado, crea:

``` 
ReviewProcess
```

y persiste de forma consistente:

``` 
ReviewProcess + InboxMessage
```

El constructor de `ReviewProcess` establece:

``` 
Status = InReview
```

### 10.1 Verificar Inbox

Consulta:

``` sql
SELECT
    Id,
    EventType,
    SourceService,
    ReceivedAtUtc,
    ProcessedAtUtc,
    RetryCount,
    Error,
    CorrelationId
FROM dbo.InboxMessages
WHERE CorrelationId = 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB'
ORDER BY ReceivedAtUtc DESC;
```

Evidencia obtenida en la ejecución exitosa:

``` 
Id             = 87708C00-3983-47CA-9A03-F83A667253FC
EventType      = ProposalSubmittedForReviewIntegrationEvent.v1
SourceService  = SIA.SchedulingService
ReceivedAtUtc  = 2026-09-13 04:49:23.3014347
ProcessedAtUtc = 2026-09-13 04:49:23.7348154
RetryCount     = 0
Error          = NULL
CorrelationId  = BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

Esto confirma que `WorkflowService` recibió y procesó el Integration
Event.

### 10.2 Verificar ReviewProcess

Consulta:

``` sql
SELECT
    Id,
    TenantId,
    AcademicLoadProposalId,
    DivisionHeadId,
    Version,
    Status,
    SubmittedAtUtc,
    CreatedAtUtc,
    UpdatedAtUtc,
    CorrelationId
FROM dbo.ReviewProcesses
WHERE AcademicLoadProposalId = '69041120-9D01-4181-8C9B-2A1624DF6075';
```

Evidencia confirmada:

``` 
Id                     = 4B86A8F9-3EDA-4426-ABA8-2182355ABF5C
TenantId               = 11111111-1111-1111-1111-111111111111
AcademicLoadProposalId = 69041120-9D01-4181-8C9B-2A1624DF6075
DivisionHeadId         = 99999999-9999-9999-9999-999999999999
Version                = 1
Status                 = 1
```

El código de dominio de `ReviewProcess` establece el estado inicial como
`InReview`; en la evidencia de la ejecución dicho estado quedó
persistido con el valor numérico `1`.

No se incluyen aquí fechas adicionales de `ReviewProcesses` que no
quedaron registradas íntegramente en la evidencia conservada.

------------------------------------------------------------------------

## 11. Trazabilidad con `CorrelationId`

Para la ejecución confirmada se utilizó:

``` 
BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
```

El valor permite relacionar el request con los registros generados
durante el flujo, especialmente:

``` 
Request
   |
   v
SchedulingService
   |
   v
OutboxMessage
   |
   v
Integration Event
   |
   v
WorkflowService
   |
   +--> InboxMessage
   |
   +--> ReviewProcess
```

Al repetir la prueba se recomienda generar un `CorrelationId` nuevo para
distinguir claramente una ejecución de otra.

------------------------------------------------------------------------

## 12. Validación de `TenantId`

El tenant utilizado es:

``` 
11111111-1111-1111-1111-111111111111
```

Debe conservarse durante todo el recorrido.

Puntos relevantes:

-   `AdminBff` obtiene el tenant mediante `ITenantCon`.
-   `SchedulingService` protege las operaciones de propuesta y obtiene
    `tenant_id` del token.
-   `ProposalDataStore.GetByIdAsync` filtra por `TenantId` y
    `ProposalId`.
-   `HasAcademicLoadsAsync` filtra por `TenantId`, `ProposalId` y estado
    activo.
-   el Integration Event transporta `TenantId`;
-   `WorkflowService` utiliza el `TenantId` recibido al construir
    `ReviewProcess`.

La prueba exitosa confirmó el mismo tenant en el proceso final de
Workflow.

------------------------------------------------------------------------

## 13. Problemas encontrados durante la primera ejecución

### 13.1 Migración pendiente en SchedulingService

**Síntoma:** el flujo no podía continuar correctamente al trabajar con
la persistencia de `SchedulingService`.

**Causa encontrada:** existía una migración pendiente y el esquema
físico de la base no correspondía completamente al modelo esperado por
el código.

**Resolución:** identificar y aplicar la migración pendiente antes de
continuar.

**Aprendizaje:** antes de modificar UseCases, contratos o DataStores por
un error de persistencia, comprobar primero que la base esté alineada
con las migraciones del servicio.

### 13.2 Integration Event no registrado para el publisher de Outbox

**Síntoma:** el evento podía estar persistido en Outbox, pero el flujo
no llegaba a `WorkflowService`.

**Causa encontrada:** el publisher compartido depende de
`OutboxEventRegistry` para convertir el `EventType` persistido en el
tipo CLR correspondiente. El nuevo
`ProposalSubmittedForReviewIntegrationEvent` necesitaba formar parte de
esa configuración.

**Resolución:** registrar/configurar el Integration Event utilizando el
mismo `EventType` que persiste `SchedulingService`.

**Aprendizaje:** implementar el evento y persistirlo en Outbox son
solamente una parte del flujo. El publisher también necesita conocer el
contrato concreto que debe deserializar y publicar.

### 13.3 Discrepancia `DivisionId` / `DivisionHeadId`

**Síntoma:** al preparar los datos apareció una diferencia semántica
entre el identificador almacenado en `AcademicStaffService` y el
esperado por el flujo de propuestas.

**Hallazgo:** `SIA_AcademicStaffDb.dbo.DivisionHeads` utiliza la columna
`DivisionId`, mientras que la propuesta, los contratos y el Integration
Event utilizan `DivisionHeadId`.

**Tratamiento en PLAN-02:** se utilizó:

``` 
99999999-9999-9999-9999-999999999999
```

en ambos puntos para completar el escenario.

**Pendiente arquitectónico:** revisar posteriormente si se trata
únicamente de una inconsistencia de naming o si representa una
diferencia conceptual del modelo. No modificar automáticamente el
esquema o los contratos como parte de PLAN-02.

------------------------------------------------------------------------

## 14. Resultado esperado

PLAN-02 se considera exitoso cuando:

``` 
PublicGateway
    -> AdminBff
        -> AcademicService / AcademicStaffService
        -> SchedulingService
            -> Proposal + AcademicLoad
            -> SubmitForReview
            -> Outbox
                -> RabbitMQ
                    -> WorkflowService
                        -> InboxMessage procesado
                        -> ReviewProcess InReview
```

y además:

-   el `TenantId` se conserva;
-   el `CorrelationId` permite rastrear la ejecución;
-   no existe acceso directo entre bases de datos de microservicios;
-   el Integration Event queda procesado sin error;
-   `WorkflowService` crea el proceso una sola vez para el `EventId`;
-   las pruebas automatizadas existentes de la solución continúan
    pasando.

------------------------------------------------------------------------

## 15. Criterios de aceptación de PLAN-02

  -------------------------------------------------------------------------------------------------
  Criterio                            Validación
  ----------------------------------- -------------------------------------------------------------
  Puede obtenerse el cono         Validado mediante
  académico necesario                 `GET /api/academic-planning/con/{educationalProgramId}`

  Puede seleccionarse un docente      Validado con
  elegible                            `TeacherId = 77777777-7777-7777-7777-777777777777`

  Puede construirse una propuesta     Validado

  Puede agregarse una carga a la      Validado
  propuesta                           

  Puede enviarse la propuesta a       Validado
  revisión                            

  El evento sale mediante Outbox      Validado

  WorkflowService recibe el evento    Validado mediante `InboxMessages`

  WorkflowService registra el proceso Validado mediante `ReviewProcesses`
  como En revisión                    

  TenantId se conserva correctamente  Validado

  CorrelationId permite rastrear el   Validado
  recorrido                           

  No existe acceso directo entre      Respetado por el flujo
  bases de servicios                  

  La prueba puede repetirse           Scripts con IDs conocidos e inserciones idempotentes

  Se documenta escenario, datos y     Este documento
  resultado esperado                  

  La solución continúa pasando        Ejecutar suite antes de cerrar/mergear la tarea
  pruebas automatizadas               
  -------------------------------------------------------------------------------------------------

------------------------------------------------------------------------

## 16. Cómo repetir la prueba

1.  Levantar SQL Server y RabbitMQ.
2.  Verificar que las migraciones de los servicios, especialmente
    `SchedulingService`, estén aplicadas.
3.  Ejecutar los scripts de datos de PLAN-02 para `AcademicService` y
    `AcademicStaffService`.
4.  Levantar `AcademicService`.
5.  Levantar `AcademicStaffService`.
6.  Levantar `SchedulingService`.
7.  Levantar `WorkflowService`.
8.  Levantar `AdminBff`.
9.  Levantar `PublicGateway`.
10. Obtener un token válido con
    `tenant_id = 11111111-1111-1111-1111-111111111111`.
11. Generar un nuevo `X-Correlation-Id`.
12. Consultar el cono académico.
13. Confirmar que el docente de prueba aparece como candidato.
14. Crear la propuesta.
15. Guardar el `ProposalId` devuelto.
16. Crear al menos una carga académica para esa propuesta.
17. Guardar el `AcademicLoadId`.
18. Enviar la propuesta a revisión.
19. Verificar `OutboxMessages` en `SchedulingService`.
20. Esperar el ciclo del publisher de Outbox.
21. Verificar `InboxMessages` en `WorkflowService`.
22. Verificar `ReviewProcesses`.
23. Confirmar `TenantId`, `CorrelationId`, `DivisionHeadId`, `Version` y
    estado `InReview`.
24. Ejecutar las pruebas automatizadas existentes de la solución.

### Nota sobre repetibilidad

La creación de propuestas puede tener reglas de unicidad para una
combinación de tenant/programa/periodo. Si una ejecución previa dejó una
propuesta activa para los mismos datos, no debe eliminarse
arbitrariamente información de negocio para "hacer pasar" la prueba.

Para una nueva ejecución debe prepararse un escenario coherente con las
reglas vigentes del dominio y con la estrategia de datos de prueba
definida por el equipo.

------------------------------------------------------------------------

## 17. Diagnóstico rápido

Si falla la prueba, revisar en este orden:

``` 
1. ¿Los servicios requeridos están levantados?
2. ¿El token contiene el tenant_id correcto?
3. ¿Las bases tienen el esquema/migraciones actuales?
4. ¿Existen los datos de AcademicService?
5. ¿Existen Teacher y DivisionHead de prueba?
6. ¿El cono académico indica IsWithinPlanningWindow = true?
7. ¿La propuesta fue creada?
8. ¿Existe al menos una AcademicLoad activa para la propuesta?
9. ¿SubmitForReview creó el OutboxMessage?
10. ¿EventType coincide con el registro de OutboxEventRegistry?
11. ¿ProcessedAtUtc del Outbox tiene valor?
12. ¿Outbox.Error contiene algún error?
13. ¿RabbitMQ está disponible?
14. ¿WorkflowService está levantado y su consumer está configurado?
15. ¿Existe InboxMessage para el EventId/CorrelationId?
16. ¿Existe ReviewProcess para el ProposalId?
```

Esta secuencia ayuda a localizar el punto exacto en que se interrumpió
el recorrido sin romper los límites entre microservicios.

------------------------------------------------------------------------

## 18. Evidencia de la ejecución exitosa

La ejecución realizada el 13 de septiembre de 2026 confirmó el recorrido
completo del primer tramo de PR-001.

Identificadores principales:

``` 
TenantId       = 11111111-1111-1111-1111-111111111111
ProgramId      = 22222222-2222-2222-2222-222222222222
AcademicPeriod = 33333333-3333-3333-3333-333333333333
TeacherId      = 77777777-7777-7777-7777-777777777777
DivisionHeadId = 99999999-9999-9999-9999-999999999999
ProposalId     = 69041120-9D01-4181-8C9B-2A1624DF6075
AcademicLoadId = 0C4AB27A-1DE7-4D1D-8D6E-BB43CD190B01
CorrelationId  = BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB
EventId        = 87708C00-3983-47CA-9A03-F83A667253FC
```

Resultado final:

``` 
SchedulingService
    -> Outbox procesado
        -> ProposalSubmittedForReviewIntegrationEvent.v1
            -> WorkflowService
                -> InboxMessage procesado sin error
                    -> ReviewProcess creado
                        -> Status = InReview
```

Con esta evidencia queda validado el primer tramo de integración
definido por PLAN-02.
