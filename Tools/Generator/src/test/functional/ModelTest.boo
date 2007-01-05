namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Functional")]
class ModelGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("model",)))

	[Test]
	def Generate():
		Assert.AreEqual(0, Main(("model", "Test")))