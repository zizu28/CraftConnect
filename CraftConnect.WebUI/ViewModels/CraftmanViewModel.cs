namespace CraftConnect.WebUI.ViewModels
{
	public record CraftmanViewModel
	(
		Guid Id,
		string Name,
		string Description,
		string ImageUrl,
		string ExperienceSummary,
		string Badge, // e.g., "Highly Rated", "Local Artisan"
		decimal Rating,
		int ReviewCount
	);
}
