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
	
	using NUnit.Framework;

	[TestFixture]
	public class JSGenerationTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void AccessingElementAttribute()
		{
			ProcessViewJS("jsgeneration/elementattribute");
			AssertReplyContains("$('aa').className = \"newclass\";");
		}

		[Test]
		public void AccessingElementAttributeDepth()
		{
			ProcessViewJS("jsgeneration/elementattributedepth");
			AssertReplyContains("$('aa').style.display = \"none\";");
		}

		[Test]
		public void AccessingElementMethod()
		{
			ProcessViewJS("jsgeneration/elementmethod");
			AssertReplyContains("$('aa').hide();");
		}

		[Test]
		public void AccessingElementMethodDepth()
		{
			ProcessViewJS("jsgeneration/elementmethoddepth");
			AssertReplyContains("$('aa').read().test();");
		}

		[Test]
		public void AccessingElementStockOperationReplace()
		{
			ProcessViewJS("jsgeneration/elementreplace");
			AssertReplyContains("$('aa').replace(\"new content\")");
		}

		[Test]
		public void AccessingElementStockOperationReplaceHtml()
		{
			ProcessViewJS("jsgeneration/elementreplacehtml");
			AssertReplyContains("$('aa').update(\"new content\")");
		}

		[Test]
		public void CollectionFirstLast()
		{
			ProcessViewJS("jsgeneration/collectionfirstlast");
			AssertReplyContains("$$('p.welcome b').first().hide();");
			AssertReplyContains("$$('p.welcome b').last().show();");
		}

		[Test]
		public void InsertHtml()
		{
			ProcessViewJS("jsgeneration/InsertHtml");
			AssertReplyContains("new Insertion.Top(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Bottom(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.After(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Before(\"aa\",\"new content\");");
		}

		[Test]
		public void MR_264()
		{
			ProcessViewJS("jsgeneration/mr264");
			AssertReplyContains(
				"Element.update(\"holder\",\"test\");\r\n" +
				"new Effect.Highlight('holder', {});");
		}

		[Test]
		public void Replace()
		{
			ProcessViewJS("jsgeneration/replace");
			AssertReplyContains("Element.replace(\"aa\",\"new content\")");
		}

		[Test]
		public void ReplaceHtml()
		{
			ProcessViewJS("jsgeneration/replacehtml");
			AssertReplyContains("Element.update(\"aa\",\"new content\");");
		}

		[Test]
		public void ReplaceHtmlUsingPartial()
		{
			ProcessViewJS("jsgeneration/replacehtmlwithpartial");
			AssertReplyContains("Element.update(\"aa\",\"You\'re hammett <br> [ a ][ b ]\")");
		}

		[Test]
		public void ShowHideToggleRemove()
		{
			ProcessViewJS("jsgeneration/multipleactions");
			AssertReplyContains("Element.show(\"a\");");
			AssertReplyContains("Element.show(\"a\",\"b\",\"c\");");
			AssertReplyContains("Element.hide(\"a\");");
			AssertReplyContains("Element.hide(\"a\",\"b\",\"c\");");
			AssertReplyContains("Element.toggle(\"a\");");
			AssertReplyContains("Element.toggle(\"a\",\"b\",\"c\");");
			AssertReplyContains("[\"a\",\"b\",\"c\"].each(Element.remove);");
		}

		[Test]
		public void SimpleCollectionAccess()
		{
			ProcessViewJS("jsgeneration/collectionaccess");
			AssertReplyContains("$$('aa');");
		}

		[Test]
		public void SimpleElementAccess()
		{
			ProcessViewJS("jsgeneration/elementaccess");
			AssertReplyContains("$('aa');");
		}

		[Test]
		public void VisualEffect()
		{
			ProcessViewJS("jsgeneration/visualeffect");
			AssertReplyContains("Effect.Highlight('element1', {});");
			AssertReplyContains("Effect.Highlight('element1', {duration:4});");
		}
	}
}