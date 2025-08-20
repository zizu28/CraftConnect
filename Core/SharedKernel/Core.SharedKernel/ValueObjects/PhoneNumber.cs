namespace Core.SharedKernel.ValueObjects
{
	public record PhoneNumber
	{
		public string CountryCode { get; init; } = string.Empty;
		public string Number { get; init; } = string.Empty;

		private PhoneNumber()
		{
			CountryCode = string.Empty;
			Number = string.Empty;
		}
		public PhoneNumber(string countryCode, string number)
		{
			CountryCode = countryCode;
			Number = number;
		}
		public override string ToString()
		{
			return $"{CountryCode} {Number}";
		}
	}
}
