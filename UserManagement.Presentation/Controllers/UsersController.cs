using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.CQRS.Queries.CustomerQueries;
using UserManagement.Application.CQRS.Queries.UserQueries;

namespace UserManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController(
		IMediator mediator,
		IUserModuleService userModuleService) : ControllerBase
	{
		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> GetCurrentUser()
		{
			var userIdClaim =  User.FindFirst(ClaimTypes.NameIdentifier);

			if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
			{
				return Unauthorized("Token does not contain a User ID claim.");
			}

			if (!Guid.TryParse(userIdClaim.Value, out var userId))
			{
				return Unauthorized("Token User ID is not a valid GUID.");
			}

			var query = new GetUserByIdQuery { UserId = userId };
			var user = await mediator.Send(query);

			if (user == null) return Unauthorized("User no longer exists.");

			return Ok(user);
		}

		[HttpPost("craftsman/batch-summaries")]
		public async Task<IActionResult> GetBatchCraftsmanSummaries([FromBody] List<Guid> ids)
		{
			var summaries = await mediator.Send(new GetBatchCraftsmanSummariesQuery { Ids = ids });
			return Ok(summaries);
		}

		[HttpPost("customer/batch-summaries")]
		public async Task<IActionResult> GetBatchCustomerSummaries([FromBody] List<Guid> ids)
		{
			var summaries = await mediator.Send(new GetBatchCustomerSummariesQuery { Ids = ids });
			return Ok(summaries);
		}

		/// <summary>
		/// Get all users - Admin only
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsersAsync()
		{
			var query = new GetAllUsersQuery();
			var users = await mediator.Send(query);
			if (users == null || !users.Any())
			{
				return NotFound("No users found.");
			}
			return Ok(users);
		}

		/// <summary>
		/// Get user by ID - Requires authentication
		/// </summary>
		[HttpGet("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> GetUserByIdAsync(Guid id)
		{
			var query = new GetUserByIdQuery { UserId = id };
			var user = await mediator.Send(query);
			if (user == null)
			{
				return NotFound($"User with ID {id} not found.");
			}
			return Ok(user);
		}

		[HttpGet("get-name/{Id:guid}")]
		public async Task<IActionResult> GetCraftsmanName([FromRoute] Guid id)
		{
			var name = await userModuleService.GetCraftsmanNameAsync(id);
			if (name == null)
			{
				return NotFound($"Craftsman with ID {id} not found.");
			}
			return Ok(name);
		}

		[HttpPost("craftsman/batch-names")]
		public async Task<IActionResult> GetBatchNames([FromBody] List<Guid> ids)
		{
			var names = await userModuleService.GetCraftsmanNamesAsync(ids);
			//var names = await mediator.Send(new GetBatchNamesQuery { Ids = ids });
			return Ok(names);
		}

		[HttpHead("craftsman/{id}")]
		public async Task<IActionResult> CheckExists(Guid id)
		{
			var exists = await mediator.Send(new CheckUserExistsQuery { UserId = id });
			return exists ? Ok() : NotFound();
		}

		[HttpGet("craftsman/{id:guid}")]
		public async Task<IActionResult> GetCraftsmanByIdAsync([FromRoute] Guid id)
		{
			var query = new GetCraftmanByIdQuery { CraftmanId = id };
			var user = await mediator.Send(query);
			if (user == null)
			{
				return NotFound($"User with ID {id} not found.");
			}
			return Ok(user);
		}

		[HttpGet("customer/{id:guid}")]
		public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
		{
			var query = new GetCustomerByIdQuery { CustomerId = id };
			var user = await mediator.Send(query);
			if (user == null)
			{
				return NotFound($"User with ID {id} not found.");
			}
			return Ok(user);
		}

		[HttpPost("register")]
		public async Task<IActionResult> RegisterNewUserAsync([FromBody] UserCreateDTO user)
		{
			var command = new RegisterUserCommand { User = user };
			var result = await mediator.Send(command);
			if (result == null)
			{
				return BadRequest("User registration failed.");
			}
			return Ok(result);
		}

		[HttpPost("register/craftman")]
		[AllowAnonymous]
		[EnableRateLimiting("registration")]
		public async Task<IActionResult> RegisterNewCraftmanAsync([FromBody] CraftmanCreateDTO craftman)
		{
			var command = new RegisterCraftmanCommand { Craftman = craftman };
			var result = await mediator.Send(command);
			if (result == null)
			{
				return BadRequest("Craftman registration failed.");
			}
			return Ok(result);
		}

		[HttpPost("register/customer")]
		[AllowAnonymous]
		[EnableRateLimiting("registration")]
		public async Task<IActionResult> RegisterNewCustomerAsync([FromBody] CustomerCreateDTO customer)
		{
			var command = new RegisterCustomerCommand { Customer = customer };
			var result = await mediator.Send(command);
			if (result == null)
			{
				return BadRequest("Customer registration failed.");
			}
			return Ok(result);
		}

		[HttpPost("resend-email")]
		public async Task<IActionResult> ResendEmailAsync([FromBody] ResendEmailCommand command)
		{
			if(command == null)
			{
				return BadRequest("");
			}
			await mediator.Send(command);
			return Ok();
		}

		/// <summary>
		/// Get user by email - Admin only (prevents email enumeration)
		/// </summary>
		[HttpGet("by-email/{email}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetUserByEmailAsync([FromRoute] string email)
		{
			var query = new GetUserByEmailQuery { Email = email };
			var user = await mediator.Send(query);
			if (user == null)
			{
				return NotFound($"User with email {email} not found.");
			}
			return Ok(user);
		}

		[HttpPost("signin")]
		[AllowAnonymous]
		[EnableRateLimiting("login")]
		public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			ArgumentNullException.ThrowIfNull(command, nameof(command));
			var loginResponse = await mediator.Send(command);
			if (loginResponse == null || string.IsNullOrEmpty(loginResponse.AccessToken))
			{
				return Unauthorized();
			}
			var userQuery = new GetUserByEmailQuery { Email = command.Email };
			var userResponseDto = await mediator.Send(userQuery);			

			return Ok(new 
			{
				loginResponse.AccessToken,
				loginResponse.RefreshToken,
				User = userResponseDto
			});
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok();
		}


		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshTokenAsync()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if(!Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken))
			{
				return Unauthorized("No refresh token provided.");
			}
			var command = new RefreshTokenCommand { RefreshToken = refreshToken };
			var newTokens = await mediator.Send(command);
			if (string.IsNullOrEmpty(newTokens.AccessToken))
			{
				return Unauthorized("Invalid refresh token.");
			}
			return Ok(newTokens);
		}
				
		[HttpGet("confirm-email")]
		[ActionName("ConfirmEmailAsync")]
		public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return BadRequest("Verification token is required");
			}
			var command = new ConfirmEmailCommand { token = token };
			var result = await mediator.Send(command);
			if (!result)
			{
				return BadRequest("Email verification/confirmation failed");
			}
			return Redirect("https://localhost:7222/login");
		}

		[HttpPost("forgot-password")]
		[AllowAnonymous]
		[EnableRateLimiting("forgot-password")]
		public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
		{
			ArgumentNullException.ThrowIfNull(command, nameof(command));
			await mediator.Send(command);
			return Ok();
		}


		/// <summary>
		/// Reset password via email token (forgot password flow)
		/// </summary>
		[HttpPost("reset-password")]
		[AllowAnonymous]
		[EnableRateLimiting("password-reset")]
		public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			ArgumentNullException.ThrowIfNull(command);
			
			try
			{
				var result = await mediator.Send(command);
				return Ok("Password has been reset successfully.");
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Change password while authenticated (requires old password)
		/// </summary>
		[HttpPost("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordWhileAuthenticatedCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Get user ID from JWT token claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
			{
				return Unauthorized("Invalid authentication token.");
			}

			// Ensure user can only change their own password
			command.UserId = userId;

			try
			{
				var result = await mediator.Send(command);
				return Ok("Password changed successfully.");
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Delete user - User can delete own account, Admin can delete any account
		/// </summary>
		[HttpDelete("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> DeleteUserAsync(Guid id)
		{
			// Get user ID from JWT token claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
			{
				return Unauthorized("Invalid authentication token.");
			}

			// Check if user is admin or deleting their own account
			var isAdmin = User.IsInRole("Admin");
			if (!isAdmin && userId != id)
			{
				return Forbid(); // 403 Forbidden - authenticated but not authorized
			}

			var command = new DeleteUserCommand { UserId = id };
			await mediator.Send(command);
			return NoContent();
		}
	}
}
