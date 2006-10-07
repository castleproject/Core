// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

import System
import System.IO
import NUnit.Framework

import Castle.MonoRail.TestSupport
import Castle.MonoRail.Framework.Tests

[TestFixture]
class BrailSubViewTestCase(AbstractTestCase):
		
	[Test]
	def BrailWillCacheSubViewsWhenUsingForwardSlash():
		DoGet("subview/useLotsOfSubViews.rails")
		sb = Text.StringBuilder()
		for i in range(50):
			sb.Append("real")
		expected = sb.ToString()
		AssertReplyEqualTo(expected)
		
		# Here we tell the controller to replace the subview with a dummy implementation
		# if brail doesn't cache it, it will use the real implementation, and the test will fail.
		sb = Text.StringBuilder()
		for i in range(50):
			sb.Append("dummy")
		DoGet("subview/useLotsOfSubViews.rails","replaceSubView=true")
		expected = sb.ToString()
		AssertReplyEqualTo(expected)
		
		
	[Test]
	def CanCallSubViews():
		DoGet("subview/index.rails")
		expected = "View With SubView Content\r\nFrom SubView"
		AssertReplyEqualTo(expected)
	
	[Test]
	def CanCallSubViewWithPath():
		DoGet("subview/SubViewWithPath.rails")
		expected = "View With SubView Content\r\nContents for heyhello View"
		AssertReplyEqualTo(expected)
	
	[Test]
	def SubViewWithLayout():
		DoGet("subview/SubViewWithLayout.rails")
		expected = "\r\nWelcome!\r\n<p>View With SubView Content\r\nFrom SubView</p>\r\nFooter"
		AssertReplyEqualTo(expected)
	
	[Test]
	def SubViewWithParameters():
		DoGet("subview/SubViewWithParameters.rails")
		expected = "View SubView Content With Parameters\r\nMonth: 0\r\nAllow Select: False"
		AssertReplyEqualTo(expected)
