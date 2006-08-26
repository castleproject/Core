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

namespace Castle.Core.Tests.Resources
{
	using System;

	using NUnit.Framework;

	using Castle.Core.Resource;


	[TestFixture]
	public class UncResourceFactoryTestCase
	{
		private UncResourceFactory resFactory = new UncResourceFactory();

		[Test]
		public void Accept()
		{
			Assert.IsTrue( resFactory.Accept( new CustomUri(@"\\server\something") ) );
			Assert.IsFalse( resFactory.Accept( new CustomUri("http://www.castleproject.org") ) );
		}

		[Test, Explicit]
		public void CreateWithAbsolutePath()
		{
			CustomUri uri = new CustomUri(@"\\hammet\C$\file.txt");

			IResource resource = resFactory.Create(uri, null);

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("The long and winding road", line);
		}

		[Test, Explicit]
		public void CreateRelative()
		{
			CustomUri uri = new CustomUri(@"\\hammet\C$\file.txt");

			IResource resource = resFactory.Create( uri, null );

			resource = resource.CreateRelative("file2.txt");

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}

		[Test, Explicit]
		[ExpectedException(typeof(ResourceException))]
		public void NonExistingResource()
		{
			resFactory.Create( new CustomUri(@"\\hammettz\C$\file1.txt") );
		}
	}
}
