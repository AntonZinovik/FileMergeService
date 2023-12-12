namespace FileMergeService.Services.Implementations;

using System.IO.Compression;

using FileMergeService.Dtos;
using FileMergeService.Exceptions;
using FileMergeService.Extensions;
using FileMergeService.Options;
using FileMergeService.Services.Interfaces;

using Microsoft.Extensions.Options;

/// <inheritdoc cref="IFileService"/>
public class FileService : IFileService
{
    /// <summary>
    /// <inheritdoc cref="FilesOptions"/>
    /// </summary>
    private readonly FilesOptions _filesOptions;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<FileService> _logger;

    /// <inheritdoc cref="IFileService"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="filesOptions">Настройки файлов.</param>
    public FileService(ILogger<FileService> logger, IOptions<FilesOptions> filesOptions)
    {
        _logger = logger;

        _filesOptions = filesOptions.Value;
    }

    /// <inheritdoc cref="IFileService.MergeFileAsync"/>
    public async Task MergeFileAsync(FileDto fileDto, CancellationToken cancellationToken)
    {
        var pathToDirectory = Path.Combine(_filesOptions.PathChunkDirectory!,
            Path.GetFileNameWithoutExtension(fileDto.FileName));

        if (!Directory.Exists(pathToDirectory))
        {
            throw new NullReferenceException("Не существует папки с частями указанного файла.");
        }

        var chunks = Directory.GetFiles(pathToDirectory)
                              .OrderBy(chunk => int.Parse(Path.GetFileNameWithoutExtension(chunk)));
        var pathToFinalFile = Path.Combine(_filesOptions.PathFileDirectory!, fileDto.FileName);

        _logger.LogInformation("Создан файл:{FileName}",fileDto.FileName);
        
        await using var fileStream = File.Open(pathToFinalFile, FileMode.OpenOrCreate);

        foreach (var chunk in chunks)
        {
            await using var zipFileStream = File.Open(chunk, FileMode.Open);

            await using var unzipStream =
                new GZipStream(zipFileStream, CompressionMode.Decompress);

            await unzipStream.CopyToAsync(fileStream, cancellationToken);
        }

        var savedHashCode = await fileStream.CalculationHashCodeAsync();

        var areHashCodesEquals = fileDto.HashCode.SequenceEqual(savedHashCode);

        if (!areHashCodesEquals)
        {
            fileStream.Close();
            File.Delete(pathToFinalFile);
            throw new HashCodeException("Хеш-суммы файлов не равны");
        }
    }

    /// <inheritdoc cref="IFileService.SaveChunkAsync"/>
    public async Task SaveChunkAsync(ChunkDto chunkDto, CancellationToken cancellationToken)
    {
        var pathToDirectory = Path.Combine(_filesOptions.PathChunkDirectory!, chunkDto.FileName);

        if (!Directory.Exists(pathToDirectory))
        {
            Directory.CreateDirectory(pathToDirectory);
        }

        var pathToFile = Path.Combine(pathToDirectory, chunkDto.ChunkName);
        await using var fileStream = File.Create(pathToFile, chunkDto.Bytes.Length);

        using var stream = new MemoryStream(chunkDto.Bytes);

        await stream.CopyToAsync(fileStream, cancellationToken);

        var savedHashCode = await fileStream.CalculationHashCodeAsync();

        var areHashCodesEquals = chunkDto.HashCode.SequenceEqual(savedHashCode);

        if (!areHashCodesEquals)
        {
            File.Delete(pathToFile);
            throw new HashCodeException("Хеш-суммы файлов не равны");
        }
    }
}