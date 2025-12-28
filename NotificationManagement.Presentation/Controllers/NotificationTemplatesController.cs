using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationManagement.Application.CQRS.Commands.TemplateCommands;
using NotificationManagement.Application.CQRS.Queries.TemplateQueries;

namespace NotificationManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("template-operations")]
public class NotificationTemplatesController(IMediator mediator) : ControllerBase
{
	/// <summary>
	/// Create a new notification template
	/// </summary>
	[HttpPost]
	[ProducesResponseType(typeof(NotificationTemplateResponseDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> CreateTemplateAsync(
		[FromBody] NotificationTemplateCreateDTO template,
		[FromHeader(Name = "X-User-Id")] Guid? createdBy = null)
	{
		var command = new CreateTemplateCommand
		{
			Template = template,
			CreatedBy = createdBy ?? Guid.Empty // Should come from JWT claims in production
		};
		
		var result = await mediator.Send(command);
		return CreatedAtAction(nameof(GetTemplateByCodeAsync), new { code = result.Code }, result);
	}

	/// <summary>
	/// Get template by unique code
	/// </summary>
	[HttpGet("code/{code}")]
	[ProducesResponseType(typeof(NotificationTemplateResponseDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTemplateByCodeAsync(string code)
	{
		var query = new GetTemplateByCodeQuery { Code = code };
		var result = await mediator.Send(query);
		
		if (result == null)
			return NotFound($"Template with code '{code}' not found.");
		
		return Ok(result);
	}

	// TODO: Add more endpoints as needed:
	// - GET /api/NotificationTemplates - Get all active templates
	// - GET /api/NotificationTemplates/{id} - Get template by ID
	// - PUT /api/NotificationTemplates/{id} - Update template
	// - DELETE /api/NotificationTemplates/{id} - Deactivate template
}
