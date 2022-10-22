﻿using System.Security.AccessControl;
using Testably.Abstractions.AccessControl.Tests.TestHelpers;

namespace Testably.Abstractions.AccessControl.Tests;

public class FileInfoAclExtensionsTests
{
	[SkippableFact]
	public void GetAccessControl_ShouldBeInitializedWithNotNullValue()
	{
		Skip.IfNot(Test.RunsOnWindows);

		FileSystemMock fileSystem = new();
		IFileSystem.IFileInfo fileInfo = fileSystem.FileInfo.New("foo");

#pragma warning disable CA1416
		FileSecurity result = fileInfo.GetAccessControl(AccessControlSections.All);
#pragma warning restore CA1416

		result.Should().NotBeNull();
	}

	[SkippableFact]
	public void SetAccessControl_ShouldChangeAccessControl()
	{
		Skip.IfNot(Test.RunsOnWindows);

		FileSystemMock fileSystem = new();
		IFileSystem.IFileInfo fileInfo = fileSystem.FileInfo.New("foo");
#pragma warning disable CA1416
		FileSecurity fileSecurity = new();

		fileInfo.SetAccessControl(fileSecurity);
		FileSecurity result = fileInfo.GetAccessControl();
#pragma warning restore CA1416

		result.Should().Be(fileSecurity);
	}
}