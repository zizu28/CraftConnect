using System.ComponentModel.DataAnnotations;

namespace CraftConnect.WASM.ViewModels
{
	public class VerificationViewModel
	{
		[Required]
		[MinLength(6, ErrorMessage = "Please enter all 6 digits.")]
		public string Code1 { get; set; }
		public string Code2 { get; set; }
		public string Code3 { get; set; }
		public string Code4 { get; set; }
		public string Code5 { get; set; }
		public string Code6 { get; set; }

		public string GetFullCode()
		{
			return $"{Code1}{Code2}{Code3}{Code4}{Code5}{Code6}";
		}
	}
}
