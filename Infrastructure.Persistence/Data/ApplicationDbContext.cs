using BookingManagement.Domain.Entities;
using Core.SharedKernel.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentManagement.Domain.Entities;
using ProductInventoryManagement.Domain.Entities;
using UserManagement.Domain.Entities;

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
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }

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
			modelBuilder.Owned<JobDetails>();
			modelBuilder.Owned<PaymentTransaction>();
			modelBuilder.Owned<Refund>();
			modelBuilder.Owned<Image>();
			modelBuilder.Owned<Inventory>();

			var applicationsToScan = AppDomain.CurrentDomain.GetAssemblies()
				.Where(a => a.FullName != null && (
						a.FullName.StartsWith("UserManagement.Infrastructure.EntityTypeConfigurations") ||
						a.FullName.StartsWith("BookingManagement.Infrastructure.EntityTypeConfigurations") || 
						a.FullName.StartsWith("ProductInventoryManagement.Infrastructure.EntityTypeConfigurations") ||
						a.FullName.StartsWith("PaymentManagement.Infrastructure.EntityTypeConfigurations")
						)
				);

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
