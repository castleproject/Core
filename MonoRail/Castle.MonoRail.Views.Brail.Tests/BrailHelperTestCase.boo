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
import Castle.MonoRail.Engine.Tests

[TestFixture]
class BrailHelperTestCase(AbstractCassiniTestCase):
	
	[Test]
	def InheritedHelpers():
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us")
		url = "/helper/inheritedhelpers.rails"
		expected = "Date formatted " + DateTime(1979, 7, 16).ToShortDateString()
		Execute(url, expected)
	
	[Test]
	def DictHelperUsage():
		url = "/helper/DictHelperUsage.rails"
		expected = """<input type="text" name="name" id="name" value="value" size="30" maxlength="20" style="something" eol="true" />"""
		Execute(url, expected)
	
	override def ObtainPhysicalDir():
		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"""..\TestSiteBrail""")
	

