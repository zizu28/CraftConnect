using Core.APIGateway.DelegatingHandlers;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Host.ConfigureSerilog();
builder.Services.RegisterSerilog();
builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowBFF", policy =>
	{
		policy.WithOrigins("https://localhost:7136")
		.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
	});
});

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ClockSkew = TimeSpan.Zero,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
		};
	});

builder.Services.AddOcelot(builder.Configuration)
	.AddDelegatingHandler<CorrelationIdDelegatingHandler>(true);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

app.UseCors("AllowBFF");

app.UseAuthentication();

app.UseAuthorization();

await app.UseOcelot();

app.Run();