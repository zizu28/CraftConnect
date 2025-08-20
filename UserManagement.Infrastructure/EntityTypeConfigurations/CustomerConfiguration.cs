using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.EntityTypeConfigurations
{
	public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
	{
		public void Configure(EntityTypeBuilder<Customer> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
			builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
			builder.Property(c => c.Username).IsRequired().HasMaxLength(50);
			builder.OwnsOne(c => c.Email, email =>
			{
				email.Property(e => e.Address).IsRequired().HasMaxLength(100).HasColumnName("Email");
				email.HasIndex(email => email.Address).IsUnique();
			});
			builder.OwnsOne(c => c.Address, address =>
			{
				address.Property(a => a.Street).IsRequired().HasMaxLength(100).HasColumnName("Street");
				address.Property(a => a.City).IsRequired().HasMaxLength(50).HasColumnName("City");
				address.Property(a => a.PostalCode).IsRequired().HasMaxLength(20).HasColumnName("PostalCode");
			});
			builder.OwnsOne(c => c.Phone, phone =>
			{
				phone.Property(p => p.Number).IsRequired().HasMaxLength(15).HasColumnName("PhoneNumber");
				phone.Property(p => p.CountryCode).IsRequired().HasMaxLength(5)
				.HasColumnName("PhoneCountryCode");
			});
			builder.Property(c => c.PreferredPaymentMethod).HasConversion(
				p => p.ToString(),
				p => Enum.Parse<PaymentMethod>(p, true));
			builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
		}
	}
}
