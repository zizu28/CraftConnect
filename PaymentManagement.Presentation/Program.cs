using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using PaymentManagement.Application.Extensions;
using PaymentManagement.Infrastructure.Extensions;
using PaymentManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddPaymentManagementApplicationConfigurations(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.AddPaymentManagementInfrastructureConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient("PaystackClient", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["Paystack:BaseUrl"] ?? "https://api.paystack.co/");
	client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Paystack:SecretKey"]}");
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("PaystackCors", policy =>
	{
		policy.WithOrigins("https://paystack.com")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	}	
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();
app.UseExceptionHandler(opt => { });
app.UseHttpsRedirection();
app.UseCors("PaystackCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
