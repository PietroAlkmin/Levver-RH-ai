namespace LevverRH.Domain.Interfaces;

public interface IPdfExtractor
{
    Task<string> ExtractTextAsync(byte[] pdfContent);
}
