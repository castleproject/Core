namespace Generator.Tests.Unit

import Generator.Extentions
import NUnit.Framework

[TestFixture, Category("Unit")]
class InflectorTest:
	[Property(TheInflector)] _inflector = Inflector.Instance
	
	[Test]
	def ToPlural():
		Assert.AreEqual('apples', TheInflector.ToPlural('apple'))
		Assert.AreEqual('apples', TheInflector.ToPlural('apples'))
		Assert.AreEqual('tomatoes', TheInflector.ToPlural('tomato'))
		Assert.AreEqual('mice', TheInflector.ToPlural('mouse'))
		Assert.AreEqual('people', TheInflector.ToPlural('person'))
		Assert.AreEqual('men', TheInflector.ToPlural('man'))
		
		Assert.AreEqual('Clients', TheInflector.ToPlural('Client'))
	
	[Test]
	def ToSingular():
		Assert.AreEqual('apple', TheInflector.ToSingular('apples'))
		Assert.AreEqual('apple', TheInflector.ToSingular('apple'))
		Assert.AreEqual('tomato', TheInflector.ToSingular('tomatoes'))
		Assert.AreEqual('mouse', TheInflector.ToSingular('mice'))
		Assert.AreEqual('person', TheInflector.ToSingular('people'))
		Assert.AreEqual('man', TheInflector.ToSingular('men'))
		
		Assert.AreEqual('Client', TheInflector.ToSingular('Client'))
		