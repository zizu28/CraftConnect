using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Infrastructure.PDFGeneration
{
	public static class QuestPDFExtension
	{
		public static IServiceCollection AddQuestPDF(this IServiceCollection services)
		{
			QuestPDF.Settings.License = LicenseType.Community;
			services.AddQuestPDF();
			services.AddSingleton<IPDFGenerator, QuestPDFGenerator>();
			return services;
		} 
	}
}
