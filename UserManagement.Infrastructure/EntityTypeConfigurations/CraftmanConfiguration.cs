using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.EntityTypeConfigurations
{
	public class CraftmanConfiguration : IEntityTypeConfiguration<Craftman>
	{
		public void Configure(EntityTypeBuilder<Craftman> builder)
		{
			builder.HasKey(c => c.Id);
			builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
			builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
			//builder.OwnsOne(c => c.Email, email =>
			//{
			//	email.Property(e => e.Address).IsRequired().HasMaxLength(100).HasColumnName("Email");
			//	email.HasIndex(email => email.Address).IsUnique();
			//});
			//builder.OwnsOne(c => c.Phone, phone =>
			//{
			//	phone.Property(p => p.Number).IsRequired().HasMaxLength(15).HasColumnName("PhoneNumber");
			//	phone.Property(p => p.CountryCode).IsRequired().HasMaxLength(5)
			//	.HasColumnName("PhoneCountryCode");
			//});
			builder.OwnsMany(c => c.Skills, skills =>
			{
				skills.Property(s => s.Name).IsRequired().HasMaxLength(100);
				skills.Property(s => s.YearsOfExperience).IsRequired();
			});
			builder.OwnsOne(c => c.HourlyRate, money =>
			{
				money.Property(m => m.Amount).IsRequired()
				.HasColumnType("decimal(18,2)")
				.HasColumnName("HourlyRateAmount");
				money.Property(m => m.Currency).IsRequired()
				.HasMaxLength(10).HasColumnName("HourlyRateCurrency");
			});
			builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
		}
	}
}
