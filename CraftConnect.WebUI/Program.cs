using CraftConnect.ServiceDefaults;
using CraftConnect.WebUI.Components;
using CraftConnect.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("Users", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:UsersUrl"] ?? throw new InvalidOperationException("API base URL is not configured."));
});
builder.Services.AddHttpClient("Bookings", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BookingsUrl"] ?? throw new InvalidOperationException("API base URL is not configured."));
});

builder.Services.AddSingleton<ThemeService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
