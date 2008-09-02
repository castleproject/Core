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
	using System.Collections.Generic;
	using Castle.Components.Pagination;
	using Castle.MonoRail.Framework.ViewComponents;
	using NUnit.Framework;
	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class SelectStylePaginationTestCase : BaseViewComponentTest
	{
		private SelectStylePagination component;
		private IPaginatedPage emptyPage, singlePage, secondPageOfThree;

		[SetUp]
		public void Init()
		{
			component = new SelectStylePagination();

			emptyPage = new Page(new string[0], 1, 10, 0);
			singlePage = new Page(new string[] { "a", "b", "c" }, 1, 4, 1);
			secondPageOfThree = new Page(new string[] { "a", "b", "c", "d" }, 2, 4, 10);

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
			component.Page = null;
			component.Initialize();
		}

		[Test]
		public void SectionsAreCorrectlyUsedWhenSupplied()
		{
			List<string> actions = new List<string>();

			SectionRender["startblock"] = delegate { actions.Add("started"); };
			SectionRender["endblock"] = delegate { actions.Add("ended"); };
			SectionRender["first"] = delegate { actions.Add("first"); };
			SectionRender["last"] = delegate { actions.Add("last"); };
			SectionRender["next"] = delegate { actions.Add("next"); };
			SectionRender["prev"] = delegate { actions.Add("prev"); };
			SectionRender["link"] = delegate { actions.Add("link"); };
			SectionRender["select"] = delegate { actions.Add("select"); };

			component.Page = singlePage;
			Request.FilePath = "/something";
			PrepareViewComponent(component);
			component.Render();

			Assert.AreEqual(7, actions.Count);
			Assert.AreEqual("started", actions[0]);
			Assert.AreEqual("first", actions[1]);
			Assert.AreEqual("prev", actions[2]);
			Assert.AreEqual("select", actions[3]);
			Assert.AreEqual("next", actions[4]);
			Assert.AreEqual("last", actions[5]);
			Assert.AreEqual("ended", actions[6]);
		}

		[Test]
		public void LinksAreNotRenderedWhenPageIsEmpty()
		{
			List<string> actions = new List<string>();

			SectionRender["startblock"] = delegate { actions.Add("started"); };
			SectionRender["endblock"] = delegate { actions.Add("ended"); };
			SectionRender["first"] = delegate { actions.Add("first"); };
			SectionRender["last"] = delegate { actions.Add("last"); };
			SectionRender["next"] = delegate { actions.Add("next"); };
			SectionRender["prev"] = delegate { actions.Add("prev"); };
			SectionRender["link"] = delegate { actions.Add("link"); };
			SectionRender["select"] = delegate { actions.Add("select"); };

			component.Page = emptyPage;
			Request.FilePath = "/something";
			PrepareViewComponent(component);
			component.Render();

			Assert.AreEqual(7, actions.Count);
			Assert.AreEqual("started", actions[0]);
			Assert.AreEqual("first", actions[1]);
			Assert.AreEqual("prev", actions[2]);
			Assert.AreEqual("select", actions[3]);
			Assert.AreEqual("next", actions[4]);
			Assert.AreEqual("last", actions[5]);
			Assert.AreEqual("ended", actions[6]);
		}

		[Test]
		public void LinksAreRenderedWhenPageIsInTheMiddle()
		{
			List<string> actions = new List<string>();

			SectionRender["startblock"] = delegate { actions.Add("started"); };
			SectionRender["endblock"] = delegate { actions.Add("ended"); };
			SectionRender["first"] = delegate { actions.Add("first"); };
			SectionRender["last"] = delegate { actions.Add("last"); };
			SectionRender["next"] = delegate { actions.Add("next"); };
			SectionRender["prev"] = delegate { actions.Add("prev"); };
			SectionRender["link"] = delegate { actions.Add("link"); };
			SectionRender["select"] = delegate { actions.Add("select"); };

			component.Page = secondPageOfThree;
			Request.FilePath = "/something";
			PrepareViewComponent(component);
			component.Render();

			Assert.AreEqual(11, actions.Count);
			Assert.AreEqual("started", actions[0]);
			Assert.AreEqual("first", actions[1]);
			Assert.AreEqual("link", actions[2]);
			Assert.AreEqual("prev", actions[3]);
			Assert.AreEqual("link", actions[4]);
			Assert.AreEqual("select", actions[5]);
			Assert.AreEqual("next", actions[6]);
			Assert.AreEqual("link", actions[7]);
			Assert.AreEqual("last", actions[8]);
			Assert.AreEqual("link", actions[9]);
			Assert.AreEqual("ended", actions[10]);
		}

		[Test]
		public void OutputNoLinksForEmptyPage()
		{
			component.UseInlineStyle = false;
			component.Page = emptyPage;
			Request.FilePath = "/something";
			PrepareViewComponent(component);
			component.Render();

			Assert.AreEqual("<div class=\"pagination\">\r\n" + 
				"<span class=\"disabled\">&laquo;&laquo;</span>" +
				"<span class=\"disabled\">&laquo;</span>" +
				"<select onchange=\"window.location.href = this.options[this.selectedIndex].value;\">\r\n" + 
				"</select>\r\n" + 
				"<span class=\"disabled\">&raquo;</span>" + 
				"<span class=\"disabled\">&raquo;&raquo;</span>\r\n" + 
				"</div>\r\n", Output);
		}

		[Test]
		public void OutputLinksForPageInTheMiddle()
		{
			component.UseInlineStyle = false;
			component.Page = secondPageOfThree;
			Request.FilePath = "/fetch";
			PrepareViewComponent(component);
			component.Render();

			Assert.AreEqual("<div class=\"pagination\">\r\n" +
				"<a href=\"/fetch?page=1\">&laquo;&laquo;</a>\r\n" +
				"<a href=\"/fetch?page=1\">&laquo;</a>\r\n" +
				"<select onchange=\"window.location.href = this.options[this.selectedIndex].value;\">\r\n" +
				"\t<option value=\"/fetch?page=1\">Page 1</option>\r\n" +
				"\t<option value=\"/fetch?page=2\" selected=\"true\">Page 2</option>\r\n" +
				"\t<option value=\"/fetch?page=3\">Page 3</option>\r\n" +
				"</select>\r\n" +
				"<a href=\"/fetch?page=3\">&raquo;</a>\r\n" +
				"<a href=\"/fetch?page=3\">&raquo;&raquo;</a>\r\n\r\n" +
				"</div>\r\n", Output);
		}
	}
}
