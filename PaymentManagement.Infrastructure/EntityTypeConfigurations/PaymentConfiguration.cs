using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.EntityTypeConfigurations
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
			builder.Property(p => p.PayerId).IsRequired();
			builder.Property(p => p.RecipientId).IsRequired();
			builder.Property(p => p.PaymentMethod)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<PaymentMethod>(v, true)).IsRequired();
			builder.Property(p => p.PaymentType)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<PaymentType>(v, true)).IsRequired();
			builder.Property(p => p.Status)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<PaymentStatus>(v, true)).IsRequired();
			builder.OwnsOne(p => p.BillingAddress, a =>
			{
				a.Property(ad => ad.Street).HasColumnName("BillingStreet").HasMaxLength(200);
				a.Property(ad => ad.City).HasColumnName("BillingCity").HasMaxLength(100);
				a.Property(ad => ad.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(20);
			});
			builder.Property(p => p.Description).HasMaxLength(500);
			builder.Property(p => p.ExternalTransactionId).HasMaxLength(100);
			builder.Property(p => p.PaymentIntentId).HasMaxLength(100);
			builder.Property(p => p.CreatedAt).IsRequired();
			builder.Property(p => p.CapturedAt);
			builder.Property(p => p.AuthorizedAt);
			builder.Property(p => p.ProcessedAt);
			builder.Property(p => p.FailureReason).HasMaxLength(500);
			builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
			builder.HasMany(p => p.Transactions).WithOne().HasForeignKey("PaymentId").OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(p => p.Refunds).WithOne().HasForeignKey("PaymentId").OnDelete(DeleteBehavior.Cascade);
		}
	}
}
