using Core.SharedKernel.DTOs;
using System.Net.Http.Json;
using CraftConnect.WASM.ViewModels;

namespace CraftConnect.WASM.Services
{
	public class UserService(IHttpClientFactory httpClientFactory) : IUserService
	{
		private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BFF");

		public async Task<HttpResponseMessage> RegisterAsync(UserCreateDTO model)
		{
			// Routes are mapped in BFF Ocelot config: /users/{everything} -> UserManagement /api/users/{everything}
			return model.Role switch
			{
				"Craftman" => await _httpClient.PostAsJsonAsync("/users/register/craftman", model),
				"Customer" => await _httpClient.PostAsJsonAsync("/users/register/customer", model),
				_ => await _httpClient.PostAsJsonAsync("/users/register", model)
			};
		}

		public async Task<HttpResponseMessage> LoginAsync(LoginUserCommand model)
		{
			// Target the BFF's local AuthController to handle Cookie generation
			return await _httpClient.PostAsJsonAsync("/api/auth/login", model);
		}

		public async Task<HttpResponseMessage> LogoutAsync()
		{
			// Target the BFF's local AuthController
			return await _httpClient.PostAsync("/api/auth/logout", null);
		}

		public async Task<HttpResponseMessage> ForgotPasswordAsync(ForgotPasswordCommand model)
		{
			return await _httpClient.PostAsJsonAsync("/users/forgot-password", model);
		}

		public async Task<HttpResponseMessage> ChangePasswordAsync(ChangePasswordCommand model)
		{
			return await _httpClient.PostAsJsonAsync("/users/change-password", model);
		}

		public async Task<HttpResponseMessage> ResendEmailAsync(ResendEmailCommand model)
		{
			return await _httpClient.PostAsJsonAsync("/users/resend-email", model);
		}

		public async Task<HttpResponseMessage> ConfirmEmailAsync(string token)
		{
			return await _httpClient.GetAsync($"/users/confirm-email?token={token}");
		}
	}
}
