using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController(IMediator mediator) : ControllerBase
	{
		[HttpGet("me")]
		[Authorize] // This works because the Cookie -> Bearer transformation happens before this hits
		public async Task<IActionResult> GetCurrentUser()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

			if (userIdClaim == null) return Unauthorized();

			var query = new GetUserByIdQuery { UserId = Guid.Parse(userIdClaim.Value) };
			var user = await mediator.Send(query);

			return Ok(user);
		}

		[HttpGet]
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

		[HttpGet("{id:guid}")]
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

		[HttpPost("register")]
		//[ValidateAntiForgeryToken]
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
		//[ValidateAntiForgeryToken]
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
		//[ValidateAntiForgeryToken]
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
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResendEmailAsync([FromBody] ResendEmailCommand command)
		{
			if(command == null)
			{
				return BadRequest("");
			}
			await mediator.Send(command);
			return Ok();
		}

		[HttpGet("by-email/{email}")]
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
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserCommand command)
		{
			var response = await mediator.Send(command);
			if (string.IsNullOrEmpty(response.AccessToken) || string.IsNullOrEmpty(response.RefreshToken))
			{
				return Unauthorized("Invalid login attempt.");
			}
			Response.Cookies.Append("X-Access-Token", response.AccessToken, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Strict,
				Secure = true,
				Expires = DateTime.UtcNow.AddMinutes(15)
			});

			var userQuery = new GetUserByEmailQuery { Email = command.Email };
			var user = await mediator.Send(userQuery);			

			return Ok(user);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			DeleteTokenCookies();
			return Ok();
		}


		[HttpPost("refresh-token")]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> RefreshTokenAsync()
		{
			if(!Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken))
			{
				return Unauthorized("No refresh token provided.");
			}
			var command = new RefreshTokenCommand { RefreshToken = refreshToken };
			var newTokens = await mediator.Send(command);
			if (string.IsNullOrEmpty(newTokens.AccessToken))
			{
				DeleteTokenCookies();
				return Unauthorized("Invalid refresh token.");
			}
			SetTokenCookies(newTokens.AccessToken, newTokens.RefreshToken);
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
			return Redirect("https://localhost:7284/login");
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
		{
			ArgumentNullException.ThrowIfNull(command, nameof(command));
			await mediator.Send(command);
			return Ok();
		}

		[HttpPost("change-password")]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordCommand command)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var result = await mediator.Send(command);
				if (result == Unit.Value)
				{
					return Ok("Password changed successfully.");
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				return BadRequest($"Password change failed. {ex.Message}");
			}
			return BadRequest("Password change failed.");
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteUserAsync(Guid id)
		{
			var command = new DeleteUserCommand { UserId = id };
			await mediator.Send(command);
			return NoContent();
		}

		private void SetTokenCookies(string accessToken, string refreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Strict,
				Secure = true,
				Expires = DateTime.UtcNow.AddMinutes(15)
			};
			Response.Cookies.Append("X-Access-Token", accessToken, cookieOptions);
			var refreshOptions = new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Strict,
				Secure = true,
				Expires = DateTime.UtcNow.AddDays(7)
			};
			Response.Cookies.Append("X-Refresh-Token", refreshToken, refreshOptions);
		}

		private void DeleteTokenCookies()
		{
			Response.Cookies.Delete("X-Access-Token");
			Response.Cookies.Delete("X-Refresh-Token");
		}
	}
}
