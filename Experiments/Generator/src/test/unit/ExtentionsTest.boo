namespace Generator.Tests.Unit

import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Unit")]
class ExtentionsTest:

	[Test]
	def ToClassName():
		Assert.AreEqual('Classname', 'classname'.ToClassName())
		Assert.AreEqual('ClassName', 'class_name'.ToClassName())
		Assert.AreEqual('ClassName', 'ClassName'.ToClassName())
		Assert.AreEqual('ClassName', 'Class name'.ToClassName())
