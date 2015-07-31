// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


#if !SILVERLIGHT && !NETCORE

namespace Castle.Core.Tests.Resources
{
	using System;
	using System.IO;

	using Castle.Core.Resource;

	using Xunit;

	public class FileResourceFactoryTestCase
	{
		private FileResourceFactory resFactory = new FileResourceFactory();
		private String basePath;

		public FileResourceFactoryTestCase()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			basePath = Path.Combine(currentDirectory, "Core.Tests" + Path.DirectorySeparatorChar + "Resources");
		}

		[Fact]
		public void Accept()
		{
			Assert.True(resFactory.Accept(new CustomUri("file://something")));
			Assert.False(resFactory.Accept(new CustomUri("http://www.castleproject.org")));
		}

		[Fact]
		public void CreateWithAbsolutePath()
		{
			String file = Path.Combine(basePath, "file1.txt");

			FileInfo fileInfo = new FileInfo(file);

			CustomUri uri = new CustomUri(fileInfo.FullName);

			IResource resource = resFactory.Create(uri, null);

			Assert.NotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Something", line);
		}

		[Fact]
		public void CreateWithRelativePath()
		{
			IResource resource = resFactory.Create(new CustomUri(basePath + "/file1.txt"));

			Assert.NotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Something", line);
		}

		[Fact]
		public void CreateWithRelativePathAndContext()
		{
			CustomUri uri = new CustomUri("file://file1.txt");

			IResource resource = resFactory.Create(uri, basePath);

			Assert.NotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Something", line);
		}

		[Fact]
		public void NonExistingResource()
		{
			Assert.Throws<ResourceException>(() =>
			{
				resFactory.Create(new CustomUri(basePath + "/Something/file1.txt"))
					.GetStreamReader();
			});
		}
	}
}

#endif