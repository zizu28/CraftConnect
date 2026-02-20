using Core.EventServices;
using Core.Logging;
using CraftConnect.ServiceDefaults;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using PaymentManagement.Application.Extensions;
using PaymentManagement.Application.Services;
using PaymentManagement.Infrastructure.Extensions;
using PaymentManagement.Presentation;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddPaymentManagementApplicationConfigurations(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.AddPaymentManagementInfrastructureConfiguration(builder.Configuration);
builder.Services.AddFluentEmailService(builder.Configuration);
//builder.Services.AddHybridCacheService(builder.Configuration);
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.RegisterSerilog();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient("PaystackClient", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["Paystack:BaseUrl"] ?? "https://api.paystack.co/");
	client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Paystack:SecretKey"]}");
});

builder.Services.AddCors(cors =>
{
	cors.AddPolicy("PaystackCors", policy =>
	{
		policy.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

builder.Services.AddScoped<IPaystackWebhookVerifier>(sp =>
{
	var secretKey = builder.Configuration["Paystack:SecretKey"];
	return new PaystackWebhookVerifier(builder.Configuration);
});

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.OnRejected = async (context, token) =>
	{
		if (context.Lease.TryGetMetadata (MetadataName.RetryAfter, out var retryAfter))
		{
			context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
		};
	};
	options.AddPolicy("authenticated", context =>
	{
		var username = context.User?.Identity?.Name ?? "anonymous";
		return RateLimitPartition.GetSlidingWindowLimiter(username, _ => new SlidingWindowRateLimiterOptions
		{
			Window = TimeSpan.FromMinutes(1),
			PermitLimit = 50,
			SegmentsPerWindow = 6,
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
			QueueLimit = 3
		});
	});
	options.AddPolicy("critical", context =>
	{
		var username = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		return RateLimitPartition.GetSlidingWindowLimiter(username, _ => new SlidingWindowRateLimiterOptions
		{
			Window = TimeSpan.FromMinutes(1),
			PermitLimit = 5,
			SegmentsPerWindow = 6,
			QueueLimit = 0
		});
	});
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
	{
		var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
		{
			Window = TimeSpan.FromMinutes(1),
			PermitLimit = 100,
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
			QueueLimit = 20
		});
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Use(async (context, next) =>
{
	context.Request.EnableBuffering();
	await next();
});
app.MapDefaultEndpoints();
app.UseExceptionHandler(opt => { });
app.UseHttpsRedirection();
app.UseCors("PaystackCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
