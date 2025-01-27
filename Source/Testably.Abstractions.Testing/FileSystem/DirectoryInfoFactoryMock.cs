﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Testably.Abstractions.Testing.Helpers;

namespace Testably.Abstractions.Testing.FileSystem;

internal sealed class DirectoryInfoFactoryMock : IDirectoryInfoFactory
{
	private readonly MockFileSystem _fileSystem;

	internal DirectoryInfoFactoryMock(MockFileSystem fileSystem)
	{
		_fileSystem = fileSystem;
	}

	#region IDirectoryInfoFactory Members

	/// <inheritdoc cref="IFileSystemEntity.FileSystem" />
	public IFileSystem FileSystem
		=> _fileSystem;

	/// <inheritdoc cref="IDirectoryInfoFactory.FromDirectoryName(string)" />
	[Obsolete("Use `IDirectoryInfoFactory.New(string)` instead")]
	public IDirectoryInfo FromDirectoryName(string directoryName)
		=> New(directoryName);

	/// <inheritdoc cref="IDirectoryInfoFactory.New(string)" />
	public IDirectoryInfo New(string path)
	{
		return DirectoryInfoMock.New(
			_fileSystem.Storage.GetLocation(path
				.EnsureValidArgument(_fileSystem, nameof(path))),
			_fileSystem);
	}

	/// <inheritdoc cref="IDirectoryInfoFactory.Wrap(DirectoryInfo)" />
	[return: NotNullIfNotNull("directoryInfo")]
	public IDirectoryInfo? Wrap(DirectoryInfo? directoryInfo)
		=> DirectoryInfoMock.New(
			_fileSystem.Storage.GetLocation(
				directoryInfo?.FullName,
				directoryInfo?.ToString()),
			_fileSystem);

	#endregion
}
