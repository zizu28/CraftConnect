using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Persistence.Data
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			Console.WriteLine("Creating ApplicationDbContext for design-time operations...");

			// Use AppContext.BaseDirectory for more reliable path resolution
			var basePath = AppContext.BaseDirectory;
			var apiProjectPath = Path.GetFullPath(Path.Combine(
				basePath,
				"..", "..", "..", "..", "Core.APIGateway"  // Adjust based on actual depth
			));

			Console.WriteLine($"Looking for API project at: {apiProjectPath}");

			if (!Directory.Exists(apiProjectPath))
			{
				// Try alternative path structure
				apiProjectPath = Path.GetFullPath(Path.Combine(
					basePath,
					"..", "..", "..", "..", "src", "Core.APIGateway"
				));

				if (!Directory.Exists(apiProjectPath))
				{
					throw new InvalidOperationException(
						$"Could not find API project. Checked paths:\n" +
						$"- {Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "Core.APIGateway"))}\n" +
						$"- {apiProjectPath}\n" +
						"Ensure the API project exists and the relative path is correct.");
				}
			}

			Console.WriteLine($"Using API project path: {apiProjectPath}");

			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
			Console.WriteLine($"Environment: {environment}");

			var configuration = new ConfigurationBuilder()
				.SetBasePath(apiProjectPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: true)
				.AddEnvironmentVariables()
				.Build();

			var connectionString = configuration.GetConnectionString("DefaultConnection");

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				// Check for other possible connection string names
				connectionString = configuration.GetConnectionString("Database")
								?? configuration.GetConnectionString("PostgreSQL");

				if (string.IsNullOrWhiteSpace(connectionString))
				{
					throw new InvalidOperationException(
						"No connection string found. Checked: 'DefaultConnection', 'Database', 'PostgreSQL'. " +
						"Please ensure your appsettings.json contains a valid connection string.");
				}
			}

			Console.WriteLine("Successfully retrieved connection string.");

			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.UseNpgsql(connectionString, options =>
			{
				options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
				options.EnableRetryOnFailure(); // Add resilience for design-time operations
			});

			Console.WriteLine("DbContext configured successfully.");

			return new ApplicationDbContext(optionsBuilder.Options);
		}
	}
}