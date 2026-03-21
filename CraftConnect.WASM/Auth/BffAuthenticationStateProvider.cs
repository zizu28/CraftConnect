using Core.SharedKernel.DTOs;
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
					var claims = BuildClaims(userInfo);
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
			catch (HttpRequestException ex)
			{
				// 503, 502, network errors etc. — treat as anonymous, don't alarm
				logger.LogWarning("Auth check returned {StatusCode} — treating as anonymous.", ex.StatusCode);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Auth check failed unexpectedly.");
			}
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public void NotifyUserLoggedIn(UserResponseDTO userInfo)
		{
			var claims = BuildClaims(userInfo);
			var identity = new ClaimsIdentity(claims, "BffAuth");
			var user = new ClaimsPrincipal(identity);
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		public void NotifyUserLoggedOut()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
		}

		private static List<Claim> BuildClaims(UserResponseDTO userInfo) =>
		[
			new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
			new(ClaimTypes.Role,           userInfo.Role  ?? string.Empty),
			new(ClaimTypes.Name,           userInfo.Email ?? string.Empty),
		];
	}
}
