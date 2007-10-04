// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	public class ComponentsTestCase : AbstractTestCase
	{
		[Test]
		public void BlockComp1()
		{
			DoGet("usingcomponents/index8.rails");
			AssertReplyEqualTo("\r\ninner content 1\r\n\r\ninner content 2\r\n");
		}

		[Test]
		public void CanGetParamsFromTheComponentInTheView()
		{
			DoGet("usingcomponents/template.rails");
			AssertReplyEqualTo("123");
		}

		[Test]
		public void CanPassParametersFromComponentToView()
		{
			DoGet("usingcomponents/withParams.rails");
			AssertReplyEqualTo("brail");
		}

		[Test]
		public void ComponentWithInvalidSection()
		{
			DoGet("usingcomponent2/ComponentWithInvalidSections.rails");

			AssertReplyContains("The section 'invalidsection' is not supported by the ViewComponent 'GridComponent'");
		}

		[Test]
		public void GridComponent1()
		{
			DoGet("usingcomponent2/GridComponent1.rails");

			AssertReplyEqualTo(
				@"<table>
    <th>EMail</th>
    <th>Phone</th>
<tr>
    <td>hammett</td>
    <td>111</td>
</tr><tr>
    <td>Peter Griffin</td>
    <td>222</td>
</tr></table>");
		}

		[Test]
		public void GridComponent2()
		{
			DoGet("usingcomponent2/GridComponent2.rails");
			AssertReplyEqualTo(
				@"<table>
    <th>EMail</th>
    <th>Phone</th>
<tr>
<td colspan=2>Nothing here</td>
</tr></table>");
		}

		[Test]
		public void InlineComponentNotOverridingRender()
		{
			string expected = "static 1\r\ndefault component view picked up automatically static 2";
			DoGet("usingcomponents/index3.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void InlineComponentUsingRender()
		{
			string expected = "static 1\r\nThis is a view used by a component static 2";
			DoGet("usingcomponents/index2.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void InlineComponentWithParam1()
		{
			DoGet("usingcomponents/index4.rails");
			AssertReplyEqualTo("Done");
		}

		[Test]
		public void SeveralComponentsInvocation()
		{
			for (int i = 0; i < 10; i++)
			{
				string expected =
					"static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n";
				DoGet("usingcomponents/index9.rails");
				AssertReplyEqualTo(expected);
			}
		}

		[Test]
		public void SimpleInlineViewComponent()
		{
			string expected = "static 1\r\nHello from SimpleInlineViewComponent\r\nstatic 2";
			DoGet("usingcomponents/index1.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void UsingCaptureFor()
		{
			DoGet("usingcomponents/captureFor.rails");
			AssertReplyEqualTo("\r\n1234 Foo, Bar");
		}

		[Test]
		public void UsingCaptureForWithLayout()
		{
			DoGet("usingcomponents/captureForWithLayout.rails");
			AssertReplyEqualTo("Numbers: 1234\r\n");
		}
	}
}