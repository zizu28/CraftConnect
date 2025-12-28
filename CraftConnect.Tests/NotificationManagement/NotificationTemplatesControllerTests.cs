using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationManagement.Application.CQRS.Commands.TemplateCommands;
using NotificationManagement.Application.CQRS.Queries.TemplateQueries;
using NotificationManagement.Presentation.Controllers;

namespace CraftConnect.Tests.NotificationManagement;

public class NotificationTemplatesControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly NotificationTemplatesController _controller;

	public NotificationTemplatesControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new NotificationTemplatesController(_mediatorMock.Object);
	}

	#region CreateTemplateAsync Tests

	[Fact]
	public async Task CreateTemplateAsync_ReturnsCreated_WhenTemplateIsCreated()
	{
		// Arrange
		var createdBy = Guid.NewGuid();
		var createDto = new NotificationTemplateCreateDTO
		{
			Name = "Welcome Email",
			Code = "WELCOME_EMAIL",
			Type = NotificationType.Welcome,
			Channel = NotificationChannel.Email,
			SubjectTemplate = "Welcome {{userName}}!",
			BodyTemplate = "Hi {{userName}}, welcome to our platform!",
			RequiredVariables = new List<string> { "userName" }
		};

		var responseDto = new NotificationTemplateResponseDTO
		{
			Id = Guid.NewGuid(),
			Name = createDto.Name,
			Code = createDto.Code,
			Type = createDto.Type,
			Channel = createDto.Channel,
			IsActive = true,
			Version = 1,
			CreatedAt = DateTime.UtcNow
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<CreateTemplateCommand>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.CreateTemplateAsync(createDto, createdBy);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result);
		Assert.Equal(201, createdResult.StatusCode);
		var returnValue = Assert.IsType<NotificationTemplateResponseDTO>(createdResult.Value);
		Assert.Equal(responseDto.Code, returnValue.Code);
		Assert.Equal(responseDto.Name, returnValue.Name);
	}

	[Fact]
	public async Task CreateTemplateAsync_UsesEmptyGuid_WhenCreatedByIsNull()
	{
		// Arrange
		var createDto = new NotificationTemplateCreateDTO
		{
			Name = "Test Template",
			Code = "TEST",
			Type = NotificationType.PasswordReset,
			Channel = NotificationChannel.Email,
			SubjectTemplate = "Subject",
			BodyTemplate = "Body",
			RequiredVariables = []
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<CreateTemplateCommand>(), default))
			.ReturnsAsync(new NotificationTemplateResponseDTO { Id = Guid.NewGuid(), Code = "TEST" });

		// Act
		await _controller.CreateTemplateAsync(createDto, null);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<CreateTemplateCommand>(cmd => cmd.CreatedBy == Guid.Empty),
			default), Times.Once);
	}

	[Fact]
	public async Task CreateTemplateAsync_PassesTemplateToMediator()
	{
		// Arrange
		var createdBy = Guid.NewGuid();
		var createDto = new NotificationTemplateCreateDTO
		{
			Name = "Password Reset",
			Code = "PASSWORD_RESET",
			Type = NotificationType.EmailVerification,
			Channel = NotificationChannel.Email,
			SubjectTemplate = "Reset your password",
			BodyTemplate = "Click here: {{resetLink}}",
			RequiredVariables = new List<string> { "resetLink" }
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<CreateTemplateCommand>(), default))
			.ReturnsAsync(new NotificationTemplateResponseDTO { Id = Guid.NewGuid(), Code = "PASSWORD_RESET" });

		// Act
		await _controller.CreateTemplateAsync(createDto, createdBy);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<CreateTemplateCommand>(cmd => 
				cmd.Template == createDto && 
				cmd.CreatedBy == createdBy),
			default), Times.Once);
	}

	#endregion

	#region GetTemplateByCodeAsync Tests

	[Fact]
	public async Task GetTemplateByCodeAsync_ReturnsOk_WhenTemplateExists()
	{
		// Arrange
		var code = "WELCOME_EMAIL";
		var responseDto = new NotificationTemplateResponseDTO
		{
			Id = Guid.NewGuid(),
			Code = code,
			Name = "Welcome Email",
			IsActive = true,
			Version = 1
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetTemplateByCodeQuery>(), default))
			.ReturnsAsync(responseDto);

		// Act
		var result = await _controller.GetTemplateByCodeAsync(code);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<NotificationTemplateResponseDTO>(okResult.Value);
		Assert.Equal(code, returnValue.Code);
	}

	[Fact]
	public async Task GetTemplateByCodeAsync_ReturnsNotFound_WhenTemplateDoesNotExist()
	{
		// Arrange
		var code = "NONEXISTENT";
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetTemplateByCodeQuery>(), default))
			.ReturnsAsync((NotificationTemplateResponseDTO?)null);

		// Act
		var result = await _controller.GetTemplateByCodeAsync(code);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		Assert.Contains(code, notFoundResult.Value?.ToString());
	}

	[Fact]
	public async Task GetTemplateByCodeAsync_PassesCodeToQuery()
	{
		// Arrange
		var code = "TEST_CODE";
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetTemplateByCodeQuery>(), default))
			.ReturnsAsync(new NotificationTemplateResponseDTO { Code = code });

		// Act
		await _controller.GetTemplateByCodeAsync(code);

		// Assert
		_mediatorMock.Verify(m => m.Send(
			It.Is<GetTemplateByCodeQuery>(q => q.Code == code),
			default), Times.Once);
	}

	#endregion
}
