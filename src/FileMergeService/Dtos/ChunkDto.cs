namespace FileMergeService.Dtos;

/// <summary>
/// Дто части файла.
/// </summary>
/// <param name="FileName">Имя файла.</param>
/// <param name="Bytes">Отправляемая часть файл.</param>
/// <param name="HashCode">Хеш-сумма файла.</param>
public record ChunkDto(string FileName, byte[] Bytes, byte[] HashCode)
{
    /// <summary>
    /// Отправляемый файл.
    /// </summary>
    public byte[] Bytes { get; } = Bytes;

    /// <summary>
    /// Имя файла.
    /// </summary>
    public string FileName { get; } = FileName;

    /// <summary>
    /// Хеш-сумма файла.
    /// </summary>
    public byte[] HashCode { get; } = HashCode;
}