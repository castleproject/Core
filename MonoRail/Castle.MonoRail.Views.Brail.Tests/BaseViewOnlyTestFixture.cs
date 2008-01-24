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

namespace Castle.MonoRail.Views.Brail.Tests
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using Castle.MonoRail.Framework.Helpers;
    using Castle.MonoRail.Framework.Services;
    using Castle.MonoRail.Framework.Test;
    using Castle.MonoRail.Framework.ViewComponents;
    using Framework;
    using NUnit.Framework;

    public class BaseViewOnlyTestFixture
    {
        private readonly string viewSourcePath;
        protected BooViewEngineOptions BrailOptions;
        protected ControllerContext ControllerContext;
        protected Hashtable Helpers;
        private string lastOutput;
        protected string Layout;
        protected MockEngineContext MockEngineContext;
        protected Hashtable PropertyBag;
        protected string Area = null;
        protected string ControllerName = "test_controller";
        protected string Action = "test_action";
    	protected DefaultViewComponentFactory ViewComponentFactory ;

    	public BaseViewOnlyTestFixture()
            : this("../../../TestSiteBrail")
        {
        }

        public BaseViewOnlyTestFixture(string viewSource)
        {
            viewSourcePath = viewSource;
        }


        public string ViewSourcePath
        {
            get { return viewSourcePath; }
        }

        [SetUp]
        public void SetUp()
        {
            Layout = null;
            PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            Helpers = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            BrailOptions = new BooViewEngineOptions();
            MockServices services = new MockServices();
            services.UrlBuilder = new DefaultUrlBuilder(new MockServerUtility(),new MockRoutingEngine());
            services.UrlTokenizer = new DefaultUrlTokenizer();
            UrlInfo urlInfo = new UrlInfo(
                "example.org", "test", "/TestBrail", "http", 80,
                "http://test.example.org/test_area/test_controller/test_action.tdd",
                Area, ControllerName, Action, "tdd", "no.idea");
            MockEngineContext = new MockEngineContext(new MockRequest(), new MockResponse(), services,
                                                      urlInfo);
            MockEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
            MockEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);
			
			ViewComponentFactory = new DefaultViewComponentFactory();
			ViewComponentFactory.Service(MockEngineContext);
        	ViewComponentFactory.Initialize();
			
			MockEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
            ControllerContext = new ControllerContext();
            ControllerContext.Helpers = Helpers;
            ControllerContext.PropertyBag = PropertyBag;
            MockEngineContext.CurrentControllerContext = ControllerContext;


            Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(MockEngineContext);
            Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(MockEngineContext);
			Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(MockEngineContext);
				Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(MockEngineContext);
        }


        public void ProcessView_StripRailsExtension(string url)
        {
            ProcessView(url.Replace(".rails", ""));
        }

        protected string ProcessView(string templatePath)
        {
            BrailOptions.SaveDirectory = Environment.CurrentDirectory;
            BrailOptions.SaveToDisk = true;
            BrailOptions.Debug = true;
            BrailOptions.BatchCompile = false;
            string viewPath = Path.Combine(viewSourcePath, "Views");

            FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader(viewPath);
            loader.AddAssemblySource(
                new AssemblySourceInfo(Assembly.GetExecutingAssembly().FullName,
                                       "Castle.MonoRail.Views.Brail.Tests.ResourcedViews"));
            BooViewEngine engine = new BooViewEngine();
            engine.Options = BrailOptions;
            engine.SetViewSourceLoader(loader);
            engine.Initialize();
            StringWriter sw = new StringWriter();
            if (string.IsNullOrEmpty(Layout) == false)
                ControllerContext.LayoutNames = new string[] {Layout,};
            MockEngineContext.CurrentControllerContext = ControllerContext;
            engine.Process(templatePath, sw, MockEngineContext, null, ControllerContext);
            lastOutput = sw.ToString();
            return lastOutput;
        }

        public void AssertReplyEqualTo(string actual)
        {
            Assert.AreEqual(actual, lastOutput);
        }

        public void AssertReplyContains(string contained)
        {
            StringAssert.Contains(contained, lastOutput);
        }
    }
}