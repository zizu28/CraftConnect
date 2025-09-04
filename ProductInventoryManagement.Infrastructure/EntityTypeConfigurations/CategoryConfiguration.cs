using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Infrastructure.EntityTypeConfigurations
{
	public class CategoryConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
			builder.Property(c => c.Description).HasMaxLength(500);
			builder.Property(c => c.CreatedAt).IsRequired();
			builder.Property(c => c.LastModified).IsRequired();
			builder.Property(c => c.RowVersion).IsRowVersion().IsConcurrencyToken();
		}
	}
}
