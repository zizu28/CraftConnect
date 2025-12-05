using BookingManagement.Application.Extensions;
using BookingManagement.Application.Services;
using BookingManagement.Infrastructure.Extensions;
using BookingManagement.Presentation;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddBookingApplicationExtensions(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.AddBookingManagementConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpClient<IUserModuleService, UserModuleHttpService>(client =>
{
	client.BaseAddress = new Uri("https://usermanagement");
});

var app = builder.Build();

app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
