using LevverRH.Domain.Interfaces;
using System.Text;
using UglyToad.PdfPig;

namespace LevverRH.Application.Services.Implementations;

public class PdfExtractor : IPdfExtractor
{
    public async Task<string> ExtractTextAsync(byte[] pdfContent)
    {
        return await Task.Run(() =>
        {
            using var document = PdfDocument.Open(pdfContent);
            var textBuilder = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine($"--- Página {page.Number} ---");
                textBuilder.AppendLine(page.Text);
            }

            return textBuilder.ToString();
        });
    }
}
