using ImageConverter.Core.Models.Converters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageConverter.Core.Tests.Models.Converters;

public class FormatConverterTests
{
    public static TheoryData<FormatConverter, IImageFormat> ConvertersAndFormats { get; } = new TheoryData<FormatConverter, IImageFormat>
    {
        { new JpgConverter(), JpegFormat.Instance },
        { new PngConverter(), PngFormat.Instance },
        { new WebpConverter(), WebpFormat.Instance }
    };

    [Theory]
    [MemberData(nameof(ConvertersAndFormats))]
    public void Save_AnyImage_WritesItsOwnFormat(FormatConverter converter, IImageFormat expectedFormat)
    {
        using Image<Rgba32> image = new Image<Rgba32>(1, 1);
        using MemoryStream output = new MemoryStream();

        converter.Save(image, output);

        IImageFormat savedFormat = Image.DetectFormat(output.ToArray());
        Assert.Equal(expectedFormat, savedFormat);
    }
}