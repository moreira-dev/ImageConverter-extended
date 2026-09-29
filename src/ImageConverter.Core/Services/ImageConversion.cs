using System.IO.Abstractions;
using ImageConverter.Core.Enums;
using ImageConverter.Core.Models;
using ImageConverter.Core.Models.Converters;
using SixLabors.ImageSharp;

namespace ImageConverter.Core.Services;

/// <summary>
/// Main class to execute image conversion across formats
/// </summary>
public class ImageConversion
{
    private readonly FormatConverter[] _converters;
    private readonly string _outputFolder;
    private readonly IFileSystem _fileSystem;
    private readonly FileNameBuilder _fileNameBuilder;
    public ImageFormats ImageFormats { get; }

    public ImageConversion(FormatConverter[] converters, string outputFolder, IFileSystem fileSystem)
    {
        _converters = converters;
        _outputFolder = outputFolder;
        _fileSystem = fileSystem;
        _fileNameBuilder = new FileNameBuilder(fileSystem);
        ImageFormats = new ImageFormats(converters);
    }

    /// <summary>
    /// Generates the full path of the output image
    /// </summary>
    /// <param name="sourcePath">E.g. "/foo/photo.webp"</param>
    /// <param name="targetFormat">E.g. "PNG"</param>
    /// <returns>E.g. "{_outputFolder}/photo-2.png"</returns>
    private string BuildOutputPath(string sourcePath, ImageFormat targetFormat)
    {
        string sourceName = _fileSystem.Path.GetFileNameWithoutExtension(sourcePath);
        string baseName = _fileNameBuilder.FindFreeBaseName(_outputFolder, sourceName, ImageFormats.AllExtensions);
        string outputExtension = ImageFormats.GetPrimaryExtensionFor(targetFormat);

        return _fileSystem.Path.Combine(_outputFolder, $"{baseName}{outputExtension}");
    }

    private FormatConverter? GetConverterFor(ImageFormat format)
    {
        return _converters.FirstOrDefault(c => c.Format == format);
    }

    public string Convert(string sourcePath, ImageFormat targetFormat)
    {
        // Create output directory if it doesn't exist
        _fileSystem.Directory.CreateDirectory(_outputFolder);

        string outputPath = BuildOutputPath(sourcePath, targetFormat);

        FormatConverter? converter = GetConverterFor(targetFormat);

        if (converter == null)
        {
            throw new NotSupportedException($"Format {targetFormat} is not supported.");
        }

        using Image image = Image.Load(sourcePath);
        using FileSystemStream output = _fileSystem.File.Create(outputPath);
        converter.Save(image, output);

        return outputPath;
    }
}
