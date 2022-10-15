using System.IO;

namespace Testably.Abstractions.Tests.FileSystem.FileInfo;

public abstract partial class FileSystemFileInfoTests<TFileSystem>
	where TFileSystem : IFileSystem
{
	[SkippableTheory]
	[AutoData]
	public void Open_ExistingFileWithCreateNewMode_ShouldThrowFileNotFoundException(
		string path)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		Exception? exception = Record.Exception(() =>
		{
			_ = sut.Open(FileMode.CreateNew);
		});

		exception.Should().BeOfType<IOException>()
		   .Which.Message.Should().Contain($"'{FileSystem.Path.GetFullPath(path)}'");
	}

	[SkippableTheory]
	[AutoData]
	public void Open_MissingFileAndIncorrectMode_ShouldThrowFileNotFoundException(
		string path)
	{
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		Exception? exception = Record.Exception(() =>
		{
			_ = sut.Open(FileMode.Open);
		});

		exception.Should().BeOfType<FileNotFoundException>()
		   .Which.Message.Should().Contain($"'{FileSystem.Path.GetFullPath(path)}'");
	}

	[SkippableTheory]
#if !NETFRAMEWORK
	[InlineAutoData(FileMode.Append, FileAccess.Write)]
#endif
	[InlineAutoData(FileMode.Open, FileAccess.ReadWrite)]
	[InlineAutoData(FileMode.Create, FileAccess.ReadWrite)]
	public void Open_ShouldUseExpectedAccessDependingOnMode(
		FileMode mode,
		FileAccess expectedAccess,
		string path)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		using FileSystemStream stream = sut.Open(mode);

		FileTestHelper.CheckFileAccess(stream).Should().Be(expectedAccess);
		FileTestHelper.CheckFileShare(FileSystem, path).Should().Be(FileShare.None);
	}

#if NETFRAMEWORK
	[SkippableTheory]
	[AutoData]
	public void Open_AppendMode_ShouldThrowArgumentException(string path)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		Exception? exception = Record.Exception(() =>
		{
			_ = sut.Open(FileMode.Append);
		});

		exception.Should().BeOfType<ArgumentException>();
	}
#endif

	[SkippableTheory]
	[InlineAutoData(FileAccess.Read, FileShare.Write)]
	[InlineAutoData(FileAccess.Write, FileShare.Read)]
	public void Open_ShouldUseGivenAccessAndShare(string path,
	                                              FileAccess access,
	                                              FileShare share)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		using FileSystemStream stream = sut.Open(FileMode.Open, access, share);

		FileTestHelper.CheckFileAccess(stream).Should().Be(access);
		FileTestHelper.CheckFileShare(FileSystem, path).Should().Be(share);
	}

	[SkippableTheory]
	[AutoData]
	public void Open_ShouldUseNoneShareAsDefault(string path,
	                                             FileAccess access)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);

		using FileSystemStream stream = sut.Open(FileMode.Open, access);

		FileTestHelper.CheckFileAccess(stream).Should().Be(access);
		FileTestHelper.CheckFileShare(FileSystem, path).Should().Be(FileShare.None);
	}

#if FEATURE_FILESYSTEM_STREAM_OPTIONS
	[SkippableTheory]
	[InlineAutoData(FileAccess.Read, FileShare.Write)]
	[InlineAutoData(FileAccess.Write, FileShare.Read)]
	public void Open_WithFileStreamOptions_ShouldUseGivenAccessAndShare(
		string path,
		FileAccess access,
		FileShare share)
	{
		FileSystem.File.WriteAllText(path, null);
		IFileSystem.IFileInfo sut = FileSystem.FileInfo.New(path);
		FileStreamOptions options = new()
		{
			Mode = FileMode.Open, Access = access, Share = share
		};

		using FileSystemStream stream = sut.Open(options);

		FileTestHelper.CheckFileAccess(stream).Should().Be(access);
		FileTestHelper.CheckFileShare(FileSystem, path).Should().Be(share);
	}
#endif
}