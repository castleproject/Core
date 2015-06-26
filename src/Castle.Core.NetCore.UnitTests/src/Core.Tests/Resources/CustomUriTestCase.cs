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

	public class CustomUriTestCase
	{
		[Fact]
		public void FileUris()
		{
			CustomUri uri1 = new CustomUri("file://c:\\mydir\\properties.config");

			Assert.Equal("c:/mydir/properties.config", uri1.Path);
			Assert.Equal(null, uri1.Host);
			Assert.Equal("file", uri1.Scheme);
			Assert.Equal(true, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}

		[Fact]
		public void FileUris2()
		{
			CustomUri uri1 = new CustomUri("file://Config/properties.config");

			Assert.Equal("Config/properties.config", uri1.Path);
			Assert.Equal(null, uri1.Host);
			Assert.Equal("file", uri1.Scheme);
			Assert.Equal(true, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}

		[Fact]
		public void FileUris3()
		{
			CustomUri uri1 = new CustomUri("e:\\somedir\\somefile.extension");

			Assert.Equal("e:/somedir/somefile.extension", uri1.Path);
			Assert.Equal(null, uri1.Host);
			Assert.Equal("file", uri1.Scheme);
			Assert.Equal(true, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void UriWithEnvironmentVariable()
		{
			string path = Environment.GetEnvironmentVariable("PATH");
			Assert.NotEmpty(path);

			CustomUri uri1 = new CustomUri("file://%PATH%");

			Assert.Equal(path, uri1.Path);
			Assert.Equal(null, uri1.Host);
			Assert.Equal("file", uri1.Scheme);
			Assert.Equal(true, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}
#endif

		[Fact]
		public void AssemblyUri()
		{
			CustomUri uri1 = new CustomUri("assembly://Assembly.Name/properties.config");

			Assert.Equal("/properties.config", uri1.Path);
			Assert.Equal("Assembly.Name", uri1.Host);
			Assert.Equal("assembly", uri1.Scheme);
			Assert.Equal(false, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}

		[Fact]
		public void AssemblyUri2()
		{
			CustomUri uri1 = new CustomUri("assembly://Assembly.Name/Some/Namespace/properties.config");

			Assert.Equal("/Some/Namespace/properties.config", uri1.Path);
			Assert.Equal("Assembly.Name", uri1.Host);
			Assert.Equal("assembly", uri1.Scheme);
			Assert.Equal(false, uri1.IsFile);
			Assert.Equal(false, uri1.IsUnc);
		}
	}
}