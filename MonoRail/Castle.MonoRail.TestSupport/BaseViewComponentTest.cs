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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Test;

	/// <summary>
	/// Base class to test view components.
	/// </summary>
	/// 
	/// <example>
	/// The following test makes sure the component rendered the inner sections correctly.
	/// <code lang="cs">
	/// [TestFixture]
	/// public class DiggStylePaginationTestCase : BaseViewComponentTest
	/// {
	///		private DiggStylePagination diggComponent;
	///		private IPaginatedPage singlePage, secondPageOfThree;
	///		
	///		[SetUp]
	///		public void Init()
	///		{
	///			diggComponent = new DiggStylePagination();
	///			singlePage = new Page(new string[] { "a", "b", "c" }, 1, 4, 1);
	///			secondPageOfThree = new Page(new string[] { "a", "b", "c", "d" }, 2, 4, 10);
	///		}
	/// 
	///		[TearDown]
	///		public void Terminate()
	///		{
	///			CleanUp();
	///		}
	/// 
	///		[Test]
	///		public void PageWithNoLinksInvokesStartAndEndSections()
	///		{
	///			List&lt;string&gt; actions = new List&lt;string&gt;();
	///			// pass mock inner sections to component
	///			SectionRender["startblock"] = delegate(IDictionary context, TextWriter writer) { actions.Add("started"); };
	///			SectionRender["endblock"] = delegate(IDictionary context, TextWriter writer) { actions.Add("ended"); };
	///			SectionRender["link"] = delegate(IDictionary context, TextWriter writer) { actions.Add("link"); };
	///			
	///			diggComponent.Page = singlePage;
	///			
	///			PrepareViewComponent(diggComponent);
	///			diggComponent.Render();
	/// 
	///			// make sure component "rendered" inner sections
	///			Assert.AreEqual(2, actions.Count);
	///			Assert.AreEqual("started", actions[0]);
	///			Assert.AreEqual("ended", actions[1]);
	///		}
	/// }
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// You must call <see cref="PrepareViewComponent"/> before testing a view component instance
	/// and you should call <see cref="CleanUp"/> after each test case (use the TearDown).
	/// </remarks>
	public abstract class BaseViewComponentTest : BaseControllerTest
	{
		private IMockViewComponentContext componentContext;
		private IViewEngine viewEngine;
		private StringWriter writer;

		/// <summary>
		/// Use this dictionary to add inner sections as available inner sections to 
		/// the view component.  
		/// </summary>
		public IDictionary<string, TestSectionRender> SectionRender;

		/// <summary>
		/// This delegate is called when the viewcomponent renders its body.
		/// </summary>
		public TestSectionRender OnBodyRender;

		/// <summary>
		/// This delegate is called when the viewcomponent renders a view
		/// </summary>
		public TestViewRender OnViewRender;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseViewComponentTest"/> class.
		/// </summary>
		protected BaseViewComponentTest()
		{
			CleanUp();
		}

		/// <summary>
		/// Initialize the view component with mock services it needs to 
		/// be functional.
		/// </summary>
		/// <param name="component">The component instance.</param>
		protected void PrepareViewComponent(ViewComponent component)
		{
			if (Context == null)
			{
				BuildEngineContext("", "Controller", "Action");
			}

			viewEngine = BuildViewEngine();

			componentContext = BuildViewComponentContext(component.GetType().Name);

			component.Init(Context, componentContext);
		}

		/// <summary>
		/// Gets the output -- ie what the viewcomponent wrote to the output stream.
		/// </summary>
		/// <value>The output.</value>
		protected string Output
		{
			get { return writer.ToString(); }
		}

		/// <summary>
		/// Cleans the up all state created to test a view component.
		/// </summary>
		protected void CleanUp()
		{
			writer = new StringWriter();
			SectionRender = new Dictionary<string, TestSectionRender>(StringComparer.InvariantCultureIgnoreCase);
			OnBodyRender = null;
			OnViewRender = null;
		}

		/// <summary>
		/// Builds the view engine.
		/// </summary>
		/// <returns></returns>
		protected virtual IViewEngine BuildViewEngine()
		{
			return new ViewEngineStub(".view", ".jsview", true);
		}

		/// <summary>
		/// Builds the view component context.
		/// </summary>
		/// <param name="viewComponentName">Name of the view component.</param>
		/// <returns></returns>
		protected virtual IMockViewComponentContext BuildViewComponentContext(string viewComponentName)
		{
			StubViewComponentContext compContext = new StubViewComponentContext(viewComponentName, writer, viewEngine);

			compContext.SectionRender = SectionRender;
			compContext.OnBodyRender = OnBodyRender;
			compContext.OnViewRender = OnViewRender;

			return compContext;
		}
	}
}
