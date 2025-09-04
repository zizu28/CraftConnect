using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Infrastructure.EntityTypeConfigurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
			builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
			builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
			builder.Property(p => p.CategoryId).IsRequired();
			builder.Property(p => p.CraftmanId).IsRequired();
			builder.OwnsOne(p => p.Inventory, inv =>
			{
				inv.Property(i => i.Quantity).IsRequired();
				inv.Property(i => i.LastUpdated).IsRequired();
			});
			builder.OwnsMany(p => p.Images, img =>
			{
				img.Property(i => i.Url).IsRequired().HasMaxLength(500);
				img.Property(i => i.AltText).HasMaxLength(200);
			});
			builder.Navigation(p => p.Inventory).IsRequired();
			builder.Property(p => p.IsActive).IsRequired();
			builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
			builder.Property(p => p.CreatedAt).IsRequired();
			builder.Property(p => p.LastModified).IsRequired();
		}
	}
}
