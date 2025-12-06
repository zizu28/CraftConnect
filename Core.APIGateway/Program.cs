using Core.APIGateway.DelegatingHandlers;
using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
//builder.Services.AddControllers();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//builder.Services.AddBackgroundJobs(builder.Configuration);
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////builder.AddServiceDefaults();
//// Add services to the container.

//builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
//builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.RegisterSerilog();
builder.Services.AddCors(opt =>
{
	opt.AddPolicy("BFF", policy =>
	{
		policy.WithOrigins("https://localhost:7272")
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

//builder.Services.AddAuthorization(options =>
//{
//	options.FallbackPolicy = new AuthorizationPolicyBuilder()
//		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//		.RequireAuthenticatedUser()
//		.Build();
//	options.AddPolicy("ApiScope", policy =>
//	{
//		policy.RequireAuthenticatedUser();
//		policy.RequireClaim("scope", "api1");
//	});
//});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

app.UseCors("BFF");

app.UseAuthentication();

app.UseAuthorization();

await app.UseOcelot();

app.Run();