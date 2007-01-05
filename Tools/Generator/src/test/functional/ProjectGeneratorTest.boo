namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Functional")]
class ProjectGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("project",)))

	[Test]
	def Generate():
		Assert.AreEqual(0, Main(("project", "TestProject")))