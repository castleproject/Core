namespace Generator.Tests.Unit

import System.IO
import NUnit.Framework
import Generator

[TestFixture, Category("Unit")]
class GeneratorTest:
	_generator as TestGenerator
	
	[SetUp]
	def Setup():
		_generator = TestGenerator()
		
	[TearDown]
	def TearDown():
		File.Delete('test.txt')
		File.Delete('testproc.txt')
		if Directory.Exists('test/test'):
			Directory.Delete('test/test')
		if Directory.Exists('test'):
			Directory.Delete('test')
	
	[Test]
	def MkDir():
		_generator.MkDir('test')
		assert Directory.Exists('test')

	[Test]
	def MkMultipleDir():
		_generator.MkDir('test/test')
		assert Directory.Exists('test/test')		
		
	[Test]
	def Process():
		using writer = StreamWriter('test.txt'):
			writer.WriteLine('Fichier de <%= "test" %>')
		_generator.TemplatesPath = './'
		_generator.Process('test.txt', 'testproc.txt')
		
		using reader = StreamReader('testproc.txt'):
			Assert.AreEqual('Fichier de test', reader.ReadLine())
 
class TestGenerator(GeneratorBase):
	def Run():
		pass