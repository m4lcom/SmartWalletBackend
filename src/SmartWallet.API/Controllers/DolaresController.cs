using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Abstractions;


namespace SmartWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DolaresController : ControllerBase
    {
        private readonly IDolarApiService _dolarService;

        public DolaresController(IDolarApiService dolarService)
        {
            _dolarService = dolarService;
        }

        [HttpGet("{tipo}")]
        public async Task<ActionResult<DolarDto>> GetByTipo( [FromRoute] string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return BadRequest(new { message = "Debe especificar el tipo de dólar." });

            try
            {
                var resultado = await _dolarService.GetDolarByTypeAsync(tipo);
                if (resultado is null) return NotFound();
                return Ok(resultado);
            }
            catch (HttpRequestException hx)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { message = "Error al llamar a la API externa.", detail = hx.Message });
            }
            catch (ArgumentException ax)
            {
                return BadRequest(new { message = ax.Message });
            }
        }
    }
}