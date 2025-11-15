using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.CQRS.Commands;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CraftmenController(IMediator mediator) : ControllerBase
	{
		[HttpPost("add-skill")]
		public async Task<IActionResult> AddSkill([FromBody] AddSkillCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await mediator.Send(command);
			if(result == Unit.Value)
			{
				return Ok($"New skill added to craftman with ID {command.CraftmanId}");
			}
			return StatusCode(500, "An error occurred while adding the skill.");
		}

		[HttpPost("update-craftman/{Id:guid}")]
		public async Task<IActionResult> UpdateCraftsmanAsync([FromBody] CraftsmanProfileUpdateDTO craftman, 
			[FromRoute] Guid id)
		{
			if(craftman == null)
			{
				return BadRequest();
			}
			var command = new UpdateCraftmanCommand { CraftmanDTO = craftman, CraftmanId = id };
			var response = await mediator.Send(command);
			if (!response.IsSuccessful)
			{
				return BadRequest("Invalid inputs");
			}
			return Ok(response);
		}

		[HttpPost("/verify-craftman-status")]
		public async Task<IActionResult> VerifyCraftmanStatusAsync([FromBody] VerifyCraftmanCommand command)
		{
			if(command == null)
			{
				return BadRequest();
			}
			await mediator.Send(command);
			return Ok("Craftman status verified successfully.");
		}
	}
}
