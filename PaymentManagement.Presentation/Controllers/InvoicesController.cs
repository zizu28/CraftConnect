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

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateInvoiceAsync(Guid id, [FromBody] UpdateInvoiceCommand command)
		{
			if (command == null || id != command.InvoiceDTO.InvoiceId)
			{
				return BadRequest("Invalid invoice data.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var updatedInvoice = await mediator.Send(command);
			if (updatedInvoice == null)
			{
				return NotFound();
			}
			return Ok(updatedInvoice);
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteInvoiceAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid invoice ID.");
			}
			var command = new CancelInvoiceCommand { InvoiceId = id };
			await mediator.Send(command);
			return NoContent();
		}
	}
}
