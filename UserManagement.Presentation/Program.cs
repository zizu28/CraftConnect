using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Extensions;
using UserManagement.Infrastructure.Extensions;
using UserManagement.Presentation;
using UserManagement.Presentation.Controllers;

// To disable automatic claim renaming
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
	{
		sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	}));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();


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

app.Run();
