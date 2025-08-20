using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//builder.AddServiceDefaults();
//builder.AddRabbitMQClient("rabbitmq");
// Add services to the container.

builder.Host.ConfigureSerilog();
builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();

//builder.Services.AddAuthentication(options =>
//{
//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//	.AddJwtBearer(options =>
//	{
//		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//		{
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidateLifetime = true,
//			ValidateIssuerSigningKey = true,
//			ValidIssuer = builder.Configuration["Jwt:Issuer"],
//			ValidAudience = builder.Configuration["Jwt:Audience"],
//			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//		};
//	});

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

await app.UseOcelot();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
