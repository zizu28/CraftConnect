using BookingManagement.Application.CQRS.Handlers.DomainEventHandlers;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingManagement.Application.Extensions
{
	public static class BookingManagementApplicationExtensions
	{
		public static IServiceCollection AddBookingApplicationExtensions(
			this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddHttpContextAccessor();
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(typeof(BookingManagementApplicationExtensions).Assembly);
				config.LicenseKey = configuration["MediatR:LicenseKey"];
			});
			services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(typeof(BookingManagementApplicationExtensions).Assembly);
			});
			services.AddValidatorsFromAssemblyContaining(typeof(BookingManagementApplicationExtensions));

			services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
			services.AddScoped<IDomainEventHandler<BookingConfirmedEvent>, ConfirmBookingEventHandler>();
			services.AddScoped<IDomainEventHandler<BookingCreatedEvent>, BookingCreatedEventHandler>();
			services.AddScoped<IDomainEventHandler<BookingCancelledEvent>, BookingCancelledEventHandler>();
			services.AddScoped<IDomainEventHandler<BookingCompletedEvent>, BookingCompleteEventHandler>();
			return services;
		}
	}
}
