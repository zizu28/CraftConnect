using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
			var returnValue = Assert.IsType<List<UserResponseDTO>>(okResult.Value);
			Assert.Equal(users.Count, returnValue.Count);
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsNotFound_WhenNoUsersExist()
		{
			// Arrange
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), default))
				.ReturnsAsync(new List<UserResponseDTO>());
			// Act
			var result = await _usersController.GetAllUsersAsync();
			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			// Act
			var result = await _usersController.GetUserByIdAsync(Guid.Empty);
			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.UserId == userId), default))
				.ReturnsAsync((UserResponseDTO)null);
			// Act
			var result = await _usersController.GetUserByIdAsync(userId);
			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByIdAsync_ReturnsOkResult_WhenUserExists()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new UserResponseDTO
			{
				UserId = userId,
				Role = "User",
			};
			_mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.UserId == userId), default))
				.ReturnsAsync(user);
			// Act
			var result = await _usersController.GetUserByIdAsync(userId);
			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsType<UserResponseDTO>(okResult.Value);
			Assert.Equal(userId, returnValue.UserId);
		}

		[Fact]
		public async Task GetUserByEmailAsync_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var email = "nonexistent@example.com";
			_mediatorMock.Setup(m => m.Send(It.Is<GetUserByEmailQuery>(q => q.Email == email), default))
				.ReturnsAsync((UserResponseDTO)null);

			// Act
			var result = await _usersController.GetUserByEmailAsync(email);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByEmailAsync_ReturnsOkResult_WhenUserExists()
		{
			// Arrange
			var email = "user@example.com";
			var user = new UserResponseDTO { Email = email };
			_mediatorMock.Setup(m => m.Send(It.Is<GetUserByEmailQuery>(q => q.Email == email), default))
				.ReturnsAsync(user);

			// Act
			var result = await _usersController.GetUserByEmailAsync(email);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUser = Assert.IsType<UserResponseDTO>(okResult.Value);
			Assert.Equal(email, returnedUser.Email);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_ForMissingUsername()
		{
			// Arrange
			var userDtoWithMissingUsername = new UserCreateDTO
			{
				Username = null,
				FirstName = "John",
				LastName = "Doe",
				Email = "john.doe@example.com",
				Password = "SecurePassword123",
				PhoneCountryCode = "+1",
				PhoneNumber = "1234567890",
				Role = "User"
			};

			// Manually add the validation error that ModelState will have
			_usersController.ModelState.AddModelError(nameof(UserCreateDTO.Username), "The Username field is required.");

			// Act
			var result = await _usersController.RegisterNewUserAsync(userDtoWithMissingUsername);

			// Assert
			// The controller should return BadRequest because ModelState is invalid.
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var userDto = new UserCreateDTO()
			{
				Username = "testuser",
				FirstName = "Test",
				LastName = "User",
				Email = "test@example.com",
				Password = "password123",
				PhoneCountryCode = "+1",
				PhoneNumber = "1234567890",
				Role = "User"
			};

			_usersController.ModelState.AddModelError("ErrorKey", "A test validation error.");

			// Act
			var result = await _usersController.RegisterNewUserAsync(userDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

			var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
			Assert.True(modelState.ContainsKey("ErrorKey"));
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsOkResult_WhenUserIsRegistered()
		{
			// Arrange
			var newUserDto = new UserCreateDTO
			{
				Username = "JohnDoe",
				FirstName = "John",
				LastName = "Doe",
				Email = "john.doe@example.com",
				Password = "SecurePassword123",
				PhoneCountryCode = "+1",
				PhoneNumber = "1234567890",
				Role = "User"
			};
			var responseDto = new UserResponseDTO();
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
				.ReturnsAsync(responseDto);

			// Act
			var result = await _usersController.RegisterNewUserAsync(newUserDto);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_WhenRegistrationFails()
		{
			// Arrange
			var user = new UserCreateDTO
			{
				Username = "JohnDoe",
				FirstName = "John",
				LastName = "Doe",
				Email = "john.doe@example.com",
				Password = "SecurePassword123",
				PhoneCountryCode = "+1",
				PhoneNumber = "1234567890",
				Role = "User"
			};
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
				.ReturnsAsync((UserResponseDTO)null);

			// Act
			var result = await _usersController.RegisterNewUserAsync(user);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Password", "Password is required.");

			// Act
			var result = await _usersController.LoginUserAsync(new LoginUserCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsUnauthorized_WhenLoginFails()
		{
			// Arrange
			_mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
				.ReturnsAsync((null, null));

			// Act
			var result = await _usersController.LoginUserAsync(new LoginUserCommand());

			// Assert
			Assert.IsType<UnauthorizedObjectResult>(result);
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
			Assert.Equal(accessToken, returnedTokens.AccessToken);
			Assert.Equal(refreshToken, returnedTokens.RefreshToken);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Token", "Token is required.");

			// Act
			var result = await _usersController.RefreshTokenAsync(new RefreshTokenCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenRefreshTokenFails()
		{
			// Arrange
			string accessToken = "" ?? null;
			string refreshToken = "" ?? null;
			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((accessToken, refreshToken));

			// Act
			var result = await _usersController.RefreshTokenAsync(new RefreshTokenCommand());

			// Assert
			Assert.IsType<UnauthorizedObjectResult>(result);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsOk_WhenTokenIsRefreshed()
		{
			// Arrange
			var newAccessToken = "new_access_token";
			var newRefreshToken = "new_refresh_token";
			var newTokens = (newAccessToken, newRefreshToken);
			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync(newTokens);

			// Act
			var result = await _usersController.RefreshTokenAsync(new RefreshTokenCommand());

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			dynamic returnedTokens = okResult.Value;
			Assert.Equal(newAccessToken, returnedTokens.Item1);
			Assert.Equal(newRefreshToken, returnedTokens.Item2);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("UserId", "User ID is required.");
			var command = new ConfirmEmailCommand();

			// Act
			var result = await _usersController.ConfirmEmailAsync(command);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsOk_WhenEmailIsConfirmed()
		{
			// Arrange
			var command = new ConfirmEmailCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default))
				.ReturnsAsync(true);

			// Act
			var result = await _usersController.ConfirmEmailAsync(command);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenEmailConfirmationFails()
		{
			// Arrange
			var command = new ConfirmEmailCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default))
				.ReturnsAsync(false);

			// Act
			var result = await _usersController.ConfirmEmailAsync(command);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ChangePasswordAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("OldPassword", "Old password is required.");

			// Act
			var result = await _usersController.ChangePasswordAsync(new ChangePasswordCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}


		[Fact]
		public async Task ChangePasswordAsync_ReturnsOk_WhenPasswordIsChanged()
		{
			// Arrange
			var command = new ChangePasswordCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default))
				.ReturnsAsync(Unit.Value);

			// Act
			var result = await _usersController.ChangePasswordAsync(command);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ChangePasswordAsync_ReturnsBadRequest_WhenPasswordChangeFails()
		{
			// Arrange
			var command = new ChangePasswordCommand();

			_mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default))
				.ThrowsAsync(new InvalidOperationException("Password change failed."));

			// Act
			var result = await _usersController.ChangePasswordAsync(command);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
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
