using CraftConnect.WebUI.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CraftConnect.WebUI.Auth
{
	public class BffAuthenticationStateProvider(
		IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
	{
		private readonly HttpClient httpClient = httpClientFactory.CreateClient("Backend");

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var userInfo = await httpClient.GetFromJsonAsync<UserResponseDTO>("/api/users/me");
				if (userInfo != null)
				{
					var claims = new List<Claim>
					{
						new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
						new(ClaimTypes.Email, userInfo.Email!),
						new(ClaimTypes.Role, userInfo.Role)
					};

					var identity = new ClaimsIdentity(claims, "BffAuth");
					var user = new ClaimsPrincipal(identity);

					return new AuthenticationState(user);
				}
			}
			catch
			{

			}
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public void NotifyUserLoggedIn(UserResponseDTO userInfo)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
				new(ClaimTypes.Email, userInfo.Email!),
				new(ClaimTypes.Role, userInfo.Role)
			};
			var identity = new ClaimsIdentity(claims, "BffAuth");
			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		// Call this from Logout button
		public void NotifyUserLoggedOut()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
		}
	}
}
