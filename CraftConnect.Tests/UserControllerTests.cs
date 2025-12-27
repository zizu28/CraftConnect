using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.Responses;
using UserManagement.Presentation.Controllers;
using System.Reflection;

namespace CraftConnect.Tests
{
	public class UserControllerTest
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly Mock<IUserModuleService> _userModuleService;
		private readonly UsersController _usersController;
		private readonly Mock<HttpContext> _httpContextMock;
		private readonly Mock<IRequestCookieCollection> _cookiesMock;

		public UserControllerTest()
		{
			_mediatorMock = new Mock<IMediator>();
			_userModuleService = new Mock<IUserModuleService>();
			
			_httpContextMock = new Mock<HttpContext>();
			_cookiesMock = new Mock<IRequestCookieCollection>();
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(r => r.Cookies).Returns(_cookiesMock.Object);
			_httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

			_usersController = new UsersController(_mediatorMock.Object, _userModuleService.Object);
			_usersController.ControllerContext = new ControllerContext { HttpContext = _httpContextMock.Object };
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsOk_WhenLoginSucceeds()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var loginCommand = new LoginUserCommand
			{
				Email = "example@user.com",
				Password = "testpassword",
				RememberMe = true
			};
			var accessToken = "access_token_mock";
			var refreshToken = "refresh_token_mock";
			_mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
				.ReturnsAsync(new LoginResponse()
				{
					AccessToken = accessToken,
					RefreshToken = refreshToken
				});

			// Act
			var result = await _usersController.LoginUserAsync(loginCommand);

			// Assert
			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			object returnedTokens = okResult.Value;
			var at = returnedTokens.GetType().GetProperty("AccessToken").GetValue(returnedTokens);
			var rt = returnedTokens.GetType().GetProperty("RefreshToken").GetValue(returnedTokens);
			Assert.Equal(accessToken, at);
			Assert.Equal(refreshToken, rt);
		}

		[Fact]
		public async Task LoginUserAsync_ReturnsUnauthorized_WhenLoginFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var loginCommand = new LoginUserCommand
			{
				Email = "example@user.com",
				Password = "wrongpassword",
				RememberMe = true
			};
			_mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
				.ReturnsAsync(new LoginResponse() 
				{ 
					AccessToken = string.Empty, 
					RefreshToken = string.Empty 
				});

			// Act
			var result = await _usersController.LoginUserAsync(loginCommand);

			// Assert
			Assert.IsType<UnauthorizedResult>(result);
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
				Password = "SecureP@ssw0rd123",
				ConfirmPassword = "SecureP@ssw0rd123",
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
				Password = "SecureP@ssw0rd456",
				ConfirmPassword = "SecureP@ssw0rd456",
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
				Password = "SecureP@ssw0rd789",
				ConfirmPassword = "SecureP@ssw0rd789",
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
			var confirmCommand = new ConfirmEmailCommand { token = "valid_token" };
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default))
				.ReturnsAsync(true);

			// Act
			var result = await _usersController.ConfirmEmailAsync(confirmCommand.token);

			// Assert
			Assert.IsType<RedirectResult>(result);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenEmailConfirmationFails()
		{
			// Arrange
			_usersController.ModelState.Clear();
			var confirmCommand = new ConfirmEmailCommand { token = "invalid_token" };
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
	public async Task ResetPasswordAsync_ReturnsOk_WhenPasswordIsReset()
	{
		// Arrange
		_usersController.ModelState.Clear();
		var resetCommand = new ResetPasswordCommand();
		_mediatorMock.Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), default))
			.ReturnsAsync(Unit.Value);

		// Act
		var result = await _usersController.ResetPasswordAsync(resetCommand);

		// Assert
		Assert.IsType<OkObjectResult>(result);
	}

		[Fact]
	public async Task ResetPasswordAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
	{
		// Arrange
		_usersController.ModelState.AddModelError("Email", "Email is required");

		// Act
		var result = await _usersController.ResetPasswordAsync(new ResetPasswordCommand());

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
	}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsOk_WhenTokenIsRefreshed()
		{
			// Arrange
			_usersController.ModelState.Clear();
			//var refreshTokenDto = new RefreshTokenCommand();
			var newAccessToken = "newAccessToken";
			var newRefreshToken = "newRefreshToken";
			
			_cookiesMock.Setup(c => c.TryGetValue("X-Refresh-Token", out It.Ref<string>.IsAny)).Returns((string key, out string value) => {
				value = "valid_refresh_token";
				return true;
			});

			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((newAccessToken, newRefreshToken));

			// Act
			var result = await _usersController.RefreshTokenAsync();

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
			//var refreshTokenDto = new RefreshTokenCommand();
			_cookiesMock.Setup(c => c.TryGetValue("X-Refresh-Token", out It.Ref<string>.IsAny)).Returns((string key, out string value) => {
				value = "valid_refresh_token";
				return true;
			});

			_mediatorMock.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), default))
				.ReturnsAsync((string.Empty, string.Empty));

			// Act
			var result = await _usersController.RefreshTokenAsync();

			// Assert
			Assert.IsType<UnauthorizedObjectResult>(result);
		}

		[Fact]
		public async Task RefreshTokenAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_usersController.ModelState.AddModelError("AccessToken", "AccessToken is required");

			// Act
			var result = await _usersController.RefreshTokenAsync();

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
	public async Task DeleteUserAsync_ReturnsNoContent_WhenUserDeletesOwnAccount()
	{
		// Arrange - User deleting their own account
		var userId = Guid.NewGuid();
		
		// Mock User.FindFirst to return the same userId
		var claimsPrincipalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
		var claim = new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString());
		claimsPrincipalMock.Setup(x => x.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)).Returns(claim);
		_usersController.ControllerContext.HttpContext.User = claimsPrincipalMock.Object;
		
		_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default))
			.ReturnsAsync(Unit.Value);

		// Act
		var result = await _usersController.DeleteUserAsync(userId);

		// Assert
		Assert.IsType<NoContentResult>(result);
	}

		[Fact]
		public async Task GetCraftsmanName_ReturnsOkResult_WhenCraftsmanExists()
		{
			// Arrange
			var craftsmanId = Guid.NewGuid();
			var craftsmanName = "John Doe";
			_userModuleService.Setup(s => s.GetCraftsmanNameAsync(craftsmanId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(craftsmanName);

			// Act
			var result = await _usersController.GetCraftsmanName(craftsmanId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(craftsmanName, okResult.Value);
		}

		[Fact]
		public async Task GetCraftsmanName_ReturnsNotFound_WhenCraftsmanDoesNotExist()
		{
			// Arrange
			var craftsmanId = Guid.NewGuid();
			_userModuleService.Setup(s => s.GetCraftsmanNameAsync(craftsmanId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((string)null);

			// Act
			var result = await _usersController.GetCraftsmanName(craftsmanId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Craftsman with ID {craftsmanId} not found.", notFoundResult.Value);
		}

		[Fact]
		public async Task GetBatchNames_ReturnsOkResult_WhenCalled()
		{
			// Arrange
			var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
			var names = new Dictionary<Guid, string> { { ids[0], "John" }, { ids[1], "Doe" } };
			_userModuleService.Setup(s => s.GetCraftsmanNamesAsync(ids, It.IsAny<CancellationToken>()))
				.ReturnsAsync(names);

			// Act
			var result = await _usersController.GetBatchNames(ids);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(names, okResult.Value);
		}
	}
}