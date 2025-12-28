using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;
using NotificationManagement.Application.CQRS.Queries.NotificationQueries;

namespace NotificationManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
	/// <summary>
	/// Send a new notification
	/// </summary>
	[HttpPost]
	[EnableRateLimiting("send-notification")]
	[ProducesResponseType(typeof(NotificationResponseDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> SendNotificationAsync([FromBody] NotificationCreateDTO notification)
	{
		var command = new SendNotificationCommand { Notification = notification };
		var result = await mediator.Send(command);
		return CreatedAtAction(nameof(GetNotificationByIdAsync), new { id = result.Id }, result);
	}

	/// <summary>
	/// Get notification by ID
	/// </summary>
	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(NotificationResponseDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetNotificationByIdAsync(Guid id)
	{
		var query = new GetNotificationByIdQuery { NotificationId = id };
		var result = await mediator.Send(query);
		
		if (result == null)
			return NotFound($"Notification with ID {id} not found.");
		
		return Ok(result);
	}

	/// <summary>
	/// Get notifications for a specific user
	/// </summary>
	[HttpGet("user/{userId:guid}")]
	[ProducesResponseType(typeof(List<NotificationResponseDTO>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetUserNotificationsAsync(
		Guid userId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 20)
	{
		var query = new GetUserNotificationsQuery
		{
			UserId = userId,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
		
		var result = await mediator.Send(query);
		return Ok(result);
	}

	/// <summary>
	/// Cancel a pending notification
	/// </summary>
	[HttpPost("{id:guid}/cancel")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> CancelNotificationAsync(Guid id, [FromBody] string? reason = null)
	{
		var command = new CancelNotificationCommand
		{
			NotificationId = id,
			Reason = reason ?? "Cancelled by user"
		};
		
		await mediator.Send(command);
		return NoContent();
	}
}
