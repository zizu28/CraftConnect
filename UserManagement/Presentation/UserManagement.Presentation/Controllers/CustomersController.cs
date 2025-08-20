using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.CQRS.Commands.CustomerCommands;
using UserManagement.Application.CQRS.Queries.CustomerQueries;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CustomersController(
		IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllCustomersAsync()
		{
			var query = new GetAllCustomersQuery();
			var result = await mediator.Send(query);
			if (result.Any())
			{
				return Ok(result);
			}
			return BadRequest("No customers found.");
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
		{
			var query = new GetCustomerByIdQuery { CustomerId = id};
			var result = await mediator.Send(query);
			if (result != null)
			{
				return Ok(result);
			}
			return NotFound($"Customer with ID {id} not found.");
		}

		[HttpPost("by-address")]
		public async Task<IActionResult> GetCustomerByAddressAsync([FromBody] CustomerAddressDTO customerAddress)
		{
			var query = new GetCustomerByAddressQuery { CustomerAddress = customerAddress };
			var result = await mediator.Send(query);
			if (result != null)
			{
				return Ok(result);
			}
			return NotFound($"Customer with on {customerAddress.Street} street in {customerAddress.City} city not found.");
		}

		[HttpPost]
		public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerCreateDTO customer)
		{
			var command = new CreateCustomerCommand { Customer = customer };
			var result = await mediator.Send(command);
			if (result.IsSuccess)
			{
				return CreatedAtAction(nameof(GetCustomerByIdAsync), new { id = result.CustomerId }, result);
			}
			return BadRequest(result.Errors);
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateCustomerAsync(Guid id, [FromBody] CustomerUpdateDTO customer)
		{
			var command = new UpdateCustomerCommand { CustomerDTO = customer, CustomerID = id };
			var result = await mediator.Send(command);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest(result.Errors);
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteCustomerAsync(Guid id)
		{
			var customer = await GetCustomerByIdAsync(id);
			if(customer is NotFoundResult)
			{
				return NotFound($"Customer with ID {id} not found.");
			}
			var command = new DeleteCustomerCommand { CustomerID = id};
			await mediator.Send(command);
			return NoContent();
		}
	}
}
