using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Presentation.Controllers;

namespace CraftConnect.Tests
{
	public class UserControllerTest
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly UsersController _usersController;

		public UserControllerTest()
		{
			_mediatorMock = new Mock<IMediator>();
			_usersController = new UsersController(_mediatorMock.Object);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsOk_WhenLoginSucceeds()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var loginCommand = new LoginUserCommand
			{
				Username = "testuser",
				Password = "testpassword"
			};
			var accessToken = "access_token_mock";
			var refreshToken = "refresh_token_mock";
			_mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
				.ReturnsAsync((accessToken, refreshToken));

			// Act
			var result = await _usersController.LoginUserAsync(loginCommand);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			dynamic returnedTokens = okResult.Value;
			Assert.Equal(accessToken, returnedTokens.Item1);
			Assert.Equal(refreshToken, returnedTokens.Item2);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsUnauthorized_WhenLoginFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var loginCommand = new LoginUserCommand
			{
				Username = "wronguser",
				Password = "wrongpassword"
			};
			_mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
				.ReturnsAsync((string.Empty, string.Empty));

			// Act
			var result = await _usersController.LoginUserAsync(loginCommand);

			// Assert
			Assert.IsType<UnauthorizedObjectResult>(result);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Username", "Username is required");

			// Act
			var result = await _usersController.LoginUserAsync(new LoginUserCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsOkResult_WhenUserIsRegistered()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var registerCommand = new UserCreateDTO() { 
				Email = "example@gmail.com",
				Password = "password123",
				ConfirmPassword = "password123",
				Role = "User",
				AgreeToTerms = true,
			};
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
				.ReturnsAsync(new UserResponseDTO());

			// Act
			var result = await _usersController.RegisterNewUserAsync(registerCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_WhenRegistrationFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var registerCommand = new UserCreateDTO()
			{
				Email = "test@gmail.com",
				Password = "password123",
				ConfirmPassword = "password123",
				Role = "User",
				AgreeToTerms = true,
			};
			// Mock returns null to simulate registration failure
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
				.ReturnsAsync((UserResponseDTO)null);

			// Act
			var result = await _usersController.RegisterNewUserAsync(registerCommand);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Name", "Name is required");

			// Act
			var result = await _usersController.RegisterNewUserAsync(null);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_ForMissingUsername()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Username", "Username is missing");
			var registerCommand = new UserCreateDTO()
			{
				Email = "example@email.com",
				Password = "password123",
				ConfirmPassword = "password123",
				Role = "User",
				AgreeToTerms = true				
			};

			// Act
			var result = await _usersController.RegisterNewUserAsync(registerCommand);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsOk_WhenEmailIsConfirmed()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var confirmCommand = new ConfirmEmailCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default))
				.ReturnsAsync(true);

			// Act
			var result = await _usersController.ConfirmEmailAsync(confirmCommand.token);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenEmailConfirmationFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var confirmCommand = new ConfirmEmailCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default))
				.ReturnsAsync(false);

			// Act
			var result = await _usersController.ConfirmEmailAsync(confirmCommand.token);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("UserId", "UserId is required");

			// Act
			var result = await _usersController.ConfirmEmailAsync(new ConfirmEmailCommand().token);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ChangePasswordAsync_ReturnsOk_WhenPasswordIsChanged()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var changeCommand = new ChangePasswordCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default))
				.ReturnsAsync(Unit.Value);

			// Act
			var result = await _usersController.ChangePasswordAsync(changeCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ChangePasswordAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("UserId", "UserId is required");

			// Act
			var result = await _usersController.ChangePasswordAsync(new ChangePasswordCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsOk_WhenTokenIsRefreshed()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var refreshTokenDto = new RefreshTokenCommand();
			var newAccessToken = "newAccessToken";
			var newRefreshToken = "newRefreshToken";
			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((newAccessToken, newRefreshToken));

			// Act
			var result = await _usersController.RefreshTokenAsync(refreshTokenDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			dynamic returnedTokens = okResult.Value;
			Assert.Equal(newAccessToken, returnedTokens.Item1);
			Assert.Equal(newRefreshToken, returnedTokens.Item2);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenRefreshTokenFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var refreshTokenDto = new RefreshTokenCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((string.Empty, string.Empty));

			// Act
			var result = await _usersController.RefreshTokenAsync(refreshTokenDto);

			// Assert
			Assert.IsType<UnauthorizedObjectResult>(result);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("AccessToken", "AccessToken is required");

			// Act
			var result = await _usersController.RefreshTokenAsync(new RefreshTokenCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsOkResult_WhenUsersExist()
		{
			// Arrange
			var users = new List<UserResponseDTO> { new(), new() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), default))
				.ReturnsAsync(users);

			// Act
			var result = await _usersController.GetAllUsersAsync();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDTO>>(okResult.Value);
			Assert.Equal(2, returnedUsers.Count());
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsNotFound_WhenNoUsersExist()
		{
			// Arrange
			var users = new List<UserResponseDTO>();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), default))
				.ReturnsAsync(users);

			// Act
			var result = await _usersController.GetAllUsersAsync();

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsOkResult_WhenUserExists()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new UserResponseDTO();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
				.ReturnsAsync(user);

			// Act
			var result = await _usersController.GetUserByIdAsync(userId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(user, okResult.Value);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
				.ReturnsAsync((UserResponseDTO)null);

			// Act
			var result = await _usersController.GetUserByIdAsync(userId);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			// Arrange
			var userId = Guid.Empty;

			// Act
			var result = await _usersController.GetUserByIdAsync(userId);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByEmailAsync_ReturnsOkResult_WhenUserExists()
		{
			// Arrange
			var userEmail = "test@example.com";
			var user = new UserResponseDTO();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default))
				.ReturnsAsync(user);

			// Act
			var result = await _usersController.GetUserByEmailAsync(userEmail);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(user, okResult.Value);
		}

		[Fact]
		public async Task GetUserByEmailAsync_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var userEmail = "nonexistent@example.com";
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default))
				.ReturnsAsync((UserResponseDTO)null);

			// Act
			var result = await _usersController.GetUserByEmailAsync(userEmail);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task DeleteUserAsync_ReturnsNoContent_WhenUserIsDeleted()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default))
				.ReturnsAsync(Unit.Value);

			// Act
			var result = await _usersController.DeleteUserAsync(userId);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}