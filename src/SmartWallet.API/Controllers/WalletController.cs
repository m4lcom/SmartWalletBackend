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
        private readonly WalletService _service;

        public WalletController(WalletService service)
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
        public ActionResult<Wallet> Create([FromBody] CreateWalletRequest request)
        {
            var wallet = _service.Create(
                request.UserId,
                request.Name,
                request.CurrencyCode,
                request.Alias,
                request.InitialBalance
            );
            return CreatedAtAction(nameof(GetById), new { id = wallet.WalletID }, wallet);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }

    public record CreateWalletRequest(Guid UserId, string Name, CurrencyCode CurrencyCode, string Alias, decimal InitialBalance);
}
