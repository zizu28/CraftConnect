using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;

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
	}
}
