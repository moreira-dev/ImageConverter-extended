using ImageConverter.Core.Models.Converters;
using ImageConverter.Core.Settings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageConverter.Core.Tests.Models.Converters;

public class JpgConverterTests
{
    [Fact]
    public void Save_TransparentPixel_WritesJpgWhitePixel()
    {
        using Image<Rgba32> image = new Image<Rgba32>(1, 1, Color.Transparent.ToPixel<Rgba32>());
        using MemoryStream output = new MemoryStream();
        JpgConverter converter = new JpgConverter(new ConverterSettings());

        converter.Save(image, output);

        using Image<Rgba32> saved = Image.Load<Rgba32>(output.ToArray());
        Assert.Equal(Color.White.ToPixel<Rgba32>(), saved[0, 0]);
    }
}
