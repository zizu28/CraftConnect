using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotificationManagement.Application.Extensions;
using NotificationManagement.Infrastructure.Extensions;
using NotificationManagement.Presentation;
using System.Text;
using System.Threading.RateLimiting;

// Clear default JWT claim mappings
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Host.ConfigureSerilog();
builder.Services.AddFluentEmailService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application layer services (MediatR, AutoMapper, FluentValidation)
builder.Services.AddNotificationApplicationExtensions(builder.Configuration);

// Add Infrastructure layer services (Repositories, Providers)
builder.Services.AddNotificationInfrastructureExtensions(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Add JWT Authentication
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
		ValidIssuer = builder.Configuration["JWT:Issuer"],
		ValidAudience = builder.Configuration["JWT:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
	};
});

// Add Authorization policies
builder.Services.AddAuthorizationBuilder()
	.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
	// Policy for sending notifications - 100 requests per minute per user
	options.AddPolicy("send-notification", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromMinutes(1),
				PermitLimit = 100,
				QueueLimit = 10
			}));

	// Policy for template operations - 20 requests per minute per IP
	options.AddPolicy("template-operations", context =>
		RateLimitPartition.GetTokenBucketLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new TokenBucketRateLimiterOptions
			{
				TokenLimit = 20,
				ReplenishmentPeriod = TimeSpan.FromMinutes(1),
				TokensPerPeriod = 20,
				QueueLimit = 5
			}));

	// Policy for preference operations - 50 requests per minute per user
	options.AddPolicy("preference-operations", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromMinutes(1),
				PermitLimit = 50,
				QueueLimit = 5
			}));

	// Global rate limit - 200 req/minute per IP
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
	{
		var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		
		// Allow localhost unlimited for development
		if (ipAddress == "127.0.0.1" || ipAddress == "::1")
		{
			return RateLimitPartition.GetNoLimiter("localhost");
		}

		return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
			new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromMinutes(1),
				PermitLimit = 200,
				QueueLimit = 0
			});
	});

	options.OnRejected = async (context, cancellationToken) =>
	{
		context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
		await context.HttpContext.Response.WriteAsync(
			"Too many requests. Please try again later.", cancellationToken);
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

// Configure exception handling
app.UseExceptionHandler();
app.AddHangfireDashBoard();

// Configure rate limiting
app.UseRateLimiter();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	//app.UseSwagger();
	//app.UseSwaggerUI(c =>
	//{
	//	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Management API v1");
	//});
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
