# Etiquetas de competencias asociadas a docentes

## 1. Objetivo

Definir funcionalmente la gestión de las competencias o capacidades asociadas a un docente dentro de SIA.

El objetivo de este documento es establecer:

* qué representa una competencia asociada a un docente;
* cómo se relaciona un docente con una etiqueta de competencia;
* quién puede registrar una competencia;
* quién puede solicitar cambios de nivel;
* quién puede validar una competencia;
* los niveles de competencia disponibles;
* el máximo de competencias que puede tener un docente;
* el seguimiento mediante `Stage`;
* la información de trazabilidad de la validación;
* las reglas para retirar competencias;
* las validaciones funcionales;
* el ownership de los conceptos involucrados.

Este documento **no define tablas, llaves foráneas, endpoints, DTOs, comandos, eventos ni código**.

La implementación técnica deberá derivarse posteriormente de estas definiciones y respetar la arquitectura de microservicios de SIA.

---

# 2. Alcance

Este documento comprende exclusivamente la relación entre:

```text
Teacher
   │
   └── CompetencyTag
```

La relación se representa funcionalmente mediante el concepto:

```text
TeacherCompetencies
```

El documento contempla:

* asociación de competencias a docentes;
* nivel de dominio de la competencia;
* registro inicial de competencias;
* solicitud de cambio de nivel;
* validación de competencias;
* seguimiento mediante `Stage`;
* información de trazabilidad;
* retiro de competencias;
* límite máximo de competencias activas;
* referencias entre `AcademicStaffService` y `AcademicService`.

Quedan fuera de este documento:

* administración general del catálogo `CompetencyTag`;
* etiquetas asociadas a materias;
* asociación entre etiquetas de docentes y etiquetas de materias;
* algoritmo de compatibilidad docente–materia;
* asignación de docentes a materias;
* asignación de tutores a grupos aperturados.

Estos conceptos deberán documentarse de forma independiente.

---

# 3. Conceptos funcionales

## 3.1 CompetencyTag

`CompetencyTag` representa una capacidad, conocimiento, especialidad o característica académica/profesional que puede utilizarse para describir las capacidades de un docente.

Una etiqueta no representa por sí misma:

* un grado académico;
* una certificación;
* un puesto;
* un tipo de contrato;
* un perfil profesional formal.

Su propósito es complementar la información profesional del docente con capacidades específicas que pueden resultar relevantes durante los procesos académicos.

Ejemplos:

* Desarrollo Web
* Bases de Datos
* Inteligencia Artificial
* Redes
* Ciberseguridad
* Administración de Proyectos
* Programación Orientada a Objetos

El catálogo de estas etiquetas pertenece a `AcademicService`.

---

# 3.2 Diferencia entre perfil profesional y competencia

El perfil profesional del docente representa información formal relacionada con su formación y trayectoria.

Una competencia permite representar capacidades específicas que complementan dicha información.

Por ejemplo:

```text
Docente
│
├── Grado académico
├── Perfil profesional
├── Tipo de contrato
├── Horas de contrato
└── Competencias
      ├── Desarrollo Web
      ├── Bases de Datos
      └── Arquitectura de Software
```

Las competencias **no sustituyen** el perfil profesional del docente.

Su propósito es proporcionar información adicional que permita describir capacidades específicas.

---

# 4. Asociación docente–competencia

## 4.1 TeacherCompetencies

`TeacherCompetencies` representa funcionalmente las competencias registradas para un docente.

La relación es de muchos a muchos:

```text
Teacher
   │
   ├── Competencia A
   ├── Competencia B
   └── Competencia C

CompetencyTag
   │
   ├── Docente A
   ├── Docente B
   └── Docente C
```

Por lo tanto:

* un docente puede tener múltiples competencias;
* una misma competencia puede estar asociada a múltiples docentes.

La competencia pertenece al catálogo administrado por `AcademicService`, mientras que la asociación de esa competencia con un docente pertenece funcionalmente a `AcademicStaffService`.

---

# 5. Máximo de competencias por docente

Un docente puede tener como máximo **12 competencias activas o vigentes**.

La cantidad máxima se aplica únicamente a las competencias que actualmente forman parte del conjunto vigente del docente.

```text
Docente
│
├── Competencia 1
├── Competencia 2
├── ...
└── Competencia 12
```

Cuando el docente ya cuenta con 12 competencias activas, no puede agregarse una nueva mientras no retire alguna de las competencias existentes.

Las competencias retiradas no deben contabilizarse para este límite.

### Ejemplo

```text
Docente A

10 competencias activas
2 competencias retiradas
```

El docente puede tener hasta **2 competencias activas adicionales**, porque actualmente tiene 10 competencias vigentes.

El límite se expresa funcionalmente como:

> `Competencias activas/vigentes ≤ 12`

---

# 6. Nivel de competencia

El nivel de dominio pertenece a la relación entre el docente y la competencia.

No pertenece al catálogo global de `CompetencyTag`.

Los niveles definidos son:

| Nivel      | Descripción                                                                                                          |
| ---------- | -------------------------------------------------------------------------------------------------------------------- |
| Básico     | El docente posee conocimientos o capacidades iniciales relacionados con la competencia.                              |
| Intermedio | El docente posee un dominio funcional de la competencia y puede aplicarla en actividades académicas o profesionales. |
| Avanzado   | El docente posee un dominio elevado y experiencia suficiente para desempeñarse ampliamente en la competencia.        |

### Ejemplo

Una misma etiqueta puede tener diferentes niveles dependiendo del docente:

```text
CompetencyTag
    Desarrollo Web

Docente A → Básico
Docente B → Intermedio
Docente C → Avanzado
```

Por lo tanto, **el nivel no forma parte de la definición global de la etiqueta**.

---

# 7. Registro inicial de una competencia

La gestión de competencias distingue dos operaciones diferentes:

1. **Registro de una competencia en el catálogo:** corresponde al Jefe de División, quien puede registrar nuevas competencias en el catálogo de `CompetencyTag`, cuya propiedad pertenece a `AcademicService`.
2. **Asociación de una competencia a un docente:** corresponde exclusivamente al docente, quien selecciona y solicita asociar una competencia existente a su perfil. El Jefe de División es responsable de validar dicha asociación.

Por lo tanto, el Jefe de División puede registrar competencias en el catálogo, pero no puede asociarlas directamente a un docente. La asociación debe ser iniciada por el propio docente.

## 7.1 Registro de una competencia en el catálogo

El Jefe de División puede registrar una nueva competencia en el catálogo general de `CompetencyTag`.

Esta operación crea una etiqueta disponible para su posterior asociación con docentes, siempre que se encuentre activa.

El registro de una etiqueta en el catálogo no significa que esta quede asociada automáticamente a ningún docente.

## 7.2 Asociación de una competencia por el docente

El docente puede seleccionar una competencia existente en el catálogo y solicitar asociarla a su perfil, indicando el nivel de dominio correspondiente.

Los niveles permitidos son:

* Básico
* Intermedio
* Avanzado

La solicitud debe respetar el límite máximo de 12 competencias activas o vigentes por docente.

Una vez enviada la solicitud, la asociación queda pendiente de validación por parte del Jefe de División.

## 7.3 Validación de la asociación

Toda asociación de una competencia a un docente requiere validación del Jefe de División.

El flujo funcional es:

```text
Jefe de División
       │
       ▼
Registra competencia en el catálogo
       │
       ▼
Competencia disponible para su selección
       │
       ▼
Docente selecciona competencia
       │
       ▼
Docente indica nivel de dominio
       │
       ▼
Solicita asociar competencia a su perfil
       │
       ▼
Pendiente de validación
       │
       ▼
Jefe de División valida la asociación
       │
       ▼
Asociación validada
```

La validación debe conservar la información de trazabilidad correspondiente:

* `ValidatedBy`: usuario que realizó la validación.
* `ValidatedAt`: fecha y hora UTC de la validación.

La asociación de una competencia no debe considerarse validada hasta que el Jefe de División complete dicha acción.


---

# 8. Validación de una competencia

Toda competencia asociada a un docente requiere validación.

El responsable de realizar la validación es el **Jefe de División**.

El flujo conceptual es:

```text
Competencia registrada
        │
        ▼
Pendiente de validación
        │
        ▼
Jefe de División valida
        │
        ▼
Competencia validada
```

La validación debe conservar información de trazabilidad:

* quién realizó la validación;
* cuándo se realizó.

---

# 9. Stage

`Stage` se utiliza exclusivamente como mecanismo funcional de seguimiento.

Su objetivo es permitir identificar en qué punto del proceso se encuentra la competencia, particularmente para determinar si una competencia:

* está pendiente de validación;
* ya fue validada;
* tiene una solicitud de cambio de nivel pendiente.

`Stage` no sustituye a `Status`.

---

## 9.1 Etapas permitidas

Las etapas funcionales definidas son:

### `PendingValidation`

Indica que la competencia ha sido registrada o modificada y requiere validación por parte del Jefe de División.

### `Validated`

Indica que la competencia se encuentra validada por el Jefe de División.

### `LevelChangeRequested`

Indica que el docente solicitó un cambio de nivel para una competencia existente y la solicitud está pendiente de revisión/validación.

### `LevelChangeValidated`

Indica que el cambio de nivel solicitado por el docente fue validado por el Jefe de División.

Estas etapas permiten dar seguimiento al ciclo funcional sin convertir `Stage` en un mecanismo de auditoría histórica.

---

## 9.2 Flujo inicial

El flujo de una competencia registrada inicialmente es:

```text
PendingValidation
        │
        │ Jefe de División valida
        ▼
Validated
```

---

## 9.3 Flujo de cambio de nivel

Cuando un docente solicita cambiar el nivel de una competencia:

```text
Validated
     │
     │ Docente solicita cambio
     ▼
LevelChangeRequested
     │
     │ Jefe de División valida
     ▼
LevelChangeValidated
```

Después de que el cambio es validado, la competencia queda vigente con el nuevo nivel.

El registro anterior **no se conserva como una competencia histórica separada**.

Ejemplo:

```text
Antes:

Desarrollo Web → Básico
Stage → Validated

Solicitud:

Docente solicita:
Básico → Intermedio

Durante la solicitud:

Desarrollo Web → Básico
Stage → LevelChangeRequested

Después de validar:

Desarrollo Web → Intermedio
Stage → LevelChangeValidated
```

La información anterior del nivel no se conserva como un registro histórico independiente.

---

# 10. Cambio de nivel de una competencia

El docente puede solicitar modificar el nivel de una competencia que ya posee.

Por ejemplo:

```text
Docente A
Desarrollo Web → Básico
```

El docente puede solicitar:

```text
Desarrollo Web
Básico → Intermedio
```

El flujo es:

```text
Docente
   │
   │ Solicita cambio
   ▼
LevelChangeRequested
   │
   │
   ▼
Jefe de División
   │
   │ Valida
   ▼
LevelChangeValidated
   │
   ▼
Nuevo nivel vigente
```

### Regla importante

Un cambio de nivel requiere **nueva validación** del Jefe de División.

El docente puede solicitar el cambio, pero no puede aprobarlo por sí mismo.

---

# 11. Modificación de una competencia

Debe distinguirse entre:

### Cambiar el nivel

Ejemplo:

```text
Desarrollo Web
Básico → Intermedio
```

Esta operación:

* es solicitada por el docente;
* requiere validación del Jefe de División;
* genera un seguimiento mediante `Stage`;
* reemplaza el nivel anterior cuando es validada;
* no conserva el nivel anterior como una competencia histórica separada.

### Modificar la definición de una competencia

Ejemplo:

```text
Desarrollo Web
        ↓
Desarrollo Web Avanzado
```

La modificación del catálogo de `CompetencyTag` corresponde al ownership de `AcademicService`.

La facultad específica para crear, modificar, activar o desactivar las etiquetas del catálogo deberá definirse dentro de la documentación correspondiente a `CompetencyTag`.

Este documento únicamente establece que una modificación del nivel de una competencia docente **no modifica el catálogo de etiquetas**.

---

# 12. Retiro de una competencia

El docente puede retirar una competencia que actualmente tenga asociada.

Por ejemplo:

```text
Docente A

Desarrollo Web
Bases de Datos
Inteligencia Artificial
```

El docente puede retirar:

```text
Inteligencia Artificial
```

La competencia deja de formar parte de las competencias activas/vigentes del docente.

Las competencias retiradas no cuentan para el límite máximo de 12 competencias activas.

### Regla

El retiro realizado por el docente no debe confundirse con la desactivación de la etiqueta en el catálogo general.

Existen dos conceptos diferentes:

```text
Retiro de competencia del docente
        ≠
Desactivación de CompetencyTag
```

El primero afecta la asociación de un docente.

El segundo afecta la disponibilidad general de la etiqueta en el catálogo.

---

# 13. Validación y trazabilidad

Cuando una competencia es validada, debe conservarse información que permita responder:

> ¿Quién validó esta competencia y cuándo?

Conceptualmente:

```text
TeacherCompetency
       │
       └── Validación
             ├── ValidatedBy
             └── ValidatedAt
```

---

## 13.1 ValidatedBy

`ValidatedBy` representa al usuario
