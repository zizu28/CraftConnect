using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;

namespace ProductInventoryManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllCategoriesAsync(CancellationToken cancellationToken)
		{
			var query = new GetAllCategoriesQuery();
			var categories = await mediator.Send(query, cancellationToken);
			return Ok(categories);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			var query = new GetCategoryByIdQuery { Id = id };
			var category = await mediator.Send(query, cancellationToken);
			if (category == null)
			{
				return NotFound();
			}
			return Ok(category);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryCreateCommand command, CancellationToken cancellationToken)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var response = await mediator.Send(command, cancellationToken);
			return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = response.CategoryId }, null);
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateCategoryAsync(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
		{
			if (id != command.CategoryUpdateDTO.CategoryId)
			{
				return BadRequest("ID in URL does not match ID in body.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await mediator.Send(command, cancellationToken);
			if (!result.IsSuccess)
			{
				return NotFound();
			}
			return NoContent();
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var command = new DeleteCategoryCommand { CategoryId = id };
			await mediator.Send(command, cancellationToken);
			return NoContent();
		}
	}
}
