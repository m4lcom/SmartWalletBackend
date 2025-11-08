# Documentación: HttpClient + Polly (DolarApi)

Última actualización: 2025-10-24

Resumen
- Explica cómo se registra y configura el `HttpClient` para `DolarApi` y las políticas de resiliencia aplicadas mediante `Polly`.
- Describe los tipos y archivos relevantes en el proyecto `SmartWallet`.

Arquitectura y archivos relevantes
- `src\SmartWallet.Infrastructure\Extensions\ServiceCollectionExtension.cs`
  - Método: `AddDolarApi(IServiceCollection, IConfiguration)` — registra `IDolarApiService` y configura `HttpClient`.
- `src\SmartWallet.Infrastructure\ExternalServices\DolarApiService.cs`
  - Implementación de `IDolarApiService` que usa el `HttpClient` inyectado.
- `src\SmartWallet.Application\Abstractions\IDolarApiService.cs`
  - Interfaz pública del servicio.
- `src\SmartWallet.Infrastructure\ExternalServices\Polly\PollyResiliencePolicies.cs`
  - Contiene métodos `GetRetryPolicy` y `GetCircuitBreakerPolicy`.
- `src\SmartWallet.Infrastructure\ExternalServices\Polly\ApiClientConfiguration.cs`
  - Modelo que define los parámetros de configuración de las políticas (retry, circuit breaker).
- `src\Contracts\Responses\DolarDto.cs`
  - DTO de respuesta utilizado por `DolarApiService`.

Configuración (appsettings.json)
- Añade o verifica la sección `DolarApi` en `src\SmartWallet.API\appsettings.json`. Ejemplo mínimo:

- Campos importantes:
  - `BaseUrl` (string) — URL base del API. Obligatorio; `AddDolarApi` lanza `InvalidOperationException` si está vacío.
  - `TimeoutSeconds` (opcional) — tiempo de espera que puede usar `DolarApiService`.
  - `Polly.*` — parámetros que controlan `RetryPolicy` y `CircuitBreaker`.

Registro de servicios
- `AddDolarApi` hace lo siguiente:
  1. `services.Configure<DolarApiOptions>(configuration.GetSection("DolarApi"))`
  2. `services.Configure<ApiClientConfiguration>(configuration.GetSection("DolarApi:Polly"))`
  3. `services.AddHttpClient<IDolarApiService, DolarApiService>((sp, client) => { ... })`
     - Valida `DolarApiOptions.BaseUrl` y asigna `client.BaseAddress`.
  4. Aplica políticas con `.AddPolicyHandler(...)` usando `PollyResiliencePolicies.GetRetryPolicy(cfg)` y `GetCircuitBreakerPolicy(cfg)`.

Políticas de resiliencia (resumen)
- Retry Policy
  - Basada en el valor `RetryCount`.
  - Implementa backoff exponencial (configurable con `RetryInitialBackoffMs` y `RetryMaxBackoffMs`).
  - Reintenta en errores transitorios típicos (5xx, timeouts, etc.). Ver `PollyResiliencePolicies` para detalles de excepciones y comportamiento exacto.
- Circuit Breaker
  - Se abre cuando el número de fallos supera `CircuitBreakerFailureThreshold`.
  - Permanencia en abierto por `CircuitBreakerDurationSeconds`.
  - Evita llamar repetidamente al API cuando está inestable.

Buenas prácticas y recomendaciones
- Producción:
  - Ajusta `RetryCount` y tiempos de backoff para evitar sobrecarga del endpoint.
  - Configura `CircuitBreakerFailureThreshold` y `CircuitBreakerDurationSeconds` basados en SLAs del proveedor externo.
  - Habilita logging estructurado para las políticas (ej., eventos de circuito abierto/close/retry).
- Telemetría:
  - Instrumenta reintentos y eventos de circuito con Application Insights, Prometheus o similar para alertas.
- Tests:
  - Crear pruebas de integración que simulen fallos (timeouts, 5xx) para validar que las políticas se disparan.
  - Considerar usar `Microsoft.Extensions.Http.Testing` o `WireMock.Net` para simular respuestas del API.
- Timeouts:
  - Además del retry/circuit, definir `TimeoutPolicy` o `HttpClient.Timeout` si es necesario para cortar llamadas largas.
- Idempotencia:
  - Asegurarse de que las operaciones reintentadas sean idempotentes o que el retry sea seguro para el endpoint.

Cómo utilizar `IDolarApiService`
- Inyectar en controladores o servicios:

Debug y diagnóstico
- Logs:
  - Añadir logs en `DolarApiService` alrededor de llamadas externas.
  - Registrar eventos de `PollyResiliencePolicies` (retries, circuit open/close).
- Ver la salida de `HttpClient` y `Polly` usando niveles de `Debug`/`Trace` para identificar patrones.
- Revisar el Output en Visual Studio: pestaña `Output` y seleccionar las fuentes correspondientes en __Output Window__.

Instrucciones para la solución y control de versiones
- Incluir `docs/httpclient-polly.md` y `docs/changelog.md` como Solution Items:
  - En __Solution Explorer__ -> botón derecho sobre la solución -> __Add__ > __Existing Item__ -> seleccionar archivos dentro de `docs/`.
- Commit sugerido:
  - `docs: add httpclient + polly documentation for DolarApi`
- Branch sugerido:
  - `docs/httpclient-polly` o según las convenciones: `docs/*`.

Notas finales
- Si se cambian los nombres de opciones o las estructuras de `ApiClientConfiguration` y `DolarApiOptions`, actualizar `AddDolarApi` y la documentación.
- Para optimizaciones avanzadas (benchmarks, perfiles de latencia) sigue la guía de performance y crea pruebas controladas. (Si deseas, puedo preparar un perfil/baseline).
