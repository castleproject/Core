// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class TransformFilterTestCase : AbstractTestCase
	{
		[Test]
		public void UppercaseFilter()
		{
			DoGet("TransformFiltered/ToUpperCase.rails");

			AssertSuccess();

			AssertReplyEqualTo("THIS IS NOT A LOWERCASE STRING");
		}

		[Test]
		public void MarkdownFilter()
		{
			DoGet("TransformFiltered/Markdown.rails");

			AssertSuccess();

			string expected = "<p>This is <a href=\"http://example.com/\" title=\"Title\">an example</a> inline link.</p>" + "\n\n" +
							  "<p><a href=\"http://example.net/\">This link</a> has no title attribute.</p>" + "\n";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void WikiFilter()
		{
			DoGet("TransformFiltered/Wiki.rails");

			AssertSuccess();

			string expected = "<table><caption><b>wiki's are really nice</b></caption><tr><td><i>and so is castle</i></td></tr></table>\n";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ExecutingOrder()
		{
			DoGet("TransformFiltered/ExecutingOrder.rails");

			AssertSuccess();

			string expected = "THIS IS NOT A LOWERCASE STRING";

			AssertReplyEqualTo(expected);
		}

	}
}
