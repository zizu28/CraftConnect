using BookingManagement.Application.Contracts;
using BookingManagement.Application.Extensions;
using BookingManagement.Infrastructure.RepositoryImplementations;
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
			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddScoped<IBookingLineItemRepository, BookingLineItemRepository>();
			services.AddScoped<IJobDetailsRepository, JobDetailsRepository>();

			services.AddMassTransit(mt =>
			{
				mt.AddEntityFrameworkOutbox<ApplicationDbContext>(config =>
				{
					config.QueryDelay = TimeSpan.FromSeconds(30);
					config.UsePostgres().UseBusOutbox();
				});
				mt.SetKebabCaseEndpointNameFormatter();
				mt.AddConsumers(typeof(BookingManagementApplicationExtensions).Assembly);				
			});

			//services.AddNpgsqlDataSource("postgresdb");
			

			return services;
		}
	}
}
