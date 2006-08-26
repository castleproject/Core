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
	using Castle.Model.Resource;
	using NUnit.Framework;

	[TestFixture]
	public class CustomUriTestCase
	{
		[Test]
		public void FileUris()
		{
			CustomUri uri1 = new CustomUri("file://c:\\mydir\\properties.config");

			Assert.AreEqual("c:/mydir/properties.config", uri1.Path);
			Assert.AreEqual(null, uri1.Host);
			Assert.AreEqual("file", uri1.Scheme);
			Assert.AreEqual(true, uri1.IsFile);
			Assert.AreEqual(false, uri1.IsUnc);
		}

		[Test]
		public void FileUris2()
		{
			CustomUri uri1 = new CustomUri("file://Config/properties.config");

			Assert.AreEqual("Config/properties.config", uri1.Path);
			Assert.AreEqual(null, uri1.Host);
			Assert.AreEqual("file", uri1.Scheme);
			Assert.AreEqual(true, uri1.IsFile);
			Assert.AreEqual(false, uri1.IsUnc);
		}

		[Test]
		public void FileUris3()
		{
			CustomUri uri1 = new CustomUri("e:\\somedir\\somefile.extension");

			Assert.AreEqual("e:/somedir/somefile.extension", uri1.Path);
			Assert.AreEqual(null, uri1.Host);
			Assert.AreEqual("file", uri1.Scheme);
			Assert.AreEqual(true, uri1.IsFile);
			Assert.AreEqual(false, uri1.IsUnc);
		}

		[Test]
		public void AssemblyUri()
		{
			CustomUri uri1 = new CustomUri("assembly://Assembly.Name/properties.config");

			Assert.AreEqual("/properties.config", uri1.Path);
			Assert.AreEqual("Assembly.Name", uri1.Host);
			Assert.AreEqual("assembly", uri1.Scheme);
			Assert.AreEqual(false, uri1.IsFile);
			Assert.AreEqual(false, uri1.IsUnc);
		}

		[Test]
		public void AssemblyUri2()
		{
			CustomUri uri1 = new CustomUri("assembly://Assembly.Name/Some/Namespace/properties.config");

			Assert.AreEqual("/Some/Namespace/properties.config", uri1.Path);
			Assert.AreEqual("Assembly.Name", uri1.Host);
			Assert.AreEqual("assembly", uri1.Scheme);
			Assert.AreEqual(false, uri1.IsFile);
			Assert.AreEqual(false, uri1.IsUnc);
		}
	}
}
