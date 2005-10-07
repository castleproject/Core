// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;


	[TestFixture]
	public class HtmlHelperTestCase 
	{
		[Test]
		public void FormTag()
		{
			HtmlHelper helper = new HtmlHelper();
			Assert.AreEqual( "<form method=\"post\" action=\"something.rails\">\r\n", 
				helper.Form("something.rails") );
		}

		[Test]
		public void EndFormTag()
		{
			HtmlHelper helper = new HtmlHelper();
			Assert.AreEqual( "</form>", helper.EndForm() );
		}

		[Test]
		public void LabelFor()
		{
			HtmlHelper helper = new HtmlHelper();
			Assert.AreEqual( "<label  for=\"x\">name</label>\r\n", helper.LabelFor("x", "name") );
		}
		
		[Test]
		public void BuildUnorderedList1()
		{
			String[] args = new String[] { "arg1", "arg2" };
			HtmlHelper helper = new HtmlHelper();
			Assert.AreEqual( "<ul>\r\n<li>arg1</li>\r\n<li>arg2</li>\r\n</ul>\r\n", 
				helper.BuildUnorderedList(args) );
		}

		[Test]
		public void BuildUnorderedList2()
		{
			String[] args = new String[] { "arg1" };
			HtmlHelper helper = new HtmlHelper();
			Assert.AreEqual( "<ul class=\"style1\">\r\n<li class=\"style2\">arg1</li>\r\n</ul>\r\n", 
				helper.BuildUnorderedList(args, "style1", "style2") );
		}
	}
}
