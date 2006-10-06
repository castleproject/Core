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
class BrailBasicFunctionality(AbstractTestCase):

	[Test]
	def AppPath():
		DoGet("apppath/index.rails")
		AssertReplyEqualTo("Current apppath is ")
	
	[Test, Ignore("Sometimes fail, but the functionality is working.")]
	def AppPathChangeOnTheFly():
		script = Path.Combine(GetPhysicalDir(),"""Views\AppPath\Index.boo""")
		newContent = "new content"
		using read = File.OpenText(script):
			old = read.ReadToEnd()
		using write = File.CreateText(script):
			write.Write(newContent)
		# Wait half a sec so Brail could pick up that a change in the file occured.
		System.Threading.Thread.Sleep(500)
		try:
			DoGet("apppath/index.rails")
			AssertReplyEqualTo(newContent)
		ensure:
			using write = File.CreateText(script):
				write.Write(old)
	
	
	[Test]
	def CommonScripts():
		DoGet("home/hellofromcommon.rails")
		expected = "Hello, Ayende"
		AssertReplyEqualTo(expected)
	
	
	[Test]
	def Nullables():
		DoGet("home/nullables.rails")
		expected = "\r\nfoo"
		AssertReplyEqualTo(expected)

	[Test]
	def Empty():
		DoGet("home/Empty.rails")
		expected = ""
		AssertReplyEqualTo(expected)

	
	[Test, Ignore("Sometimes fail, but the functionality is working.")]
	def CommonScriptsChangeOnTheFly():
		common = Path.Combine(GetPhysicalDir(),"""Views\CommonScripts\Hello.boo""")
		using read = File.OpenText(common ):
			old = read.ReadToEnd()
		new = """
def SayHello(name as string):
	return "Hello, \${name}! Modified!" 
end"""
		using write = File.CreateText(common ):
			write.Write(new)
		expected = "Hello, Ayende! Modified!"
		# Have to wait for the common scripts recompilation otherwise you get random test failure since the request
		# sometimes gets there faster you can recompile and it gets the old version.
		System.Threading.Thread.Sleep(500)
		try:
			DoGet("home/hellofromcommon.rails")
			AssertReplyEqualTo(expected)
		ensure:	
			using write = File.CreateText(common ):
				write.Write(old)
		#Have to wait again to make sure that the second recompilation happened, since otherwise a second test
		# might get the modified version.
		System.Threading.Thread.Sleep(500)
	
	[Test]
	def PreProcessor():
		DoGet("home/preprocessor.rails")
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
		AssertReplyEqualTo(expected)
