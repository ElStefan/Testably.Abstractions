﻿using System.IO;
using System.Threading;

namespace Testably.Abstractions.Testing.Tests;

public sealed class FileSystemMockFileSystemWatcherTests : IDisposable
{
	/// <summary>
	///     Default number of messages before the buffer overflows is 64:<br />
	///     <c>internal buffer size / bytes per message = 8192 / 128 = 64</c>
	/// </summary>
	public const int DefaultMaxMessages = 64;

	public string BasePath => _directoryCleaner.BasePath;
	public FileSystemMock FileSystem { get; }
	private readonly FileSystemInitializer.IDirectoryCleaner _directoryCleaner;

	public FileSystemMockFileSystemWatcherTests()
	{
		FileSystem = new FileSystemMock();
		_directoryCleaner = FileSystem.SetCurrentDirectoryToEmptyTemporaryDirectory();
		FileSystem.Initialize();
	}

	#region IDisposable Members

	/// <inheritdoc />
	public void Dispose()
		=> _directoryCleaner.Dispose();

	#endregion

	[SkippableTheory]
	[AutoData]
	[Trait(nameof(Testing), nameof(FileSystemMock.FileSystemWatcherMock))]
	public void Error_DefaultTo64Messages_ShouldBeTriggeredWhenBufferOverflows(
		string path)
	{
		FileSystem.Directory.CreateDirectory(path);
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		ManualResetEventSlim block1 = new();
		ManualResetEventSlim block2 = new();
		ErrorEventArgs? result = null;
		fileSystemWatcher.Error += (_, eventArgs) =>
		{
			result = eventArgs;
			block1.Set();
			block2.Set();
		};
		fileSystemWatcher.Deleted += (_, _) =>
		{
			block1.Wait(10000);
		};
		fileSystemWatcher.EnableRaisingEvents = true;
		FileSystem.Directory.Delete(path);
		for (int i = 0; i <= DefaultMaxMessages; i++)
		{
			if (block1.IsSet)
			{
				break;
			}

			FileSystem.Directory.CreateDirectory($"{i}_{path}");
		}

		block2.Wait(10000).Should().BeTrue();
		fileSystemWatcher.Dispose();
		result.Should().NotBeNull();
		result!.GetException().Should().BeOfType<InternalBufferOverflowException>();
	}

	[SkippableTheory]
	[InlineAutoData(4096)]
	[InlineAutoData(8192)]
	[Trait(nameof(Testing), nameof(FileSystemMock.FileSystemWatcherMock))]
	public void Error_ShouldBeTriggeredWhenBufferOverflows(
		int internalBufferSize, string path)
	{
		int maxMessages = internalBufferSize / 128;
		FileSystem.Directory.CreateDirectory(path);
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		ManualResetEventSlim block1 = new();
		ManualResetEventSlim block2 = new();
		ErrorEventArgs? result = null;
		fileSystemWatcher.Error += (_, eventArgs) =>
		{
			result = eventArgs;
			block1.Set();
			block2.Set();
		};
		fileSystemWatcher.Deleted += (_, _) =>
		{
			block1.Wait(5000);
		};
		fileSystemWatcher.EnableRaisingEvents = true;
		fileSystemWatcher.InternalBufferSize = internalBufferSize;
		FileSystem.Directory.Delete(path);
		for (int i = 0; i <= maxMessages; i++)
		{
			if (block1.IsSet)
			{
				break;
			}

			FileSystem.Directory.CreateDirectory($"{i}_{path}");
		}

		block2.Wait(5000).Should().BeTrue();
		fileSystemWatcher.Dispose();
		result.Should().NotBeNull();
		result!.GetException().Should().BeOfType<InternalBufferOverflowException>();
	}

	[SkippableTheory]
	[AutoData]
	[Trait(nameof(Testing), nameof(FileSystemMock.FileSystemWatcherMock))]
	public void InternalBufferSize_ShouldResetQueue(string path1, string path2)
	{
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		ManualResetEventSlim block1 = new();
		ManualResetEventSlim block2 = new();
		ErrorEventArgs? result = null;
		fileSystemWatcher.Error += (_, eventArgs) =>
		{
			result = eventArgs;
			block1.Set();
			block2.Set();
		};
		fileSystemWatcher.Created += (_, _) =>
		{
			block1.Wait(100);
		};
		fileSystemWatcher.EnableRaisingEvents = true;
		FileSystem.Directory.CreateDirectory(path1);
		for (int i = 0; i < DefaultMaxMessages; i++)
		{
			if (block1.IsSet)
			{
				break;
			}

			FileSystem.Directory.CreateDirectory($"{i}_{path1}");
		}

		fileSystemWatcher.InternalBufferSize = 4196;

		FileSystem.Directory.CreateDirectory(path2);
		for (int i = 0; i < 4196 / 128; i++)
		{
			if (block1.IsSet)
			{
				break;
			}

			FileSystem.Directory.CreateDirectory($"{i}_{path2}");
		}

		block2.Wait(100).Should().BeFalse();
		fileSystemWatcher.Dispose();
		result.Should().BeNull();
	}

#if FEATURE_FILESYSTEMWATCHER_ADVANCED
	[SkippableTheory]
	[AutoData]
	[Trait(nameof(Testing), nameof(FileSystemMock.FileSystemWatcherMock))]
	public void Filter_ShouldResetFiltersToOnlyContainASingleValue(
		string[] filters, string expectedFilter)
	{
		IFileSystem.IFileSystemWatcher fileSystemWatcher =
			FileSystem.FileSystemWatcher.New(BasePath);
		foreach (string filter in filters)
		{
			fileSystemWatcher.Filters.Add(filter);
		}

		fileSystemWatcher.Filters.Count.Should().Be(filters.Length);

		fileSystemWatcher.Filter = expectedFilter;

		fileSystemWatcher.Filters.Count.Should().Be(1);
		fileSystemWatcher.Filters.Should().ContainSingle(expectedFilter);
		fileSystemWatcher.Filter.Should().Be(expectedFilter);
	}
#endif
}