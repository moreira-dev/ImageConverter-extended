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
    /// Writes the image to the stream, using the class's Encoder.
    /// The caller must open and close the stream
    /// </summary>
    public void Save(Image image, Stream output)
    {
        using Image prepared = PrepareImage(image);
        prepared.Save(output, Encoder);
    }

    /// <summary>
    /// Allows Converters to modify the image before saving it
    /// </summary>
    protected virtual Image PrepareImage(Image image)
    {
        return image.Clone(context => { });
    }
}