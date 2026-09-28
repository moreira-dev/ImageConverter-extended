using ImageConverter.Core.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ImageConverter.Core.Models.Converters;

public abstract class FormatConverter
{
    public abstract ImageFormat Format { get; }
    public abstract string[] SupportedExtensions { get; }
    protected abstract ImageEncoder Encoder { get; }

    /// <summary>
    /// Saves the image in disk, using the class's Encoder
    /// </summary>
    public void Save(Image image, string outputPath)
    {
        using Image prepared = PrepareImage(image);
        prepared.Save(outputPath, Encoder);
    }

    /// <summary>
    /// Allows Converters to modify the image before saving it
    /// </summary>
    protected virtual Image PrepareImage(Image image)
    {
        return image.Clone(context => { });
    }
}