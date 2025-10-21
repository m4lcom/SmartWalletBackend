using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Contracts.Responses;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;
using SmartWallet.Contracts.Requests;



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

        [HttpGet]
        public ActionResult<IEnumerable<Wallet>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Wallet> GetById(Guid id)
        {
            var wallet = _service.GetById(id);
            if (wallet == null) return NotFound();
            return Ok(wallet);
        }
        [HttpPost]
        public ActionResult<WalletResponse> Create([FromBody] WalletRequest request)
        {
            var wallet = _service.Create(
                request.UserId,
                request.Name,
                request.CurrencyCode,
                request.Alias,
                request.InitialBalance
            );

            var response = new WalletResponse
            {
                WalletId = wallet.WalletID,
                UserId = wallet.UserID,
                Name = wallet.Name,
                Alias = wallet.Alias,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = wallet.WalletID }, response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }

    
}
