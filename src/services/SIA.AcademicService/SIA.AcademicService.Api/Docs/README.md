ACAD-15 — Definir modelo funcional de prerrequisitos entre materias
Objetivo
Definir cómo AcademicService representará los prerrequisitos académicos entre materias antes de implementar persistencia y endpoints.
El modelo deberá permitir representar que una materia dentro de un StudyPlan depende académicamente de otra materia del mismo plan, sin incorporar estudiantes, inscripciones ni validaciones de avance escolar.
________________________________________
Contexto
AcademicService administra la estructura académica de la institución, incluyendo:
•	AcademicPeriod 
•	EducationalProgram 
•	StudyPlan 
•	StudyPlanSubject 
•	Subject 
•	ServiceComplementary 
Subject representa el catálogo de materias y debe poder reutilizarse en diferentes planes de estudio sin duplicar el registro de la materia.
StudyPlanSubject representa la participación de una materia dentro de un determinado StudyPlan.
________________________________________
Definición de prerrequisito
Un prerrequisito representa una dependencia académica entre dos materias.
Ejemplo:
Cálculo Diferencial
        ↓
Cálculo Integral
Esto significa que:
Cálculo Integral depende académicamente de Cálculo Diferencial.
El prerrequisito representa únicamente la estructura académica definida en el plan de estudios.
No representa que un estudiante haya cursado o aprobado la materia.
________________________________________
Ubicación del prerrequisito
El prerrequisito deberá pertenecer al contexto de StudyPlanSubject.
No se deberá agregar el prerrequisito directamente a Subject, debido a que una misma materia puede participar en diferentes planes de estudio y su relación académica puede depender de dicho contexto.
La estructura conceptual es:
Subject
   │
   │
   ▼
StudyPlanSubject
   │
   ├── StudyPlan
   ├── Semester
   ├── IsRequired
   └── Prerequisite
De esta manera, una misma materia puede participar en diferentes planes sin duplicar el registro de Subject.
________________________________________
Relación de dependencia
StudyPlanSubject deberá permitir identificar la materia de la cual depende académicamente.
Conceptualmente:
Materia dependiente
        │
        ▼
Materia requerida
Ejemplo:
Cálculo Integral
        │
        └── depende de ──→ Cálculo Diferencial
La referencia deberá realizarse utilizando el identificador de la participación correspondiente dentro del StudyPlan.
Esto permite que la relación permanezca dentro del contexto del plan y evita depender de una relación global entre Subject.
________________________________________
Uso de IsRequired
IsRequired deberá representar si la materia dentro del StudyPlan requiere una materia antecedente.
Conceptualmente:
IsRequired = false
significa que la materia no tiene prerrequisito.
IsRequired = true
significa que la materia tiene una dependencia académica definida.
________________________________________
Semestre y prerrequisito
El semestre y el prerrequisito son conceptos independientes.
El hecho de que una materia normalmente se encuentre en un semestre determinado no significa que automáticamente sea prerrequisito de una materia de un semestre posterior.
Por ejemplo:
Semestre 1
Cálculo Diferencial

Semestre 2
Cálculo Integral
puede existir la relación:
Cálculo Diferencial
        ↓
Cálculo Integral
pero el sistema no deberá inferir automáticamente la relación solamente a partir del número de semestre.
Esto es importante debido a que la estructura de un plan de estudios puede permitir movimientos de materias entre semestres siempre que se respeten las relaciones académicas correspondientes.
________________________________________
Materias compartidas entre planes
Subject deberá mantenerse como una entidad reutilizable.
Ejemplo:
Subject
Cálculo Diferencial
puede participar en:
StudyPlanSubject
        │
        ├── Plan ISIC-2010-224
        │
        └── Otro StudyPlan
La relación de prerrequisito deberá establecerse sobre la participación de la materia dentro del plan (StudyPlanSubject) y no sobre Subject.
Esto evita duplicar materias únicamente porque participan en diferentes planes de estudio.
________________________________________
Mismo plan de estudios
Una materia solamente podrá depender de otra materia que participe en el mismo StudyPlan.
Ejemplo válido:
ISIC-2010-224

Cálculo Diferencial
        ↓
Cálculo Integral
No deberá ser posible representar:
Plan A
Cálculo Diferencial
        ↓
Plan B
Cálculo Integral
como un prerrequisito.
El prerrequisito es una regla interna de la estructura académica de un plan.
________________________________________
Relaciones duplicadas
No deberá ser posible registrar dos veces la misma relación de dependencia dentro de un mismo StudyPlan.
Ejemplo:
Cálculo Diferencial
        ↓
Cálculo Integral
no deberá registrarse nuevamente para el mismo contexto.
La combinación que representa la relación deberá mantenerse única considerando el contexto correspondiente, incluyendo TenantId.
________________________________________
TenantId
El modelo deberá respetar el aislamiento por TenantId.
Una materia dentro de un StudyPlan solamente podrá establecer una dependencia con otra materia perteneciente al mismo contexto de tenant.
No deberán existir relaciones de prerrequisito entre materias pertenecientes a diferentes tenants.
________________________________________
Materias de especialidad
Las materias de especialidad forman parte de determinados planes de estudio y pueden cambiar entre diferentes versiones o configuraciones académicas.
El modelo de prerrequisitos no deberá asumir que las materias de especialidad son permanentes ni deberá incorporar reglas específicas para su administración.
Cuando una materia de especialidad participe en un StudyPlan, podrá establecer sus relaciones académicas bajo las mismas reglas definidas para las demás materias.
________________________________________
ServiceComplementary
ServiceComplementary forma parte de la estructura de un StudyPlan, pero no representa una materia.
Puede representar conceptos académicos como:
•	Actividades complementarias. 
•	Servicio social. 
Estas actividades cuentan con reglas de créditos propias y no deberán modelarse como Subject únicamente para reutilizar la estructura de materias.
Por lo tanto, ServiceComplementary queda fuera de la relación de prerrequisitos definida en esta tarea.
El estudiante podrá posteriormente requerir cumplir estas condiciones dentro de su trayectoria académica, pero dicha validación pertenece a otros procesos y no forma parte de ACAD-15.
________________________________________
Fuera de alcance
Esta tarea NO contempla:
•	Estudiantes. 
•	Inscripción. 
•	Reinscripción. 
•	Historial académico. 
•	Calificaciones. 
•	Aprobación de materias por estudiantes. 
•	Validación de avance reticular. 
•	Validación de cumplimiento de prerrequisitos por estudiantes. 
•	Oferta académica. 
•	Grupos. 
•	Horarios. 
•	Carga académica. 
•	Servicio social como proceso operativo. 
•	Actividades complementarias como proceso operativo. 
La responsabilidad de esta tarea se limita a definir cómo representar la estructura académica de dependencia entre materias.
________________________________________
Modelo funcional propuesto
Conceptualmente:
StudyPlan
    │
    ├── StudyPlanSubject
    │       │
    │       ├── Subject
    │       ├── Semester
    │       ├── IsRequired
    │       └── PrerequisiteStudyPlanSubjectId
    │
    ├── StudyPlanSubject
    │       │
    │       └── Subject
    │
    └── ServiceComplementary
Ejemplo:
StudyPlan: ISIC-2010-224
        │
        ├── StudyPlanSubject
        │       Subject: Cálculo Diferencial
        │       Semester: 1
        │       IsRequired: false
        │
        └── StudyPlanSubject
                Subject: Cálculo Integral
                Semester: 2
                IsRequired: true
________________________________________
Reglas funcionales
1.	Un prerrequisito representa una dependencia académica entre materias. 
2.	La dependencia pertenece al contexto de StudyPlanSubject. 
3.	Subject debe permanecer reutilizable entre diferentes planes de estudio. 
4.	El prerrequisito no deberá definirse como una propiedad global de Subject. 
5.	IsRequired indica que la materia depende de otra materia. 
6.	Cuando IsRequired = true, deberá existir una materia requerida asociada. 
7.	Cuando IsRequired = false, la materia no deberá tener una dependencia de prerrequisito. 
8.	La materia requerida y la materia dependiente deberán pertenecer al mismo StudyPlan. 
9.	Una materia no podrá depender de sí misma. 
10.	No deberán existir relaciones duplicadas.  
11.	El modelo deberá respetar TenantId. 
12.	El semestre no deberá utilizarse para inferir automáticamente prerrequisitos. 
13.	Una materia podrá tener múltiples prerrequisitos, sujeto a las reglas académicas que posteriormente se definan. 
14.	No se incorporarán estudiantes, inscripciones ni historial académico. 
15.	AcademicService representa la estructura académica del prerrequisito, pero no determina si un estudiante cumple dicha condición.


