namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Functional")]
class ControllerGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("controller",)))

	[Test]
	def Generate():
		Assert.AreEqual(0, Main(("controller", "SomeTest", "Index")))