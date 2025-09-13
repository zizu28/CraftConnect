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

		[HttpGet]
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

		[HttpPost]
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

		[HttpPut("{id:guid}")]
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

		[HttpPut]
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

		[HttpPut]
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

		[HttpPut]
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

		[HttpPut]
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

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeletePaymentAsync(Guid id, string email)
		{
			var command = new DeletePaymentCommand { Id = id, Email =  email };
			await mediator.Send(command);
			return NoContent();
		}
	}
}
