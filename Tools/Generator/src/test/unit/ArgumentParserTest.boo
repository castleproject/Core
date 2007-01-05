namespace Generator.Tests.Unit

import Generator
import NUnit.Framework

[TestFixture, Category("Unit")]
class ArgumentParserTest:
	_parser as ArgumentParser

	[SetUp]
	def SetUp():
		_parser = ArgumentParser(Test(Name: 'Test1'))
		
	[Test]
	def GetUsage():
		Assert.AreEqual("Name Name2 [Name3] [options]", _parser.GetUsage())
		
	[Test]
	def GetHelp():
		Assert.AreEqual("""Arguments:
    Name            The name
    Name2           The second name
    Name3           The third name
Options:
    -f, --force     Force it ?""", _parser.GetHelp())

	[Test]
	def SetArguments():
		ret = _parser.SetArguments(('Test3', 'Test4'))
		test = _parser.Target as Test
		Assert.AreEqual('Test3', test.Name)
		Assert.AreEqual('Test4', test.Name2)
		Assert.AreEqual(0, len(ret))
	
	[Test]	
	def SetExtraArguments():
		ret = _parser.SetArguments(('Test1', 'Test2', 'Test3', 'Test4'))
		Assert.AreEqual(('Test4',), ret)
		
	[Test]
	def SetLongOptions():
		ret = _parser.SetArguments(('Test', '--force'))
		test = _parser.Target as Test
		Assert.IsTrue(test.Force, "Should be forced")
		Assert.AreEqual(0, len(ret))

	[Test]
	def SetShortOptions():
		ret = _parser.SetArguments(('Test', '-f'))
		test = _parser.Target as Test
		Assert.IsTrue(test.Force, "Should be forced")
		Assert.AreEqual(0, len(ret))

	[Test]
	def SetArgumentsAndOptionsLast():
		ret = _parser.SetArguments(('Test3', 'Test4', '--force'))
		test = _parser.Target as Test
		Assert.AreEqual('Test3', test.Name)
		Assert.AreEqual('Test4', test.Name2)
		Assert.IsTrue(test.Force, "Should be forced")
		Assert.AreEqual(0, len(ret))
		
	[Test]
	def SetArgumentsAndOptionsFirst():
		ret = _parser.SetArguments(('-f', 'Test3', 'Test4'))
		test = _parser.Target as Test
		Assert.AreEqual('Test3', test.Name)
		Assert.AreEqual('Test4', test.Name2)
		Assert.IsTrue(test.Force, "Should be forced")
		Assert.AreEqual(0, len(ret))
		
	[Test]
	def IsValid():
		ret = _parser.SetArguments(('Test',))
		Assert.IsFalse(_parser.IsValid(), "Not valid, missing 1 argument")
		_parser.SetArguments(('Test', 'Test'))
		Assert.IsTrue(_parser.IsValid(), "Shoud be valid")
		Assert.AreEqual(0, len(ret))
		
	class Test:
		[Property(Name), Argument("The name")] _name as string
		[Property(Name2), Argument("The second name")] _name2 as string
		[Property(Name3), Argument("The third name", Optional: true)] _name3 as string
		[Property(Force), Option('force', 'f', "Force it ?")] _force as bool
