# Definition of Done - SIA

## Objetivo

La Definition of Done define cuándo una tarea puede considerarse terminada.

Su propósito es evitar que una tarea pase a Finalizado solo porque aparentemente funciona, sin haber sido revisada, probada o documentada correctamente.

## Regla general

Una tarea está terminada cuando cumple con los criterios funcionales, técnicos y de calidad acordados por el equipo.

## Requisitos mínimos

Una tarea está Done si cumple con lo siguiente:

1. La solución compila correctamente.
2. No genera errores.
3. No genera advertencias nuevas.
4. No rompe pruebas existentes.
5. No rompe pruebas de arquitectura.
6. Respeta la responsabilidad del servicio afectado.
7. No accede a bases de datos ajenas.
8. No comparte entidades internas entre servicios.
9. Tiene Pull Request revisado cuando aplica.
10. Fue validada funcionalmente cuando aplica.
11. La documentación fue actualizada si cambió arquitectura, flujo, contrato o regla.
12. El ticket de Jira tiene evidencia suficiente para cerrarse.

## Cambios de arquitectura

Si una tarea modifica arquitectura, estructura de carpetas, referencias entre proyectos, contratos, eventos o infraestructura, debe ser revisada por un senior o por el líder técnico.

## Cambios funcionales

Si una tarea modifica un flujo de negocio, debe ser validada con la Product Owner funcional o con quien represente el proceso institucional.

## Regla de cierre

Una tarea no debe pasar a Finalizado si solo fue implementada pero no fue probada.

## Excepción

Las tareas exclusivamente documentales pueden cerrarse cuando el documento exista, esté ubicado correctamente y haya sido revisado.
