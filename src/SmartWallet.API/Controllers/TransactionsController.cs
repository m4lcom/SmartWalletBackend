using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Contracts.Requests;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // --- consultas ---

        // --- obtiene una transaccion por id ---
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null) return NotFound();
            return Ok(MapToResponse(transaction));
        }

        // --- obtiene todas las transacciones de una wallet especifica ---
        [HttpGet("wallet/{walletId:guid}")]
        public async Task<IActionResult> GetByWallet(Guid walletId)
        {
            var transactions = await _transactionService.GetByWalletAsync(walletId);
            return Ok(transactions.Select(MapToResponse));
        }

        // --- obtiene todas las transacciones dentro de un rango de fechas ---
        [HttpGet("range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var transactions = await _transactionService.GetByDateRangeAsync(from, to);
            return Ok(transactions.Select(MapToResponse));
        }
            
        // --- operaciones de dominio ---

        // --- crear deposito ---
        [HttpPost("deposits")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var currency = Enum.Parse<CurrencyCode>(request.CurrencyCode, ignoreCase: true);
            var transaction = await _transactionService.CreateDepositAsync(
                request.WalletId,
                request.Amount,
                currency
            );
            return Ok(MapToResponse(transaction));
        }

        // --- crear retiro ---
        [HttpPost("withdrawals")]
        public async Task<IActionResult> Withdrawal([FromBody] WithdrawalRequest request)
        {
            var currency = Enum.Parse<CurrencyCode>(request.CurrencyCode, ignoreCase: true);
            var transaction = await _transactionService.CreateWithdrawalAsync(
                request.WalletId,
                request.Amount,
                currency
            );
            return Ok(MapToResponse(transaction));
        }

        // --- crear transferencia ---
        [HttpPost("transfers")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var currency = Enum.Parse<CurrencyCode>(request.CurrencyCode, ignoreCase: true);
            var transaction = await _transactionService.CreateTransferAsync(
                request.SourceWalletId,
                request.DestinationWalletId,
                request.Amount,
                currency
            );
            return Ok(MapToResponse(transaction));
        }
        // --- marca una transaccion como fallida ---
        [HttpPatch("{id:guid}/fail")]
        public async Task<IActionResult> MarkAsFailed(Guid id)
        {
            var transaction = await _transactionService.MarkAsFailedAsync(id);
            return Ok(MapToResponse(transaction));  
        }

        // --- marca una transaccion como cancelada ---
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> MarkAsCanceled(Guid id)
        {
            var transaction = await _transactionService.MarkAsCanceledAsync(id);
            return Ok(MapToResponse(transaction));
        }

        // --- mapper privado ---
        private static TransactionResponse MapToResponse(Transaction transaction) => new TransactionResponse(
                transaction.Id,
                transaction.CreatedAt,
                transaction.Type.ToString(),
                transaction.Amount,
                transaction.CurrencyCode.ToString(),
                transaction.Status.ToString(),
                transaction.WalletId,
                transaction.DestinationWalletId
            );
    }
}

