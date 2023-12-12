namespace FileMergeService.Services.Interfaces;

using FileMergeService.Dtos;

/// <summary>
/// Сервис для работ с файлом.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Объединить чанки файла.
    /// </summary>
    /// <param name="fileDto">Дто исходного файла.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    Task MergeFileAsync(FileDto fileDto, CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить чанк.
    /// </summary>
    /// <param name="chunkDto">Дто чанка.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    Task SaveChunkAsync(ChunkDto chunkDto, CancellationToken cancellationToken);
}