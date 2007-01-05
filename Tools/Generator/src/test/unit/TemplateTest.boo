namespace Generator.Tests.Unit

import System
import System.IO
import NUnit.Framework
import Generator

[TestFixture, Category("Unit")]
class TemplateTest:
	[Test]
	def ProcessFlatText():
		t = Template(GetTestFile("test"))
		Assert.IsTrue(t.Process().StartsWith("test"))
		t = Template(GetTestFile("test encore"))
		Assert.IsTrue(t.Process().StartsWith("test encore"))
		
	[Test]
	def ProcessSimpleTag():
		code = 't<%= \'es\' %>t'
		t = Template(GetTestFile(code))
		Assert.IsTrue(t.Process().StartsWith('test'))
	
	[Test]
	def ProcessLoopTag():
		code = '<% for i in range(5): %><%= i %><% end %>'
		t = Template(GetTestFile(code))
		Assert.AreEqual('01234', t.Process())
	
	[Test]
	def ProcessWithVarReplace():
		code = '<%= TestVar %>'
		t = Template(GetTestFile(code))
		Assert.IsTrue(t.Process({'TestVar':'test'}).StartsWith('test'))
	
	[Test]
	def ProcessWithListVarReplace():
		code = '<% for i in aList: %><%= i %><% end %>'
		aList = (1, 2, 3)
		t = Template(GetTestFile(code))
		Assert.IsTrue(t.Process({'aList':aList}).StartsWith('123'))
	
	[Test]
	def ProcessWithQuotes():
		code = 'This is a "test"'
		t = Template(GetTestFile(code))
		Assert.AreEqual(code + Environment.NewLine, t.Process())
	
	[Test]
	def ProcessWithSpecialBooChars():
		code = 'This is a ${test}'
		t = Template(GetTestFile(code))
		Assert.AreEqual(code + Environment.NewLine, t.Process())
	
	[Test]
	def ProcessWithSpecialBooCharsWithVar():
		myvar = "cool"
		test = "${myvar}ish"
		code = 'This is <%= test %>'
		t = Template(GetTestFile(code))
		Assert.AreEqual("This is coolish" + Environment.NewLine, t.Process({'test':test}))
	
	private def GetTestFile(content as string) as string:
		filename = "test.txt"
		File.Delete(filename)
		using file = StreamWriter(filename):
			file.WriteLine(content)
		return filename
	
	[TearDown]
	def Teardown():
		File.Delete('test.txt')
