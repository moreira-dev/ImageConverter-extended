using System.IO.Abstractions;
using System.Text.Json;

namespace ImageConverter.Core.Settings;

/// <summary>
/// Reads and writes the settings.json file
/// </summary>
/// <param name="filePath">E.g. "~/Library/Application Support/ImageConverter/settings.json"</param>
public class SettingsFile(IFileSystem fileSystem, string filePath)
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public string FilePath { get; } = filePath;

    /// <summary>
    /// Reads the settings file and creates it if it doesn't exist
    /// </summary>
    public ConverterSettings Load()
    {
        if (!fileSystem.File.Exists(FilePath))
        {
            ConverterSettings defaults = new ConverterSettings();
            Save(defaults);

            return defaults;
        }

        string json = fileSystem.File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<ConverterSettings>(json, _jsonOptions) ?? new ConverterSettings();
    }

    public void Save(ConverterSettings settings)
    {
        string? folder = fileSystem.Path.GetDirectoryName(FilePath);

        if (folder != null)
        {
            fileSystem.Directory.CreateDirectory(folder);
        }

        fileSystem.File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, _jsonOptions));
    }
}
