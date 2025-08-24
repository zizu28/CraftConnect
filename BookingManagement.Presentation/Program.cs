using BookingManagement.Application.Extensions;
using BookingManagement.Infrastructure.Extensions;
using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NodaTime.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient("rabbitmq");

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(opt =>
	{
		opt.JsonSerializerOptions.ConfigureForNodaTime(NodaTime.DateTimeZoneProviders.Tzdb);
	});
builder.Services.AddBookingApplicationExtensions(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.AddControllers();
builder.Services.AddBookingManagementConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
	o => o.UseNodaTime()));
//builder.Services.AddSqlServer<ApplicationDbContext>("CraftConnectDb");


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
