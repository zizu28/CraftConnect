using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;

namespace ProductInventoryManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
		{
			var query = new GetAllProductsQuery();
			var response = await mediator.Send(query, cancellationToken);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetProductsByCategoryAsync([FromQuery] string category, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(category))
			{
				return BadRequest("Category parameter is required.");
			}
			var query = new GetProductByCategoryQuery { Category = category };
			var response = await mediator.Send(query, cancellationToken);
			return Ok(response);
		}


		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var query = new GetProductByIdQuery { ProductId = id };
			var response = await mediator.Send(query, cancellationToken);
			if (!response.IsSuccess)
			{
				return NotFound(response);
			}
			return Ok(response);
		}
		[HttpPost]
		public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
		{
			var response = await mediator.Send(command, cancellationToken);
			return CreatedAtAction(nameof(GetProductById), new { id = response.ProductId }, response);
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
		{
			if (id != command.ProductDTO.ProductId)
			{
				return BadRequest("ID in URL does not match ID in body.");
			}
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var response = await mediator.Send(command, cancellationToken);
			if (!response.IsSuccess)
			{
				return NotFound(response);
			}
			return Ok(response);
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
		{
			if (id == Guid.Empty)
			{
				return BadRequest("Invalid product ID.");
			}
			await mediator.Send(new DeleteProductCommand { ProductId = id }, cancellationToken);
			return NoContent();
		}
	}
}
