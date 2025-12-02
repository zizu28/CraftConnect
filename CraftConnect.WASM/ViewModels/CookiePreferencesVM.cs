namespace CraftConnect.WASM.ViewModels
{
	public class CookiePreferencesVM
	{
		public bool StrictlyNecessary { get; set; } = true;
		public bool AnalyticsCookies { get; set; } = false;
		public bool MarketingCookies { get; set; } = false;
	}
}
