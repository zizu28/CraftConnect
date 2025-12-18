using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.EntityTypeConfigurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(u => u.Id);
			builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
			builder.Property(u => u.FirstName).HasMaxLength(50);
			builder.Property(u => u.LastName).HasMaxLength(50);
			builder.OwnsOne(u => u.Email, email =>
			{
				email.Property(e => e.Address).IsRequired().HasMaxLength(100).HasColumnName("Email");
				email.HasIndex(e => e.Address).IsUnique();
			});
			builder.Property(u => u.IsEmailConfirmed).IsRequired().HasDefaultValue(false);
			builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
			builder.OwnsOne(u => u.Phone, phone =>
			{
				phone.Property(p => p.Number).IsRequired().HasMaxLength(15).HasColumnName("PhoneNumber");
				phone.Property(p => p.CountryCode).IsRequired().HasMaxLength(5)
				.HasColumnName("PhoneCountryCode");
			});
			builder.HasMany(u => u.RefreshTokens).WithOne()
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade); ;
			builder.Property(b => b.RowVersion).IsRowVersion().ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
			builder.Property(u => u.Role).HasConversion(
				role => role.ToString(),
				roleString => Enum.Parse<UserRole>(roleString, true))
				.IsRequired();
			builder.Property(u => u.CreatedAt.ToUniversalTime()).IsRequired().HasDefaultValueSql("GETUTCDATE()");
		}
	}
}
