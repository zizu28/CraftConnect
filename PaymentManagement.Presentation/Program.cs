using Core.EventServices;
using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using PaymentManagement.Application.Extensions;
using PaymentManagement.Infrastructure.Extensions;
using PaymentManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler(opt => { });
app.UseHttpsRedirection();
app.UseCors("PaystackCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
