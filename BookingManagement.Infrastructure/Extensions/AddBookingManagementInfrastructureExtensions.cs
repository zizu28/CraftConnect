using BookingManagement.Application.Consumers;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.SAGA;
using BookingManagement.Infrastructure.RepositoryImplementations;
using BookingManagement.Infrastructure.SAGAS;
using Core.EventServices;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationManagement.Application.Consumers;
using PaymentManagement.Application.Consumers;

namespace BookingManagement.Infrastructure.Extensions
{
	public static class AddBookingManagementInfrastructureExtensions
	{
		public static IServiceCollection AddBookingManagementConfiguration(this IServiceCollection services, 
			IConfiguration configuration, IHostEnvironment environment)
		{
			services.AddMassTransit(mt =>
			{
				// Outbox (transactional messaging)
				mt.AddEntityFrameworkOutbox<ApplicationDbContext>(config =>
				{
					config.QueryDelay = TimeSpan.FromSeconds(1);
					config.UseSqlServer().UseBusOutbox();
				});

				mt.SetKebabCaseEndpointNameFormatter();

				mt.AddSagaStateMachine<BookingToPaymentSaga, BookingToPaymentState>()
				.EntityFrameworkRepository(r =>
				{
					r.ConcurrencyMode = ConcurrencyMode.Optimistic;
					r.ExistingDbContext<ApplicationDbContext>();
					r.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
				});
				mt.AddConsumers(typeof(ConfirmBookingCommandConsumer).Assembly); // BookingManagement
				mt.AddConsumers(typeof(InitiatePaymentCommandConsumer).Assembly); // PaymentManagement
				mt.AddConsumers(typeof(SendBookingConfirmationNotificationCommandConsumer).Assembly); // Notification

				if (environment.IsDevelopment())
				{
					mt.UsingInMemory((context, cfg) =>
					{
						cfg.ConfigureEndpoints(context);
					});
				}
				else
				{
					mt.UsingAmazonSqs((context, cfg) =>
					{
						cfg.Host("us-east-1", h =>
						{
							h.AccessKey(configuration["AWS:AccessKey"]!);
							h.SecretKey(configuration["AWS:SecretKey"]!);
							h.Scope("CraftConnect", true);
						});
						cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("dev", false));
						cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
					});
				}
			});

			services.AddScoped<IMessageBroker, MassTransitMessageBroker>();
			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddScoped<IBookingLineItemRepository, BookingLineItemRepository>();
			services.AddScoped<ICustomerProjectRepository, CustomerProjectRepository>();
			services.AddScoped<ICraftsmanProposalRepository, CraftsmanProposalRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}
