ADR-01- Decisiones arquitectónicas de WorkFlowService

Estado: Aceptado
Fecha: 19 de octubre de 2026
Versión: 1.0
Ámbito: SIA.WorkflowService y SIA.SchedulingService

1. Contexto
SIA requiere de una arquitectura y de servicios para el proceso de gestionar la revision de propuestas
de cargas academicas antes de que sean aprobadas.

El proceso comprende el envio deuna propuesta a revision, esa propuesta tiene que tener obligatoriamente por lo menos una carga academica asiganada, la evaluacionm de la propuesta por parte del responsable correspondiente y la comunicacion de la decision al servicio que inicia y administra la propuesta.

Para obtener y mantener una separarcion clara entre servicios cada uno tiene responsabilidades, la arquitectura establece que SIA.SchedulingService administra las propuestas y cargas academicas de cada docente, la cual integra su oferta academica y sus horas de clases frente al grupo, mientras que SIA.WorkflowServices gestiona y valida su proceso de revisión de cada propuesta.

Ambos servicios se comunican mediante eventos de integracion, conservando sus estados y la version de las propuestas correspondientes.


2. Decisiones arquitectónicas
2.1. WorkflowService administra el proceso de revisión
SIA.WorkflowService es responsable de gestionar el proceso de revision de cada propuesta  de cargas academicas enviadas, representado por la entidad ReviewProcess.

Esta responsabilidad incluye la creación del proceso de revisión, el registro de las decisiones que podemos tomar que es aprobación o devolución para corrección, si es devuelto para una corrección obligatoriamente se tiene que poner las observaciones asociadas a las devoluciones.

Un proceso de revision inicia en estado InReview, una vez que el responsable haya decidido, no admite una nueva decisión.

SIA.Scheduling conserva la responsabilidad sobre la propuesta academica y la administración de su estado, La decisión tomada en WorkflowService se comuinica mediante eventos de integración para que SchedulingService aplique el cambio de estado y la accion correspondiente.


2.2. SchedulingService conserva la propiedad de la propuesta
SchedulingService es respondable de crear y administrar las propuestas de cargas academicas. Cada propuesta contiene los datos necesarios para indentidicar el instituto educativo, el programa educativo, el periodo academico y el responsable de divison.

WorkflowService no almacena como tal una copia de la propuesta. la entidad ReviewProcess conserva unicamente el identificador de la propuesta(AcademicLoadProposalId) y los demas datos que son necesarios para administrar la propuesta: TenantId, DivisionHeadId, Version y CorrelationId. El jefe de division se conserva ya que va ligado a la propuesta quien es el que hace todo este proceso para que en este servicio el coordinador sea el responsable de aceptarla o mandarla a revision. Esta separacion mantiene las responsabilidades de ambos servicios dentro de su respectivos dominios.


2.3. La comunicación entre servicios utiliza eventos de integración
La comunicacion entre servicios SchedulingService y WorkflowSerivice se realizan mediante eventos de integracion.

Cuando una propuesta se envia a revisión, SchedulingService publica ProposalSubmittedForReviewIntegrationEvent mediante outbox, dentro de la misma operacion que persiste el cambio de estado. Workflow consume el evento y crea el ReviewProcess, todo este proceso ignorando los eventos cuyos EventId ya han sido procesados.

Cuando WorkflowService registra una decision, publica ApprovedEvent o CorrectionRequiredEvent ambos con la version del proceso en la que de encuentran. SchedulingService consume el evento, ignora las decisiones cuyo EventId ya ha sido procesado y aplica el estado correspondiente.

Cuando la decisión es una aprobación, SchedulingService verifica que la versión corresponda a la vigente, que la propuesta tenga cargas académicas asociadas y que se encuentre en estado SubmittedForReview. Una vez terminado el proceso formaliza la aprobación y publica AcademicLoadApprovedIntegrationEvent para comunicar el resultado con los demas servicios.

ApprovedEvent representa la decisión registrada por WorkflowService. AcademicLoadApprovedIntegrationEvent representa la aprobación formal de la propuesta por SchedulingService. Son eventos distintos, publicados por servicios distintos.

Este mecanismo permite que cada servicio gestione sus propias responsabilidades sin modificar directamente las entidades internas del otro servicio.


2.4. Las observaciones pertenecen al proceso de revisión
La obervaciones relacionadas con una devolución para corrección son administradas por WorkflowService y quedan asociadas al ReviewProcess correspondiente a la entidad ReviewObservation.

Cada observación contiene un tipo(ObservationTarget) y el identificador de la propuesta observado, la descripción de la corección requerida tiene que ir de acorde con el tipo de correción, tiene que tener coherencia. los datos necesarios para relacionarla con el proceso y la decisión: ReviewProcessId, TenantId, CreatedBy y CorrelationId.

Para devolver una propuesta se requiere al menos una observación. Cuando se devuelve una propuesta, WorkflowService registra las observaciones y publica CorrectionRequiredEvent. SchedulingService actualiza el estado de la propuesta, mientras que las observaciones permanecen bajo la responsabilidad de WorkflowService.


2.5. El regreso para corrección es RequiresCorrection, no Rejected
El flujo solo contemple 2 decisiones: aprobar una propuesta o devolverla para corrección.

Un regreso para corrección se representa como RequiresCorrection, tanto como WorkflowService(ReviewStatus.RequiresCorrection), en SchedulingService(ProposalStatus.RequiresCorrection).  No se utiliza Rejected, los estados(enum) de ReviewStatus definen unicamente InReview, Approved y RequiresCorrection.

En SchedulingService Una propuesta en estado RequiresCorrection puede editarse su carga academica y enviarse nuevamente a revisión.


2.6. ReviewVersion identifica la versión del envío a revisión
SchedulingService administra la versión de revisión mediante la propiedad ReviewVersion de la entidad Proposal.

Una propuesta nueva comienza con ReviewVersion = 0. Cada vez que se envía correctamente a revisión, la versión aumenta en uno y el estado de la propuesta cambia a SubmittedForReview. Solo pueden enviarse a revisión las propuestas en borrador o en RequiresCorrection.

El evento ProposalSubmittedForReviewIntegrationEvent transmite el valor vigente de ReviewVersion en su propiedad Version. WorkflowService guarda ese valor en ReviewProcess.Version (siempre mayor que cero) y lo devuelve en la propiedad Version de ApprovedEvent y CorrectionRequiredEvent.

Al recibir una decisión, SchedulingService compara la versión del evento con la versión vigente de la propuesta:
- Si la versión recibida es anterior a la vigente, la decisión se descarta y se registra una advertencia.
- Si la versión recibida es superior a la vigente, se genera ProposalReviewVersionAheadException.
- Si coincide, la decisión se aplica.

De esta manera, cada reenvío después de una devolución para corrección representa una nueva versión de revisión, y las decisiones se aplican a la versión correspondiente.


2.7. TenantId y CorrelationId mantienen el aislamiento y la trazabilidad
Los servicios conservan TenantId y CorrelationId durante las operaciones y los intercambios de eventos.

TenantId identifica al instituto educativo al que pertenece la propuesta y permite recuperar y procesar los datos dentro del contexto del instituto correspondiente.

CorrelationId permite relacionar las operaciones y los mensajes asociados a un mismo flujo de trabajo, facilitando su seguimiento y diagnóstico. Está presente en los eventos de integración, en ReviewProcess y en ReviewObservation. Además, ReviewProcess conserva el CorrelationId de la decisión (DecisionCorrelationId) por separado del de su creación.

Ambos identificadores son obligatorios en las entidades y eventos de WorkflowService y forman parte de la información utilizada para mantener el aislamiento entre institutos y la trazabilidad de las interacciones entre servicios.


3. Consecuencias
3.1. Separación de responsabilidades
La arquitectura mantiene una división clara entre la administración de las propuestas académicas y la gestión de su revisión.

SchedulingService conserva la propiedad de la propuesta y administra sus cambio de estado, mientras que WorkflowService gestiona el proceso de revisión y las observaciones asociadas a las decisiones.


3.2. Comunicación desacoplada
Los servicios intercambian información mediante eventos de integración y contratos definidos para ese propósito. Esto permite comunicar las decisiones sin que WorkflowService tenga que administrar directamente los datos internos de SchedulingService.


3.3. Trazabilidad de las revisiones
El uso de ReviewVersion permite distinguir los distintos envíos de una propuesta y relacionar las decisiones con la versión correspondiente. La propagación de TenantId y CorrelationId permite mantener el contexto del instituto educativo y seguir las operaciones entre servicios.


3.4. Compromisos de la comunicación por eventos
- Consistencia eventual: el estado de la propuesta en SchedulingService se actualiza cuando consume la decisión de WorkflowService, no en el mismo instante en que esta se registra.
- Eventos duplicados: el procesamiento puede recibir un mismo evento más de una vez. Ambos servicios lo analizan ignorando los eventos cuyo EventId ya fue procesado.
- Eventos de versiones distintas: las decisiones de versiones anteriores se descartan y las de versiones superiores generan una excepción, conforme a lo descrito en la sección 2.6.
- Datos de la propuesta en Workflow: al no copiar la propuesta, WorkflowService no dispone de sus datos completos y depende de los identificadores y metadatos recibidos en los eventos.


4. Estado de la decisión
Este ADR registra la separación de responsabilidades entre SchedulingService y WorkflowService, la comunicación mediante eventos de integración, el tratamiento de las devoluciones para corrección y el uso de versionado, aislamiento y trazabilidad.

El documento describe la arquitectura existente y no introduce nuevas reglas funcionales.