using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;
using NotificationManagement.Application.CQRS.Queries.NotificationQueries;
using NotificationManagement.Presentation.Controllers;

namespace CraftConnect.Tests.NotificationManagement;

public class NotificationsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly NotificationsController _controller;

	public NotificationsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new NotificationsController(_mediatorMock.Object);
	}

	#region SendNotificationAsync Tests

	[Fact]
	public async Task SendNotificationAsync_ReturnsCreated_WhenNotificationIsSent()
	{
		// Arrange
		var createDto = new NotificationCreateDTO
		{
			RecipientId = Guid.NewGuid(),
			RecipientEmail = "test@example.com",
			Type = NotificationType.Welcome,
			Channel = NotificationChannel.Email,
			Subject = "Test Subject",
			Body = "Test Body",
			Priority = NotificationPriority.Normal
		};

		var responseDto = new NotificationResponseDTO
		{
			Id = Guid.NewGuid(),
			RecipientId = createDto.RecipientId,
			RecipientEmail = createDto.RecipientEmail,
			Type = createDto.Type,
			Channel = createDto.Channel,
			Status = NotificationStatus.Sent,
			Subject = createDto.Subject,
			Body = createDto.Body,
			CreatedAt = DateTime.UtcNow
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<SendNotificationCommand>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.SendNotificationAsync(createDto);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result);
		Assert.Equal(201, createdResult.StatusCode);
		var returnValue = Assert.IsType<NotificationResponseDTO>(createdResult.Value);
		Assert.Equal(responseDto.Id, returnValue.Id);
		Assert.Equal(responseDto.RecipientEmail, returnValue.RecipientEmail);
	}

	[Fact]
	public async Task SendNotificationAsync_CallsMediatorWithCorrectCommand()
	{
		// Arrange
		var createDto = new NotificationCreateDTO
		{
			RecipientId = Guid.NewGuid(),
			RecipientEmail = "test@example.com",
			Type = NotificationType.BookingCreated,
			Channel = NotificationChannel.Email,
			Subject = "Booking Confirmed",
			Body = "Your booking is confirmed"
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<SendNotificationCommand>(), default))
			.ReturnsAsync(new NotificationResponseDTO { Id = Guid.NewGuid() });

		// Act
		await _controller.SendNotificationAsync(createDto);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<SendNotificationCommand>(cmd => cmd.Notification == createDto),
			default), Times.Once);
	}

	#endregion

	#region GetNotificationByIdAsync Tests

	[Fact]
	public async Task GetNotificationByIdAsync_ReturnsOk_WhenNotificationExists()
	{
		// Arrange
		var notificationId = Guid.NewGuid();
		var responseDto = new NotificationResponseDTO
		{
			Id = notificationId,
			RecipientEmail = "test@example.com",
			Status = NotificationStatus.Sent,
			Subject = "Test",
			Body = "Test body"
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.GetNotificationByIdAsync(notificationId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<NotificationResponseDTO>(okResult.Value);
		Assert.Equal(notificationId, returnValue.Id);
	}

	[Fact]
	public async Task GetNotificationByIdAsync_ReturnsNotFound_WhenNotificationDoesNotExist()
	{
		// Arrange
		var notificationId = Guid.NewGuid();
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), default))
			.ReturnsAsync((NotificationResponseDTO?)null);

		// Act
		var result = await _controller.GetNotificationByIdAsync(notificationId);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		Assert.Contains(notificationId.ToString(), notFoundResult.Value?.ToString());
	}

	#endregion

	#region GetUserNotificationsAsync Tests

	[Fact]
	public async Task GetUserNotificationsAsync_ReturnsOk_WithNotificationList()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var notifications = new List<NotificationResponseDTO>
		{
			new() { Id = Guid.NewGuid(), RecipientId = userId, Subject = "Test 1" },
			new() { Id = Guid.NewGuid(), RecipientId = userId, Subject = "Test 2" }
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserNotificationsQuery>(), default))
			.ReturnsAsync(notifications);

		// Act
		var result = await _controller.GetUserNotificationsAsync(userId, 1, 20);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<List<NotificationResponseDTO>>(okResult.Value);
		Assert.Equal(2, returnValue.Count);
		Assert.All(returnValue, n => Assert.Equal(userId, n.RecipientId));
	}

	[Fact]
	public async Task GetUserNotificationsAsync_UsesPaginationParameters()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var pageNumber = 2;
		var pageSize = 10;

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserNotificationsQuery>(), default))
			.ReturnsAsync(new List<NotificationResponseDTO>());

		// Act
		await _controller.GetUserNotificationsAsync(userId, pageNumber, pageSize);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<GetUserNotificationsQuery>(q => 
				q.UserId == userId && 
				q.PageNumber == pageNumber && 
				q.PageSize == pageSize),
			default), Times.Once);
	}

	#endregion

	#region CancelNotificationAsync Tests

	[Fact]
	public async Task CancelNotificationAsync_ReturnsNoContent_WhenCancellationSucceeds()
	{
		// Arrange
		var notificationId = Guid.NewGuid();
		var reason = "User requested cancellation";

		_mediatorMock.Setup(m => m.Send(It.IsAny<CancelNotificationCommand>(), default))
			.ReturnsAsync(Unit.Value);

		// Act
		var result = await _controller.CancelNotificationAsync(notificationId, reason);

		// Assert
		Assert.IsType<NoContentResult>(result);
	}

	[Fact]
	public async Task CancelNotificationAsync_UsesDefaultReason_WhenReasonIsNull()
	{
		// Arrange
		var notificationId = Guid.NewGuid();

		_mediatorMock.Setup(m => m.Send(It.IsAny<CancelNotificationCommand>(), default))
			.ReturnsAsync(Unit.Value);

		// Act
		await _controller.CancelNotificationAsync(notificationId, null);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<CancelNotificationCommand>(cmd => 
				cmd.NotificationId == notificationId && 
				cmd.Reason == "Cancelled by user"),
			default), Times.Once);
	}

	#endregion
}
