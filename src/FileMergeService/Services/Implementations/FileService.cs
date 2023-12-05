namespace FileMergeService.Services.Implementations;

using System.IO.Compression;

using FileMergeService.Dtos;
using FileMergeService.Extensions;
using FileMergeService.Options;
using FileMergeService.Services.Interfaces;

using FileTransferService.Exceptions;

using Microsoft.Extensions.Options;

public class FileService : IFileService
{
    /// <summary>
    /// Настройки файлов.
    /// </summary>
    private readonly FilesOptions _filesOptions;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<FileService> _logger;

    /// <inheritdoc cref="IFileService"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="splitOptions">Настройки файлов.</param>
    public FileService(ILogger<FileService> logger, IOptions<FilesOptions> splitOptions)
    {
        _logger = logger;

        _filesOptions = splitOptions.Value;

        if (string.IsNullOrEmpty(_filesOptions.PathChunkDirectory))
        {
            throw new ArgumentNullException("Указанны некоректные настройки файлов");
        }
    }

    /// <inheritdoc cref="IFileService.MergeFile"/>
    public async Task MergeFile(FileDto fileDto, CancellationToken cancellationToken)
    {
        // Сортируем все чанки в правильном порядке
        var chunks = Directory.GetFiles(_filesOptions.PathChunkDirectory!)
                              .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x)));
        var pathToFinalFile = Path.Combine(_filesOptions.PathFileDirectory!, fileDto.FileName);

        await using var fileStream = File.Open(pathToFinalFile, FileMode.OpenOrCreate);

        // Перекладываем всё в него
        foreach (var chunk in chunks)
        {
            await using var zipFileStream = File.Open(chunk, FileMode.Open);

            await using var unzipStream =
                new GZipStream(zipFileStream, CompressionMode.Decompress);

            await unzipStream.CopyToAsync(fileStream, cancellationToken);
        }

        var savedHashCode = await fileStream.CalculationHashCodeAsync();

        var result = fileDto.HashCode.SequenceEqual(savedHashCode);

        if (!result)
        {
            fileStream.Close();
            File.Delete(pathToFinalFile);
            throw new HashCodeException("Хеш-суммы файлов не равны");
        }
    }

    /// <inheritdoc cref="IFileService.SaveAsync"/>
    public async Task SaveAsync(ChunkDto chunkDto, CancellationToken cancellationToken)
    {
        var pathToFile = Path.Combine(_filesOptions.PathChunkDirectory!, chunkDto.FileName);
        await using var fileStream = File.Create(pathToFile, chunkDto.Bytes.Length);

        using var stream = new MemoryStream(chunkDto.Bytes);

        await stream.CopyToAsync(fileStream, cancellationToken);

        var savedHashCode = await fileStream.CalculationHashCodeAsync();

        var result = chunkDto.HashCode.SequenceEqual(savedHashCode);

        if (!result)
        {
            File.Delete(pathToFile);
            throw new HashCodeException("Хеш-суммы файлов не равны");
        }
    }
}