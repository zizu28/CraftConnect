using Core.SharedKernel.Domain;
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

		//builder.Services.AddValidatorsFromAssemblyContaining<CraftsmanProfileUpdateValidator>();
		builder.Services.AddScoped<IValidator<CraftsmanProfileUpdateDTO>, CraftsmanProfileUpdateValidator>();
		builder.Services.AddScoped<IValidator<WorkEntry>, WorkEntryValidator>();
		builder.Services.AddScoped<IValidator<Project>, ProjectValidator>();
		builder.Services.AddScoped<IValidator<SkillsDTO>, SkillsDTOValidator>();
		builder.Services.AddTransient<CookieHandler>();
		builder.Services.AddHttpClient("UserManagement", client =>
		{
			client.BaseAddress = new Uri("https://localhost:7235");
		});
		builder.Services.AddHttpClient("BFF", client =>
		{
			var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
						  ?? throw new InvalidOperationException("API URL missing in appsettings.json");
			client.BaseAddress = new Uri(baseUrl);
		})
		.AddHttpMessageHandler<CookieHandler>();

		builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend"));

		builder.Services.AddAuthorizationCore();

		builder.Services.AddScoped<BffAuthenticationStateProvider>();

		builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
			sp.GetRequiredService<BffAuthenticationStateProvider>());

		builder.Services.AddLogging();
		builder.Services.AddSingleton<ThemeService>();

		var app = builder.Build();

		await app.RunAsync();
	}
}