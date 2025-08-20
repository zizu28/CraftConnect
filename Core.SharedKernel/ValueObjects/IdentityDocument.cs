using Core.SharedKernel.Enums;

namespace Core.SharedKernel.ValueObjects
{
	public record IdentityDocument
	{
		public string DocumentNumber { get; init; }
		public string IssuingCountry { get; init; }
		public DateTime ExpirationDate { get; init; }
		public DocumentType Type { get; set; }
		public IdentityDocument(DocumentType type, string documentNumber, string issuingCountry)
		{
			Type = type;
			DocumentNumber = documentNumber;
			IssuingCountry = issuingCountry;
		}
		public override string ToString()
		{
			return $"{DocumentNumber} - {IssuingCountry} - {ExpirationDate.ToShortDateString()}";
		}
	}
}
