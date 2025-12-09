using Core.SharedKernel.DTOs;
using CraftConnect.WASM.ViewModels;

namespace CraftConnect.WASM.Services
{
	public interface IUserService
	{
		Task<HttpResponseMessage> RegisterAsync(UserCreateDTO model);
		Task<HttpResponseMessage> LoginAsync(LoginUserCommand model);
		Task<HttpResponseMessage> LogoutAsync();
		Task<HttpResponseMessage> ForgotPasswordAsync(ForgotPasswordCommand model);
		Task<HttpResponseMessage> ChangePasswordAsync(ChangePasswordCommand model);
		Task<HttpResponseMessage> ResendEmailAsync(ResendEmailCommand model);
		Task<HttpResponseMessage> ConfirmEmailAsync(string token);
	}
}
