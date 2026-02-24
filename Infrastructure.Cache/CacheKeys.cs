namespace Infrastructure.Cache
{
	public static class CacheKeys
	{
		// ── UserManagement ──────────────────────────────────────────────────
		public const string AllUsers               = "users:all";
		public static string UserById(Guid id)     => $"users:{id}";
		public static string UserByEmail(string e) => $"users:email:{e}";

		// ── CraftsmanManagement ─────────────────────────────────────────────
		public const string AllCraftsmen               = "craftsmen:all";
		public static string CraftsmanById(Guid id)   => $"craftsmen:{id}";

		// ── BookingManagement ───────────────────────────────────────────────
		public const string AllBookings             = "bookings:all";
		public static string BookingById(Guid id)   => $"bookings:{id}";

		// ── ProductInventoryManagement ──────────────────────────────────────
		public const string AllProducts             = "products:all";
		public static string ProductById(Guid id)   => $"products:{id}";

		public const string AllCategories           = "categories:all";
		public static string CategoryById(Guid id)  => $"categories:{id}";
	}
}
