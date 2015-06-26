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

	using Xunit;

	public class AssemblyResourceFactoryTestCase
	{
		public AssemblyResourceFactoryTestCase()
		{
			resFactory = new AssemblyResourceFactory();
		}

		private AssemblyResourceFactory resFactory;
#if NETCORE
	// TODO: Replace GetExecutingAssembly
        private static readonly String AssemblyName = "Castle.Core.Tests_NETCORE";
#else
		private static readonly String AssemblyName = Assembly.GetExecutingAssembly().FullName;
#endif
		private const String ResPath = "Resources";

		[Fact]
		public void Accept()
		{
			Assert.True(resFactory.Accept(new CustomUri("assembly://something/")));
			Assert.False(resFactory.Accept(new CustomUri("file://something")));
			Assert.False(resFactory.Accept(new CustomUri("http://www.castleproject.org")));
		}

#if NETCORE
		[Fact(Skip = "Assembly.Load(string) is not present on .NET Core")]
#else
		[Fact]
#endif
		public void CanHandleBundleResource()
		{
			IResource resource =
				new AssemblyBundleResource(
					new CustomUri("assembly://" + AssemblyName + "/CastleTests.Core.Tests.Resources.MoreRes.TestRes/content1"));

			Assert.NotNull(resource);
			var line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Content content", line);
		}

#if NETCORE
		[Fact(Skip = "Assembly.Load(string) is not present on .NET Core")]
#else
		[Fact]
#endif
		public void CreateWithAbsolutePath()
		{
			var resource =
				resFactory.Create(new CustomUri("assembly://" + AssemblyName + "/CastleTests.Core.Tests.Resources.file1.txt"));

			Assert.NotNull(resource);
			var line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Something", line);
		}
	}
}