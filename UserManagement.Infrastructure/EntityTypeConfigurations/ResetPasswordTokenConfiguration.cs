using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.EntityTypeConfigurations
{
	public class ResetPasswordTokenConfiguration : IEntityTypeConfiguration<ResetPasswordToken>
	{
		public void Configure(EntityTypeBuilder<ResetPasswordToken> builder)
		{
			builder.HasKey(e => e.ResetPasswordTokenId);
			builder.Property(e => e.TokenValue).IsRequired().HasMaxLength(256);
			builder.HasIndex(e => e.TokenValue).IsUnique();
			builder.HasIndex(e => e.UserId).IsUnique();
			builder.Property(e => e.UserId).IsRequired();
			builder.HasOne(e => e.User).WithMany()
				.HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}
