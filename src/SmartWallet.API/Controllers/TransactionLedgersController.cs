using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Contracts.Responses;
using SmartWallet.Domain.Entities;


namespace SmartWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionLedgersController : ControllerBase
    {
        private readonly ITransactionLedgerService _ledgerService;

        public TransactionLedgersController (ITransactionLedgerService ledgerservice)
        {
            _ledgerService = ledgerservice;
        }

        // --- consultas --- 
        // --- obtiene un ledger por su identificador unico. ---
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ledger = await _ledgerService.GetByIdAsync(id);
            if(ledger == null) return NotFound();
            return Ok(MapToResponse(ledger));
        }

        // --- obtiene todos los ledger asociados a una wallet especifica.
        [HttpGet("wallet/{walletId:guid}")]
        public async Task<IActionResult> GetByWallet(Guid walletId)
        {
            var ledgers = await _ledgerService.GetByWalletAsync(walletId);
            return Ok(ledgers.Select(MapToResponse));
        }

        // --- obtiene todos los ledgers relacionados a una transaccion especifica. ---
        [HttpGet("transaction/{transactionId:guid}")]
        public async Task<IActionResult> GetByTransaction(Guid transactionId)
        {
            var ledgers = await _ledgerService.GetByTransactionAsync(transactionId);
            return Ok(ledgers.Select(MapToResponse));
        }

        // --- obtiene todos los ledgers dentro de un rango de fechas. ---
        [HttpGet("range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var ledgers = await _ledgerService.GetByDateRangeAsync(from, to);
            return Ok(ledgers.Select(MapToResponse));
        }

        // --- mapper privado ---
        private static TransactionLedgerResponse MapToResponse(TransactionLedger ledger) => new TransactionLedgerResponse(
            ledger.Id,
            ledger.Timestamp,
            ledger.Type.ToString(),
            ledger.Amount,
            ledger.CurrencyCode.ToString(),
            ledger.Status.ToString(),
            ledger.WalletId,
            ledger.TransactionId,
            ledger.Metadata);

    }
}
