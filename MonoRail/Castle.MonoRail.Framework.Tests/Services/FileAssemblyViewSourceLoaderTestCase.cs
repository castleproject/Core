// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System.IO;
	using Castle.MonoRail.Framework.Providers;
	using Castle.MonoRail.Framework.Services;
	using Container;
	using NUnit.Framework;
	
	[TestFixture]
	public class FileAssemblyViewSourceLoaderTestCase
	{
		private FileAssemblyViewSourceLoader loader;

		[SetUp]
		public void SetUp()
		{
			loader = new FileAssemblyViewSourceLoader();
			loader.ViewRootDir = Path.GetFullPath(System.Configuration.ConfigurationManager.AppSettings["tests.src"]);
		}

		[Test]
		public void LoadFromFileSystem()
		{
			Assert.IsFalse(loader.HasSource("contentinfs2.vm"));
			Assert.IsTrue(loader.HasSource("contentinfs.vm"));
			Assert.IsNotNull(loader.GetViewSource("contentinfs.vm"));
		}

		[Test]
		public void LoadFromAssembly()
		{
			loader.AddAssemblySource(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			Assert.IsFalse(loader.HasSource("Content/contentinassembly2.vm"));
			Assert.IsTrue(loader.HasSource("Content/contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content/contentinassembly.vm"));

			Assert.IsFalse(loader.HasSource("Content\\contentinassembly2.vm"));
			Assert.IsTrue(loader.HasSource("Content\\contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content\\contentinassembly.vm"));
		}

		[Test]
		public void ListViews()
		{
			loader.AddAssemblySource(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			string[] views = loader.ListViews("Content");
			
			Assert.IsNotNull(views);
			Assert.AreEqual(4, views.Length);
			Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "contentinassembly.vm", views[0]);
			Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "notinassembly.vm", views[1]);
            Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "zdonotlist.bad", views[2]);
			Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "contentinassembly.vm", views[3]);
            

			foreach(string view in views)
			{
				Assert.IsTrue(loader.HasSource(view));
				Assert.IsNotNull(loader.GetViewSource(view));
			}
		}
        [Test]
        public void ListViewsWithOptionalFileExtensions()
        {
            loader.AddAssemblySource(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

            string[] views = loader.ListViews("Content",".vm");

            Assert.IsNotNull(views);
            Assert.AreEqual(3, views.Length);
            Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "contentinassembly.vm", views[0]);
            Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "notinassembly.vm", views[1]);
            Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "contentinassembly.vm", views[2]);


            foreach (string view in views)
            {
                Assert.IsTrue(loader.HasSource(view));
                Assert.IsNotNull(loader.GetViewSource(view));
            }
        }
	}

	[TestFixture]
	public class FileAssemblyViewSourceLoaderWithoutViewDirectoryTestCase
	{
		private FileAssemblyViewSourceLoader loader;

		[SetUp]
		public void SetUp()
		{
			loader = new FileAssemblyViewSourceLoader();
			loader.ViewRootDir = Path.GetFullPath(@"c:\idontexist");
		}

		[Test]
		public void DoesNotThrowException_IfSubscribingToViewSourceChangedEvent_AndViewFolderIsMissing()
		{
			loader.ViewChanged += delegate
			                      {
			                      	//do nothing
			                      };
		}

		[Test]
		public void LoadFromAssembly()
		{
			loader.AddAssemblySource(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			Assert.IsFalse(loader.HasSource("Content/contentinassembly2.vm"));
			Assert.IsTrue(loader.HasSource("Content/contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content/contentinassembly.vm"));

			Assert.IsFalse(loader.HasSource("Content\\contentinassembly2.vm"));
			Assert.IsTrue(loader.HasSource("Content\\contentinassembly.vm"));
			Assert.IsNotNull(loader.GetViewSource("Content\\contentinassembly.vm"));
		}

		[Test]
		public void ListViews()
		{
			loader.AddAssemblySource(new AssemblySourceInfo("Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests"));

			string[] views = loader.ListViews("Content");

			Assert.IsNotNull(views);
			Assert.AreEqual(1, views.Length);
			Assert.AreEqual(@"Content" + Path.DirectorySeparatorChar + "contentinassembly.vm", views[0]);

			foreach (string view in views)
			{
				Assert.IsTrue(loader.HasSource(view));
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
			AddService(typeof(ITransformFilterDescriptorProvider), new DefaultTransformFilterDescriptorProvider());
		}
	}
}
