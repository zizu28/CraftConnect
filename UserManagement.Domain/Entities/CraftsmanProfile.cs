using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace UserManagement.Domain.Entities
{
	public class CraftsmanProfile : AggregateRoot
	{
		public Guid UserId { get; private set; }
		public CraftConnectUser User { get; private set; } = new();
		public string Title { get; private set; } = string.Empty;
		public string Bio { get; private set; } = string.Empty;
		public List<Guid> SkillIds { get; private set; } = [];
		public List<Guid> PortfolioItemIds { get; private set; } = [];
		public GeoLocation Location { get; private set; } = new(0, 0);

		private CraftsmanProfile() { }

		public void UpdateProfile(string title, string bio, UserAddress location)
		{
			Title = title;
			Bio = bio;
			Location = location.Location!;
		}

		public void AddSkill(Guid skillId)
		{
			var isPresent = SkillIds.Contains(skillId);
			if (!isPresent) SkillIds.Add(skillId);
		}

		public void RemoveSkill(Guid skillId)
		{
			SkillIds.Remove(skillId);
		}

		public void AddPortfolioItem(Guid documentId)
		{
			var isPresent = PortfolioItemIds.Contains(documentId);
			if(!isPresent) PortfolioItemIds.Add(documentId);
		}
	}
}
