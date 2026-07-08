# Reglas Arquitectónicas de SIA

## Regla 1
Cada servicio es dueño de su propia base de datos.

## Regla 2
Ningún servicio puede acceder directamente a la base de datos de otro servicio.

## Regla 3
Ningún servicio puede usar entidades de dominio de otro servicio.

## Regla 4
La comunicación entre servicios debe realizarse mediante contratos públicos o eventos de integración.

## Regla 5
Todo cambio importante de estado debe generar un evento.

## Regla 6
El API Gateway no contiene lógica de negocio.

## Regla 7
Los BFF no contienen reglas de negocio institucional.

## Regla 8
El frontend no orquesta procesos institucionales complejos.

## Regla 9
Todo proceso institucional debe tener estado, responsable y trazabilidad.

## Regla 10
No se crea un servicio nuevo sin justificación clara de dominio.
