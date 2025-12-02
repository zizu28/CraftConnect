namespace CraftConnect.WASM.Services
{
	public class ThemeService
	{
		public event Action? OnThemeChanged;
		public string CurrentTheme { get; private set; } = "light"; // Default theme

		public void ToggleTheme()
		{
			CurrentTheme = CurrentTheme == "light" ? "dark" : "light";
			NotifyThemeChanged();
		}

		private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
	}
}
