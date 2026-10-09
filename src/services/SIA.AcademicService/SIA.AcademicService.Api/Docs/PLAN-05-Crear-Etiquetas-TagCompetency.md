# CompetencyTag — Catálogo general de etiquetas de competencias

## 1. Objetivo

Definir el comportamiento funcional del catálogo general de etiquetas de competencias de SIA, propiedad de `AcademicService`. El catálogo permite mantener conceptos que posteriormente podrán utilizarse para identificar afinidades relacionadas con docentes y materias, y apoyar la asignación de carga académica.

## 2. Alcance funcional

Este documento comprende exclusivamente la administración del catálogo general de etiquetas:

* Registrar una etiqueta.
* Identificar coincidencias y mostrar sugerencias durante el registro o la modificación.
* Modificar el nombre y la descripción de una etiqueta.
* Activar e inactivar una etiqueta.
* Validar permisos, datos obligatorios y posibles duplicidades.

La inactivación es lógica: la etiqueta permanece registrada. Este documento no define cómo se asocian las etiquetas a docentes o materias, ni cómo se asignan tutores a grupos aperturados. Esos procesos se especifican por separado.

## 3. Actor autorizado

**Jefe de División**

Es el único actor autorizado funcionalmente para registrar etiquetas, modificar su nombre o descripción y cambiar su estado.

El sistema debe comprobar que el actor tenga los permisos correspondientes al ejecutar cada operación; no basta con haber validado el acceso al ingresar al sistema.

## 4. Información funcional de la etiqueta

| Campo          | Obligatorio                     | Descripción                                                                                             |
| -------------- | ------------------------------- | ------------------------------------------------------------------------------------------------------- |
| `Name`         | Sí                              | Nombre que identifica el concepto de competencia. No puede estar vacío ni contener únicamente espacios. |
| `Description`  | Sí                              | Descripción del concepto de competencia. No puede estar vacía ni contener únicamente espacios.          |
| `Status`       | Sí, administrado por el sistema | Condición de la etiqueta: activa o inactiva. No se utiliza un campo adicional `IsActive`.               |
| `CreatedAtUtc` | Sí, administrado por el sistema | Fecha y hora UTC en que se creó el registro. Se establece una sola vez.                                 |
| `UpdatedAtUtc` | Sí, administrado por el sistema | Fecha y hora UTC de la última modificación de la etiqueta o de su estado.                               |

La etiqueta conserva su identidad al modificarse o reactivarse. Cambiar su nombre o descripción no constituye el registro de una etiqueta nueva.

## 5. Reglas de negocio

* **RN-01. Autorización para administrar:** solo el Jefe de División puede registrar etiquetas, modificar su nombre o descripción y activar o inactivar etiquetas.
* **RN-02. Verificación por operación:** el sistema verifica los permisos del actor al ejecutar cada operación de administración.
* **RN-03. Nombre obligatorio:** toda etiqueta debe tener un nombre válido. No se acepta un valor vacío ni uno compuesto únicamente por espacios.
* **RN-04. Descripción obligatoria:** toda etiqueta debe tener una descripción. No se acepta un valor vacío ni uno compuesto únicamente por espacios.
* **RN-05. Estado de la etiqueta:** `Status` representa si la etiqueta está activa o inactiva. La inactivación no elimina físicamente el registro.
* **RN-06. Prevención de duplicados:** no se permite registrar una etiqueta duplicada o equivalente a otra existente, independientemente de que la existente esté activa o inactiva.
* **RN-07. Normalización para comparar:** la comparación automática inicial debe ignorar diferencias de mayúsculas y minúsculas y espacios sobrantes al inicio o al final del nombre.
* **RN-08. Coincidencia activa:** si se identifica una etiqueta equivalente activa, el sistema informa que ya existe y bloquea el registro de otra etiqueta equivalente.
* **RN-09. Coincidencia inactiva:** si se identifica una etiqueta equivalente inactiva, el sistema recomienda reutilizarla o reactivarla, en lugar de crear un nuevo registro equivalente.
* **RN-10. Similitud aproximada:** cuando el nombre sea similar a una etiqueta existente, pero no se pueda determinar que sea equivalente, el sistema muestra sugerencias. La similitud por sí sola no bloquea el registro; el Jefe de División decide si reutiliza una etiqueta sugerida o registra una nueva.
* **RN-11. Equivalencia semántica:** el sistema no debe asumir que dos nombres representan el mismo concepto únicamente por una similitud aproximada. Por ejemplo, “Machine Learning” y “Aprendizaje automático” podrían ser equivalentes, pero esa equivalencia no debe decidirse automáticamente solo por la similitud.
* **RN-12. Validación al modificar:** al cambiar el nombre, el sistema ejecuta las mismas validaciones de duplicidad que aplica al registrar, comparando también con etiquetas inactivas. La etiqueta que se está modificando no debe considerarse duplicada de sí misma.
* **RN-13. Reactivación sin duplicidad:** antes de reactivar una etiqueta, el sistema verifica que no exista una etiqueta equivalente que impida la reactivación.
* **RN-14. Duplicado previamente inactivado:** una etiqueta inactivada por ser duplicada de otra no puede reactivarse mientras persista esa duplicidad. Primero debe resolverse la duplicidad.
* **RN-15. Identidad y asociaciones:** reactivar una etiqueta conserva el mismo registro, su identidad y las asociaciones existentes.
* **RN-16. Estado independiente de las asociaciones:** reactivar una etiqueta no activa automáticamente asociaciones que tengan su propia condición de actividad.
* **RN-17. Fecha de creación:** `CreatedAtUtc` se establece al crear la etiqueta y no cambia en modificaciones posteriores.
* **RN-18. Fecha de actualización:** `UpdatedAtUtc` se actualiza cuando cambia el nombre, la descripción o el estado de la etiqueta.
* **RN-19. Sin eliminación física ordinaria:** las operaciones funcionales de este catálogo no eliminan físicamente una etiqueta; para retirarla del uso se utiliza la inactivación.

## 6. Flujo principal para registrar una etiqueta

1. El Jefe de División solicita registrar una etiqueta.
2. El sistema verifica que el actor tenga permiso para registrar etiquetas.
3. El sistema solicita el nombre y la descripción.
4. El Jefe de División captura ambos valores.
5. El sistema valida que el nombre y la descripción sean obligatorios y no estén vacíos ni compuestos únicamente por espacios.
6. El sistema compara el nombre capturado con los nombres de las etiquetas activas e inactivas, ignorando diferencias de mayúsculas/minúsculas y espacios sobrantes al inicio o al final.
7. Si existe una coincidencia equivalente activa, el sistema informa que la etiqueta ya existe y no permite crear otra.
8. Si existe una coincidencia equivalente inactiva, el sistema informa de la coincidencia y recomienda reutilizarla o reactivarla; no registra otra etiqueta equivalente.
9. Si solo existen coincidencias aproximadas cuya equivalencia no está confirmada, el sistema presenta sugerencias para que el Jefe de División las valore.
10. Si no hay una equivalencia que impida el registro, el sistema permite continuar. Las sugerencias por similitud no bloquean por sí solas la operación.
11. El Jefe de División confirma el registro.
12. El sistema crea la etiqueta con estado activo y establece `CreatedAtUtc` y `UpdatedAtUtc` con la fecha y hora UTC de creación.
13. El sistema informa que el registro se realizó correctamente.

## 7. Flujo para modificar una etiqueta

1. El Jefe de División selecciona una etiqueta existente y solicita modificarla.
2. El sistema verifica el permiso para modificar etiquetas y que el registro exista.
3. El sistema presenta el nombre y la descripción actuales.
4. El Jefe de División modifica uno o ambos campos.
5. El sistema valida que el nombre y la descripción sigan siendo obligatorios y válidos.
6. Si cambió el nombre, el sistema compara el nuevo valor con las demás etiquetas activas e inactivas aplicando las reglas de duplicidad. La propia etiqueta no se considera una coincidencia duplicada.
7. Si se detecta una equivalencia que impide el cambio, el sistema informa la coincidencia y no guarda el nombre duplicado. Si solo se detecta similitud aproximada, presenta sugerencias sin bloquear automáticamente el cambio.
8. Si las validaciones se cumplen, el sistema guarda los cambios y actualiza `UpdatedAtUtc`. `CreatedAtUtc` permanece sin cambios.
9. El sistema informa que la modificación se realizó correctamente.

## 8. Flujo para activar e inactivar una etiqueta

### 8.1. Inactivar

1. El Jefe de División selecciona una etiqueta y solicita inactivarla.
2. El sistema verifica el permiso correspondiente y que la etiqueta exista.
3. El sistema comprueba el estado actual.
4. Si ya está inactiva, informa que no es necesario cambiar el estado.
5. Si está activa, el sistema cambia `Status` a inactiva, conserva el registro y sus asociaciones, y actualiza `UpdatedAtUtc`.
6. El sistema informa que la etiqueta fue inactivada.

### 8.2. Activar o reactivar

1. El Jefe de División selecciona una etiqueta inactiva y solicita activarla.
2. El sistema verifica el permiso correspondiente y que la etiqueta exista.
3. El sistema comprueba el estado actual.
4. Si ya está activa, informa que no es necesario cambiar el estado.
5. Antes de reactivar, el sistema verifica si existe una etiqueta equivalente que impida la operación, considerando etiquetas activas e inactivas y las reglas de comparación acordadas.
6. Si persiste una duplicidad que impide la reactivación —incluido el caso de una etiqueta inactivada por duplicidad—, el sistema informa el motivo y no cambia el estado.
7. Si no existe una duplicidad que lo impida, el sistema cambia `Status` a activa y actualiza `UpdatedAtUtc`. No crea un registro nuevo ni activa automáticamente asociaciones independientes.
8. El sistema informa que la etiqueta fue activada.

## 9. Validaciones y excepciones

| Situación                                                               | Comportamiento esperado                                                                |
| ----------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| El actor no tiene permiso para la operación                             | Rechazar la operación e informar que no está autorizado. No modificar información.     |
| Nombre vacío o compuesto únicamente por espacios                        | Rechazar el registro o modificación y solicitar un nombre válido.                      |
| Descripción vacía o compuesta únicamente por espacios                   | Rechazar el registro o modificación y solicitar una descripción válida.                |
| Nombre equivalente a una etiqueta activa                                | Informar que ya existe y bloquear el registro o cambio de nombre duplicado.            |
| Nombre equivalente a una etiqueta inactiva                              | Recomendar reutilizarla o reactivarla y evitar crear otra etiqueta equivalente.        |
| Nombre similar, sin equivalencia confirmada                             | Mostrar sugerencias; no bloquear automáticamente por similitud aproximada.             |
| Intento de reactivar una etiqueta con duplicidad vigente                | Rechazar la reactivación e informar que debe resolverse la duplicidad.                 |
| Etiqueta inexistente al modificar o cambiar estado                      | Informar que no se encontró el registro y no efectuar cambios.                         |
| Solicitud de activar una etiqueta ya activa o inactivar una ya inactiva | Informar que la etiqueta ya tiene el estado solicitado; no crear cambios innecesarios. |

Las sugerencias por similitud aproximada apoyan la decisión del Jefe de División; no equivalen a una determinación automática de duplicidad.

## 10. Seguridad

* El sistema debe comprobar los permisos del Jefe de División en cada operación de registro, modificación, activación e inactivación.
* Un actor sin autorización no puede realizar estas operaciones, aunque conozca o seleccione una etiqueta existente.
* Los campos `CreatedAtUtc` y `UpdatedAtUtc` son administrados por el sistema; no son valores que el usuario pueda establecer libremente.
* Si una operación es rechazada por autorización o validación, el sistema no debe guardar cambios parciales.

## 11. Información consumida y generada

### Información consumida

* Nombre y descripción capturados por el Jefe de División.
* Identificador de la etiqueta seleccionada para modificar o cambiar de estado.
* Estado actual de la etiqueta.
* Información de las etiquetas activas e inactivas necesaria para detectar coincidencias y generar sugerencias.
* Permisos del actor para la operación solicitada.

### Información generada o actualizada

* Registro de etiqueta con nombre, descripción, estado y fechas de seguimiento.
* Resultado de validación, incluidos mensajes de coincidencia, duplicidad o sugerencias.
* En una modificación aceptada, actualización del nombre y/o descripción y de `UpdatedAtUtc`.
* En un cambio de estado aceptado, actualización de `Status` y de `UpdatedAtUtc`.

Las fechas proporcionan seguimiento básico de creación y última modificación. No representan un historial detallado de quién realizó cada cambio ni de los valores anteriores.

## 12. Fuera de alcance

Este documento no define:

* La asociación de etiquetas a perfiles de docentes.
* La asociación de etiquetas a materias.
* La asignación de tutores a grupos aperturados.
* La selección o asignación automática de docentes a carga académica.
* Un historial avanzado de auditoría con usuario, valores anteriores y valores nuevos.
* La eliminación física de etiquetas como operación ordinaria.
* Contratos técnicos, endpoints, DTO, comandos, eventos, clases, tablas o relaciones de base de datos.
