using System.ComponentModel.DataAnnotations.Schema;

namespace Core.SharedKernel.ValueObjects
{
	public record Proposal
	{
		public Guid Id { get; set; }
		public string CraftsmanName { get; set; } = string.Empty;
		public string CraftsmanAvatarUrl { get; set; } = string.Empty;
		public double CraftsmanRating { get; set; }
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Price { get; set; }
		public string Excerpt { get; set; } = string.Empty;
		public string ProfileUrl { get; set; } = string.Empty;
		public string ProposalUrl { get; set; } = string.Empty;
	}
}
