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
import Castle.MonoRail.Views.Brail
import Castle.MonoRail.Framework.Tests

[TestFixture]
class LanguageFeatures(AbstractTestCase):

	[Test]
	def NullableProperties():
		expected = """<?xml version="1.0" ?>
<html>
<h1>BarBaz</h1>
</html>"""
		#should not raise null exception
		DoGet("Home/nullableProperties.rails")
		AssertReplyEqualTo(expected)