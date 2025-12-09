using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		[AllowAnonymous]
		public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserCommand command)
		{
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
		public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
		{
			ArgumentNullException.ThrowIfNull(command, nameof(command));
			await mediator.Send(command);
			return Ok();
		}

		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordCommand command)
		{
			ArgumentNullException.ThrowIfNull(command);
			var result = await mediator.Send(command);
			if (result == Unit.Value)
			{
				return Ok("Password changed successfully.");
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
	}
}
