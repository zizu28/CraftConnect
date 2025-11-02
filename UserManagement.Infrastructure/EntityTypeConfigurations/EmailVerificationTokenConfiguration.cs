using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.EntityTypeConfigurations
{
	public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
	{
		public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
		{
			builder.HasKey(e => e.EmailVerificationTokenId);
			builder.Property(e => e.TokenValue).IsRequired().HasMaxLength(256);
			builder.HasIndex(e => e.TokenValue).IsUnique();
			builder.HasIndex(e => e.UserId).IsUnique();
			builder.Property(e => e.UserId).IsRequired();
			builder.HasOne(e => e.User).WithMany()
				.HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}
