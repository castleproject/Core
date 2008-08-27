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

namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Core.Logging;
	using AspView.Compiler;
	using Framework.Descriptors;
	using Framework.Helpers;
	using Framework.Resources;
	using Framework.Services;
	using Framework.Test;
	using Framework;
	using NUnit.Framework;

	public abstract class IntegrationViewTestFixture
	{
		protected ControllerContext ControllerContext;
		protected HelperDictionary Helpers;
		private string lastOutput;
		protected string Layout;
		protected StubEngineContext StubEngineContext;
		protected Hashtable PropertyBag;
		protected string Area = null;
		protected string ControllerName = "test_controller";
		protected string Action = "test_action";
		protected DefaultViewComponentFactory ViewComponentFactory;
		protected AspViewEngine viewEngine;

		protected IntegrationViewTestFixture()
		{
			SetUp();
		}

		[SetUp]
		public void SetUp()
		{
			string siteRoot = GetSiteRoot();
			string viewPath = Path.Combine(siteRoot, "RenderingTests\\Views");
			Layout = null;
			PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			Helpers = new HelperDictionary();
			StubMonoRailServices services = new StubMonoRailServices();
			services.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine());
			services.UrlTokenizer = new DefaultUrlTokenizer();
			UrlInfo urlInfo = new UrlInfo(
				"example.org", "test", "/TestBrail", "http", 80,
				"http://test.example.org/test_area/test_controller/test_action.tdd",
				Area, ControllerName, Action, "tdd", "no.idea");
			StubEngineContext = new StubEngineContext(new StubRequest(), new StubResponse(), services,
													  urlInfo);
			StubEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
			StubEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);
			StubEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
			StubEngineContext.AddService<ILoggerFactory>(new ConsoleFactory());
			StubEngineContext.AddService<IViewSourceLoader>(new FileAssemblyViewSourceLoader(viewPath));
			

			ViewComponentFactory = new DefaultViewComponentFactory();
			ViewComponentFactory.Service(StubEngineContext);
			ViewComponentFactory.Initialize();

			ControllerContext = new ControllerContext();
			ControllerContext.Helpers = Helpers;
			ControllerContext.PropertyBag = PropertyBag;
			StubEngineContext.CurrentControllerContext = ControllerContext;


			Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(StubEngineContext);
			Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(StubEngineContext);
			Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(StubEngineContext);
			Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(StubEngineContext);


			//FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader(viewPath);
//			loader.AddAssemblySource(
//				new AssemblySourceInfo(Assembly.GetExecutingAssembly().FullName,
//									   "Castle.MonoRail.Views.Brail.Tests.ResourcedViews"));

			viewEngine = new AspViewEngine();
			viewEngine.Service(StubEngineContext);
			AspViewEngineOptions options = new AspViewEngineOptions();
			options.CompilerOptions.AutoRecompilation = true;
			options.CompilerOptions.KeepTemporarySourceFiles = false;
			ICompilationContext context = 
				new CompilationContext(
					new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),
					new DirectoryInfo(siteRoot),
					new DirectoryInfo(Path.Combine(siteRoot, "RenderingTests\\Views")),
					new DirectoryInfo(siteRoot));

			List<ICompilationContext> compilationContexts = new List<ICompilationContext>();
			compilationContexts.Add(context);
			viewEngine.Initialize(compilationContexts, options);
		}

		public string ProcessView(string templatePath)
		{
			StringWriter sw = new StringWriter();
			if (string.IsNullOrEmpty(Layout) == false)
				ControllerContext.LayoutNames = Layout.Split(',');
			StubEngineContext.CurrentControllerContext = ControllerContext;
			viewEngine.Process(templatePath, sw, StubEngineContext, null, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected virtual string GetSiteRoot()
		{
			DirectoryInfo siteRoot = GetSiteRootWhenRunningAsPartOfCastleBuild();
			if (siteRoot == null)
			{
				siteRoot = GetSiteRootWhenRunningInVisualStudio();
			}

			if (siteRoot == null) throw new Exception("Cannot resolve site root");
			return siteRoot.FullName;
		}

		private static DirectoryInfo GetSiteRootWhenRunningInVisualStudio()
		{
			DirectoryInfo current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			while (current != null && current.Name != "Castle.MonoRail.Views.AspView.Tests")
			{
				current = current.Parent;
			}
			return current;
		}

		private static DirectoryInfo GetSiteRootWhenRunningAsPartOfCastleBuild()
		{
			DirectoryInfo current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			DirectoryInfo[] directories = current.GetDirectories("AspView_TestViews_SiteRoot");

			if (directories.Length == 1)
			{
				return directories[0];
			}

			return null;
		}

		protected void AddResource(string name, string resourceName, Assembly asm)
		{
			IResourceFactory resourceFactory = new DefaultResourceFactory();
			ResourceDescriptor descriptor = new ResourceDescriptor(
				null,
				name,
				resourceName,
				null,
				null);
			IResource resource = resourceFactory.Create(
				descriptor,
				asm);
			ControllerContext.Resources.Add(name, resource);
		}

		/*
		protected string RenderStaticWithLayout(string staticText)
		{
			if (string.IsNullOrEmpty(Layout) == false)
				ControllerContext.LayoutNames = new string[] { Layout, };
			StubEngineContext.CurrentControllerContext = ControllerContext;

			BooViewEngine.RenderStaticWithinLayout(staticText, StubEngineContext, null, ControllerContext);
			lastOutput = ((StringWriter)StubEngineContext.Response.Output)
				.GetStringBuilder().ToString();
			return lastOutput;
		}
		*/
		public void AssertReplyEqualTo(string expected)
		{
			Assert.AreEqual(expected, lastOutput);
		}

		public void AssertReplyContains(string contained)
		{
			Assert.IsTrue(lastOutput.Contains(contained));
		}
	}
}
