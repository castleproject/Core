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
	using NUnit.Framework;
	using TestSiteNVelocity.Components;
	using TestSiteNVelocity.Controllers;

	[TestFixture]
	public class ComponentsTestCase : BaseViewOnlyTestFixture
	{
		protected override void BeforEachTest()
		{
			ViewComponentFactory.Inspect(typeof(BlockViewComponent2).Assembly);
		}

		[Test]
		public void CaptureForDirective()
		{
			ProcessView_StripRailsExtension("usingcomponent/capturefordirective.rails");

			AssertSuccess();

			AssertReplyContains(@"=navbar= some content =navbar=");
		}

		[Test]
		public void CaptureForComponent()
		{
			ProcessView_StripRailsExtension("usingcomponent/capturefor.rails");

			AssertSuccess();

			AssertReplyContains(@"=navbar= some content =navbar=");
		}

		[Test]
		public void CaptureForComponentAppend()
		{
			ProcessView_StripRailsExtension("usingcomponent/captureforappend.rails");

			AssertSuccess();

			AssertReplyContains(@"=2= some content =3=  =4=");
		}

		[Test]
		public void CaptureForComponentAppendBefore()
		{
			ProcessView_StripRailsExtension("usingcomponent/captureforappendbefore.rails");

			AssertSuccess();

			AssertReplyContains(@"=2= some content	=4= 	=3=");
		}

		[Test]
		public void InlineComponentUsingRenderText()
		{
			ProcessView_StripRailsExtension("usingcomponent/index1.rails");

			AssertSuccess();

			AssertReplyEqualTo("static 1\r\nHello from SimpleInlineViewComponent static 2");
		}

		[Test]
		public void InlineComponentUsingRender()
		{
			ProcessView_StripRailsExtension("usingcomponent/index6.rails");

			AssertSuccess();

			AssertReplyEqualTo("static 1\r\nThis is a view used by a component static 2");
		}

		[Test]
		public void InlineComponentUsingTemplatedRender()
		{
			PropertyBag.Add("var1", "v1");
			PropertyBag.Add("var2", "v2");
			PropertyBag.Add("fromPropertyBag", "items from property bag");

			ProcessView_StripRailsExtension("usingcomponent/inlinecomponentusingtemplatedrender.rails");

			AssertSuccess();

			AssertReplyEqualTo("v1 v2 [contains items from property bag] v1 v2 [contains items from property bag]");
		}

		[Test]
		public void InlineComponentNotOverridingRender()
		{
			var items = new ArrayList();

			items.Add("1");
			items.Add("2");

			PropertyBag.Add("items", items);

			ProcessView_StripRailsExtension("usingcomponent/index3.rails");

			AssertSuccess();

			AssertReplyEqualTo("static 1\r\ndefault component view picked up automatically static 2");
		}

		[Test]
		public void InlineComponentWithParam1()
		{
			ProcessView_StripRailsExtension("usingcomponent/index4.rails");

			AssertSuccess();

			AssertOutput("Done 1");
		}

		[Test]
		public void BlockComp1()
		{
			ProcessView_StripRailsExtension("usingcomponent/index5.rails");

			AssertSuccess();

			AssertOutput("  item 0\r\n  item 1\r\n  item 2\r\n");
		}

		[Test]
		public void BlockWithinForEach()
		{
			ArrayList items = new ArrayList();

			items.Add("1");
			items.Add("2");

			PropertyBag.Add("items", items);

			ProcessView_StripRailsExtension("usingcomponent/index8.rails");

			AssertSuccess();

			AssertOutput("inner content 1\r\ninner content 2\r\n");
		}

		[Test]
		public void SeveralComponentsInvocation()
		{
			for(int i = 0; i < 10; i++)
			{
				ProcessView_StripRailsExtension("usingcomponent/index9.rails");

				AssertSuccess();

				AssertOutput("static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n");
			}
		}

		[Test]
		public void ArrayListAsComponentParam()
		{
			ArrayList items = new ArrayList();

			items.Add("1");
			items.Add("2");

			PropertyBag.Add("items", items);

			ProcessView_StripRailsExtension("usingcomponent/index10.rails");

			AssertSuccess();

			AssertOutput("static 1\n1 2  static 2");
		}

		[Test]
		public void ComponentWithInvalidSection()
		{
			try
			{
				ProcessView_StripRailsExtension("usingcomponent2/ComponentWithInvalidSections.rails");

				Assert.Fail("Expected ViewComponentException");
			}
			catch(ViewComponentException ex)
			{
				Assert.AreEqual("The section 'invalidsection' is not supported by the ViewComponent 'GridComponent'", ex.Message);
			}
		}

		[Test]
		public void GridComponent1()
		{
			ArrayList items = new ArrayList();

			items.Add(new Contact("hammett", "111"));
			items.Add(new Contact("Peter Griffin", "222"));

			PropertyBag.Add("contacts", items);

			ProcessView_StripRailsExtension("usingcomponent2/GridComponent1.rails");

			AssertReplyContains("<table>    <th>EMail</th>\r\n    <th>Phone</th>\r\n  <tr>    <td>hammett</td>\r\n    <td>111</td>\r\n  </tr><tr>    <td>Peter Griffin</td>\r\n    <td>222</td>\r\n  </tr></table>");
		}

		[Test]
		public void GridComponent2()
		{
			ProcessView_StripRailsExtension("usingcomponent2/GridComponent2.rails");

			AssertReplyContains("<table>    <th>EMail</th>\r\n    <th>Phone</th>\r\n  <tr><td colspan=2>Nothing here</td>\r\n  </tr></table>");
		}

		[Test]
		public void ComponentAndParams1()
		{
			ProcessView_StripRailsExtension("usingcomponent2/ComponentAndParams1.rails");

			AssertReplyContains("1 2 True Something hello");
		}
		
		[Test]
		public void ChildContentComponent1()
		{
			ProcessView_StripRailsExtension("usingcomponent2/ChildContentComponent1.rails");
			AssertReplyContains("View content and Something");
		}
		
		[Test]
		public void ChildContentComponent2()
		{
			ProcessView_StripRailsExtension("usingcomponent2/ChildContentComponent2.rails");
			AssertReplyContains("View content and 1 2 True Something hello");
		}

		[Test]
		public void CanRenderMultipleDynamicComponents() 
		{
			ArrayList cmps = new ArrayList(2);
			cmps.Add("SimpleInlineViewComponent");
			cmps.Add("SimpleInlineViewComponent2");
			PropertyBag.Add("components", cmps);

			ProcessView_StripRailsExtension("usingcomponent/dynamiccomponent.rails");
			AssertReplyContains("Hello from SimpleInlineViewComponent");
			AssertReplyContains("This is a view used by a component");
		}

 		[Test]
		public void CanRenderMultipleDynamicComponentsWithParameters()
		{
			IDictionary params1 = new Hashtable(2);
			params1.Add("arg1", "1");
			params1.Add("arg2", "2");
			IDictionary params2 = new Hashtable(1);
			params2.Add("arg1", "A");

			//Use and ArrayList so we can repeat the "keys"
			IList cmps = new ArrayList(3);
			cmps.Add(new DictionaryEntry("InlineComponentWithParams", params1));
			cmps.Add(new DictionaryEntry("InlineComponentWithParams", params2));
			cmps.Add(new DictionaryEntry("InlineComponentWithParams", null));
			PropertyBag.Add("components", cmps);

			ProcessView_StripRailsExtension("usingcomponent/dynamiccomponentwithparameters.rails");
			AssertReplyContains("arg1: '1', arg2: '2'");
			AssertReplyContains("arg1: 'A', arg2: ''"); //params dictionary didn't contain arg2
			AssertReplyContains("arg1: '', arg2: ''"); //params dictionary null
		}

		[Test]
		public void AutoParameterBindingMustBeAbleToPerformSimpleConversions()
		{
			ProcessView_StripRailsExtension("usingcomponent2/autoparameterbinding1.rails");
			AssertReplyContains("'1' 'xpto' 'something.castle?val=1'");
		}

		[Test]
		public void AutoParameterBinding_IdIsARequiredParameter()
		{
			try
			{
				ProcessView_StripRailsExtension("usingcomponent2/autoparameterbinding2.rails");
				Assert.Fail("Expected ViewComponentException");
			}
			catch(ViewComponentException ex)
			{
				Assert.AreEqual("The parameter 'Id' is required by the ViewComponent " +
				"AutoParameterBinding but was not passed or had a null value", ex.Message);
			}
		}

		private void AssertOutput(String expected)
		{
			Assert.AreEqual(NormalizeWhitespace(expected), NormalizeWhitespace(lastOutput));
		}

		private static string NormalizeWhitespace(String s)
		{
			return s.Replace("\r\n", "\n");
		}
	}
}
