using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.EntityTypeConfigurations
{
	public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
	{
		public void Configure(EntityTypeBuilder<Invoice> builder)
		{
			builder.HasKey(i => i.Id);
			builder.Property(i => i.DueDate).IsRequired();
			builder.Property(i => i.Status)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<InvoiceStatus>(v, true)).IsRequired().HasMaxLength(50);
			builder.Property(i => i.Type)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<InvoiceType>(v, true)).IsRequired().HasMaxLength(50);
			builder.Property(i => i.IssuedBy).IsRequired();
			builder.Property(i => i.IssuedTo).IsRequired();
			builder.Property(i => i.BookingId).IsRequired();
			builder.Property(i => i.OrderId).IsRequired();
			builder.Property(i => i.IssuedDate).IsRequired();
			builder.Property(i => i.DueDate).IsRequired();
			builder.Property(i => i.PaidDate).IsRequired();
			builder.OwnsOne(i => i.SubTotal, a =>
			{
				a.Property(p => p.Amount).HasColumnName("SubTotal_Amount").IsRequired();
				a.Property(p => p.Currency).HasColumnName("SubTotal_Currency").IsRequired().HasMaxLength(3);
			});
			builder.OwnsOne(i => i.TotalAmount, a =>
			{
				a.Property(p => p.Amount).HasColumnName("Total_Amount").IsRequired();
				a.Property(p => p.Currency).HasColumnName("TotalAmount_Currency").IsRequired().HasMaxLength(3);
			});
			builder.OwnsOne(i => i.TaxAmount, a =>
			{
				a.Property(p => p.Amount).HasColumnName("Tax_Amount").IsRequired();
				a.Property(p => p.Currency).HasColumnName("TaxAmount_Currency").IsRequired().HasMaxLength(3);
			});
			builder.OwnsOne(i => i.DiscountAmount, a =>
			{
				a.Property(p => p.Amount).HasColumnName("Discount_Amount").IsRequired();
				a.Property(p => p.Currency).HasColumnName("DiscountAmount_Currency").IsRequired().HasMaxLength(3);
			});
			builder.OwnsOne(i => i.Recipient, a =>
			{
				a.Property(p => p.Name).HasColumnName("Recipient_Name").IsRequired().HasMaxLength(200);
				a.OwnsOne(e => e.Email, em =>
				{
					em.Property(p => p.Address).HasColumnName("Recipient_Email_Address").IsRequired().HasMaxLength(200);
				});
				a.OwnsOne(a => a.PhoneNumber, phone =>
				{
					phone.Property(p => p.Number).HasColumnName("Recipient_Phone_Number").IsRequired().HasMaxLength(20);
					phone.Property(p => p.CountryCode).HasColumnName("Recipient_Phone_CountryCode").IsRequired().HasMaxLength(5);
				});
				a.Property(p => p.CompanyName).HasColumnName("Recipient_CompanyName").IsRequired().HasMaxLength(200);
				a.Property(p => p.Type)
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<InvoiceRecipientType>(v, true)).IsRequired().HasMaxLength(50)
					.HasColumnName("Recipient_Type");
				a.Property(p => p.RegistrationNumber).HasColumnName("Recipient_RegistrationNumber").HasMaxLength(100);
				a.Property(p => p.TaxId).HasColumnName("Recipient_TaxId").HasMaxLength(100);
			});
			builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
			builder.Property(i => i.Currency).IsRequired().HasMaxLength(3);
			builder.Property(i => i.TaxRate).IsRequired().HasColumnType("decimal(5,2)");
			builder.Property(i => i.Notes).HasMaxLength(1000);
			builder.Property(i => i.Terms).HasMaxLength(1000);
			builder.Property(i => i.RowVersion).IsRowVersion().IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
			builder.HasIndex(i => i.InvoiceNumber).IsUnique();
			builder.OwnsMany(i => i.LineItems, li =>
			{
				li.Property(p => p.Description).IsRequired().HasMaxLength(1000);
				li.Property(p => p.Quantity).IsRequired().HasColumnType("decimal(18,2)");
				li.OwnsOne(p => p.UnitPrice, unitP =>
				{
					unitP.Property(p => p.Amount).HasColumnName("UnitPrice_Amount").IsRequired();
					unitP.Property(p => p.Currency).HasColumnName("UnitPrice_Currency").IsRequired().HasMaxLength(3);
				});
				li.OwnsOne(p => p.TotalPrice, totalP => 
				{
					totalP.Property(p => p.Amount).HasColumnName("UnitPrice_Amount").IsRequired();
					totalP.Property(p => p.Currency).HasColumnName("UnitPrice_Currency").IsRequired().HasMaxLength(3); 
				});
				li.Property(p => p.ItemCode).HasMaxLength(100);
				li.Property(p => p.InvoiceId).IsRequired();
				li.HasKey(p => p.Id);
				li.ToTable("InvoiceLineItems");
			});
			builder.HasMany(i => i.Payments).WithOne()
				.HasForeignKey(p => p.InvoiceId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
