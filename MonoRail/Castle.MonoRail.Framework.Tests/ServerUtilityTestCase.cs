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
	
	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;


	[TestFixture]
	public class ServerUtilityTestCase : AbstractMRTestCase
	{
		[Test]
		public void UrlEncode()
		{
			DoGet("ServerUtility/UrlEncode.rails", "content=this is a path");

			AssertReplyEqualTo("this+is+a+path");
		}

		[Test]
		public void HtmlEncode()
		{
			DoGet("ServerUtility/HtmlEncode.rails", "content=<html>content</html>");

			AssertReplyEqualTo("&lt;html&gt;content&lt;/html&gt;");
		}
	
		[Test]
		public void UrlPathEncode()
		{
			DoGet("ServerUtility/UrlPathEncode.rails", @"content=My path folder\is this one");

			AssertReplyEqualTo(@"My%20path%20folder\is%20this%20one");
		}
	
		[Test]
		public void JavaScriptEscape()
		{
			DoGet("ServerUtility/JavaScriptEscape.rails", "content=some js \" content \"");

			AssertReplyEqualTo("some+js+%22+content+%22");
		}
	}
}
