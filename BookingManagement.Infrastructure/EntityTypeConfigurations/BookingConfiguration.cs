using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.EntityTypeConfigurations
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.HasKey(b => b.Id);
			builder.Property(b => b.CraftmanId).IsRequired();
			builder.Property(b => b.CustomerId).IsRequired();
			builder.Property(b => b.Status)
				.IsRequired()
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<BookingStatus>(v, true));
			builder.OwnsOne(b => b.ServiceAddress, sa =>
			{
				sa.Property(a => a.City).IsRequired().HasMaxLength(100).HasColumnName("City");
				sa.Property(a => a.Street).IsRequired().HasMaxLength(100).HasColumnName("Street");
				sa.Property(a => a.PostalCode).IsRequired().HasMaxLength(5).HasColumnName("PostalCode");
			});
			builder.OwnsOne(b => b.Details, details =>
			{
				details.Property(d => d.Description).IsRequired().HasMaxLength(1000);
			});
			
			builder.Property(b => b.StartDate).IsRequired().HasColumnType("timestamp without timezone");
			builder.Property(b => b.EndDate).IsRequired().HasColumnType("timestamp without timezone");
			builder.Property(b => b.CreatedAt).IsRequired().HasColumnType("time without time zone");
			builder.HasMany(b => b.LineItems).WithOne().HasForeignKey(bi => bi.BookingId);
			builder.Ignore(b => b.DomainEvents);
			builder.Property(b => b.RowVersion).IsRowVersion()
				.HasColumnType("xmin").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
		}
	}
}
