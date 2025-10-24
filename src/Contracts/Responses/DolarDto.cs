using System;
using System.Text.Json.Serialization;

namespace Contracts.Responses
{
    public class DolarDto
    {
        public string? Moneda { get; set; }
        public string? Casa { get; set; }
        public string? Nombre { get; set; }

        public decimal Compra { get; set; }
        public decimal Venta { get; set; }

        public DateTimeOffset FechaActualizacion { get; set; }
    }
}