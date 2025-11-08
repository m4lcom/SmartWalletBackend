using Contracts.Responses;
using Microsoft.Extensions.Options;
using SmartWallet.Application.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartWallet.Infrastructure.ExternalServices
{
    public class DolarApiService : IDolarApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DolarApiService(HttpClient httpClient, IOptions<DolarApiOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.BaseUrl))
                throw new ArgumentException("DolarApi BaseUrl no está configurada.", nameof(options));

            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        public async Task<DolarDto?> GetDolarByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("El tipo de dólar es requerido.", nameof(type));

            var relative = $"/v1/dolares/{type}";

            using var response = await _httpClient.GetAsync(relative).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<DolarDto>(_jsonOptions).ConfigureAwait(false);
        }
    }
}
