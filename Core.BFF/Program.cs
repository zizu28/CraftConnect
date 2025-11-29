using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddOpenApi();

builder.Services.AddControllers(); // For the local AuthController
builder.Services.AddHttpClient();  // To talk to UserManagement for Login
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
			var user = transformContext.HttpContext.User;
			if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[YARP] ❌ User is ANONYMOUS. The Browser did not send the 'CraftConnect.Auth' cookie.");
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"[YARP] ✅ User is Authenticated: {user!.Identity!.Name}");
				Console.ResetColor();
			}				

			var accessToken = await transformContext.HttpContext
				.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access-token");
			if (!string.IsNullOrEmpty(accessToken))
			{
				transformContext.ProxyRequest.Headers.Remove("Authorization");
				transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
				Console.WriteLine("[YARP] Token successfully attached.");
			}
			else
			{
				Console.WriteLine("[YARP] Warning: No access-token found in Cookie Ticket.");
			}
		});
	});

builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowBlazor", policy =>
	{
		policy.WithOrigins("https://localhost:7284") // Blazor URL
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