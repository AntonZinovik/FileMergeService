namespace FileMergeService.Controllers;

using FileMergeService.Dtos;
using FileMergeService.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Котроллер для работы с файлами.
/// </summary>
[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    /// <inheritdoc cref="IFileService"/>
    private readonly IFileService _fileService;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<FilesController> _logger;

    /// <inheritdoc cref="FilesController"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="fileService">Сервис для работ с файлом.</param>
    public FilesController(ILogger<FilesController> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    /// <summary>
    /// Объединение частей файла.
    /// </summary>
    /// <param name="fileDto">Дто исходного файла.</param>
    /// <param name="cancellationToken">CancellationToken cancellationToken</param>
    /// <response code="204">Когда удалось объединить файл.</response>
    /// <response code="400">Когда хеш-суммы файлов разные.</response>
    [HttpPost("Merge")]
    public async Task<IActionResult> Merge([FromBody] FileDto fileDto, CancellationToken cancellationToken)
    {
        await _fileService.MergeFile(fileDto, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Сохранить чанк файла.
    /// </summary>
    /// <param name="chunkDto">Дто чанка.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <response code="204">Когда удалось объединить файл.</response>
    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] ChunkDto chunkDto, CancellationToken cancellationToken)
    {
        await _fileService.SaveAsync(chunkDto, cancellationToken);
        return NoContent();
    }
}