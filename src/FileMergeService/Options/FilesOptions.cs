namespace FileMergeService.Options;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Настройки файлов. 
/// </summary>
public class FilesOptions
{
    /// <summary>
    /// Директория для сохранения частей файла.
    /// </summary>
    [Required(ErrorMessage = "Указан не корректный путь")]
    public string? PathChunkDirectory { get; set; }

    /// <summary>
    /// Директория для сохранения файла после объединения.
    /// </summary>
    [Required(ErrorMessage = "Указан не корректный путь")]
    public string? PathFileDirectory { get; set; }
}