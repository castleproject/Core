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
	public class BrailBugsTestCase : AbstractTestCase
	{
		[Test]
		public void MR_233_TagInDoubleQuotes()
		{
			DoGet("bugs/mr_233.rails");
			AssertSuccess();
			AssertReplyEqualTo("<body id=\"123\">");
		}


		[Test]
		public void MR_262_DynamicComponents()
		{
			DoGet("usingcomponents/DynamicComponents.rails");
			AssertSuccess();
			AssertReplyEqualTo("default component view picked up automaticallyThis is a view used by a component");
		}

		[Test]
		public void MR_285_ViewName_Is_Reserved_CompilerKeyword()
		{
			DoGet("bugs/add.rails");
			AssertSuccess();
			AssertReplyContains("Success");
		}

		[Test]
		public void MR_299_Inline_SubView()
		{
			DoGet("bugs/inlineSubView.rails");
			AssertReplyContains("Success");
		}
	}
}