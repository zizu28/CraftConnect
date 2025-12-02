using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddOpenApi();

builder.Services.AddControllers(); // For the local AuthController
builder.Services.AddHttpClient("UserManagement", client =>
{
	client.BaseAddress = new Uri("https://localhost:7235");
});
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
	options.Cookie.Name = "CraftConnect.Auth";
	options.Cookie.HttpOnly = true;
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.Path = "/";
	options.Events = new CookieAuthenticationEvents
	{
		OnSigningOut = ctx =>
		{
			return Task.CompletedTask;
		},
		OnRedirectToLogin = ctx =>
		{
			ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return Task.CompletedTask;
		},
		OnRedirectToAccessDenied = context =>
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return Task.CompletedTask;
		}
	};
});

builder.Services.AddReverseProxy()
	.LoadFromMemory(GetRoutes(), GetClusters())
	.AddTransforms(builderContext =>
	{
		builderContext.AddRequestTransform(async transformContext =>
		{
			if (!transformContext.HttpContext.User.Identity!.IsAuthenticated)
			{
				Console.WriteLine($"[BFF YARP] ❌ Incoming request unauthenticated: {transformContext.HttpContext.Request.Path}");
				return;
			}

			var accessToken = await transformContext.HttpContext.GetTokenAsync("access-token");

			if (!string.IsNullOrEmpty(accessToken))
			{
				transformContext.ProxyRequest.Headers.Remove("Authorization");
				transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
				Console.WriteLine($"[BFF YARP] ✅ Token attached for: {transformContext.HttpContext.Request.Path}");
			}
			else
			{
				Console.WriteLine($"[BFF YARP] ⚠️ User is logged in, but 'access-token' is NULL.");
			}
		});
	});

builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowBlazor", policy =>
	{
		policy.WithOrigins("https://localhost:7222") // Blazor URL
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});


var app = builder.Build();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Maps the local AuthController
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