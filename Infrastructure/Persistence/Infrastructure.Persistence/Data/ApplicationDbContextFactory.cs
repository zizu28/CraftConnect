using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Data
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if(string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException(
					$"Connection string 'DefaultConnection' not found. " +
					$"Ensure appsettings.json is in Core.APIGateway project root " +
					$"and the factory path is correct. Current base path being checked: {Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..")}"
				);
			}
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.UseSqlServer(connectionString, options =>
			{
				options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
			});
			return new ApplicationDbContext(optionsBuilder.Options);
		}
	}
}
