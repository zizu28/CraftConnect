using Core.BFF.RequestsEntities;
using Core.BFF.ResponseEntities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Core.BFF.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController(IHttpClientFactory httpClientFactory) : ControllerBase
	{
		private readonly HttpClient httpClient = httpClientFactory.CreateClient();

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
		{
			ArgumentNullException.ThrowIfNull(command, nameof(command));
			httpClient.BaseAddress = new Uri("https://localhost:7235");
			var response = await httpClient.PostAsJsonAsync("/api/users/signin", command);
			if (!response.IsSuccessStatusCode)
			{
				return Unauthorized("Invalid credentials");
			}
			var loginResponse = await response.Content.ReadFromJsonAsync<UpstreamLoginResponse>();
			
			if(string.IsNullOrEmpty(loginResponse!.AccessToken) || string.IsNullOrEmpty(loginResponse.RefreshToken))
			{
				return Unauthorized();
			}
			
			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, command.Email),
				new(ClaimTypes.NameIdentifier, loginResponse.User.UserId.ToString())
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var authProperties = new AuthenticationProperties
			{
				IsPersistent = command.RememberMe,
				ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
			};
			authProperties.StoreTokens(
			[
				new AuthenticationToken { Name = "access_token", Value = loginResponse.AccessToken },
				new AuthenticationToken { Name = "refresh_token", Value = loginResponse.RefreshToken }
			]);

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(identity),
				authProperties);

			return Ok(loginResponse.User);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok();
		}
	}
}
