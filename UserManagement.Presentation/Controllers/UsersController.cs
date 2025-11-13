using MediatR;
using Microsoft.AspNetCore.Mvc;
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

		[HttpPost("register/Craftman")]
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

		[HttpGet("by-email")]
		public async Task<IActionResult> GetUserByEmailAsync([FromQuery] string email)
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
			return Ok(response);
		}

		[HttpPost("refresh-token")]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var newTokens = await mediator.Send(command);
			if (string.IsNullOrEmpty(newTokens.AccessToken) || string.IsNullOrEmpty(newTokens.RefreshToken))
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
			return Redirect("https://localhost:7284/login");
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
	}
}
