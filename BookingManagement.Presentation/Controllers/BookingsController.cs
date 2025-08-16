using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.CQRS.Commands.JobDetailsCommands;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BookingsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllBookingAsync()
		{
			var query = new GetAllBookingQuery();
			var result = await mediator.Send(query);

			if (result == null || !result.Any())
			{
				return NotFound();
			}

			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetBookingByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid booking ID.");
			}
			var query = new GetBookingByIdQuery { Id = id };
			var result = await mediator.Send(query);
			if (result == null)
			{
				return NotFound($"Booking with ID {id} not found.");
			}
			return Ok(result);
		}

		[HttpGet("details/{details}")]
		public async Task<IActionResult> GetBookingByDetailsAsync([FromRoute] string details)
		{
			if (string.IsNullOrWhiteSpace(details))
			{
				return BadRequest("Details parameter cannot be empty.");
			}
			var query = new GetBookingByDetailsQuery { Description = details };
			var result = await mediator.Send(query);
			if (result == null)
			{
				return NotFound($"Booking with details '{details}' not found.");
			}
			return Ok(result);
		}

		[HttpPost("create-booking")]
		public async Task<IActionResult> CreateBookingAsync([FromBody] CreateBookingCommand command)
		{
			if (command == null)
			{
				return BadRequest("Booking data cannot be null.");
			}
			var result = await mediator.Send(command);
			if (result == null)
			{
				return BadRequest("Failed to create booking.");
			}
			return Ok(result);
		}

		[HttpPut("update-booking")]
		public async Task<IActionResult> UpdateBookingAsync([FromBody] UpdateBookingCommand command)
		{
			if (command == null || command.BookingId == Guid.Empty)
			{
				return BadRequest("Invalid booking data.");
			}
			var result = await mediator.Send(command);
			if (result == null)
			{
				return NotFound($"Booking with ID {command.BookingId} not found.");
			}
			return Ok(result);
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteBookingAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid booking ID.");
			}
			var command = new DeleteBookingCommand { BookingId = id };
			await mediator.Send(command);
			return NoContent();
		}

		[HttpPost("complete-booking")]
		public async Task<IActionResult> CompleteBookingAsync([FromBody] CompleteBookingCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Validation Errors");
			}
			await mediator.Send(command);
			return Ok($"Booking with ID {command.BookingId} completed.");
		}

		[HttpPost("confirm-booking")]
		public async Task<IActionResult> ConfirmBookingAsync([FromBody] ConfirmBookingCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Validation Errors");
			}
			await mediator.Send(command);
			return Ok($"Booking with ID {command.BookingId} confirmed.");
		}

		[HttpPost("{id:guid}/add-line-item")]
		public async Task<IActionResult> AddLineItemToBookingAsync(Guid id, [FromBody] BookingLineItemCreateCommand command)
		{
			if (id == Guid.Empty || command == null)
			{
				return BadRequest("Invalid booking ID or line item data.");
			}
			command.BookingId = id;
			var result = await mediator.Send(command);
			if (result == null)
			{
				return NotFound($"Booking with ID {id} not found.");
			}
			return Ok(result);
		}
	}
}