// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace AspectSharp.Lang.Tests
{
	using System;

	using antlr;

	using NUnit.Framework;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for ParserAspectTestCase.
	/// </summary>
	[TestFixture]
	public class ParserAspectTestCase : ParserTestCaseBase
	{
		[Test]
		public void ParsingAspectEmptyDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual("MyNamespace.MyType", def.TargetType.SingleType.TypeName);
			Assert.AreEqual(TargetStrategyEnum.SingleType, def.TargetType.TargetStrategy);
		}

		[Test]
		public void ParsingAspectForAssignable()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ assignableFrom(Customer) ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual(TargetStrategyEnum.Assignable, def.TargetType.TargetStrategy);
			Assert.AreEqual("Customer", def.TargetType.AssignType.TypeName);
		}

		[Test]
		[ExpectedException(typeof (MismatchedTokenException))]
		public void InvalidAspectForAssignable()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ assignableFrom() ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
		}

		[Test]
		public void ParsingAspectForCustomMatcher()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ customMatcher(MyMatcher) ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual(TargetStrategyEnum.Custom, def.TargetType.TargetStrategy);
			Assert.AreEqual("MyMatcher", def.TargetType.CustomMatcherType.TypeName);
		}

		[Test]
		public void ParsingAspectForNamespace()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ my.namespace.types ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual(TargetStrategyEnum.Namespace, def.TargetType.TargetStrategy);
			Assert.AreEqual("my.namespace.types", def.TargetType.NamespaceRoot);
			Assert.IsFalse(def.TargetType.IncludeSubNamespace);
		}

		[Test]
		public void ParsingAspectForNamespaceWithExcludes()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ my.namespace.types excludes(Customer;Author) ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual(TargetStrategyEnum.Namespace, def.TargetType.TargetStrategy);
			Assert.AreEqual("my.namespace.types", def.TargetType.NamespaceRoot);
			Assert.IsFalse(def.TargetType.IncludeSubNamespace);
			Assert.AreEqual(2, def.TargetType.Excludes.Count);
		}

		[Test]
		[ExpectedException(typeof (MismatchedTokenException))]
		public void InvalidAspectForCustomMatcher()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for [ customMatcher() ] \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
		}

		[Test]
		public void ParsingAspectEmptyDeclarationWithFullType()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType in My.New.Assembly \r\n" +
					"end");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Aspects);
			Assert.AreEqual(1, conf.Aspects.Count);
			AspectDefinition def = conf.Aspects[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("XPTO", def.Name);
			Assert.AreEqual("MyNamespace.MyType", def.TargetType.SingleType.TypeName);
			Assert.AreEqual("My.New.Assembly", def.TargetType.SingleType.AssemblyReference.AssemblyName);
		}

		[Test]
		public void ParsingAspectWithMixinRefDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
					"" +
					"  include \"customer\"" +
					"" +
					"" +
					"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.Mixins.Count);

			MixinDefinition typeName = def.Mixins[0];
			Assert.AreEqual(TargetTypeEnum.Link, typeName.TypeReference.TargetType);
			Assert.AreEqual("customer", typeName.TypeReference.LinkRef);
		}

		[Test]
		public void ParsingAspectWithSingleMixinDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
					"" +
					"  include MyNamespace.Type in MyAssembly " +
					"" +
					"" +
					"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.Mixins.Count);

			MixinDefinition typeName = def.Mixins[0];
			Assert.AreEqual(TargetTypeEnum.Type, typeName.TypeReference.TargetType);
			Assert.AreEqual("MyNamespace.Type", typeName.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", typeName.TypeReference.AssemblyReference.AssemblyName);
		}

		[Test]
		public void ParsingAspectWithAFewMixinDeclarations()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
					"" +
					"  include MyNamespace.Type1 in MyAssembly1 " +
					"  include MyNamespace.Type2 in MyAssembly2 " +
					"  include MyNamespace.Type3 in MyAssembly3 " +
					"" +
					"" +
					"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(3, def.Mixins.Count);

			MixinDefinition typeName = def.Mixins[0];
			Assert.AreEqual(TargetTypeEnum.Type, typeName.TypeReference.TargetType);
			Assert.AreEqual("MyNamespace.Type1", typeName.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly1", typeName.TypeReference.AssemblyReference.AssemblyName);

			typeName = def.Mixins[1];
			Assert.AreEqual(TargetTypeEnum.Type, typeName.TypeReference.TargetType);
			Assert.AreEqual("MyNamespace.Type2", typeName.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly2", typeName.TypeReference.AssemblyReference.AssemblyName);

			typeName = def.Mixins[2];
			Assert.AreEqual(TargetTypeEnum.Type, typeName.TypeReference.TargetType);
			Assert.AreEqual("MyNamespace.Type3", typeName.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly3", typeName.TypeReference.AssemblyReference.AssemblyName);
		}

		[Test]
		[ExpectedException(typeof (NoViableAltException))]
		public void ParsingInvalidAspectEmptyDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
					"");
			parser.Parse();
		}
	}
}