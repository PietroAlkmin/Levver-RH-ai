namespace LevverRH.Application.Services.Interfaces;

/// <summary>
/// Serviço para upload e gerenciamento de arquivos
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Faz upload de um arquivo e retorna a URL/caminho
    /// </summary>
    /// <param name="fileStream">Stream do arquivo</param>
    /// <param name="fileName">Nome do arquivo</param>
    /// <param name="contentType">Tipo do arquivo (ex: application/pdf)</param>
    /// <param name="folder">Pasta de destino (ex: "curriculos", "documentos")</param>
    /// <returns>URL ou caminho do arquivo salvo</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder);

    /// <summary>
    /// Deleta um arquivo
    /// </summary>
    /// <param name="fileUrl">URL ou caminho do arquivo</param>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Valida se o arquivo é permitido (extensão e tamanho)
    /// </summary>
    /// <param name="fileName">Nome do arquivo</param>
    /// <param name="fileSize">Tamanho em bytes</param>
    /// <returns>Mensagem de erro ou null se válido</returns>
    string? ValidateFile(string fileName, long fileSize);

    /// <summary>
    /// Extrai texto de um arquivo PDF ou DOCX
    /// </summary>
    /// <param name="fileStream">Stream do arquivo</param>
    /// <param name="contentType">Tipo do arquivo</param>
    /// <returns>Texto extraído</returns>
    Task<string> ExtractTextFromFileAsync(Stream fileStream, string contentType);
}
