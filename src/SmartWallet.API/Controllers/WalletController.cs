using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;


namespace SmartWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetById(Guid id)
        {
            var wallet = await _service.GetByIdAsync(id);
            if (wallet == null) return NotFound();
            return Ok(wallet);
        }

        [HttpPost]
        public async Task<ActionResult<WalletResponse>> CreateAsync([FromBody] WalletRequest request)
        {
            if (!Enum.TryParse<CurrencyCode>(request.CurrencyCode, true, out var currency))
                return BadRequest("Código de moneda inválido.");

            var wallet = await _service.CreateAsync(
                request.UserId,
                request.Name,
                currency,
                request.Alias,
                request.InitialBalance
            );


            var response = new WalletResponse
            {
                Id = wallet.Id,
                UserId = wallet.UserID,
                Name = wallet.Name,
                CurrencyCode = currency.ToString(),
                Alias = wallet.Alias,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, response);
        }

    }

    
}
