namespace FileMergeService.Options;

/// <summary>
/// Настройки файлов. 
/// </summary>
public class FilesOptions
{
    /// <summary>
    /// Директория для сохранения частей файла.
    /// </summary>
    public string? PathChunkDirectory { get; set; }

    /// <summary>
    /// Директория для сохранения файла после объединения.
    /// </summary>
    public string? PathFileDirectory { get; set; }
}