using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationManagement.Application.CQRS.Commands.PreferenceCommands;
using NotificationManagement.Application.CQRS.Queries.PreferenceQueries;

namespace NotificationManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("preference-operations")]
public class NotificationPreferencesController(IMediator mediator) : ControllerBase
{
	/// <summary>
	/// Create or update user notification preferences
	/// </summary>
	[HttpPost]
	[ProducesResponseType(typeof(NotificationPreferenceResponseDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> CreateOrUpdatePreferenceAsync([FromBody] NotificationPreferenceCreateDTO preference)
	{
		var command = new CreateOrUpdatePreferenceCommand { Preference = preference };
		var result = await mediator.Send(command);
		return Ok(result);
	}

	/// <summary>
	/// Get all notification preferences for a user
	/// </summary>
	[HttpGet("user/{userId:guid}")]
	[ProducesResponseType(typeof(List<NotificationPreferenceResponseDTO>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetUserPreferencesAsync(Guid userId)
	{
		var query = new GetUserPreferencesQuery { UserId = userId };
		var result = await mediator.Send(query);
		return Ok(result);
	}

	/// <summary>
	/// Get user preference for a specific notification type
	/// </summary>
	[HttpGet("user/{userId:guid}/type/{notificationType}")]
	[ProducesResponseType(typeof(NotificationPreferenceResponseDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPreferenceByTypeAsync(Guid userId, NotificationType notificationType)
	{
		var query = new GetPreferenceByTypeQuery
		{
			UserId = userId,
			NotificationType = notificationType
		};
		
		var result = await mediator.Send(query);
		
		if (result == null)
			return NotFound($"Preference for user {userId} and type {notificationType} not found.");
		
		return Ok(result);
	}

	// TODO: Add more endpoints as needed:
	// - PUT /api/NotificationPreferences/{id} - Update specific preference
	// - DELETE /api/NotificationPreferences/{id} - Reset to defaults
}
