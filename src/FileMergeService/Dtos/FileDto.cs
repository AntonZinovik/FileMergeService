﻿namespace FileMergeService.Dtos;

/// <summary>
/// Дто отправляемого файла.
/// </summary>
/// <param name="FileName">Имя файла.</param>
/// <param name="Bytes">Отправляемый файл.</param>
/// <param name="HashCode">Хеш-сумма файла.</param>
public record FileDto(string FileName, byte[] HashCode)
{
    /// <summary>
    /// Имя файла.
    /// </summary>
    public string FileName { get; } = FileName;

    /// <summary>
    /// Хеш-сумма файла.
    /// </summary>
    public byte[] HashCode { get; } = HashCode;
}