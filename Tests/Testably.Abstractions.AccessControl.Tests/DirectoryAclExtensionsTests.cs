﻿using System.Security.AccessControl;
using Testably.Abstractions.AccessControl.Tests.TestHelpers;

namespace Testably.Abstractions.AccessControl.Tests;

public class DirectoryAclExtensionsTests
{
	[SkippableFact]
	public void Create_ShouldChangeAccessControl()
	{
		Skip.IfNot(Test.RunsOnWindows);

		FileSystemMock fileSystem = new();
		IFileSystem.IDirectoryInfo directoryInfo = fileSystem.DirectoryInfo.New("foo");
#pragma warning disable CA1416
		DirectorySecurity directorySecurity = new();

		fileSystem.Directory.CreateDirectory("foo", directorySecurity);
		DirectorySecurity result = directoryInfo.GetAccessControl();
#pragma warning restore CA1416

		result.Should().Be(directorySecurity);
	}

	[SkippableFact]
	public void GetAccessControl_ShouldBeInitializedWithNotNullValue()
	{
		Skip.IfNot(Test.RunsOnWindows);

		FileSystemMock fileSystem = new();
		fileSystem.Directory.CreateDirectory("foo");

#pragma warning disable CA1416
		DirectorySecurity result =
			fileSystem.Directory.GetAccessControl("foo", AccessControlSections.All);
#pragma warning restore CA1416

		result.Should().NotBeNull();
	}

	[SkippableFact]
	public void SetAccessControl_ShouldChangeAccessControl()
	{
		Skip.IfNot(Test.RunsOnWindows);

		FileSystemMock fileSystem = new();
		fileSystem.Directory.CreateDirectory("foo");
#pragma warning disable CA1416
		DirectorySecurity directorySecurity = new();

		fileSystem.Directory.SetAccessControl("foo", directorySecurity);
		DirectorySecurity result = fileSystem.Directory.GetAccessControl("foo");
#pragma warning restore CA1416

		result.Should().Be(directorySecurity);
	}
}