# SIA.AcademicService

## Responsabilidad

Gestionar la información académica del sistema, actuando como fuente de verdad para las entidades y procesos que
pertenecen al dominio académico. 
Este servicio es responsable de la persistencia, validación y exposición de la información académica bajo su contexto de negocio.

## Lo que sí hace

- Administra las entidades académicas definidas dentro de su dominio.
- Expone endpoints para la gestión de información académica.
- Valida reglas de negocio relacionadas con el dominio académico.
- Publica eventos de integración cuando ocurren cambios relevantes en sus datos.
- Mantiene la consistencia entre la base de datos y los eventos publicados mediante el patrón Outbox.
- Permite que otros servicios consuman información académica a través de contratos definidos.

## Lo que no hace

- No administra autenticación ni autorización de usuarios.
- No accede directamente a bases de datos de otros servicios.
- No contiene lógica de horarios, notificaciones, documentos o flujos de trabajo externos a su dominio.
- No comparte entidades de dominio con otros microservicios.
- No realiza integraciones directas con servicios externos fuera de las definidas mediante contratos o eventos.

## Base de datos

**SIA_AcademicService**

Esta base de datos es propiedad exclusiva de AcademicService y constituye la fuente oficial de información 
para los datos académicos administrados por este servicio.

## Eventos que publica

Actualmente:

- `SubjectCreatedIntegrationEvent`

Futuros eventos esperados:

- `SubjectUpdatedIntegrationEvent`
- `SubjectDeletedIntegrationEvent`
- Eventos relacionados con carreras, programas educativos, periodos escolares, planes de estudio, materias, docentes,
- responsables de división, grupos aulas y recursos, parámetros y configuraciones.

## Eventos que consume

Actualmente:

- Ninguno.

Futuros eventos dependerán de las necesidades de integración con otros servicios del ecosistema SIA.

## Reglas críticas

- Ningún servicio accede directamente a la base de datos de este servicio.
- Este servicio no comparte entidades de dominio con otros servicios.
- La comunicación externa se realiza mediante contratos o eventos.
- Todo evento de integración debe publicarse utilizando el patrón Outbox.
- La base de datos de AcademicService es la única fuente de verdad para la información bajo su responsabilidad.
- Los cambios de estado relevantes deben generar eventos de integración para mantener sincronizados los demás servicios.
- Los contratos públicos deben mantenerse compatibles o versionarse cuando existan cambios incompatibles.

## Dominio

Actualmente este servicio administra:

- `Subjects (Materias)`

Próximamente:
- Careers (Carreras)
- Academic Programs (Programas educativos)
- Academic Periods (Periodos escolares) 
- Curricula (Planes de estudio) 
- Teachers (Docentes)
- Division Heads (Responsables de división) 
- Groups (Grupos)
- Classrooms (Aulas)
- Laboratories (Laboratorios)
- AcademicResources (Recursos disponibles)
