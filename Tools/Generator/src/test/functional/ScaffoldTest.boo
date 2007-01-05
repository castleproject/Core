namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Functional")]
class ScaffoldGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("scaffold",)))

	[Test]
	def Generate():
		Assert.AreEqual(0, Main(("scaffold", "Test", "Wow")))