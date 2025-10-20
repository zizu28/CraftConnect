using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;

namespace PaymentManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllPaymentsAsync()
		{
			var query = new GetAllPaymentsQuery();
			var payments = await mediator.Send(query);
			return Ok(payments);
		}
		
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetPaymentByIdAsync(Guid id)
		{
			var query = new GetPaymentByIdQuery { PaymentId = id };
			var payment = await mediator.Send(query);
			if (payment == null)
			{
				return NotFound();
			}
			return Ok(payment);
		}

		[HttpGet("transactions")]
		public async Task<IActionResult> GetAllPaymentTransactionsAsync([FromQuery] Guid? paymentId = null)
		{
			var query = new GetAllPaymentTransactionsQuery { PaymentId = paymentId ?? Guid.Empty };
			var transactions = await mediator.Send(query);
			return Ok(transactions);
		}

		[HttpGet("{id:guid}/transactions")]
		public async Task<IActionResult> GetPaymentTransactionsByPaymentIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid payment ID.");
			}
			
			var query = new GetAllPaymentTransactionsQuery { PaymentId = id };
			var transactions = await mediator.Send(query);
			return Ok(transactions);
		}

		[HttpGet("availabe-refund-amount")]
		public async Task<IActionResult> GetAvailableRefundAmountAsync([FromRoute] Guid paymentId)
		{
			if (paymentId == Guid.Empty)
			{
				return BadRequest("Invalid payment ID.");
			}
			var query = new GetAvailableRefundAmountQuery { PaymentId = paymentId };
			var amount = await mediator.Send(query);
			return Ok(amount);
		}

		[HttpGet("verify-payment")]
		public async Task<IActionResult> VerifyPaymentAsync([FromBody] VerifyPaymentCommand command)
		{
			if(command == null)
			{
				return BadRequest("Command data is null.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await mediator.Send(command);
			var (message, isValid) = result;
			if (!isValid)
			{
				return BadRequest(message);
			}
			return Ok(result);
		}

		[HttpPost("create-payment")]
		public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentCommand command)
		{
			if (command == null)
			{
				return BadRequest("Payment data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var createdPayment = await mediator.Send(command);
			return Ok(createdPayment);
		}

		[HttpPut("update-payment/{id:guid}")]
		public async Task<IActionResult> UpdatePaymentAsync(Guid id, [FromBody] UpdatePaymentCommand command)
		{
			if (command == null || id != command.PaymentDTO.PaymentId)
			{
				return BadRequest("Invalid payment data.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var updatedPayment = await mediator.Send(command);
			if (updatedPayment == null)
			{
				return NotFound();
			}
			return Ok(updatedPayment);
		}

		[HttpPut("authorize-payment")]
		public async Task<IActionResult> AuthorizePaymentAsync([FromBody] AuthorizePaymentCommand command)
		{
			if (command == null)
			{
				return BadRequest("Command data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut("complete-payment")]
		public async Task<IActionResult> CompletePaymentAsync([FromBody] CompletePaymentCommand command)
		{
			if (command == null)
			{
				return BadRequest("Command data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut("fail-payment")]
		public async Task<IActionResult> FailPaymentAsync([FromBody] FailedPaymentCommand command)
		{
			if (command == null)
			{
				return BadRequest("Command data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut("refund-payment")]
		public async Task<IActionResult> RefundPaymentAsync([FromBody] RefundPaymentCommand command)
		{
			if (command == null)
			{
				return BadRequest("Command data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpDelete("delete-payment/{id:guid}")]
		public async Task<IActionResult> DeletePaymentAsync(Guid id, string email)
		{
			var command = new DeletePaymentCommand { Id = id, Email =  email };
			await mediator.Send(command);
			return NoContent();
		}
	}
}
