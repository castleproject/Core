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

[TestFixture]
class OutputMethodTestCase(AbstractMRTestCase):

	[Test]
	def HtmlAttribute():
		expected = """
Some text that will be &lt;html&gt; encoded"""
		DoGet( "/OutputMethods/HtmlOutputAttribute.rails")
		AssertReplyEqualsTo(expected)
	
	[Test]
	def RawAttribute():
		expected = """
1<2 && 3>4"""
		DoGet("/OutputMethods/RawOutputAttribute.rails")
		AssertReplyEqualsTo(expected)
	
	[Test]
	def MarkDownAttribute():
		expected = '\r\n<p><a href="http://www.ayende.com/">Ayende Rahien</a>, <strong>Rahien</strong>.</p>\n'

		DoGet( "/OutputMethods/MarkDownOutputAttribute.rails")
		AssertReplyEqualsTo(expected)
