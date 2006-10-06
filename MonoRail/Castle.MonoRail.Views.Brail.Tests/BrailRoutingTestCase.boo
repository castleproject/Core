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
import Castle.MonoRail.Framework

import Castle.MonoRail.TestSupport
import Castle.MonoRail.Framework.Tests

[TestFixture]
class BrailRoutingTestCase(AbstractTestCase):
	[Test]
	def BlogRoutingRule():
		DoGet("blog/posts/2005/07/")
		expected = "Blog: year=2005 month=7";
		AssertReplyEqualTo(expected);
	
	[Test]
	def NewsRoutingRule():
		DoGet("news/2004/11/")
		expected = "News: year=2004 month=11";
		AssertReplyEqualTo(expected);
