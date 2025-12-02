namespace CraftConnect.WASM.ViewModels
{

	public class CraftsmanPublicProfileVM
	{
		public string Name { get; set; } = "Eleanor Vance";
		public string Location { get; set; } = "Portland, Oregon";
		public string Bio { get; set; } = "With over 15 years of experience in custom woodworking...";
		public string AvatarUrl { get; set; } = "/images/eleanor-avatar.jpg";
		public decimal AvgRating { get; set; } = 4.5M;
		public int ReviewCount { get; set; } = 23;

		public List<string> Skills { get; set; } = [
			"Joinery", "Woodturning", "Carving", "Finishing", "Custom Furniture Design", "Restoration", "Cabinet Making"
		];

		public List<PortfolioItemVM> Portfolio { get; set; } = [];
		public List<ReviewVM> Reviews { get; set; } = [];
	}
}
