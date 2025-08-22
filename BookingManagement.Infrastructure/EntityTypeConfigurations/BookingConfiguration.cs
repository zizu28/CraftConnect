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
			builder.OwnsOne(b => b.Duration, duration =>
			{
				duration.Property(d => d.Start.ToUniversalTime()).HasColumnName("DurationStart");
				duration.Property(d => d.End.ToUniversalTime()).HasColumnName("DurationEnd");
			});
			builder.HasMany(b => b.LineItems).WithOne().HasForeignKey(bi => bi.BookingId);
			builder.Ignore(b => b.DomainEvents);
		}
	}
}
