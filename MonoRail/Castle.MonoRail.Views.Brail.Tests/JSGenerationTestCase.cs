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
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class JSGenerationTestCase : AbstractTestCase
	{
		[Test]
		public void AccessingElementAttribute()
		{
			DoGet("jsgeneration/elementattribute.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').className = \"newclass\";");
		}

		[Test]
		public void AccessingElementAttributeDepth()
		{
			DoGet("jsgeneration/elementattributedepth.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').style.display = \"none\";");
		}

		[Test]
		public void AccessingElementMethod()
		{
			DoGet("jsgeneration/elementmethod.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').hide();");
		}

		[Test]
		public void AccessingElementMethodDepth()
		{
			DoGet("jsgeneration/elementmethoddepth.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').read().test();");
		}

		[Test]
		public void AccessingElementStockOperationReplace()
		{
			DoGet("jsgeneration/elementreplace.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').replace(\"new content\")");
		}

		[Test]
		public void AccessingElementStockOperationReplaceHtml()
		{
			DoGet("jsgeneration/elementreplacehtml.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').update(\"new content\")");
		}

		[Test]
		public void CollectionFirstLast()
		{
			DoGet("jsgeneration/collectionfirstlast.rails");
			AssertSuccess();
			AssertReplyContains("$$('p.welcome b').first().hide();");
			AssertReplyContains("$$('p.welcome b').last().show();");
		}

		[Test]
		public void InsertHtml()
		{
			DoGet("jsgeneration/InsertHtml.rails");
			AssertSuccess();
			AssertReplyContains("new Insertion.Top(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Bottom(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.After(\"aa\",\"new content\");");
			AssertReplyContains("new Insertion.Before(\"aa\",\"new content\");");
		}

		[Test]
		public void MR_264()
		{
			DoGet("jsgeneration/mr264.rails");
			AssertReplyContains(
				"Element.update(\"holder\",\"test\");\r\n" +
				"new Effect.Highlight('holder', {});");
		}

		[Test]
		public void Replace()
		{
			DoGet("jsgeneration/replace.rails");
			AssertSuccess();
			AssertReplyContains("Element.replace(\"aa\",\"new content\")");
		}

		[Test]
		public void ReplaceHtml()
		{
			DoGet("jsgeneration/replacehtml.rails");
			AssertSuccess();
			AssertReplyContains("Element.update(\"aa\",\"new content\");");
		}

		[Test]
		public void ReplaceHtmlUsingPartial()
		{
			DoGet("jsgeneration/replacehtmlwithpartial.rails");
			AssertSuccess();
			AssertReplyContains("Element.update(\"aa\",\"You\'re hammett <br> [ a ][ b ]\")");
		}

		[Test]
		public void ShowHideToggleRemove()
		{
			DoGet("jsgeneration/multipleactions.rails");
			AssertSuccess();
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
			DoGet("jsgeneration/collectionaccess.rails");
			AssertSuccess();
			AssertReplyContains("$$('aa');");
		}

		[Test]
		public void SimpleElementAccess()
		{
			DoGet("jsgeneration/elementaccess.rails");
			AssertSuccess();
			AssertReplyContains("$('aa');");
		}

		[Test]
		public void VisualEffect()
		{
			DoGet("jsgeneration/visualeffect.rails");
			AssertSuccess();
			AssertReplyContains("Effect.Highlight('element1', {});");
			AssertReplyContains("Effect.Highlight('element1', {duration:4});");
		}
	}
}