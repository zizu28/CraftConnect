using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using UserManagement.Application.Extensions;
using UserManagement.Infrastructure.Extensions;
using UserManagement.Presentation;
using UserManagement.Presentation.Controllers;


System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// --- 1. SERVICE REGISTRATION ---
builder.Host.ConfigureSerilog();
builder.Services.AddControllers()
	.AddApplicationPart(typeof(UsersController).Assembly)
	.AddApplicationPart(typeof(CraftmenController).Assembly);
builder.Services.AddUserManagementConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddUserApplicationExtensions(builder.Configuration.GetSection("MediatR"));
builder.Services.AddMessageBroker(builder.Configuration);
//builder.AddSqlServerDbContext<ApplicationDbContext>("CraftConnectDB"); // For Aspire orchestration

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
	{
		sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	}));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

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

// Rate Limiting - Built-in .NET Rate Limiter (no external package needed)
builder.Services.AddRateLimiter(options =>
{
	// Policy for login endpoint - 5 requests per minute per IP
	options.AddPolicy("login", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromMinutes(1),
				PermitLimit = 5,
				QueueLimit = 0
			}));

	// Policy for password reset - 3 requests per hour per IP
	options.AddPolicy("password-reset", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromHours(1),
				PermitLimit = 3,
				QueueLimit = 0
			}));

	// Policy for forgot password - 3 requests per hour per IP
	options.AddPolicy("forgot-password", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromHours(1),
				PermitLimit = 3,
				QueueLimit = 0
			}));

	// Policy for registration - 3 requests per hour per IP
	options.AddPolicy("registration", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromHours(1),
				PermitLimit = 3,
				QueueLimit = 0
			}));

	// Global rate limit - 10 req/sec per IP
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
				Window = TimeSpan.FromSeconds(1),
				PermitLimit = 10,
				QueueLimit = 0
			});
	});

	// Customize the response when rate limit is exceeded
	options.OnRejected = async (context, cancellationToken) =>
	{
		context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
		await context.HttpContext.Response.WriteAsJsonAsync(new
		{
			error = "Too many requests. Please try again later.",
			retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
				? (double?)retryAfter.TotalSeconds 
				: null
		}, cancellationToken);
	};
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler(opt => { });
app.AddHangfireDashBoard();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowBlazorOrigin");

// Apply rate limiting BEFORE authentication
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
