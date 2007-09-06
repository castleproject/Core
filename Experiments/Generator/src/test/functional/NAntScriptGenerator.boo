namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Functional")]
class NAntScriptGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("nantscript",)))

