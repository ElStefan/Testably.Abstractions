using System.Threading;

namespace Testably.Abstractions.Tests;

public abstract partial class FileSystemFileSystemWatcherTests<TFileSystem>
	where TFileSystem : IFileSystem
{
	[SkippableTheory]
	[AutoData]
	[FileSystemTests.FileSystemWatcher(nameof(IFileSystem.IFileSystemWatcher
	   .EnableRaisingEvents))]
	public void EnableRaisingEvents_SetToFalse_ShouldStop(string path1, string path2)
	{
		FileSystem.Initialize().WithSubdirectory(path1).WithSubdirectory(path2);
		ManualResetEventSlim ms = new();
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		fileSystemWatcher.Deleted += (_, _) =>
		{
			ms.Set();
		};
		fileSystemWatcher.EnableRaisingEvents = true;
		FileSystem.Directory.Delete(path1);
		ms.Wait(10000).Should().BeTrue();
		ms.Reset();

		fileSystemWatcher.EnableRaisingEvents = false;

		FileSystem.Directory.Delete(path2);
		ms.Wait(30).Should().BeFalse();
	}

	[SkippableTheory]
	[AutoData]
	[FileSystemTests.FileSystemWatcher(nameof(IFileSystem.IFileSystemWatcher
	   .EnableRaisingEvents))]
	public void EnableRaisingEvents_ShouldBeInitializedAsFalse(string path)
	{
		FileSystem.Initialize().WithSubdirectory(path);
		ManualResetEventSlim ms = new();
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		fileSystemWatcher.Deleted += (_, _) =>
		{
			ms.Set();
		};

		FileSystem.Directory.Delete(path);

		ms.Wait(30).Should().BeFalse();
	}
}