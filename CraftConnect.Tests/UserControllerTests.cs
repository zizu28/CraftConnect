using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CraftConnect.Controllers;
using CraftConnect.Application.Features.Queries.Users.LoginUser;
using CraftConnect.Application.Features.Commands.Users.RegisterNewUser;
using CraftConnect.Application.Features.Commands.Users.ConfirmEmail;
using CraftConnect.Application.Features.Commands.Users.ChangePassword;
using CraftConnect.Application.Features.Commands.Users.DeleteUser;
using CraftConnect.Application.Features.Queries.Users.GetUserById;
using CraftConnect.Application.Features.Queries.Users.GetAllUsers;
using CraftConnect.Application.Features.Queries.Users.GetUserByEmail;
using CraftConnect.Application.Features.Commands.Users.RefreshToken;
using CraftConnect.Application.DTOs.Users.Commands.RefreshToken;
using System.Threading.Tasks;
using System.Collections.Generic;

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
			var registerCommand = new RegisterNewUserCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterNewUserCommand>(), default))
				.ReturnsAsync(true);

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
			var registerCommand = new RegisterNewUserCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<RegisterNewUserCommand>(), default))
				.ReturnsAsync(false);

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
			var result = await _usersController.RegisterNewUserAsync(new RegisterNewUserCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterNewUserAsync_ReturnsBadRequest_ForMissingUsername()
		{
			// Arrange
			_usersController.ModelState.AddModelError("Username", "Username is missing");
			var registerCommand = new RegisterNewUserCommand
			{
				// Username is not set
				Email = "test@example.com",
				Password = "password123",
				FirstName = "Test",
				LastName = "User"
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
			var result = await _usersController.ConfirmEmailAsync(confirmCommand);

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
			var result = await _usersController.ConfirmEmailAsync(confirmCommand);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("UserId", "UserId is required");

			// Act
			var result = await _usersController.ConfirmEmailAsync(new ConfirmEmailCommand());

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
				.ReturnsAsync(true);

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
			var refreshTokenDto = new RefreshTokenDto { AccessToken = "oldAccessToken", RefreshToken = "oldRefreshToken" };
			var newAccessToken = "newAccessToken";
			var newRefreshToken = "newRefreshToken";
			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((newAccessToken, newRefreshToken));

			// Act
			var result = await _usersController.RefreshTokenAsync(refreshTokenDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			dynamic returnedTokens = okResult.Value;
			Assert.Equal(newAccessToken, returnedTokens.AccessToken);
			Assert.Equal(newRefreshToken, returnedTokens.RefreshToken);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenRefreshTokenFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var refreshTokenDto = new RefreshTokenDto { AccessToken = "invalidToken", RefreshToken = "invalidRefreshToken" };
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
			var result = await _usersController.RefreshTokenAsync(new RefreshTokenDto());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsOkResult_WhenUsersExist()
		{
			// Arrange
			var users = new List<GetUserByIdQuery> { new GetUserByIdQuery(), new GetUserByIdQuery() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), default)).ReturnsAsync(users);

			// Act
			var result = await _usersController.GetAllUsersAsync();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUsers = Assert.IsAssignableFrom<IEnumerable<GetUserByIdQuery>>(okResult.Value);
			Assert.Equal(2, returnedUsers.Count());
		}

		[Fact]
		public async Task GetAllUsersAsync_ReturnsNotFound_WhenNoUsersExist()
		{
			// Arrange
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), default)).ReturnsAsync(new List<GetUserByIdQuery>());

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
			var user = new GetUserByIdQuery();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default)).ReturnsAsync(user);

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default)).ReturnsAsync((GetUserByIdQuery)null);

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
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetUserByEmailAsync_ReturnsOkResult_WhenUserExists()
		{
			// Arrange
			var userEmail = "test@example.com";
			var user = new GetUserByEmailQuery();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default)).ReturnsAsync(user);

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default)).ReturnsAsync((GetUserByEmailQuery)null);

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _usersController.DeleteUserAsync(userId);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}