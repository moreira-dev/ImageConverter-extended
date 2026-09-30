namespace ImageConverter.Core.Settings;

/// <summary>
/// The values stored in settings.json
/// </summary>
public record ConverterSettings
{
    public const string SameFolderAsOriginal = ".";

    public string OutputFolder { get; set; } = SameFolderAsOriginal;

    public int JpgQuality { get; set; } = 95;

    public bool AiNaming { get; set; }
}
