using BookingManagement.Application.Extensions;
using BookingManagement.Application.Services;
using BookingManagement.Infrastructure.Extensions;
using BookingManagement.Presentation;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Contracts;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
//builder.AddSqlServerDbContext<ApplicationDbContext>("CraftConnectDB"); // For Aspire orchestration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpClient("UserModule", client =>
{
	client.BaseAddress = new Uri("https://localhost:7235");
});

builder.Services.AddScoped<IUserModuleService, UserModuleHttpService>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
});

builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowBlazorOrigin", policy =>
	{
		policy.WithOrigins("https://localhost:7222")
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	});
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler(opt => { });
app.AddHangfireDashBoard();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowBlazorOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
