#if !DEBUG || !DISABLE_TESTS_REALFILESYSTEM
using Testably.Abstractions.Tests.TestHelpers.Traits;
using Xunit.Abstractions;

namespace Testably.Abstractions.Tests.Real;

public static partial class RealFileSystem
{
	// ReSharper disable once UnusedMember.Global
	[Collection(nameof(RealFileSystem))]
	[SystemTest(nameof(RealFileSystem))]
	public sealed class FileSystemWatcherTests :
		FileSystemFileSystemWatcherTests<FileSystem>,
		IDisposable
	{
		/// <inheritdoc cref="FileSystemFileSystemWatcherTests{TFileSystem}.BasePath" />
		public override string BasePath => _directoryCleaner.BasePath;

		private readonly FileSystemInitializer.IDirectoryCleaner _directoryCleaner;

		public FileSystemWatcherTests(ITestOutputHelper testOutputHelper)
			: base(new FileSystem(), new TimeSystem())
		{
			_directoryCleaner = FileSystem
			   .SetCurrentDirectoryToEmptyTemporaryDirectory(testOutputHelper.WriteLine);
		}

		#region IDisposable Members

		/// <inheritdoc cref="IDisposable.Dispose()" />
		public void Dispose()
			=> _directoryCleaner.Dispose();

		#endregion
	}
}
#endif