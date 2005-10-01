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
class BrailBasicFunctionality(BasicFunctionalityTestCase):

	override def ObtainPhysicalDir():
		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"""..\TestSiteBrail""")
	
	[Test]
	def AppPath():
		url = "/apppath/index.rails"
		expected = "Current apppath is "
		
		Execute(url, expected)
	
	[Test]
	def AppPathChangeOnTheFly():
		script = Path.Combine(ObtainPhysicalDir(),"""Views\AppPath\Index.boo""")
		using read = File.OpenText(script):
			old = read.ReadToEnd()
		new = """new content"""
		using write = File.CreateText(script):
			write.Write(new)
		url = "/apppath/index.rails"
		expected = new
		# Wait half a sec so Brail could pick up that a change in the file occured.
		System.Threading.Thread.Sleep(100)
		try:
			Execute(url, expected)
		ensure:
			using write = File.CreateText(script):
				write.Write(old)
	
	
	[Test]
	def CommonScripts():
		url = "/home/hellofromcommon.rails"
		expected = "Hello, Ayende"
		Execute(url, expected)
	
	[Test]
	def CommonScriptsChangeOnTheFly():
		common = Path.Combine(ObtainPhysicalDir(),"""Views\CommonScripts\Hello.boo""")
		using read = File.OpenText(common ):
			old = read.ReadToEnd()
		new = """
def SayHello(name as string):
	return "Hello, \${name}! Modified!" 
end"""
		using write = File.CreateText(common ):
			write.Write(new)
		url = "/home/hellofromcommon.rails"
		expected = "Hello, Ayende! Modified!"
		# Have to wait for the common scripts recompilation otherwise you get random test failure since the request
		# sometimes gets there faster you can recompile and it gets the old version.
		System.Threading.Thread.Sleep(100)
		try:
			Execute(url, expected)
		ensure:	
			using write = File.CreateText(common ):
				write.Write(old)
	
	[Test]
	def PreProcessor():
		url = "/home/preprocessor.rails"
		expected = """
<html>
<body>
<title>AYENDE</title>
<body>
<ul>
<li>0</li><li>1</li><li>2</li>
</ul>
</body>
</html>
"""
		Execute(url, expected)
