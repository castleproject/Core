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
	using NUnit.Framework;
	
	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class PostBackTestCase : AbstractTestCase
	{
		[Test]
		public void NotPostBack()
		{
			DoGet("postback/index.rails");

			AssertSuccess();

			AssertReplyEqualTo( "PostBack = False" );
		}

		[Test]
		public void PostBack()
		{
			DoGet("postback/hello.rails", "name=Craig");

			AssertSuccess();

			AssertReplyContains("Hello Craig!");
		}

		[Test]
		public void NullCustomArgs()
		{
			DoGet("postback/customargs.rails");

			AssertSuccess();

			AssertPropertyBagEntryEquals("i", 0);
			AssertPropertyBagEntryEquals("d", 0);
			AssertPropertyBagEntryEquals("s", null);
		}
	}
}
