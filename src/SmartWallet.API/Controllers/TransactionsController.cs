using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Contracts.Requests;
using SmartWallet.Domain.Entities;

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

        // --- creacion de transacciones ---

        // --- crear deposito ---
        [HttpPost("deposits")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var transaction = await _transactionService.DepositAsync(
                request.WalletId,
                request.Amount,
                request.CurrencyCode
            );

            var response = MapToResponse(transaction);
            return Ok(response);
        }

        // --- crear retiro ---
        [HttpPost("withdrawals")]
        public async Task<IActionResult> WithdrawalRequest([FromBody] WithdrawalRequest request)
        {
            var transaction = await _transactionService.WithdrawAsync(
                request.WalletId,
                request.Amount,
                request.CurrencyCode
            );

            var response = MapToResponse(transaction);
            return Ok(response);
        }

        // --- crear transferencia ---
        [HttpPost("transfers")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var transaction = await _transactionService.TransferAsync(
                request.SourceWalletId,
                request.DestinationWalletId,
                request.Amount,
                request.CurrencyCode
            );

            var response = MapToResponse(transaction);
            return Ok(response);
        }

        // --- consultas ---
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null) return NotFound();

            var response = MapToResponse(transaction);
            return Ok(response);
        }

        [HttpGet("wallet/{walletId:guid}")]
        public async Task<IActionResult> GetByWallet(Guid walletId)
        {
            var transactions = await _transactionService.GetByWalletAsync(walletId);
            var responses = transactions.Select(MapToResponse);
            return Ok(responses);
        }

        // --- maper privado ---
        private static TransactionResponse MapToResponse(Transaction transaction)
        {
            return new TransactionResponse(
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
}

