using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Application;
using UserManagement.Application.Extensions;
using UserManagement.Infrastructure.Extensions;
using UserManagement.Presentation;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---

builder.Host.ConfigureSerilog();
builder.Services.AddControllers();
builder.Services.AddUserManagementConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddUserApplicationExtensions(builder.Configuration.GetSection("MediatR"));
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

// --- 2. AUTHENTICATION CONFIGURATION ---
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
	opt.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};

	// LOCAL AUTH: This allows UserManagement Controllers to see "User.Identity"
	opt.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			// Look for token in Cookie first
			if (context.Request.Cookies.TryGetValue("X-Access-Token", out var token))
			{
				context.Token = token;
			}
			return Task.CompletedTask;
		},
		OnAuthenticationFailed = context =>
		{
			Console.WriteLine($"[Auth] Validation Failed: {context.Exception.Message}");
			return Task.CompletedTask;
		}
	};
});

// --- 3. YARP CONFIGURATION (THE BFF PROXY) ---
// Removed the manual HttpClient/Handler - YARP does this better.
builder.Services.AddReverseProxy()
	.LoadFromMemory(GetRoutes(), GetClusters())
	.AddTransforms(builderContext =>
	{
		builderContext.AddRequestTransform(transformContext =>
		{
			var cookieName = "X-Access-Token";

			if (transformContext.HttpContext.Request.Cookies.TryGetValue(cookieName, out var token))
			{
				// Remove existing Auth header if any
				transformContext.ProxyRequest.Headers.Remove("Authorization");
				// Attach the Cookie value as a Bearer Token for Ocelot
				transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {token}");
			}
			return ValueTask.CompletedTask;
		});
	});

// --- 4. CORS CONFIGURATION ---
builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowBlazorOrigin", policy =>
	{
		policy.WithOrigins("https://localhost:7284")
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	});
});

var app = builder.Build();

// --- 5. PIPELINE ---

app.UseExceptionHandler(opt => { });
app.MapDefaultEndpoints();
app.AddHangfireDashBoard();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowBlazorOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.Run();


// --- 6. YARP ROUTING ---
static ClusterConfig[] GetClusters()
{
	return
	[
		new ClusterConfig
		{
			ClusterId = "OcelotCluster",
			Destinations = new Dictionary<string, DestinationConfig>
			{
				{"Ocelot", new DestinationConfig
					{
						Address = "https://localhost:7272"
					}
				}
			}
		}
	];
}

static RouteConfig[] GetRoutes()
{
	return
	[
		new RouteConfig
		{
			RouteId = "ToOcelot",
			ClusterId = "OcelotCluster",
			Match = new RouteMatch
			{
				// Catch everything else that isn't a User Controller
				Path = "{**catch-all}"
			}
		}
	];
}