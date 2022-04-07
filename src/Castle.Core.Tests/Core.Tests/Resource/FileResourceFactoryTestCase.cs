// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Resource.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	[TestFixture]
	public class FileResourceFactoryTestCase
	{
		private readonly FileResourceFactory resFactory = new FileResourceFactory();
		private string basePath;

		[SetUp]
		public void Init()
		{
			var currentDirectory = TestContext.CurrentContext.TestDirectory;
			basePath = Path.Combine(currentDirectory, "Core.Tests" + Path.DirectorySeparatorChar + "Resource");
		}

		[Test]
		public void Accept()
		{
			Assert.IsTrue( resFactory.Accept( new CustomUri("file://something") ) );
			Assert.IsFalse( resFactory.Accept( new CustomUri("http://www.castleproject.org") ) );
		}

		[Test]
		public void CreateWithAbsolutePath()
		{
			string file = Path.Combine(basePath, "file1.txt");

			FileInfo fileInfo = new FileInfo(file);

			CustomUri uri = new CustomUri(fileInfo.FullName);

			IResource resource = resFactory.Create(uri, null);

			Assert.IsNotNull(resource);
			string line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		public void CreateWithRelativePath()
		{
			IResource resource = resFactory.Create( new CustomUri(basePath + "/file1.txt") );

			Assert.IsNotNull(resource);
			string line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		public void CreateWithRelativePathAndContext()
		{
			CustomUri uri = new CustomUri("file://file1.txt");

			IResource resource = resFactory.Create( uri, basePath );

			Assert.IsNotNull(resource);
			string line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		public void NonExistingResource()
		{
			IResource resource = resFactory.Create(new CustomUri(basePath + "/Something/file1.txt"));

			Assert.Throws<ResourceException>(() => resource.GetStreamReader());
		}
	}
}
