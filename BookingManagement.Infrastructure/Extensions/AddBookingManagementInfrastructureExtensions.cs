using BookingManagement.Application.Contracts;
using BookingManagement.Application.Extensions;
using BookingManagement.Infrastructure.RepositoryImplementations;
using Core.EventServices;
using Infrastructure.Persistence.Data;
using MassTransit;
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

			return services;
		}
	}
}
