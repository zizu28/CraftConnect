using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CraftConnect.WebUI.Utilities
{
	public static class JwtParser
	{
		public static IEnumerable<Claim> ParseClaimsFromJwt(string token)
		{
			List<Claim> claims = [];
			if (string.IsNullOrEmpty(token))
			{
				return claims;
			}
			try
			{
				string payload = token.Split(".")[1];
				string jsonBytes = ParseBase64WithoutPadding(payload);
				var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
				if (keyValuePairs == null)
				{
					return claims;
				}
				foreach (var kvp in keyValuePairs)
				{
					if (kvp.Value is JsonElement { ValueKind: JsonValueKind.Array } element)
					{
						foreach (var item in element.EnumerateArray())
						{
							claims.Add(new Claim(kvp.Key, item.ToString() ?? ""));
						}
					}
					claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
				}
				
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Error parsing JWT: {ex.Message}");
			}
			return claims;
		}

		private static string ParseBase64WithoutPadding(string payload)
		{
			payload = payload.Replace("-", "+").Replace("_", "/");
			switch(payload.Length % 4)
			{
				case 2: payload += "=="; break;
				case 3: payload += "="; break;
			}
			var bytes = Convert.FromBase64String(payload);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
