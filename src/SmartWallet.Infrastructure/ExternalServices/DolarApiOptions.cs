namespace SmartWallet.Infrastructure.ExternalServices
{
    public class DolarApiOptions
    {
        // BaseUrl base de la API (sin finalizar necesariamente en /)
        public string BaseUrl { get; set; } = "https://dolarapi.com";

        // Endpoint por defecto relativo (opcional)
        public string? DefaultEndpoint { get; set; } = "/v1/dolares";
    }
}