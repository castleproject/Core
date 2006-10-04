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

namespace Castle.MonoRail.Framework.Tests
{
	using System.IO;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services;

	using NUnit.Framework;

	
	[TestFixture]
	public class FileAssemblyViewSourceLoaderTestCase
	{
		private FileAssemblyViewSourceLoader loader;

		[SetUp]
		public void SetUp()
		{
			loader = new FileAssemblyViewSourceLoader();
#if DOTNET2
			loader.ViewRootDir = Path.GetFullPath(System.Configuration.ConfigurationManager.AppSettings["tests.src"]);
#else
			loader.ViewRootDir = Path.GetFullPath(System.Configuration.ConfigurationSettings.AppSettings["tests.src"]);
#endif
			loader.Service(new TestServiceContainer());
		}

		[Test]
		public void LoadFromFileSystem()
		{
			Assert.IsFalse(loader.HasTemplate("contentinfs2.vm"));
			Assert.IsTrue(loader.HasTemplate("contentinfs.vm"));
			Assert.IsNotNull(loader.GetViewSource("contentinfs.vm"));
		}

		[Test]
		public void LoadFromAssembly()
		{
			loader.AdditionalSources.Add(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			Assert.IsFalse(loader.HasTemplate("Content/contentinassembly2.vm"));
			Assert.IsTrue(loader.HasTemplate("Content/contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content/contentinassembly.vm"));

			Assert.IsFalse(loader.HasTemplate("Content\\contentinassembly2.vm"));
			Assert.IsTrue(loader.HasTemplate("Content\\contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content\\contentinassembly.vm"));
		}

		[Test]
		public void ListViews()
		{
			loader.AdditionalSources.Add(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			string[] views = loader.ListViews("Content");
			
			Assert.IsNotNull(views);
			Assert.AreEqual(3, views.Length);
			Assert.AreEqual(@"Content\contentinassembly.vm", views[0]);
			Assert.AreEqual(@"Content\notinassembly.vm", views[1]);
			Assert.AreEqual(@"content.contentinassembly.vm", views[2]);

			foreach(string view in views)
			{
				Assert.IsTrue(loader.HasTemplate(view));
				Assert.IsNotNull(loader.GetViewSource(view));
			}
		}
	}

	internal class TestServiceContainer : AbstractServiceContainer
	{
		public TestServiceContainer()
		{
			AddService(typeof(IControllerTree), new DefaultControllerTree());
			AddService(typeof(IResourceDescriptorProvider), new DefaultResourceDescriptorProvider());
			AddService(typeof(IRescueDescriptorProvider), new DefaultRescueDescriptorProvider());
			AddService(typeof(ILayoutDescriptorProvider), new DefaultLayoutDescriptorProvider());
			AddService(typeof(IHelperDescriptorProvider), new DefaultHelperDescriptorProvider());
			AddService(typeof(IFilterDescriptorProvider), new DefaultFilterDescriptorProvider());
		}
	}
}
