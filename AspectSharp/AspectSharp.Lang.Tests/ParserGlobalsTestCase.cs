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

	using NUnit.Framework;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for ParserGlobalsTestCase.
	/// </summary>
	[TestFixture]
	public class ParserGlobalsTestCase : ParserTestCaseBase
	{
		[Test]
		public void ParsingMixinDeclaration()
		{
			AspectParser parser = CreateParser(
				"mixins \r\n" +
				"[" + 
				"\"customer\" : CustomerMixin" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Mixins);
			Assert.AreEqual(1, conf.Mixins.Count);
			MixinEntryDefinition def = conf.Mixins[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("customer", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("CustomerMixin", def.TypeReference.TypeName);
		}

		[Test]
		public void ParsingMixinDeclarationWithAssembly()
		{
			AspectParser parser = CreateParser(
				"mixins \r\n" +
				"[" + 
				"\"customer\" : Namespace.CustomerMixin in MyAssembly" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Mixins);
			Assert.AreEqual(1, conf.Mixins.Count);
			MixinEntryDefinition def = conf.Mixins[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("customer", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.CustomerMixin", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

		}

		[Test]
		public void ParsingMixinsDeclarations()
		{
			AspectParser parser = CreateParser(
				"mixins \r\n" +
				"[" + 
				"  \"key1\" : Namespace.CustomerMixin1 in MyAssembly;" +
				"  \"key2\" : Namespace.CustomerMixin2 in MyAssembly;" +
				"  \"key3\" : Namespace.CustomerMixin3 in MyAssembly" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Mixins);
			Assert.AreEqual(3, conf.Mixins.Count);

			MixinEntryDefinition def = conf.Mixins[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("key1", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.CustomerMixin1", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

			def = conf.Mixins[1];
			Assert.IsNotNull(def);
			Assert.AreEqual("key2", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.CustomerMixin2", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

			def = conf.Mixins[2];
			Assert.IsNotNull(def);
			Assert.AreEqual("key3", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.CustomerMixin3", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);
		}

		[Test]
		public void ParsingInvalidMixinsDeclarations()
		{
			try
			{
				AspectParser parser = CreateParser(
					"mixins \r\n" +
					"[" + 
					"  key1 : Namespace.CustomerMixin1 in MyAssembly" +
					"]");
				parser.Parse();

				Assert.Fail("Invalid language content");
			}
			catch(Exception)
			{
				// Excepted
			}

			try
			{
				AspectParser parser = CreateParser(
					"mixins \r\n" +
					"[" + 
					"  \"key1\" : Namespace.CustomerMixin1 in MyAssembly" +
					"");
				parser.Parse();

				Assert.Fail("Invalid language content");
			}
			catch(Exception)
			{
				// Excepted
			}
		}

		[Test]
		public void ParsingInterceptorDeclaration()
		{
			AspectParser parser = CreateParser(
				"interceptors \r\n" +
				"[" + 
				"\"customer\" : Interceptor" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Interceptors);
			Assert.AreEqual(1, conf.Interceptors.Count);
			InterceptorEntryDefinition def = conf.Interceptors[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("customer", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Interceptor", def.TypeReference.TypeName);
		}

		[Test]
		public void ParsingInterceptorDeclarationWithAssembly()
		{
			AspectParser parser = CreateParser(
				"interceptors \r\n" +
				"[" + 
				"\"customer\" : Namespace.Interceptor in MyAssembly" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Interceptors);
			Assert.AreEqual(1, conf.Interceptors.Count);
			InterceptorEntryDefinition def = conf.Interceptors[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("customer", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.Interceptor", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);
		}

		[Test]
		public void ParsingInterceptorDeclarations()
		{
			AspectParser parser = CreateParser(
				"interceptors \r\n" +
				"[" + 
				"  \"key1\" : Namespace.Interceptor1 in MyAssembly;" +
				"  \"key2\" : Namespace.Interceptor2 in MyAssembly;" +
				"  \"key3\" : Namespace.Interceptor3 in MyAssembly" +
				"]");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Interceptors);
			Assert.AreEqual(3, conf.Interceptors.Count);

			InterceptorEntryDefinition def = conf.Interceptors[0];
			Assert.IsNotNull(def);
			Assert.AreEqual("key1", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.Interceptor1", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

			def = conf.Interceptors[1];
			Assert.IsNotNull(def);
			Assert.AreEqual("key2", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.Interceptor2", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

			def = conf.Interceptors[2];
			Assert.IsNotNull(def);
			Assert.AreEqual("key3", def.Key);
			Assert.AreEqual(TargetTypeEnum.Type, def.TypeReference.TargetType);
			Assert.AreEqual("Namespace.Interceptor3", def.TypeReference.TypeName);
			Assert.AreEqual("MyAssembly", def.TypeReference.AssemblyReference.AssemblyName);

		}

		[Test]
		public void ParsingInvalidInterceptorDeclarations()
		{
			try
			{
				AspectParser parser = CreateParser(
					"interceptors \r\n" +
					"[" + 
					"  key1 : Namespace.Interceptor in MyAssembly" +
					"]");
				parser.Parse();

				Assert.Fail("Invalid language content");
			}
			catch(Exception)
			{
				// Excepted
			}

			try
			{
				AspectParser parser = CreateParser(
					"interceptors \r\n" +
					"[" + 
					"  \"key1\" : Namespace.Interceptor in MyAssembly" +
					"");
				parser.Parse();

				Assert.Fail("Invalid language content");
			}
			catch(Exception)
			{
				// Excepted
			}
		}
	}
}
