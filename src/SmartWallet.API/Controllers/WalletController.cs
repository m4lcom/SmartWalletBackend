using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWallet.Application.Services;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<WalletResponse>>> GetAll()
        {
            var wallets = await _service.GetAllAsync();
            var responses = wallets.Select(w => new WalletResponse
            {
                Id = w.Id,
                UserId = w.UserID,
                UserName = w.User?.Name ?? string.Empty,
                UserEmail = w.User?.Email ?? string.Empty,
                Name = w.Name,
                CurrencyCode = w.CurrencyCode.ToString(),
                Alias = w.Alias,
                Balance = w.Balance,
                CreatedAt = w.CreatedAt
            }).ToList();

            return Ok(responses);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<List<WalletResponse>>> GetByUser(Guid userId)
        {
            var wallets = await _service.GetByUserIdAsync(userId);
            if (wallets == null || !wallets.Any()) return NotFound();

            var responses = wallets.Select(w => new WalletResponse
            {
                Id = w.Id,
                UserId = w.UserID,
                UserName = w.User?.Name ?? string.Empty,
                UserEmail = w.User?.Email ?? string.Empty,
                Name = w.Name,
                CurrencyCode = w.CurrencyCode.ToString(),
                Alias = w.Alias,
                Balance = w.Balance,
                CreatedAt = w.CreatedAt
            }).ToList();

            return Ok(responses);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpGet("by-alias/{alias}")]
        public async Task<ActionResult<WalletResponse>> GetByAlias(string alias)
        {
            var wallet = await _service.GetByAliasAsync(alias);
            if (wallet == null) return NotFound();

            var response = new WalletResponse
            {
                Id = wallet.Id,
                UserId = wallet.UserID,
                UserName = wallet.User?.Name ?? string.Empty,
                UserEmail = wallet.User?.Email ?? string.Empty,
                Name = wallet.Name,
                CurrencyCode = wallet.CurrencyCode.ToString(),
                Alias = wallet.Alias,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt
            };

            return Ok(response);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetById(Guid id)
        {
            var wallet = await _service.GetByIdAsync(id);
            if (wallet == null) return NotFound();
            return Ok(wallet);
        }

        [Authorize(Policy = "SameUserOrAdmin")]
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
                UserName = wallet.User?.Name ?? string.Empty,
                UserEmail = wallet.User?.Email ?? string.Empty,
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
