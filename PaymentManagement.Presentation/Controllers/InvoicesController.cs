using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;

namespace PaymentManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InvoicesController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllInvoicesAsync()
		{
			var query = new GetAllInvoicesQuery();
			var invoices = await mediator.Send(query);
			return Ok(invoices);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetInvoiceByIdAsync(Guid id)
		{
			var query = new GetInvoiceByIdQuery { InvoiceId = id };
			var invoice = await mediator.Send(query);
			if (invoice == null)
			{
				return NotFound();
			}
			return Ok(invoice);
		}

		[HttpGet]
		public async Task<IActionResult> GetDaysUntilDueAsync([FromRoute] Guid invoiceId)
		{
			if (invoiceId == Guid.Empty)
			{
				return BadRequest("Invalid invoice ID.");
			}
			var query = new GetDaysUntilDueQuery { InvoiceId = invoiceId };
			var result = await mediator.Send(query);
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetOutstandingAmountAsync([FromRoute] Guid invoiceId)
		{
			if (invoiceId == Guid.Empty)
			{
				return BadRequest("Invalid invoice ID.");
			}
			var query = new GetOutstandingAmountQuery { InvoiceId = invoiceId };
			var result = await mediator.Send(query);
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateInvoiceAsync([FromBody] CreateInvoiceCommand command)
		{
			if (command == null)
			{
				return BadRequest("Invoice data is null.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var createdInvoice = await mediator.Send(command);
			return Ok(createdInvoice);
		}

		[HttpPut]
		public async Task<IActionResult> MarkInvoiceAsPaidAsync([FromRoute] Guid invoiceId)
		{
			if (invoiceId == Guid.Empty)
			{
				return BadRequest("Invalid invoice ID.");
			}
			var command = new MarkAsPaidCommand { InvoiceId = invoiceId };
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> MarkAsOverdueAsync([FromRoute] Guid invoiceId, [FromQuery] string recipientEmail)
		{
			if (invoiceId == Guid.Empty || string.IsNullOrWhiteSpace(recipientEmail))
			{
				return BadRequest("Invalid invoice ID or recipient email.");
			}
			var command = new MarkAsOverDueCommand { InvoiceId = invoiceId, RecipientEmail = recipientEmail };
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> ApplyDiscountAsync([FromRoute] Guid invoiceId, [FromQuery] decimal discountAmount, [FromQuery] string currency)
		{
			if (invoiceId == Guid.Empty || discountAmount <= 0 || string.IsNullOrWhiteSpace(currency))
			{
				return BadRequest("Invalid input parameters.");
			}
			var command = new ApplyDiscountCommand
			{
				InvoiceId = invoiceId,
				DiscountAmount = discountAmount,
				Currency = currency
			};
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> AddLineItemAsync([FromRoute] Guid invoiceId, [FromBody] AddLineItemCommand command)
		{
			if (invoiceId == Guid.Empty || command == null || invoiceId != command.InvoiceId)
			{
				return BadRequest("Invalid input parameters.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> RemoveLineItemAsync([FromRoute] Guid invoiceId, [FromQuery] Guid lineItemId)
		{
			if (invoiceId == Guid.Empty || lineItemId == Guid.Empty)
			{
				return BadRequest("Invalid input parameters.");
			}
			var command = new RemoveLineItemCommand
			{
				InvoiceId = invoiceId,
				LineItemId = lineItemId
			};
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> UpdateLineItemAsync([FromRoute] Guid invoiceId, [FromBody] UpdateLineItemCommand command)
		{
			if (invoiceId == Guid.Empty || command == null || invoiceId != command.InvoiceId)
			{
				return BadRequest("Invalid input parameters.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> UpdateDueDateAsync([FromRoute] Guid invoiceId, [FromBody] DateTime newDueDate)
		{
			if (invoiceId == Guid.Empty || newDueDate == default)
			{
				return BadRequest("Invalid input parameters.");
			}
			var command = new UpdateDueDateCommand { InvoiceId = invoiceId, UpdatedDueDate = newDueDate };
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> SendInvoiceAsync([FromRoute] Guid invoiceId, [FromQuery] string recipientEmail)
		{
			if(invoiceId == Guid.Empty || string.IsNullOrEmpty(recipientEmail))
			{
				return BadRequest("Invalid input parameters");
			}
			var command = new SendInvoiceCommand { InvoiceId = invoiceId, RecipientEmail = recipientEmail };
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPut]
		public async Task<IActionResult> CancelInvoiceAsync(Guid id, [FromBody] string reason)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid invoice ID.");
			}
			var command = new CancelInvoiceCommand { InvoiceId = id, Reason = reason };
			await mediator.Send(command);
			return NoContent();
		}
	}
}
