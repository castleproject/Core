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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class JSGenerationTestCase : AbstractTestCase
	{
		[Test]
		public void SimpleElementAccess()
		{
			DoGet("jsgeneration/elementaccess.rails");
			AssertSuccess();
			AssertReplyContains(@"$('aa');");
		}

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
		public void AccessingElementStockOperationReplaceHtml()
		{
			DoGet("jsgeneration/elementreplacehtml.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').update('new content')");
		}

		[Test]
		public void AccessingElementStockOperationReplace()
		{
			DoGet("jsgeneration/elementreplace.rails");
			AssertSuccess();
			AssertReplyContains("$('aa').replace('new content')");
		}
	}
}
