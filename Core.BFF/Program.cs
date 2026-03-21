using CraftConnect.ServiceDefaults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddOpenApi();

builder.Services.AddControllers(); // For the local AuthController
builder.Services.AddHttpClient("Gateway", client =>
{
	client.BaseAddress = new Uri("https://localhost:7272");
	client.DefaultRequestHeaders.Add("X-Client-Id", "BFF");
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
	options.ExpireTimeSpan = TimeSpan.FromHours(1);
	options.SlidingExpiration = true;
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
			// Always identify this proxy as the BFF for rate limiting
			transformContext.ProxyRequest.Headers.Remove("X-Client-Id");
			transformContext.ProxyRequest.Headers.Add("X-Client-Id", "BFF");

			// Always attempt to read the token — IsAuthenticated may be false
			// for YARP-proxied routes if no RequireAuthorization is set on the route
			var accessToken = await transformContext.HttpContext.GetTokenAsync("access-token");

			if (!string.IsNullOrEmpty(accessToken))
			{
				transformContext.ProxyRequest.Headers.Remove("Authorization");
				transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
				Console.WriteLine($"[BFF YARP] ✅ Token attached for: {transformContext.HttpContext.Request.Path}");
			}
			else
			{
				Console.WriteLine($"[BFF YARP] ⚠️ No access-token for: {transformContext.HttpContext.Request.Path} (authenticated={transformContext.HttpContext.User.Identity?.IsAuthenticated})");
			}
		});
	});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowBlazor", builder =>
	{
		builder.WithOrigins("https://localhost:7222")
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials();
	});
});


var app = builder.Build();

app.MapDefaultEndpoints();

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
			//HttpRequest = new ForwarderRequestConfig
			//{
			//	Version = new Version(1, 1),
			//	VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
			//}
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