using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Domain.Entities;

namespace UserManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CraftmenController(
		IMediator mediator,
		IFileStorageService fileStorageService) : ControllerBase
	{
		[HttpGet("get-craftsmen")]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllCraftsmen()
		{
			var query = new GetAllCraftmenQuery();
			var result = await mediator.Send(query);
			return Ok(result);
		}

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

		[HttpPut("update-craftsman/{id:guid}")]
		[Consumes("application/json")]
		public async Task<IActionResult> UpdateCraftsmanAsync([FromRoute] Guid id, CraftsmanProfileUpdateDTO craftman)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
								?? User.FindFirst("sub")
								?? User.FindFirst("id");
			if (userIdClaim == null) return Unauthorized();

			var tokenUserId = Guid.Parse(userIdClaim.Value);
			if (tokenUserId != id)
			{
				return Forbid();
			}

			if (craftman == null)
			{
				return BadRequest();
			}
			var command = new UpdateCraftmanCommand { CraftmanDTO = craftman, CraftmanId = id };
			var response = await mediator.Send(command);
			if (!response.IsSuccessful)
			{
				return BadRequest($"Invalid inputs: Errors: {response.Errors}");
			}
			return Ok();
		}

		[HttpPost("/verify-craftman-status")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> VerifyCraftmanStatusAsync([FromBody] VerifyCraftmanCommand command)
		{
			if(command == null)
			{
				return BadRequest();
			}
			await mediator.Send(command);
			return Ok("Craftman status verified successfully.");
		}

		[HttpPost("upload-avatar")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UploadAvatar(IFormFile file)
		{
			if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
			if (!file.ContentType.StartsWith("image/")) return BadRequest("Invalid file type.");
			using var stream = file.OpenReadStream();
			var url = await fileStorageService.UploadFileAsync(stream, file.FileName, file.ContentType);
			return Ok(url);
		}
	}
}
