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

namespace Castle.Core.Tests.Resources
{
	using System;

	using Castle.Core.Resource;

	using Xunit;

	public class UncResourceFactoryTestCase
	{
		private UncResourceFactory resFactory;

		public UncResourceFactoryTestCase()
		{
			resFactory = new UncResourceFactory();
		}

		[Fact]
		public void Accept()
		{
			Assert.True(resFactory.Accept(new CustomUri(@"\\server\something")));
			Assert.False(resFactory.Accept(new CustomUri("http://www.castleproject.org")));
		}

#if !SILVERLIGHT && !NETCORE // Silverlight test runner does not handle explicit tests

		[Fact(Skip = "Explicit")]
		public void CreateWithAbsolutePath()
		{
			CustomUri uri = new CustomUri(@"\\hammet\C$\file.txt");

			IResource resource = resFactory.Create(uri, null);

			Assert.NotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.Equal("The long and winding road", line);
		}

		[Fact(Skip = "Explicit")]
		public void CreateRelative()
		{
			CustomUri uri = new CustomUri(@"\\hammet\C$\file.txt");

			IResource resource = resFactory.Create(uri, null);

			resource = resource.CreateRelative("file2.txt");

			Assert.NotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.Equal("Something", line);
		}

		[Fact(Skip = "Explicit")]
		public void NonExistingResource()
		{
			Assert.Throws<ResourceException>(() =>
			{
				resFactory.Create(new CustomUri(@"\\hammettz\C$\file1.txt"))
					.GetStreamReader();
			});
		}
#endif
	}
}