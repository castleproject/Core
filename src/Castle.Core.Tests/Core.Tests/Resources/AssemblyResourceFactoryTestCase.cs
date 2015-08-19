// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace CastleTests.Core.Tests.Resources
{
	using System;
	using System.Reflection;

	using Castle.Core.Resource;

	using NUnit.Framework;

	[TestFixture]
	public class AssemblyResourceFactoryTestCase
	{
#if FEATURE_XUNITNET
		public AssemblyResourceFactoryTestCase()
#else
		[SetUp]
		public void SetUp()
#endif
		{
			resFactory = new AssemblyResourceFactory();
		}

		private AssemblyResourceFactory resFactory;
		private static readonly String AssemblyName = Assembly.GetExecutingAssembly().FullName;
		private const String ResPath = "Resources";

		[Test]
		public void Accept()
		{
			Assert.IsTrue(resFactory.Accept(new CustomUri("assembly://something/")));
			Assert.IsFalse(resFactory.Accept(new CustomUri("file://something")));
			Assert.IsFalse(resFactory.Accept(new CustomUri("http://www.castleproject.org")));
		}

		[Test]
		public void CanHandleBundleResource()
		{
			IResource resource =
				new AssemblyBundleResource(
					new CustomUri("assembly://" + AssemblyName + "/CastleTests.Core.Tests.Resources.MoreRes.TestRes/content1"));

			Assert.IsNotNull(resource);
			var line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Content content", line);
		}

		[Test]
		public void CreateWithAbsolutePath()
		{
			var resource = resFactory.Create(new CustomUri("assembly://" + AssemblyName + "/CastleTests.Core.Tests.Resources.file1.txt"));

			Assert.IsNotNull(resource);
			var line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}
	}
}