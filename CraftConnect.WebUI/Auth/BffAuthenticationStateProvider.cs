using CraftConnect.WebUI.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CraftConnect.WebUI.Auth
{
	public class BffAuthenticationStateProvider(
		//IHttpClientFactory httpClientFactory,
		ILoggerFactory loggerFactory) : AuthenticationStateProvider
	{
		//private readonly HttpClient httpClient = httpClientFactory.CreateClient("Backend");
		private readonly ILogger logger = loggerFactory.CreateLogger<BffAuthenticationStateProvider>();

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var httpClient = new HttpClient
				{
					BaseAddress = new Uri("https://localhost:7235")
				};
				var userInfo = await httpClient.GetFromJsonAsync<UserResponseDTO>("/api/users/me");
				if (userInfo != null)
				{
					var claims = new List<Claim>
					{
						new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
						new(ClaimTypes.Email, userInfo.Email!),
						new(ClaimTypes.Role, userInfo.Role),
						new(ClaimTypes.Name, userInfo.Email!)
					};

					var identity = new ClaimsIdentity(claims, "BffAuth");
					var user = new ClaimsPrincipal(identity);
					logger.LogInformation("User restored from Cookie: {Email}", userInfo.Email);
					return new AuthenticationState(user);
				}
			}
			catch(Exception ex)
			{
				logger.LogWarning(ex, "CheckAuthenticationState failed. Defaulting to Anonymous.");
			}
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			return new AuthenticationState(anonymous);
		}

		public void NotifyUserLoggedIn(UserResponseDTO userInfo)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
				new(ClaimTypes.Email, userInfo.Email!),
				new(ClaimTypes.Role, userInfo.Role),
				new(ClaimTypes.Name, userInfo.Email!)
			};
			var identity = new ClaimsIdentity(claims, "BffAuth");
			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		public void NotifyUserLoggedOut()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
		}
	}
}
