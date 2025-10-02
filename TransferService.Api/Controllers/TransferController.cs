using Microsoft.AspNetCore.Mvc;
using TransferService.Application.Services;

namespace TransferService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        public TransferController(ITransferService transferService) => _transferService = transferService;

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMoneyRequest request)
        {
            var transaction = await _transferService.SendMoneyAsync(request.SenderId, request.ReceiverId, request.Amount, request.Currency);
            return Ok(transaction);
        }

        [HttpPost("receive")]
        public async Task<IActionResult> Receive([FromBody] ReceiveMoneyRequest request)
        {
            var transaction = await _transferService.ReceiveMoneyAsync(request.TransactionCode, request.ReceiverId);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelTransactionRequest request)
        {
            var transaction = await _transferService.CancelTransactionAsync(request.TransactionCode);
            if (transaction == null) return BadRequest();
            return Ok(transaction);
        }

        // DTOs
        public record SendMoneyRequest(Guid SenderId, Guid ReceiverId, decimal Amount, string Currency);
        public record ReceiveMoneyRequest(string TransactionCode, Guid ReceiverId);
        public record CancelTransactionRequest(string TransactionCode);
    }
}
