# Pruebas de SIA Platform

## Responsabilidad

Definir el estándar común para crear, organizar, ejecutar y revisar las pruebas automatizadas de SIA Platform.

Este documento busca que todos los servicios utilicen las mismas convenciones de estructura, nombres, aislamiento y validación.

La normalización no significa que todos los servicios deban tener la misma cantidad de pruebas. Cada servicio debe cubrir las reglas y riesgos que realmente le corresponden.

## Lo que sí hace

- Define la ubicación de las pruebas.
- Define la estructura de los proyectos de pruebas.
- Establece convenciones para archivos, clases y métodos.
- Define la cobertura mínima esperada por capa.
- Establece reglas para el uso de fakes.
- Define cómo validar eventos, Outbox y auditoría.
- Establece reglas de aislamiento y determinismo.
- Define los criterios mínimos para enviar pruebas a revisión.
- Sirve como referencia para normalizar pruebas existentes.

## Lo que no hace

- No sustituye los criterios de aceptación de cada tarea.
- No obliga a que todos los servicios tengan la misma cantidad de pruebas.
- No convierte una prueba unitaria en una prueba de integración.
- No permite conectarse a bases de datos o servicios externos desde pruebas unitarias.
- No sustituye la revisión técnica del Pull Request.
- No define reglas funcionales propias de cada dominio.

## Ubicación de las pruebas

Las pruebas unitarias de cada servicio deben permanecer dentro de su propio proyecto:

```text
src/services/SIA.{Service}/SIA.{Service}.Tests
```

Las pruebas transversales deben ubicarse bajo la carpeta `tests` según su propósito:

```text
tests/architecture
tests/building-blocks
tests/contract
tests/integration
```

Responsabilidad de cada ubicación:

- `architecture`: reglas de dependencias y estructura.
- `building-blocks`: componentes compartidos.
- `contract`: compatibilidad de contratos entre servicios.
- `integration`: interacción real entre componentes.

Una prueba de integración no debe colocarse dentro de un proyecto de pruebas unitarias.

## Tecnología estándar

Los proyectos de pruebas unitarias utilizan:

- xUnit.
- `Microsoft.NET.Test.Sdk`.
- `xunit.runner.visualstudio`.
- `coverlet.collector`.
- `Assert` de xUnit.

Las versiones deben mantenerse alineadas con las utilizadas por el repositorio.

No se deben agregar otros frameworks de mocks o aserciones sin justificación y revisión técnica.

## Estructura del proyecto

Cada proyecto de pruebas debe utilizar las siguientes carpetas cuando correspondan:

```text
Application
Application/UseCases
Domain
Infrastructure
Common/Fakes
```

Ejemplo:

```text
SIA.IdentityService.Tests
Application/UseCases/Users
Domain
Infrastructure/Persistence
Infrastructure/Security
Common/Fakes
```

Reglas:

- Las carpetas de funcionalidades deben escribirse en plural.
- El namespace debe coincidir con la ubicación del archivo.
- No deben mezclarse carpetas equivalentes en singular y plural.
- No deben agregarse carpetas vacías al `.csproj`.
- `Infrastructure` solo debe existir cuando contenga pruebas reales.
- Las pruebas de entidades deben ubicarse directamente dentro de `Domain`.
- Los fakes compartidos deben ubicarse dentro de `Common/Fakes`.

## Nombres de archivos y clases

El archivo y la clase deben utilizar el nombre del elemento probado seguido de `Tests` en plural.

Ejemplos correctos:

```text
UserTests.cs
CreateUserUseCaseTests.cs
PasswordHasherTests.cs
```

Ejemplo de clase:

```csharp
public sealed class CreateUserUseCaseTests
{
}
```

No utilizar:

```text
CreateUserUseCaseTest.cs
TestCreateUser.cs
CreateUserTesting.cs
```

## Nombres de métodos

Los nombres deben estar escritos en inglés y describir:

- El método probado.
- El escenario.
- El resultado esperado.

Formatos oficiales:

```text
Method_WithCondition_ShouldExpectedResult
Method_WhenCondition_ShouldExpectedResult
```

`With` se utiliza para describir los datos o el estado de entrada.

`When` se utiliza para describir una condición especial, ausencia o conflicto.

`Should` describe el resultado esperado.

Ejemplos correctos:

```text
Constructor_WithValidData_ShouldCreateActiveUser
Constructor_WithEmptyTenantId_ShouldThrowArgumentException
ExecuteAsync_WithValidData_ShouldCreateUser
ExecuteAsync_WhenUserDoesNotExist_ShouldThrowNotFound
Deactivate_ShouldSetStatusToInactive
```

Ejemplos incorrectos:

```text
Constructor_ValidData_CreateUser
ExecuteAsync_UserNotFound_Throw
Deactivate_StatusFalse
Test1
PruebaCrearUsuario
```

Los nombres deben ser claros, pero no innecesariamente largos.

## Estructura de una prueba

Cada prueba debe seguir el patrón Arrange, Act, Assert:

- Arrange: preparar datos y dependencias.
- Act: ejecutar el comportamiento.
- Assert: verificar el resultado.

No es obligatorio escribir comentarios con los nombres de las fases. Los espacios entre bloques son suficientes cuando la prueba es clara.

Ejemplo:

```csharp
[Fact]
public async Task ExecuteAsync_WithValidData_ShouldCreateUser()
{
  var tenantId = Guid.NewGuid();
  var correlationId = Guid.NewGuid();
  var dataStore = new FakeUserDataStore();
  var useCase = new CreateUserUseCase(dataStore);

  var response = await useCase.ExecuteAsync(CreateRequest(tenantId), correlationId, CancellationToken.None);

  Assert.Equal(tenantId, response.TenantId);
  Assert.Equal(correlationId, response.CorrelationId);
  Assert.NotNull(dataStore.AddedUser);
}
```

Las firmas, llamadas y expresiones que caben claramente en una sola línea deben permanecer en una sola línea.

## Pruebas de Domain

Las pruebas de Domain deben cubrir, cuando aplique:

- Creación válida de la entidad.
- Normalización de textos.
- Identificadores obligatorios.
- Textos obligatorios.
- Rangos numéricos.
- Fechas válidas.
- Cambios de estado.
- Actualización de `UpdatedAtUtc`.
- Soft delete.
- Restauración.
- Idempotencia.
- Manejo de `EntityVersion`.

No es necesario crear pruebas diferentes para combinaciones que representen exactamente el mismo comportamiento.

## Pruebas de Application

Cada caso de uso debe cubrir los escenarios relevantes para su comportamiento.

Como mínimo debe evaluarse, cuando aplique:

- Flujo exitoso.
- Entidad inexistente.
- Duplicado o conflicto.
- Regla de negocio inválida.
- Aislamiento por `TenantId`.
- Respuesta generada.
- `CorrelationId`.
- Interacción con el DataStore.
- Evento de integración.
- Acción de auditoría.
- Usuario actor.
- Ausencia de efectos secundarios cuando el flujo falla.

Una prueba de error debe comprobar que no se guardó, actualizó ni publicó información.

No se establece una cantidad fija de pruebas por caso de uso. La cantidad depende de las reglas y riesgos que se deban cubrir.

## Pruebas de Infrastructure

Infrastructure debe probarse cuando contenga lógica propia.

Ejemplos:

- Hash y verificación de contraseñas.
- Generación y validación de tokens.
- Conversión de entidades a registros persistentes.
- Serialización de auditoría.
- Serialización de eventos.
- Traducción de respuestas de servicios externos.
- Deduplicación.
- Outbox.
- Inbox.

No es obligatorio crear pruebas para clases que solamente delegan directamente a una librería sin agregar comportamiento propio.

No deben agregarse carpetas vacías para aparentar cobertura.

## Uso de fakes

Los fakes deben ser pequeños, deterministas y utilizarse únicamente cuando ayuden a aislar un caso de uso.

Reglas:

- Un fake reutilizado debe ubicarse en `Common/Fakes`.
- Un fake utilizado por una sola clase puede declararse dentro del archivo de pruebas.
- El nombre debe utilizar el formato `Fake{Dependency}`.
- Los resultados configurables deben tener nombres claros.
- Los argumentos relevantes deben capturarse para poder verificarlos.
- No deben conectarse a SQL Server.
- No deben conectarse a RabbitMQ.
- No deben realizar llamadas HTTP.
- No deben reproducir reglas de negocio.
- No deben contener funciones que ninguna prueba utiliza.

Cuando un DataStore recibe una entidad o un evento, el fake debe capturar el objeto recibido.

Ejemplo:

```csharp
public User? AddedUser { get; private set; }

public UserCreatedIntegrationEvent? AddedEvent { get; private set; }
```

Es preferible comprobar el contenido capturado en lugar de validar únicamente un indicador como:

```csharp
UserAdded = true;
```

## Eventos, Outbox y auditoría

Cuando un caso de uso genere eventos o auditoría, las pruebas deben comprobar los campos relevantes:

- Tipo de evento.
- Identificador de la entidad.
- `TenantId`.
- `CorrelationId`.
- Versión del evento.
- Versión de la entidad.
- Acción de auditoría.
- Usuario actor.
- Actor `null` para operaciones anónimas o de sistema.

Las pruebas unitarias validan que el evento sea entregado al DataStore dentro de la operación correspondiente.

La publicación física en RabbitMQ pertenece a las pruebas de integración.

No debe simularse una publicación exitosa para afirmar que RabbitMQ fue probado.

## Uso de Fact y Theory

Utilizar `[Fact]` cuando la prueba represente un solo escenario.

Utilizar `[Theory]` cuando el mismo comportamiento deba validarse con diferentes entradas.

Los datos de un `[Theory]` pueden proporcionarse mediante:

- `InlineData`.
- `MemberData`.
- `ClassData`.

No se deben crear métodos idénticos que solamente cambien un valor de entrada.

Cada conjunto de datos de un `[Theory]` cuenta como un caso ejecutado. Por esta razón, el Explorador de pruebas puede mostrar más pruebas que métodos declarados.

## Aislamiento

Una prueba unitaria debe poder ejecutarse:

- Sin conexión a internet.
- Sin Azure SQL.
- Sin RabbitMQ.
- Sin servicios externos.
- Sin depender del orden de ejecución.
- Sin reutilizar información persistida.
- Sin depender de la hora local del equipo.

Reglas:

- Preferir fechas fijas.
- Utilizar `DateTime.UtcNow` solo cuando el valor exacto no forme parte de la aserción.
- No utilizar `Thread.Sleep`.
- No compartir instancias mutables entre pruebas.
- Cada prueba debe preparar su propio estado.
- No incluir secretos ni cadenas de conexión.

## Aserciones

Las aserciones deben validar resultados observables y relevantes.

Una prueba puede contener varias aserciones cuando todas comprueben el mismo comportamiento.

No deben validarse diferentes comportamientos independientes dentro del mismo método.

Además de la respuesta, debe verificarse el cambio de estado o la interacción relevante.

Ejemplo:

```csharp
Assert.Equal(tenantId, response.TenantId);
Assert.Equal(correlationId, response.CorrelationId);
Assert.NotNull(dataStore.AddedUser);
Assert.NotNull(dataStore.AddedEvent);
Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId);
```

No se deben agregar aserciones que repitan la implementación sin comprobar una regla útil.

## Ejecución

Para ejecutar todas las pruebas desde la raíz del repositorio:

```bash
dotnet test SIA.Platform.slnx
```

Para ejecutar únicamente las pruebas de un servicio:

```bash
dotnet test src/services/SIA.IdentityService/SIA.IdentityService.Tests/SIA.IdentityService.Tests.csproj
```

En Visual Studio se debe agrupar el Explorador de pruebas por proyecto antes de reportar la cantidad de pruebas.

Los resultados anteriores deben actualizarse cuando se agreguen o eliminen pruebas.

## Pull Request

Una tarea de pruebas puede enviarse a revisión cuando:

- El proyecto compila sin errores.
- No introduce advertencias nuevas.
- Todas las pruebas nuevas pasan.
- Las pruebas existentes continúan pasando.
- Los nombres respetan este estándar.
- Las carpetas y namespaces coinciden.
- Los fakes contienen únicamente lo necesario.
- Se cubren los flujos exitosos y errores relevantes.
- Los eventos y auditorías se validan cuando aplican.
- No existen pruebas vacías.
- No existen pruebas deshabilitadas.
- No existe código comentado.
- No se agregaron carpetas vacías al `.csproj`.
- No se agregaron secretos.
- No se agregaron dependencias externas innecesarias.
- El PR indica la cantidad de pruebas del proyecto afectado.

La cantidad de pruebas no sustituye la revisión de su calidad.

## Adopción del estándar

Este documento es obligatorio para pruebas nuevas y para pruebas modificadas después de su incorporación.

La normalización de pruebas existentes debe realizarse por servicio y mediante tareas delimitadas.

No deben mezclarse refactorizaciones masivas de pruebas con cambios funcionales no relacionados.

Cuando se detecten pruebas fuera del estándar:

- Se deben corregir las pruebas que formen parte del alcance actual.
- Se debe registrar una tarea separada si la corrección excede el alcance.
- No debe copiarse una convención inconsistente a archivos nuevos.

IdentityService se utiliza como referencia inicial por su separación entre Domain, Application e Infrastructure.

A partir de la incorporación de este documento, `tests/README.md` es la fuente canónica para todos los servicios.

## Reglas críticas

- Las pruebas unitarias no utilizan infraestructura externa.
- Cada prueba prepara su propio estado.
- Los nombres deben describir escenario y resultado.
- Los namespaces deben coincidir con las carpetas.
- Los fakes deben capturar los datos relevantes.
- Los errores deben comprobar la ausencia de efectos secundarios.
- Los eventos deben validarse por contenido.
- Las carpetas vacías no cuentan como cobertura.
- Las pruebas nuevas deben respetar este estándar.
- Ninguna tarea se considera terminada únicamente por tener una cantidad alta de pruebas.
