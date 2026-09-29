using System.IO.Abstractions.TestingHelpers;
using ImageConverter.Core.Services;

namespace ImageConverter.Core.Tests.Services;

public class FileNameBuilderTests
{
    [Fact]
    public void FindFreeBaseName_NameTakenInOneFormat_AddsNumberForAll()
    {
        string folder = "/out";
        string baseName = "photo";
        string takenFilePath = "/out/photo.png";
        string expectedName = "photo-2";
        string[] extensions = new[] { ".jpg", ".png", ".webp" };
        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.AddEmptyFile(takenFilePath);
        FileNameBuilder builder = new FileNameBuilder(fileSystem);

        string freeName = builder.FindFreeBaseName(folder, baseName, extensions);

        Assert.Equal(expectedName, freeName);
    }
}
