using System.IO.Abstractions;
using ImageConverter.Core.AI;
using ImageConverter.Core.Enums;
using ImageConverter.Core.Models;
using ImageConverter.Core.Models.Converters;
using ImageConverter.Core.Settings;
using SixLabors.ImageSharp;

namespace ImageConverter.Core.Services;

/// <summary>
/// Main class to execute image conversion across formats
/// </summary>
public class ImageConversion
{
    private readonly FormatConverter[] _converters;
    private readonly ConverterSettings _settings;
    private readonly IFileSystem _fileSystem;
    private readonly FileNameBuilder _fileNameBuilder;
    private readonly ModelFile _modelFile;
    public ImageFormats ImageFormats { get; }

    public bool IsAiModelDownloaded
    {
        get { return _modelFile.Exists; }
    }

    public ImageConversion(FormatConverter[] converters, ConverterSettings settings, IFileSystem fileSystem, string settingsFolder)
    {
        _converters = converters;
        _settings = settings;
        _fileSystem = fileSystem;
        _fileNameBuilder = new FileNameBuilder(fileSystem);
        _modelFile = new ModelFile(fileSystem, settingsFolder);
        ImageFormats = new ImageFormats(converters);
    }

    /// <summary>
    /// Generates the full path of the output image
    /// </summary>
    /// <param name="sourcePath">E.g. "/foo/photo.webp"</param>
    /// <param name="converter">The converter for the output format</param>
    /// <returns>E.g. "/foo/photo-2.png", or "/foo/photo-valley.png" with AI naming on</returns>
    private string BuildOutputPath(string sourcePath, FormatConverter converter)
    {
        string outputFolder = _settings.OutputFolder;

        if (outputFolder == ConverterSettings.SameFolderAsOriginal)
        {
            outputFolder = _fileSystem.Path.GetDirectoryName(sourcePath) ?? string.Empty;
        }

        // TODO loading the model each conversion is expensive. Consider cases with bulk conversions
        using ImageNamer imageNamer = new ImageNamer(_modelFile, _settings);
        string proposedName = imageNamer.BuildBaseName(sourcePath);
        string baseName = _fileNameBuilder.FindFreeBaseName(outputFolder, proposedName, converter.SupportedExtensions);
        string outputExtension = ImageFormats.GetPrimaryExtensionFor(converter.Format);

        return _fileSystem.Path.Combine(outputFolder, $"{baseName}{outputExtension}");
    }

    // TODO we can probably rename this to LoadAiModel and load/unload selectively for bulk processes
    public void CheckAiModel()
    {
        using ImageNamer imageNamer = new ImageNamer(_modelFile, _settings);
        imageNamer.GetClassifier();
    }

    private FormatConverter? GetConverterFor(ImageFormat format)
    {
        return _converters.FirstOrDefault(c => c.Format == format);
    }

    public string Convert(string sourcePath, ImageFormat targetFormat)
    {
        FormatConverter? converter = GetConverterFor(targetFormat);

        if (converter == null)
        {
            throw new NotSupportedException($"Format {targetFormat} is not supported.");
        }

        string outputPath = BuildOutputPath(sourcePath, converter);

        using Image image = Image.Load(sourcePath);
        using FileSystemStream output = _fileSystem.File.Create(outputPath);
        converter.Save(image, output);

        return outputPath;
    }
}
