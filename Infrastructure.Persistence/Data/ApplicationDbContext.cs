using BookingManagement.Domain.Entities;
using Core.SharedKernel.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime;

namespace Infrastructure.Persistence.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
	{
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Craftman> Craftmen { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<BookingLineItem> BookingLineItems { get; set; }
		public DbSet<JobDetails> JobDetails { get; set; }
		//public DbSet<Product> Products { get; set; }
		//public DbSet<Category> Categories { get; set; }
		//public DbSet<Inventory> Inventories { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(opt =>
			{
				opt.UseNodaTime();
			});
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.AddInboxStateEntity();
			modelBuilder.AddOutboxMessageEntity();
			modelBuilder.AddOutboxStateEntity();

			modelBuilder.Owned<Money>();
			modelBuilder.Owned<Email>();
			modelBuilder.Owned<PhoneNumber>();
			modelBuilder.Owned<UserAddress>();
			modelBuilder.Owned<GeoLocation>();
			modelBuilder.Owned<Skill>();

			//modelBuilder.Owned<DateTimeRange>();
			modelBuilder.Owned<Address>();

			//modelBuilder.Owned<Image>();

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

			var applicationsToScan = AppDomain.CurrentDomain.GetAssemblies()
				.Where(a => a.FullName != null && (
						a.FullName.StartsWith("UserManagement.Infrastructure.EntityTypeConfigurations") ||
						a.FullName.StartsWith("BookingManagement.Infrastructure.EntityTypeConfigurations") 
						//|| a.FullName.StartsWith("ProductInventoryManagement.Infrastructure.EntityTypeConfigurations")
						));

			foreach (var assembly in applicationsToScan)
			{
				try
				{
					Console.WriteLine($"Attempting to apply configurations from assembly: {assembly.FullName}");
					modelBuilder.ApplyConfigurationsFromAssembly(assembly);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error applying configurations from assembly {assembly.FullName}: {ex.Message}");
				}
			}
		}
	}
}
