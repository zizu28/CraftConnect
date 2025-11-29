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
			builder.OwnsMany(c => c.Skills, skills =>
			{
				skills.Property(s => s.Name).IsRequired().HasMaxLength(100);
				skills.Property(s => s.YearsOfExperience).IsRequired();
			});
			builder.OwnsMany(c => c.WorkExperience, we =>
			{
				we.Property(w => w.Company).IsRequired().HasMaxLength(100);
				we.Property(w => w.Responsibilities).HasMaxLength(100);
				we.Property(w => w.Position).HasMaxLength(100);
				we.Property(w => w.StartDate).IsRequired();
				we.Property(w => w.EndDate).IsRequired();
			});
			builder.OwnsMany(c => c.Portfolio, portfolio =>
			{
				portfolio.Property(p => p.Title).HasMaxLength(100);
				portfolio.Property(p => p.Description).HasMaxLength(500);
				portfolio.Property(p => p.ImageUrl).HasMaxLength(150);
			});
			builder.Property(c => c.Profession).HasMaxLength(100).HasDefaultValue(null);
			builder.Property(c => c.Location).HasMaxLength(100).HasDefaultValue(null);
			builder.Property(c => c.ProfileImageUrl).HasMaxLength(100).HasDefaultValue(null);
			builder.Property(c => c.CreatedAt.ToUniversalTime()).IsRequired().HasDefaultValueSql("GETUTCDATE()");
		}
	}
}
