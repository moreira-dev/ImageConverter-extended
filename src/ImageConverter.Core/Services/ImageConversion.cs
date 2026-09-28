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
    public ImageFormats ImageFormats { get; }

    public ImageConversion(FormatConverter[] converters)
    {
        _converters = converters;
        ImageFormats = new ImageFormats(converters);
    }

    private string BuildOutputPath(string sourcePath, ImageFormat targetFormat)
    {
        string outputExtension = ImageFormats.GetPrimaryExtensionFor(targetFormat);

        string directory = Path.GetDirectoryName(sourcePath) ?? "/";
        string fileName = Path.GetFileNameWithoutExtension(sourcePath);
        string outputPath = Path.Combine(directory, $"{fileName}{outputExtension}");

        // TODO what should we do if the outputPath already exists? - we override for now

        return outputPath;
    }

    private FormatConverter? GetConverterFor(ImageFormat format)
    {
        return _converters.FirstOrDefault(c => c.Format == format);
    }

    public string Convert(string sourcePath, ImageFormat targetFormat)
    {
        string outputPath = BuildOutputPath(sourcePath, targetFormat);

        FormatConverter? converter = GetConverterFor(targetFormat);

        if (converter == null)
        {
            throw new NotSupportedException($"Format {targetFormat} is not supported.");
        }

        using Image image = Image.Load(sourcePath);
        converter.Save(image, outputPath);

        return outputPath;
    }
}