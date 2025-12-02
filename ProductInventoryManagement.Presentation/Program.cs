using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using ProductInventoryManagement.Application.Extensions;
using ProductInventoryManagement.Infrastructure.Extensions;
using ProductInventoryManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Host.ConfigureSerilog();
builder.Services.AddFluentEmailService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.ProductInventoryManagementInfrastructureServices(builder.Configuration);
builder.Services.AddProductInventoryManagementApplication(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
