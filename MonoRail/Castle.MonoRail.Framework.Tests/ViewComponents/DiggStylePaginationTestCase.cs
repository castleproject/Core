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

namespace Castle.MonoRail.Framework.Tests.ViewComponents
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Castle.Components.Pagination;
	using Castle.MonoRail.Framework.ViewComponents;
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;

	[TestFixture]
	public class DiggStylePaginationTestCase : BaseViewComponentTest
	{
		private DiggStylePagination diggComponent;
		private IPaginatedPage singlePage, secondPageOfThree;

		[SetUp]
		public void Init()
		{
			diggComponent = new DiggStylePagination();

			singlePage = new Page(new string[] {"a", "b", "c"}, 1, 4, 1);
			secondPageOfThree = new Page(new string[] {"a", "b", "c", "d"}, 2, 4, 10);

			BuildEngineContext("area", "controller", "action");
		}

		[TearDown]
		public void Terminate()
		{
			CleanUp();
		}

		[Test, ExpectedException(typeof(ViewComponentException), ExpectedMessage = "The DiggStylePagination requires a view component " +
		                                                         "parameter named 'page' which should contain 'IPaginatedPage' instance"
			)]
		public void ThrowsExceptionIfNoPageWasSupplied()
		{
			diggComponent.Page = null;
			diggComponent.Initialize();
		}

		[Test]
		public void PageWithNoLinksInvokesStartAndEndSections()
		{
			List<string> actions = new List<string>();

			SectionRender["startblock"] = delegate { actions.Add("started"); };
			SectionRender["endblock"] = delegate { actions.Add("ended"); };
			SectionRender["link"] = delegate { actions.Add("link"); };

			diggComponent.Page = singlePage;
			Request.FilePath = "/something";
			PrepareViewComponent(diggComponent);
			diggComponent.Render();

			Assert.AreEqual(2, actions.Count);
			Assert.AreEqual("started", actions[0]);
			Assert.AreEqual("ended", actions[1]);
		}

		[Test]
		public void PageWithLinksInvokesStartAndEndAndLinkSections()
		{
			List<string> actions = new List<string>();

			SectionRender["startblock"] = delegate { actions.Add("started"); };
			SectionRender["endblock"] = delegate { actions.Add("ended"); };
			SectionRender["link"] = delegate { actions.Add("link"); };
			
			diggComponent.Page = secondPageOfThree;
			Request.FilePath = "/something";
			PrepareViewComponent(diggComponent);
			diggComponent.Render();

			Assert.AreEqual(6, actions.Count);
			Assert.AreEqual("started", actions[0]);
			Assert.AreEqual("link", actions[1]);
			Assert.AreEqual("link", actions[2]);
			Assert.AreEqual("link", actions[3]);
			Assert.AreEqual("link", actions[4]);
			Assert.AreEqual("ended", actions[5]);
		}

		[Test]
		public void PageWithNoLinksPrintsNoLinks()
		{
			SectionRender["startblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("started"); };
			SectionRender["endblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("ended"); };

			diggComponent.UseInlineStyle = false;
			diggComponent.Page = singlePage;
			Request.FilePath = "/something";
			PrepareViewComponent(diggComponent);
			diggComponent.Render();

			Assert.AreEqual("started<span class=\"disabled\">&laquo; prev</span>\r\n" + 
				"<span class=\"current\">1</span>\r\n" +
				"<span class=\"disabled\">next &raquo;</span>ended", Output);
		}

		[Test]
		public void PageWithPrevNextPrintsCustomizedLinks()
		{
			SectionRender["startblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("started"); };
			SectionRender["endblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("ended"); };
			SectionRender["prev"] = delegate(IDictionary context, TextWriter writer) { writer.Write("customprev"); };
			SectionRender["next"] = delegate(IDictionary context, TextWriter writer) { writer.Write("customnext"); };

			diggComponent.UseInlineStyle = false;
			diggComponent.Page = singlePage;
			Request.FilePath = "/something";
			PrepareViewComponent(diggComponent);
			diggComponent.Render();

			Assert.AreEqual("started<span class=\"disabled\">customprev</span>\r\n" +
				"<span class=\"current\">1</span>\r\n" +
				"<span class=\"disabled\">customnext</span>ended", Output);
		}

		[Test]
		public void PageWithLinksPrintsLinks()
		{
			SectionRender["startblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("started"); };
			SectionRender["endblock"] = delegate(IDictionary context, TextWriter writer) { writer.Write("ended"); };
			SectionRender["link"] = delegate(IDictionary context, TextWriter writer)
			{
				writer.Write(" <{0} {1} {2}> ", context["pageIndex"], context["url"], context["text"]);
			};

			diggComponent.UseInlineStyle = false;
			diggComponent.Page = secondPageOfThree;
			Request.FilePath = "/something";
			PrepareViewComponent(diggComponent);
			diggComponent.Render();

			Assert.AreEqual("started <1 /something?page=1 &laquo; prev>  " +
				"<1 /something?page=1 1> \r\n<span class=\"current\">2</span>\r\n " +
				"<3 /something?page=3 3>  <3 /something?page=3 next &raquo;> ended", Output);
		}

		[Test]
		public void SupportsSections()
		{
			Assert.IsTrue(diggComponent.SupportsSection("startblock"), "Supports startblock Section");
			Assert.IsTrue(diggComponent.SupportsSection("endblock"), "Supports endblock Section");
			Assert.IsTrue(diggComponent.SupportsSection("link"), "Supports Links Section");
			Assert.IsFalse(diggComponent.SupportsSection("NotSupported"), "Unsupported section");
		}
	}
}
