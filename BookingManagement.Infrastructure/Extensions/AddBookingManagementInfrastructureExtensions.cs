using BookingManagement.Application.Contracts;
using BookingManagement.Infrastructure.RepositoryImplementations;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingManagement.Infrastructure.Extensions
{
	public static class AddBookingManagementInfrastructureExtensions
	{
		public static IServiceCollection AddBookingManagementConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddMassTransit(mt =>
			//{
			//	mt.AddConsumersFromNamespaceContaining(typeof(AddBookingManagementInfrastructureExtensions));
			//});
			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddScoped<IBookingLineItemRepository, BookingLineItemRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
