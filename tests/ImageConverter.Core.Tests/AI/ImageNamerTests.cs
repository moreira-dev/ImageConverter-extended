using System.IO.Abstractions.TestingHelpers;
using ImageConverter.Core.AI;
using ImageConverter.Core.Settings;

namespace ImageConverter.Core.Tests.AI;

public class ImageNamerTests
{
    [Fact]
    public void BuildBaseName_AiNamingOff_KeepsOriginalName()
    {
        string sourcePath = "/photos/dog.jpg";
        string expectedName = "dog";
        ModelFile modelFile = new ModelFile(new MockFileSystem(), "/models/model.onnx");
        using ImageNamer namer = new ImageNamer(modelFile, new ConverterSettings { AiNaming = false });

        string baseName = namer.BuildBaseName(sourcePath);

        Assert.Equal(expectedName, baseName);
    }

    [Fact]
    public void ToSlug_LabelWithUnderscores_ReturnsLowerCaseWithHyphens()
    {
        string label = "Golden_retriever";
        string expectedSlug = "golden-retriever";

        string slug = ImageNamer.ToSlug(label);

        Assert.Equal(expectedSlug, slug);
    }
}
