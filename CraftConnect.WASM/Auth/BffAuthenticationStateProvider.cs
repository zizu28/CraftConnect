using CraftConnect.WASM.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace CraftConnect.WASM.Auth
{
	public class BffAuthenticationStateProvider(
		IHttpClientFactory httpClientFactory,
		ILoggerFactory loggerFactory) : AuthenticationStateProvider
	{
		private readonly HttpClient httpClient = httpClientFactory.CreateClient("BFF");
		private readonly ILogger logger = loggerFactory.CreateLogger<BffAuthenticationStateProvider>();

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var userInfo = await httpClient.GetFromJsonAsync<UserResponseDTO?>("/users/me");
				if (userInfo != null)
				{
					var claims = new List<Claim>
					{
						new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
						new(ClaimTypes.Role, userInfo.Role),
						new(ClaimTypes.Name, userInfo.Email!)
					};

					var identity = new ClaimsIdentity(claims, "BffAuth");
					var user = new ClaimsPrincipal(identity);
					logger.LogInformation("User restored from Cookie: {Email}", userInfo.Email);
					return new AuthenticationState(user);
				}
			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				logger.LogInformation("User is not logged in (401 from Backend).");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Auth check failed unexpectedly.");
			}
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public void NotifyUserLoggedIn(UserResponseDTO userInfo)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
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
