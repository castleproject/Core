// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model.Tests.Resources
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.Model.Resource;


	[TestFixture]
	public class FileResourceFactoryTestCase
	{
		private FileResourceFactory resFactory = new FileResourceFactory();
		private String basePath = "../Castle.Model.Tests/Resources";

		[TestFixtureSetUp]
		public void Init()
		{
			basePath = Path.Combine(Directory.GetCurrentDirectory(), basePath); 
		}

		[Test]
		public void Accept()
		{
			Assert.IsTrue( resFactory.Accept( new Uri("file://something") ) );
			Assert.IsFalse( resFactory.Accept( new Uri("http://www.castleproject.org") ) );
		}

		[Test]
		public void CreateWithAbsolutePath()
		{
			String file = Path.Combine(basePath, "file1.txt");

			FileInfo fileInfo = new FileInfo(file);

			Uri uri = new Uri(fileInfo.FullName);

			IResource resource = resFactory.Create( uri, null );

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		public void CreateWithRelativePath()
		{
			IResource resource = resFactory.Create( new Uri(basePath + "/file1.txt") );

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		public void CreateWithRelativePathAndContext()
		{
			Uri uri = new Uri("file://file1.txt");

			IResource resource = resFactory.Create( uri, basePath );

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test]
		[ExpectedException(typeof(ResourceException))]
		public void NonExistingResource()
		{
			resFactory.Create( new Uri(basePath + "/Something/file1.txt") );
		}
	}
}
