namespace Generator.Tests.Functional

import System.IO
import Generator
import Generator.Extentions
import NUnit.Framework

class GeneratorTestCase:
	TEST_DIR = "../functional_test"
	_currentDir as string

	[SetUp]
	def SetUp():
		Directory.CreateDirectory(TEST_DIR)
		_currentDir = Directory.GetCurrentDirectory()
		Directory.SetCurrentDirectory(TEST_DIR)
		
	[TearDown]
	def TearDown():
		try:
			Directory.Delete(TEST_DIR, true)
		except e:
			pass
		Directory.SetCurrentDirectory(_currentDir)
