using Microsoft.AspNetCore.Mvc;
using TransferService.Application.Request;
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
            var transaction = await _transferService.SendMoneyAsync(request);
            return Ok(transaction);
        }

        [HttpPost("receive")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            var transaction = await _transferService.WithdrawAsync(request);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelTransactionRequest request)
        {
            var transaction = await _transferService.CancelTransactionAsync(request);
            if (transaction == null) return BadRequest();
            return Ok(transaction);
        }

        [HttpPut("customerBlocked")]
        public async Task<IActionResult> CustomerBlocked([FromBody] CustomerBlockedRequest request)
        {
            await _transferService.CancelPendingTransfersForBlockedCustomerAsync(request);
            return Ok();
        }
    }
}
