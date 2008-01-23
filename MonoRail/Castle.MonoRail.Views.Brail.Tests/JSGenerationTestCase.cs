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
			ProcessView_StripRailsExtension("jsgeneration/elementattribute.rails");
			AssertReplyContains("$('aa').className = \"newclass\";");
		}

		[Test]
		public void AccessingElementAttributeDepth()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementattributedepth.rails");
			AssertReplyContains("$('aa').style.display = \"none\";");
		}

		[Test]
		public void AccessingElementMethod()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementmethod.rails");
			AssertReplyContains("$('aa').hide();");
		}

		[Test]
		public void AccessingElementMethodDepth()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementmethoddepth.rails");
			AssertReplyContains("$('aa').read().test();");
		}

		[Test]
		public void AccessingElementStockOperationReplace()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementreplace.rails");
			AssertReplyContains("$('aa').replace(\"new content\")");
		}

		[Test]
		public void AccessingElementStockOperationReplaceHtml()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementreplacehtml.rails");
			AssertReplyContains("$('aa').update(\"new content\")");
		}

		[Test]
		public void CollectionFirstLast()
		{
			ProcessView_StripRailsExtension("jsgeneration/collectionfirstlast.rails");
			AssertReplyContains("$$('p.welcome b').first().hide();");
			AssertReplyContains("$$('p.welcome b').last().show();");
		}

		[Test]
		public void InsertHtml()
		{
			ProcessView_StripRailsExtension("jsgeneration/InsertHtml.rails");
			AssertReplyContains("new Insertion.Top(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Bottom(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.After(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Before(\"aa\",\"new content\");");
		}

		[Test]
		public void MR_264()
		{
			ProcessView_StripRailsExtension("jsgeneration/mr264.rails");
			AssertReplyContains(
				"Element.update(\"holder\",\"test\");\r\n" +
				"new Effect.Highlight('holder', {});");
		}

		[Test]
		public void Replace()
		{
			ProcessView_StripRailsExtension("jsgeneration/replace.rails");
			AssertReplyContains("Element.replace(\"aa\",\"new content\")");
		}

		[Test]
		public void ReplaceHtml()
		{
			ProcessView_StripRailsExtension("jsgeneration/replacehtml.rails");
			AssertReplyContains("Element.update(\"aa\",\"new content\");");
		}

		[Test]
		public void ReplaceHtmlUsingPartial()
		{
			ProcessView_StripRailsExtension("jsgeneration/replacehtmlwithpartial.rails");
			AssertReplyContains("Element.update(\"aa\",\"You\'re hammett <br> [ a ][ b ]\")");
		}

		[Test]
		public void ShowHideToggleRemove()
		{
			ProcessView_StripRailsExtension("jsgeneration/multipleactions.rails");
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
			ProcessView_StripRailsExtension("jsgeneration/collectionaccess.rails");
			AssertReplyContains("$$('aa');");
		}

		[Test]
		public void SimpleElementAccess()
		{
			ProcessView_StripRailsExtension("jsgeneration/elementaccess.rails");
			AssertReplyContains("$('aa');");
		}

		[Test]
		public void VisualEffect()
		{
			ProcessView_StripRailsExtension("jsgeneration/visualeffect.rails");
			AssertReplyContains("Effect.Highlight('element1', {});");
			AssertReplyContains("Effect.Highlight('element1', {duration:4});");
		}
	}
}