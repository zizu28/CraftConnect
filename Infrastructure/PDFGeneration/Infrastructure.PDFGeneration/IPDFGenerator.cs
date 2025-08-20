namespace Infrastructure.PDFGeneration
{
	public interface IPDFGenerator
	{
		Task<byte[]> GeneratePDFAsync<T>(T model, string documentTitle = "Document") where T : class;
		Task GeneratePDFToFileAsync<T>(T model, string outputPath, string documentTitle = "Document") where T : class;
	}
}
