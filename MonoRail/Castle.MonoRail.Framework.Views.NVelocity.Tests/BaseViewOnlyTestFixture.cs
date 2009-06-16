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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using Core;
	using Descriptors;
	using Framework.JSGeneration;
	using Framework.JSGeneration.Prototype;
	using global::NVelocity.Runtime.Directive;
	using Helpers;
	using NUnit.Framework;
	using Providers;
	using Resources;
	using Services;
	using Test;

	public abstract class BaseViewOnlyTestFixture : ServiceProviderLocator.IAccessorStrategy, IEngineContextLocator
	{
		protected string Action = "test_action";
		protected string Area;
		protected Controller Controller;
		protected ControllerContext ControllerContext;
		protected string ControllerName = "test_controller";
		protected HelperDictionary Helpers;
		protected string lastOutput;
		protected string[] Layouts;
		protected Hashtable PropertyBag;
		protected StubEngineContext EngineContext;
		protected StubResponse Response;
		protected NVelocityViewEngine VelocityViewEngine;
		protected DefaultViewComponentFactory ViewComponentFactory;
		protected string ViewSourcePath;

		protected BaseViewOnlyTestFixture()
			: this(ViewLocations.TestSiteNVelocity)
		{
			ServiceProviderLocator.Instance.AddLocatorStrategy(this);
			EngineContextLocator.Instance.AddLocatorStrategy(this);
		}

		protected BaseViewOnlyTestFixture(string viewSource)
		{
			ViewSourcePath = viewSource;
		}

		protected string Layout
		{
			set {
				Layouts = value == null ? null : new[] {value};
			}
		}

		[SetUp]
		public void SetUp()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;

			Layout = null;
			PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			Helpers = new HelperDictionary();
			var services = new StubMonoRailServices
			               	{
			               		UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine()),
			               		UrlTokenizer = new DefaultUrlTokenizer(),
			               		CacheProvider = new StubCacheProvider()
			               	};
			services.AddService(typeof(ICacheProvider), services.CacheProvider);

			var urlInfo = new UrlInfo(
				"example.org", "test", "", "http", 80,
				"http://test.example.org/test_area/test_controller/test_action.tdd",
				Area, ControllerName, Action, "tdd", "no.idea");
			Response = new StubResponse();
			EngineContext = new StubEngineContext(new StubRequest(), Response, services,
			                                          urlInfo);

			services.AddService(typeof(IEngineContext), EngineContext);
			EngineContext.AddService<IEngineContext>(EngineContext);

			EngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
			EngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);
			EngineContext.AddService<ICacheProvider>(services.CacheProvider);

			ViewComponentFactory = new DefaultViewComponentFactory();
			ViewComponentFactory.Service(EngineContext);
			ViewComponentFactory.Initialize();

			EngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
			services.AddService(typeof(IViewComponentFactory), ViewComponentFactory);

			EngineContext.AddService<IViewComponentDescriptorProvider>(new DefaultViewComponentDescriptorProvider());
			services.AddService(typeof(IViewComponentDescriptorProvider), EngineContext.GetService<IViewComponentDescriptorProvider>());

			ControllerContext = new ControllerContext { Helpers = Helpers, PropertyBag = PropertyBag };
			EngineContext.CurrentControllerContext = ControllerContext;

			Helpers["formhelper"] = Helpers["form"] = new FormHelper(EngineContext);
			Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(EngineContext);
			Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(EngineContext);
			Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(EngineContext);
			Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(EngineContext);

			string viewPath = Path.Combine(ViewSourcePath, "Views");

			var loader = new FileAssemblyViewSourceLoader(viewPath);

			services.ViewSourceLoader = loader;
			services.AddService(typeof(IViewSourceLoader), services.ViewSourceLoader);
			EngineContext.AddService<IViewSourceLoader>(services.ViewSourceLoader);

			Controller = new BaseTestFixtureController();
			Controller.Contextualize(EngineContext, ControllerContext);

			VelocityViewEngine = new NVelocityViewEngine();
			services.AddService(typeof(IViewEngine), VelocityViewEngine);
			EngineContext.AddService<IViewEngine>(VelocityViewEngine);

			VelocityViewEngine.SetViewSourceLoader(loader);
			VelocityViewEngine.Service(services);
			VelocityViewEngine.Initialize();

			var viewEngineManager = new DefaultViewEngineManager();
			viewEngineManager.RegisterEngineForExtesionLookup(VelocityViewEngine);
			services.EmailTemplateService = new EmailTemplateService(viewEngineManager);

			BeforEachTest();
		}

		protected virtual void BeforEachTest()
		{
		}


		public void ProcessView_StripRailsExtension(string url)
		{
			ProcessView(url.Replace(".rails", ""));
		}

		public void ProcessView_StripRailsExtension(string url, params String[] queryStringParams)
		{
			for(var i = 0; i < queryStringParams.Length; i++)
			{
				string[] parts = queryStringParams[i].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				PropertyBag[parts[0]] = parts[1];
			}
			ProcessView_StripRailsExtension(url);
		}

		protected string ProcessView(string templatePath)
		{
			var sw = new StringWriter();
			ControllerContext.LayoutNames = Layouts;
			EngineContext.CurrentControllerContext = ControllerContext;
			VelocityViewEngine.Process(templatePath, sw, EngineContext, Controller, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected string ProcessViewJS(string templatePath)
		{
			var sw = new StringWriter();
			ControllerContext.LayoutNames = Layouts;
			EngineContext.CurrentControllerContext = ControllerContext;
			var engineManager = new DefaultViewEngineManager();
			engineManager.RegisterEngineForView(VelocityViewEngine);
			engineManager.RegisterEngineForExtesionLookup((VelocityViewEngine));
			var codeGenerator =
				new JSCodeGenerator(EngineContext.Server,
				                    engineManager,
				                    EngineContext, null, ControllerContext, EngineContext.Services.UrlBuilder);

			IJSGenerator jsGen = new PrototypeGenerator(codeGenerator);

			codeGenerator.JSGenerator = jsGen;

			var info = new JSCodeGeneratorInfo(codeGenerator, jsGen,
			                                   new object[] {new ScriptaculousExtension(codeGenerator)},
			                                   new object[] {new ScriptaculousExtension(codeGenerator)});

			VelocityViewEngine.GenerateJS(templatePath, sw, info, EngineContext, null, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected void AddResource(string name, string resourceName, Assembly asm)
		{
			IResourceFactory resourceFactory = new DefaultResourceFactory();
			var descriptor = new ResourceDescriptor(
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

		protected string RenderStaticWithLayout(string staticText)
		{
			ControllerContext.LayoutNames = Layouts;
			EngineContext.CurrentControllerContext = ControllerContext;

			VelocityViewEngine.RenderStaticWithinLayout(staticText, EngineContext, null, ControllerContext);
			lastOutput = ((StringWriter) Response.Output)
				.GetStringBuilder().ToString();
			return lastOutput;
		}

		public void AssertReplyEqualTo(string expected)
		{
			Assert.AreEqual(expected, lastOutput);
		}

		public void AssertReplyContains(string contained)
		{
			StringAssert.Contains(contained, lastOutput);
		}

		public void AssertSuccess()
		{
			Assert.AreEqual(200, Response.StatusCode);
		}

		public void AssertStatusCode(int statusCode)
		{
			Assert.AreEqual(statusCode, Response.StatusCode);
		}

		/// <summary>
 		/// Asserts that the response was a redirect to the specified
 		/// <c>url</c>
 		/// </summary>
 		protected void AssertRedirectedTo(String url)
 		{
 			Assert.AreEqual(302, Response.StatusCode, "Redirect status not used");
 			AssertHasHeader("Location");
 			Assert.AreEqual(url, Response.Headers["Location"]);
 		}

  		/// <summary>
 		/// Asserts that response contains the specified header.
 		/// </summary>
 		/// <param name="headerName">value to assert to</param>
 		protected void AssertHasHeader(String headerName)
 		{
 				Assert.IsTrue(Response.Headers[headerName] != null, "Header '{0}' was not found", headerName);
 		}

		public IServiceProviderEx LocateProvider()
		{
			return EngineContext.Services;
		}

		public IEngineContext LocateCurrentContext()
		{
			return EngineContext;
		}
	}

	public class BaseTestFixtureController : Controller
	{
	}
}