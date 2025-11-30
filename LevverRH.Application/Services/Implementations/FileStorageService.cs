using LevverRH.Application.Services.Interfaces;

namespace LevverRH.Application.Services.Implementations;

public class FileStorageService : IFileStorageService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".doc" };
    private static readonly string[] AllowedContentTypes = { 
        "application/pdf", 
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/msword"
    };
    
    private readonly string _uploadsBasePath;

    public FileStorageService(string uploadsBasePath = "uploads")
    {
        _uploadsBasePath = uploadsBasePath;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder)
    {
        try
        {
            // Gera nome único para o arquivo
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Cria pasta se não existir (relativo ao diretório de execução)
            var uploadPath = Path.Combine(_uploadsBasePath, folder);
            Directory.CreateDirectory(uploadPath);

            // Caminho completo do arquivo
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Salva o arquivo
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            // Retorna caminho relativo
            return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao fazer upload do arquivo: {ex.Message}", ex);
        }
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.CompletedTask;

            var filePath = Path.Combine(_uploadsBasePath, fileUrl);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Log do erro mas não falha - arquivo pode já ter sido deletado
            Console.WriteLine($"Erro ao deletar arquivo {fileUrl}: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    public async Task<byte[]> DownloadFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new ArgumentException("URL do arquivo não pode ser vazia");

            var filePath = Path.Combine(_uploadsBasePath, fileUrl);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {fileUrl}");

            return await File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao baixar arquivo {fileUrl}: {ex.Message}", ex);
        }
    }

    public string? ValidateFile(string fileName, long fileSize)
    {
        // Valida tamanho
        if (fileSize > MaxFileSizeBytes)
        {
            return $"Arquivo muito grande. Tamanho máximo: {MaxFileSizeBytes / 1024 / 1024}MB";
        }

        if (fileSize == 0)
        {
            return "Arquivo vazio";
        }

        // Valida extensão
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return $"Tipo de arquivo não permitido. Permitidos: {string.Join(", ", AllowedExtensions)}";
        }

        return null; // Válido
    }

    public async Task<string> ExtractTextFromFileAsync(Stream fileStream, string contentType)
    {
        // Por enquanto retorna vazio - implementaremos na Fase 2
        // Aqui usaremos bibliotecas como iTextSharp (PDF) ou DocumentFormat.OpenXml (DOCX)
        await Task.CompletedTask;
        return string.Empty;
    }
}
