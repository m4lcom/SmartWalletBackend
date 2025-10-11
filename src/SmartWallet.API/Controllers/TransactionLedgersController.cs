using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Contracts.Responses;


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
            if (ledger == null) return NotFound();

            var response = new TransactionLedgerResponse(
                ledger.Id,
                ledger.Timestamp,
                ledger.Type.ToString(),
                ledger.Amount,
                ledger.CurrencyCode.ToString(),
                ledger.Status.ToString(),
                ledger.SourceWalletId,
                ledger.DestinationWalletId,
                ledger.SourceTransactionId,
                ledger.DestinationTransactionId,
                ledger.Metadata
            );

            return Ok(response);
        }

        // --- obtiene todos los ledger asociados a una wallet especifica.
        [HttpGet("wallet/{walletId:guid}")]
        public async Task<IActionResult> GetByWallet(Guid walletId)
        {
            var ledgers = await _ledgerService.GetByWalletAsync(walletId);

            var responses = ledgers.Select(l => new TransactionLedgerResponse(
                l.Id,
                l.Timestamp,
                l.Type.ToString(),
                l.Amount,
                l.CurrencyCode.ToString(),
                l.Status.ToString(),
                l.SourceWalletId,
                l.DestinationWalletId,
                l.SourceTransactionId,
                l.DestinationTransactionId,
                l.Metadata
            ));

            return Ok(responses);
        }

        // --- obtiene todos los ledgers relacionados a una transaccion especifica. ---
        [HttpGet("transaction/{transactionId:guid}")]
        public async Task<IActionResult> GetByTransaction(Guid transactionId)
        {
            var ledgers = await _ledgerService.GetByTransactionAsync(transactionId);

            var responses = ledgers.Select(l => new TransactionLedgerResponse(
                l.Id,
                l.Timestamp,
                l.Type.ToString(),
                l.Amount,
                l.CurrencyCode.ToString(),
                l.Status.ToString(),
                l.SourceWalletId,
                l.DestinationWalletId,
                l.SourceTransactionId,
                l.DestinationTransactionId,
                l.Metadata
            ));

            return Ok(responses);
        }

        // --- obtiene todos los ledgers dentro de un rango de fechas. ---
        [HttpGet("range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var ledgers = await _ledgerService.GetByDateRangeAsync(from, to);

            var responses = ledgers.Select(l => new TransactionLedgerResponse(
                l.Id,
                l.Timestamp,
                l.Type.ToString(),
                l.Amount,
                l.CurrencyCode.ToString(),
                l.Status.ToString(),
                l.SourceWalletId,
                l.DestinationWalletId,
                l.SourceTransactionId,
                l.DestinationTransactionId,
                l.Metadata
            ));

            return Ok(responses);
        }

        // --- cambios de estado ---

        // --- marca un ledger como completado. ---
        [HttpPatch("{id:guid}/complete")]
        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            var ledger = await _ledgerService.MarkAsCompletedAsync(id);

            var response = new TransactionLedgerResponse(
                ledger.Id,
                ledger.Timestamp,
                ledger.Type.ToString(),
                ledger.Amount,
                ledger.CurrencyCode.ToString(),
                ledger.Status.ToString(),
                ledger.SourceWalletId,
                ledger.DestinationWalletId,
                ledger.SourceTransactionId,
                ledger.DestinationTransactionId,
                ledger.Metadata
            );

            return Ok(response);
        }

        // --- marca un ledger como fallido. ---
        [HttpPatch("{id:guid}/fail")]
        public async Task<IActionResult> MarkAsFailed(Guid id)
        {
            var ledger = await _ledgerService.MarkAsFailedAsync(id);

            var response = new TransactionLedgerResponse(
                ledger.Id,
                ledger.Timestamp,
                ledger.Type.ToString(),
                ledger.Amount,
                ledger.CurrencyCode.ToString(),
                ledger.Status.ToString(),
                ledger.SourceWalletId,
                ledger.DestinationWalletId,
                ledger.SourceTransactionId,
                ledger.DestinationTransactionId,
                ledger.Metadata
            );

            return Ok(response);
        }

        // --- marca un ledger como cancelado. ---
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> MarkAsCanceled(Guid id)
        {
            var ledger = await _ledgerService.MarkAsCanceledAsync(id);

            var response = new TransactionLedgerResponse(
                ledger.Id,
                ledger.Timestamp,
                ledger.Type.ToString(),
                ledger.Amount,
                ledger.CurrencyCode.ToString(),
                ledger.Status.ToString(),
                ledger.SourceWalletId,
                ledger.DestinationWalletId,
                ledger.SourceTransactionId,
                ledger.DestinationTransactionId,
                ledger.Metadata
            );

            return Ok(response);
        }

    }
}
