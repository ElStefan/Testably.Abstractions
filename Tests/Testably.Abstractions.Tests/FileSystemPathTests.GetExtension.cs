namespace Testably.Abstractions.Tests;

public abstract partial class FileSystemPathTests<TFileSystem>
    where TFileSystem : IFileSystem
{
    [Fact]
    [FileSystemTests.Path(nameof(IFileSystem.IPath.GetExtension))]
    public void GetExtension_Null_ShouldReturnNull()
    {
        string? result = FileSystem.Path.GetExtension(null);

        result.Should().BeNull();
    }

    [Theory]
    [AutoData]
    [FileSystemTests.Path(nameof(IFileSystem.IPath.GetExtension))]
    public void GetExtension_ShouldReturnDirectory(
        string directory, string filename, string extension)
    {
        string path = directory + FileSystem.Path.DirectorySeparatorChar + filename +
                      "." + extension;

        string result = FileSystem.Path.GetExtension(path);

        result.Should().Be("." + extension);
    }

#if FEATURE_SPAN
    [Theory]
    [AutoData]
    [FileSystemTests.Path(nameof(IFileSystem.IPath.GetExtension))]
    public void GetExtension_Span_ShouldReturnDirectory(
        string directory, string filename, string extension)
    {
        string path = directory + FileSystem.Path.DirectorySeparatorChar + filename +
                      "." + extension;

        ReadOnlySpan<char> result = FileSystem.Path.GetExtension(path.AsSpan());

        result.ToString().Should().Be("." + extension);
    }
#endif
}