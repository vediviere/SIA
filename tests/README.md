## Estándar de pruebas de SIA Platform

## 1. Objetivo

* Este documento define el estándar obligatorio para crear, organizar y revisar pruebas en SIA Platform.

* Su propósito es que todos los servicios utilicen las mismas convenciones de estructura, nombres, aislamiento y validación, independientemente del desarrollador que implemente las pruebas.

* La normalización no significa que todos los servicios deban tener la misma cantidad de pruebas. Cada servicio debe cubrir las reglas y riesgos que realmente le corresponden, pero debe hacerlo siguiendo una estructura común.

## 2. Alcance

Este estándar aplica a:

* Pruebas unitarias de `Domain`.

* Pruebas unitarias de `Application`.

* Pruebas unitarias de `Infrastructure`.

* Fakes utilizados para aislar dependencias.

* Pruebas de integración, contrato, arquitectura y Building Blocks ubicadas bajo tests/.

No sustituye los criterios de aceptación de cada tarea ni las reglas de la Definition of Done.

## 3. Tipos y ubicación de pruebas

### 3.1 Pruebas unitarias de servicios

* Las pruebas unitarias de un servicio deben permanecer dentro de su propio proyecto:

src/services/SIA.{Service}/SIA.{Service}.Tests

Estas pruebas no deben requerir bases de datos, brokers, servicios remotos ni infraestructura externa.

### 3.2 Pruebas transversales

* Las pruebas compartidas o transversales deben ubicarse según su propósito:

tests/
├── architecture/       Reglas de dependencias y estructura
├── building-blocks/    Building Blocks compartidos
├── contract/           Compatibilidad de contratos entre servicios
└── integration/        Interacción real entre componentes

Una prueba de integración no debe colocarse dentro de una carpeta de pruebas unitarias solo para reutilizar su proyecto.

## 4. Tecnología estándar

Los proyectos de pruebas unitarias utilizan:

* xUnit como framework de pruebas.

* Microsoft.NET.Test.Sdk para descubrimiento y ejecución.

* xunit.runner.visualstudio para integración con Visual Studio.

* coverlet.collector para recopilación de cobertura.

* Assert de xUnit como biblioteca estándar de aserciones.

Las versiones deben mantenerse alineadas con las utilizadas por el repositorio. No se deben agregar frameworks de mocks o aserciones diferentes sin justificación y revisión técnica.

## 5. Estructura canónica del proyecto

Cada proyecto de pruebas debe seguir esta estructura:

SIA.{Service}.Tests/
├── Application/
│   └── UseCases/
│       └── {FeaturePlural}/
│           └── {UseCase}Tests.cs
├── Domain/
│   └── {Entity}Tests.cs
├── Infrastructure/
│   ├── Persistence/
│   ├── Security/
│   └── {TechnologyBoundary}/
└── Common/
    └── Fakes/
        └── Fake{Dependency}.cs

Reglas:

* Las carpetas de funcionalidades deben escribirse en plural: Users, Persons, Buildings.

* El namespace debe coincidir exactamente con la ubicación del archivo.

* No deben mezclarse carpetas equivalentes en singular y plural.

* No deben agregarse carpetas vacías al .csproj.

* Infrastructure solo debe existir cuando contenga pruebas reales.

* Las pruebas de entidades deben utilizar una sola ubicación dentro del servicio. Para servicios nuevos se utilizará directamente Domain/{Entity}Tests.cs.

## 6. Nombres de archivos y clases

El archivo y la clase deben usar el nombre del elemento probado seguido de Tests en plural:

UserTests.cs
CreateUserUseCaseTests.cs
PasswordHasherTests.cs

Ejemplo:

public sealed class CreateUserUseCaseTests
{
}

No utilizar:

`CreateUserUseCaseTest.cs`
`TestCreateUser.cs`
`CreateUserTesting.cs`

## 7. Nombres de métodos de prueba

Los nombres deben estar en inglés y describir comportamiento, escenario y resultado.

Formato:

* Method_WithCondition_ShouldExpectedResult
* Method_WhenCondition_ShouldExpectedResult

Uso recomendado:

`With` describe los datos o estado de entrada.

`When` describe una condición especial, ausencia o conflicto.

`Should` describe el resultado esperado.

Ejemplos correctos:

* Constructor_WithValidData_ShouldCreateActiveUser
* Constructor_WithEmptyTenantId_ShouldThrowArgumentException
* ExecuteAsync_WithValidData_ShouldCreateUser
* ExecuteAsync_WhenUserDoesNotExist_ShouldThrowNotFound
* Deactivate_ShouldSetStatusToInactive

Ejemplos incorrectos:

* Constructor_ValidData_CreateUser
* ExecuteAsync_UserNotFound_Throw
* Deactivate_StatusFalse
* Test1
* PruebaCrearUsuario

Los nombres deben ser claros, pero no innecesariamente largos. Se debe evitar repetir información que ya aporta el nombre de la clase.

## 8. Estructura Arrange, Act, Assert

Cada prueba debe separar visualmente las tres fases:

`Arrange`: preparar datos y dependencias.

`Act`: ejecutar el comportamiento.

`Assert`: verificar el resultado.

No es obligatorio escribir comentarios Arrange, Act y Assert; los saltos de línea son suficientes cuando la prueba permanece clara.

`[Fact]`
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

Las firmas, llamadas y expresiones que caben claramente en una sola línea deben permanecer en una sola línea.

## 9. Cobertura mínima por capa

### 9.1 Domain

Las pruebas de Domain deben cubrir, cuando aplique:

* Creación válida de la entidad.

* Normalización de texto: Trim, mayúsculas o minúsculas.

* Identificadores obligatorios.

* Textos obligatorios.

* Rangos numéricos y fechas válidas.

* Cambios de estado.

* Actualización de UpdatedAtUtc.

* Soft delete y restauración.

* Idempotencia cuando una operación pueda repetirse.

Incremento o conservación de EntityVersion cuando corresponda.

No es necesario crear una prueba distinta para combinaciones que no representen comportamientos diferentes. Para múltiples entradas equivalentes debe considerarse [Theory].

### 9.2 Application

Cada caso de uso debe cubrir los escenarios relevantes, no una cantidad fija de pruebas.

Como mínimo se debe evaluar:

* Flujo exitoso.

* Entidad inexistente cuando exista una consulta previa.

* Duplicado o conflicto cuando exista una regla de unicidad.

* Regla de negocio inválida.

* Aislamiento por TenantId cuando aplique.

* Respuesta y CorrelationId.

* Interacción esperada con el DataStore.

* Creación del evento de integración cuando aplique.

* Acción de auditoría y actor cuando aplique.

* Ausencia de efectos secundarios cuando el flujo falla.

Una prueba de error debe comprobar también que no se guardó, actualizó o publicó información.

### 9.3 Infrastructure

`Infrastructure` debe probarse cuando contenga lógica propia, por ejemplo:

`Hash` y verificación de contraseñas.

Generación y validación de `tokens`.

Conversión de entidades a registros `persistentes`.

Serialización de auditoría o eventos.

Traducción de respuestas de un servicio externo.

Reglas de deduplicación, `Outbox` o `Inbox`.

No es obligatorio crear pruebas de Infrastructure para clases que solo delegan directamente a una librería sin lógica adicional. Tampoco deben dejarse carpetas vacías para aparentar cobertura.

## 10. Uso de Fakes

Los `fakes` deben ser pequeños, deterministas y creados únicamente cuando ayuden a aislar el caso de uso.

Reglas:

* Un `fake` reutilizado por varias clases debe ubicarse en `Common/Fakes`.

* Un `fake` utilizado por una sola clase puede declararse como clase privada dentro del archivo de pruebas.

* El nombre debe usar el formato `Fake{Dependency}`.

* Los resultados configurables deben exponerse con nombres explícitos.

* Los argumentos relevantes deben capturarse para poder verificarlos.

* No deben conectarse a `SQL Server`, `RabbitMQ`, `HTTP` ni otros recursos externos.

* No deben reproducir reglas de negocio que pertenecen a la implementación real.

* No deben acumular funciones que ninguna prueba utiliza.

Cuando se guarda una entidad o un evento, se debe capturar el objeto recibido:

`public User? AddedUser { get; private set; }`

`public UserCreatedIntegrationEvent? AddedEvent { get; private set; }`

Es preferible comprobar el contenido capturado en lugar de validar únicamente un indicador booleano como UserAdded = true.

## 11. Eventos, Outbox y auditoría

Cuando un caso de uso genere eventos o auditoría, las pruebas deben comprobar los campos que forman parte del contrato funcional:

* Tipo de evento.

* Identificador de la entidad.

* TenantId.

* CorrelationId.

* Versión del evento o de la entidad.

* Acción de auditoría.

* Usuario actor, incluyendo null para operaciones anónimas o de sistema.

Las pruebas unitarias validan que el evento se entregue al `DataStore` dentro de la operación esperada. La publicación física en `RabbitMQ` corresponde a pruebas de integración.

No debe simularse una publicación exitosa para afirmar que `RabbitMQ` fue probado.

## 12. Fact y Theory

Utilizar `[Fact]` cuando la prueba represente un solo escenario.

Utilizar `[Theory]` con `[InlineData]`, `[MemberData]` o `[ClassData]` cuando el mismo comportamiento deba validarse con diferentes entradas.

No se deben crear múltiples métodos idénticos que solo cambien un valor de entrada.

Cada conjunto de datos de un `[Theory]` cuenta como un caso ejecutado. Por esa razón, el número mostrado por el Explorador de pruebas puede ser mayor que la cantidad de métodos declarados.

## 13. Aislamiento y determinismo

Una prueba unitaria debe poder ejecutarse:

* Sin conexión a internet.

* Sin `Azure SQL`.

* Sin `RabbitMQ`.

* Sin depender del orden de otras pruebas.

* Sin reutilizar información persistida por otra prueba.

* Sin depender de la hora local de la computadora.

Reglas:

* Preferir fechas fijas en lugar de `DateTime.Now`.

* Utilizar `DateTime.UtcNow` solo cuando el valor exacto no forme parte de la aserción.

* No utilizar `Thread.Sleep` para esperar resultados.

* No compartir instancias mutables entre pruebas.

* Cada prueba debe preparar su propio estado.

Los secretos y cadenas de conexión no deben incluirse en proyectos de pruebas unitarias.

## 14. Aserciones

Las aserciones deben validar resultados observables y relevantes.

Una prueba puede tener varias aserciones cuando todas comprueben el mismo comportamiento. No debe validar varios comportamientos independientes dentro del mismo método.

Además de la respuesta, debe verificarse el cambio de estado o la interacción relevante:

`Assert.Equal(tenantId, response.TenantId)`;
`Assert.Equal(correlationId, response.CorrelationId)`;
`Assert.NotNull(dataStore.AddedUser)`;
`Assert.NotNull(dataStore.AddedEvent)`;
`Assert.Equal(correlationId, dataStore.AddedEvent.CorrelationId)`;

No se deben escribir aserciones que repitan la implementación sin comprobar una regla o resultado útil.

## 15. Ejecución

Desde la raíz del repositorio:

`dotnet test SIA.Platform.slnx`

Para ejecutar únicamente las pruebas de un servicio:

`dotnet test src/services/SIA.IdentityService/SIA.IdentityService.Tests/SIA.IdentityService.Tests.csproj`

En Visual Studio se debe agrupar el Explorador de pruebas por proyecto antes de reportar la cantidad de pruebas. Los resultados anteriores deben limpiarse o actualizarse cuando se agreguen o eliminen pruebas.

## 16. Criterios para Pull Request

Una tarea de pruebas puede enviarse a revisión cuando:

* El proyecto de pruebas compila sin errores.

* No introduce advertencias nuevas.

* Todas las pruebas nuevas pasan.

* Las pruebas existentes continúan pasando.

* Los nombres siguen este estándar.

* Las carpetas y namespaces coinciden.

* Los `fakes` solo contienen lo necesario.

* Se cubren flujos exitosos y errores relevantes.

* Los eventos y auditorías se validan cuando aplican.

* No existen pruebas vacías, deshabilitadas o comentadas.

* No se agregaron carpetas vacías al .csproj.

* No se agregaron secretos ni dependencias externas innecesarias.

* El PR indica la cantidad de pruebas del proyecto afectado, no la cantidad global mostrada por Visual Studio.

La cantidad de pruebas no reemplaza la revisión de su calidad.

## 17. Adopción del estándar

Este documento es obligatorio para pruebas nuevas y para pruebas modificadas después de su incorporación.

La normalización de pruebas existentes debe realizarse por servicio y mediante tareas delimitadas. No se deben mezclar refactorizaciones masivas de pruebas con cambios funcionales no relacionados.

Cuando una tarea detecte pruebas fuera del estándar:

* Debe corregir las pruebas que formen parte directa de su alcance.

* Debe registrar una tarea separada si la corrección excede el alcance actual.

* No debe copiar una convención inconsistente a nuevos archivos.
