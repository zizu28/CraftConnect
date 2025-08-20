using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PDFGeneration
{
	public class QuestPDFGenerator : IPDFGenerator
	{
		public Task<byte[]> GeneratePDFAsync<T>(T model, string documentTitle = "Document") where T : class
		{
			var document = CreateDocument(model, documentTitle);
			return Task.FromResult(document.GeneratePdf());
		}

		public async Task GeneratePDFToFileAsync<T>(T model, string outputPath, string documentTitle = "Document") where T : class
		{
			var document = CreateDocument(model, documentTitle);
			document.GeneratePdf();
			await Task.CompletedTask;
		}

		private Document CreateDocument<T>(T model, string title) where T : class
		{
			return Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(2, Unit.Centimetre);
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Header().Text(title)
					.SemiBold()
					.FontSize(18)
					.FontColor(Colors.Blue.Medium);

					page.Content()
					.PaddingVertical(1, Unit.Centimetre)
					.Column(column =>
					{
						column.Item().Component(new ModelComponent<T>(model));
					});

					page.Footer()
					.AlignCenter()
					.Text(x =>
					{
						x.Span("Page ");
						x.CurrentPageNumber();
					});
				});
			});
		}
	}

	internal class ModelComponent<T>(T model) : IComponent where T : class
	{
		private readonly T _model = model;

		public void Compose(IContainer container)
		{
			container.Column(column =>
			{
				column.Item().Text($"Type: {typeof(T).Name}");
				foreach(var prop in typeof(T).GetProperties())
				{
					column.Item().Text($"{prop.Name}: {prop.GetValue(_model) ?? "NA"}");
				}
			});
		}
	}
}
