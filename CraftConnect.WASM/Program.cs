using Core.SharedKernel.Domain;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;
using CraftConnect.WASM;
using CraftConnect.WASM.Auth;
using CraftConnect.WASM.DelegateHandlers;
using CraftConnect.WASM.Services;
using CraftConnect.WASM.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UserManagement.Domain.Entities;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped<IValidator<CraftsmanProfileUpdateDTO>, CraftsmanProfileUpdateValidator>();
		builder.Services.AddScoped<IValidator<WorkEntry>, WorkEntryValidator>();
		builder.Services.AddScoped<IValidator<Project>, ProjectValidator>();
		builder.Services.AddScoped<IValidator<SkillsDTO>, SkillsDTOValidator>();
		builder.Services.AddScoped<IValidator<CustomerUpdateDTO>, CustomerProfileUpdateValidator>();
		builder.Services.AddScoped<SignupVMValidator>();
		builder.Services.AddTransient<CookieHandler>();
		builder.Services.AddHttpClient("BookingManagement", client =>
		{
			client.BaseAddress = new Uri("https://localhost:7285");
		});
		builder.Services.AddHttpClient("UserManagement", client =>
		{
			client.BaseAddress = new Uri("https://localhost:7235");
		});
		builder.Services.AddHttpClient("BFF", client =>
		{
			client.BaseAddress = new Uri("https://localhost:7136");
		})
		.AddHttpMessageHandler<CookieHandler>();

		builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BFF"));

		builder.Services.AddAuthorizationCore();

		builder.Services.AddScoped<BffAuthenticationStateProvider>();

		builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
			sp.GetRequiredService<BffAuthenticationStateProvider>());

		builder.Services.AddLogging();
		builder.Services.AddSingleton<ThemeService>();
		builder.Services.AddScoped<IUserService, UserService>();

		var app = builder.Build();

		await app.RunAsync();
	}
}