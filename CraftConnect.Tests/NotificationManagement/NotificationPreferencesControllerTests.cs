using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationManagement.Application.CQRS.Commands.PreferenceCommands;
using NotificationManagement.Application.CQRS.Queries.PreferenceQueries;
using NotificationManagement.Presentation.Controllers;

namespace CraftConnect.Tests.NotificationManagement;

public class NotificationPreferencesControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly NotificationPreferencesController _controller;

	public NotificationPreferencesControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new NotificationPreferencesController(_mediatorMock.Object);
	}

	#region CreateOrUpdatePreferenceAsync Tests

	[Fact]
	public async Task CreateOrUpdatePreferenceAsync_ReturnsOk_WhenPreferenceIsCreated()
	{
		// Arrange
		var createDto = new NotificationPreferenceCreateDTO
		{
			UserId = Guid.NewGuid(),
			NotificationType = NotificationType.PaymentReceived,
			EmailEnabled = true,
			SmsEnabled = false,
			PushEnabled = true,
			InAppEnabled = true,
			QuietHoursStart = new TimeOnly(22, 0),
			QuietHoursEnd = new TimeOnly(8, 0),
			Frequency = NotificationFrequency.Daily
		};

		var responseDto = new NotificationPreferenceResponseDTO
		{
			Id = Guid.NewGuid(),
			UserId = createDto.UserId,
			NotificationType = createDto.NotificationType,
			EmailEnabled = createDto.EmailEnabled,
			SmsEnabled = createDto.SmsEnabled,
			QuietHoursStart = createDto.QuietHoursStart,
			QuietHoursEnd = createDto.QuietHoursEnd,
			Frequency = createDto.Frequency,
			CreatedAt = DateTime.UtcNow
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrUpdatePreferenceCommand>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.CreateOrUpdatePreferenceAsync(createDto);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<NotificationPreferenceResponseDTO>(okResult.Value);
		Assert.Equal(responseDto.UserId, returnValue.UserId);
		Assert.Equal(responseDto.NotificationType, returnValue.NotificationType);
	}

	[Fact]
	public async Task CreateOrUpdatePreferenceAsync_PassesPreferenceToMediator()
	{
		// Arrange
		var createDto = new NotificationPreferenceCreateDTO
		{
			UserId = Guid.NewGuid(),
			NotificationType = NotificationType.SystemAlert,
			EmailEnabled = false,
			SmsEnabled = false,
			PushEnabled = false,
			InAppEnabled = true
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrUpdatePreferenceCommand>(), default))
			.ReturnsAsync(new NotificationPreferenceResponseDTO { Id = Guid.NewGuid() });

		// Act
		await _controller.CreateOrUpdatePreferenceAsync(createDto);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<CreateOrUpdatePreferenceCommand>(cmd => cmd.Preference == createDto),
			default), Times.Once);
	}

	#endregion

	#region GetUserPreferencesAsync Tests

	[Fact]
	public async Task GetUserPreferencesAsync_ReturnsOk_WithPreferenceList()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var preferences = new List<NotificationPreferenceResponseDTO>
		{
			new() { Id = Guid.NewGuid(), UserId = userId, NotificationType = NotificationType.PaymentReceived },
			new() { Id = Guid.NewGuid(), UserId = userId, NotificationType = NotificationType.SystemAlert }
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserPreferencesQuery>(), default))
			.ReturnsAsync(preferences);

		// Act
		var result = await _controller.GetUserPreferencesAsync(userId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<List<NotificationPreferenceResponseDTO>>(okResult.Value);
		Assert.Equal(2, returnValue.Count);
		Assert.All(returnValue, p => Assert.Equal(userId, p.UserId));
	}

	[Fact]
	public async Task GetUserPreferencesAsync_ReturnsEmptyList_WhenNoPreferencesExist()
	{
		// Arrange
		var userId = Guid.NewGuid();
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserPreferencesQuery>(), default))
			.ReturnsAsync(new List<NotificationPreferenceResponseDTO>());

		// Act
		var result = await _controller.GetUserPreferencesAsync(userId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<List<NotificationPreferenceResponseDTO>>(okResult.Value);
		Assert.Empty(returnValue);
	}

	#endregion

	#region GetPreferenceByTypeAsync Tests

	[Fact]
	public async Task GetPreferenceByTypeAsync_ReturnsOk_WhenPreferenceExists()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var notificationType = NotificationType.BookingConfirmed;
		var responseDto = new NotificationPreferenceResponseDTO
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			NotificationType = notificationType,
			EmailEnabled = true,
			SmsEnabled = true
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetPreferenceByTypeQuery>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.GetPreferenceByTypeAsync(userId, notificationType);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<NotificationPreferenceResponseDTO>(okResult.Value);
		Assert.Equal(userId, returnValue.UserId);
		Assert.Equal(notificationType, returnValue.NotificationType);
	}

	[Fact]
	public async Task GetPreferenceByTypeAsync_ReturnsNotFound_WhenPreferenceDoesNotExist()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var notificationType = NotificationType.PaymentFailed;
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetPreferenceByTypeQuery>(), default))
			.ReturnsAsync((NotificationPreferenceResponseDTO?)null);

		// Act
		var result = await _controller.GetPreferenceByTypeAsync(userId, notificationType);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		Assert.Contains(userId.ToString(), notFoundResult.Value?.ToString());
		Assert.Contains(notificationType.ToString(), notFoundResult.Value?.ToString());
	}

	[Fact]
	public async Task GetPreferenceByTypeAsync_PassesParametersToQuery()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var notificationType = NotificationType.PasswordChanged;
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetPreferenceByTypeQuery>(), default))
			.ReturnsAsync(new NotificationPreferenceResponseDTO { UserId = userId, NotificationType = notificationType });

		// Act
		await _controller.GetPreferenceByTypeAsync(userId, notificationType);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<GetPreferenceByTypeQuery>(q => 
				q.UserId == userId && 
				q.NotificationType == notificationType),
			default), Times.Once);
	}

	#endregion
}
