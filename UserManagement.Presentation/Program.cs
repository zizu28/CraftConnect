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
using UserManagement.Presentation.Controllers;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

// To disable automatic claim renaming
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---

builder.Host.ConfigureSerilog();
builder.Services.AddControllers().AddApplicationPart(typeof(UsersController).Assembly);
builder.Services.AddUserManagementConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddUserApplicationExtensions(builder.Configuration.GetSection("MediatR"));
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
	{
		sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	}));
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

	opt.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
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

builder.Services.AddReverseProxy()
	.LoadFromMemory(GetRoutes(), GetClusters())
	.AddTransforms(builderContext =>
	{
		builderContext.AddRequestTransform(transformContext =>
		{
			Console.WriteLine($"[YARP] Inspecting request: {transformContext.HttpContext.Request.Path}");
			var cookieName = "X-Access-Token";

			if (transformContext.HttpContext.Request.Cookies.TryGetValue(cookieName, out var token))
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"[YARP] ✅ Cookie Found! Token length: {token.Length}");
				Console.ResetColor();

				transformContext.ProxyRequest.Headers.Remove("Authorization");
				transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {token}");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[YARP] ❌ NO Cookie found in request!");
				Console.WriteLine("       Available Cookies: " + string.Join(", ", transformContext.HttpContext.Request.Cookies.Keys));
				Console.ResetColor();
			}
			return ValueTask.CompletedTask;
		});
	});

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
				Path = "{**catch-all}"
			}
		}
	];
}