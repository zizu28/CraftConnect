using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace NotificationManagement.Infrastructure.Converters;

public class DictionaryToJsonConverter : ValueConverter<Dictionary<string, string>?, string?>
{
	public DictionaryToJsonConverter() 
		: base(
			v => SerializeDictionary(v),
			v => DeserializeDictionary(v))
	{
	}

	private static string? SerializeDictionary(Dictionary<string, string>? dictionary)
	{
		return dictionary == null 
			? null 
			: JsonSerializer.Serialize(dictionary);
	}

	private static Dictionary<string, string>? DeserializeDictionary(string? json)
	{
		return string.IsNullOrEmpty(json) 
			? null 
			: JsonSerializer.Deserialize<Dictionary<string, string>>(json);
	}
}
