using Microsoft.JSInterop;

namespace CraftConnect.WASM.Services
{
	public class ThemeService
	{
		private readonly IJSRuntime _jsRuntime;
		private bool _initialized;

		public event Action? OnThemeChanged;
		public string CurrentTheme { get; private set; } = "dark"; // Default theme

		public ThemeService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		/// <summary>
		/// Initialize the theme from localStorage (call once on app startup).
		/// </summary>
		public async Task InitializeAsync()
		{
			if (_initialized) return;

			try
			{
				var storedTheme = await _jsRuntime.InvokeAsync<string>("themeInterop.getStoredTheme");
				if (!string.IsNullOrEmpty(storedTheme))
				{
					CurrentTheme = storedTheme;
				}
			}
			catch
			{
				// During prerendering or if JS not available, use default
				CurrentTheme = "dark";
			}

			_initialized = true;
		}

		/// <summary>
		/// Toggle between light and dark themes.
		/// </summary>
		public async Task ToggleThemeAsync()
		{
			CurrentTheme = CurrentTheme == "light" ? "dark" : "light";
			await ApplyThemeAsync();
			NotifyThemeChanged();
		}

		/// <summary>
		/// Set a specific theme.
		/// </summary>
		public async Task SetThemeAsync(string theme)
		{
			if (theme != "light" && theme != "dark") return;
			CurrentTheme = theme;
			await ApplyThemeAsync();
			NotifyThemeChanged();
		}

		private async Task ApplyThemeAsync()
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync("themeInterop.setTheme", CurrentTheme);
			}
			catch
			{
				// Silently fail if JS runtime not available
			}
		}

		// Keep the synchronous toggle for backward compatibility
		public void ToggleTheme()
		{
			CurrentTheme = CurrentTheme == "light" ? "dark" : "light";
			_ = ApplyThemeAsync();
			NotifyThemeChanged();
		}

		private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
	}
}
