// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	/// Summary description for ParserPointcutTestCase.
	/// </summary>
	[TestFixture]
	public class ParserPointcutTestCase : ParserTestCaseBase
	{
		[Test]
		public void ParsingSimplePointcutDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(*)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.PointCuts.Count);
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.IsNotNull( pointcut );
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( AllMethodSignature.Instance, pointcut.Method );
		}

		[Test]
		public void ParsingPropertyPointcutDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut property(*)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.PointCuts.Count);
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.IsNotNull( pointcut );
			Assert.AreEqual( PointCutFlags.Property, pointcut.Flags );
			Assert.AreEqual( AllMethodSignature.Instance, pointcut.Method );
		}

		[Test]
		public void ParsingMethodAndPropertyPointcutDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method|property(*)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.PointCuts.Count);
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.IsNotNull( pointcut );
			Assert.AreEqual( PointCutFlags.Property | PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( AllMethodSignature.Instance, pointcut.Method );
		}

		[Test]
		public void ParsingMethodAndPropertyWritePointcutDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method|propertywrite(*)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			Assert.AreEqual(1, def.PointCuts.Count);
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.IsNotNull( pointcut );
			Assert.AreEqual( PointCutFlags.PropertyWrite | PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( AllMethodSignature.Instance, pointcut.Method );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingNoArgument()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(* DoSomething)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 0, pointcut.Method.Arguments.Length );
			Assert.IsTrue( pointcut.Method.AllRetTypes );
			Assert.IsTrue( !pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingAlsoWithNoArgument()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(* DoSomething())" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 0, pointcut.Method.Arguments.Length );
			Assert.IsTrue( pointcut.Method.AllRetTypes );
			Assert.IsTrue( !pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingAllArgument()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(* DoSomething(*))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 1, pointcut.Method.Arguments.Length );
			Assert.IsTrue( pointcut.Method.AllRetTypes );
			Assert.IsTrue( pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingAllArgumentsAndIntRetType()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(int DoSomething(*))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 1, pointcut.Method.Arguments.Length );
			Assert.IsTrue( !pointcut.Method.AllRetTypes );
			Assert.AreEqual( "int", pointcut.Method.RetType );
			Assert.IsTrue( pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingWithTwoArgumentsAndRetType()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(int DoSomething(string, int))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 2, pointcut.Method.Arguments.Length );
			Assert.IsTrue( !pointcut.Method.AllRetTypes );
			Assert.AreEqual( "int", pointcut.Method.RetType );
			Assert.IsTrue( !pointcut.Method.AllArguments );
			Assert.AreEqual( "string", pointcut.Method.Arguments[0] );
			Assert.AreEqual( "int", pointcut.Method.Arguments[1] );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodDoSomethingWithTwoArgumentsAndRegEx()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(int DoSomething(string, *))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 2, pointcut.Method.Arguments.Length );
			Assert.IsTrue( !pointcut.Method.AllRetTypes );
			Assert.AreEqual( "int", pointcut.Method.RetType );
			Assert.IsTrue( !pointcut.Method.AllArguments );
			Assert.AreEqual( "string", pointcut.Method.Arguments[0] );
			Assert.AreEqual( "*", pointcut.Method.Arguments[1] );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodWithRegExp()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(* DoS.*(*))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoS.*", pointcut.Method.MethodName );
			Assert.AreEqual( 1, pointcut.Method.Arguments.Length );
			Assert.IsTrue( pointcut.Method.AllRetTypes );
			Assert.IsTrue( pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingPointcutDeclarationForAllMethodsReturningString()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(String (*))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( ".*", pointcut.Method.MethodName );
			Assert.AreEqual( 1, pointcut.Method.Arguments.Length );
			Assert.IsTrue( !pointcut.Method.AllRetTypes );
			Assert.IsTrue( pointcut.Method.AllArguments );
			Assert.AreEqual( "string", pointcut.Method.RetType );
		}

		[Test]
		public void ParsingPointcutDeclarationForMethodWithRegExpOnReturnType()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method(strin.* DoSomething(*))" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Method, pointcut.Flags );
			Assert.AreEqual( "DoSomething", pointcut.Method.MethodName );
			Assert.AreEqual( 1, pointcut.Method.Arguments.Length );
			Assert.IsTrue( !pointcut.Method.AllRetTypes );
			Assert.IsTrue( pointcut.Method.AllArguments );
			Assert.AreEqual( "strin.*", pointcut.Method.RetType );
		}

		[Test]
		public void ParsingPointcutDeclarationForPropertyNameNoArguments()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut property(* Name)" + 
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual( PointCutFlags.Property, pointcut.Flags );
			Assert.AreEqual( "Name", pointcut.Method.MethodName );
			Assert.AreEqual( 0, pointcut.Method.Arguments.Length );
			Assert.IsTrue( pointcut.Method.AllRetTypes );
			Assert.IsTrue( !pointcut.Method.AllArguments );
		}

		[Test]
		public void ParsingInterceptorRefForProperty()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut property(* Name)" + 
				"    advice(\"logger\")" +
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual(1, pointcut.Advices.Count);
			InterceptorDefinition adv = pointcut.Advices[0];
			Assert.IsNotNull( adv );
			Assert.AreEqual( "logger", adv.TypeReference.LinkRef );
		}

		[Test]
		public void ParsingInterceptorTypeForProperty()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut property(* Name)" + 
				"    advice( My.NS.Interceptor in My.Assembly )" +
				" end" + 
				" " + 
				"end");
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pointcut = def.PointCuts[0];
			Assert.AreEqual(1, pointcut.Advices.Count);
			InterceptorDefinition adv = pointcut.Advices[0];
			Assert.IsNotNull( adv );
			Assert.AreEqual( TargetTypeEnum.Type, adv.TypeReference.TargetType );
			Assert.AreEqual( "My.NS.Interceptor", adv.TypeReference.TypeName );
			Assert.AreEqual( "My.Assembly", adv.TypeReference.AssemblyReference.AssemblyName );
		}

		protected void DoParsing(string method, string access, string type, string name, string param1)
		{
			string allparams;
			if (param1 != null)
				allparams = "(" + param1 + ")";
			else
				allparams = string.Empty;					
				
			AspectParser parser = CreateParser(string.Format(
				"aspect XPTO for MyNamespace.MyType \n\n" +
				" pointcut {0}({1} {2} {3}{4}) \n" +
				"   advice(My.NS.Interceptor in My.Assembly ) \n" +
				" end \n" +
				"end", method, access, type, name, allparams));
			string s = string.Format("{0}({1} {2} {3}{4})", method, access, type, name, allparams);
			Console.WriteLine(s);
			EngineConfiguration conf = parser.Parse();
			AspectDefinition def = conf.Aspects[0];
			PointCutDefinition pc = def.PointCuts[0];
			if (name == "*")
				name = ".*";
			Assert.AreEqual(string.Format("({0} {1} {2}({3}))", access == "" ? "*" : access, type, name, param1 == null ? "" : param1), pc.Method.ToString());
			Assert.AreEqual(method == "method", ((pc.Flags & PointCutFlags.Method) != 0), "method check: " + s);
			Assert.AreEqual(access == "*" || access == "", pc.Method.AllAccess, "AllAccess: " + s);
			Assert.AreEqual(type == "*", pc.Method.AllRetTypes, "AllRetTypes: " + s);
			Assert.AreEqual(name, pc.Method.MethodName, "MethodName: " + s);
			Assert.AreEqual(access == "" ? "*" : access, pc.Method.Access, "Access: " + s);
			Assert.AreEqual(type, pc.Method.RetType, "RetType: " + s);
			if (param1 == null || param1.Length == 0)
				Assert.AreEqual(0, pc.Method.Arguments.Length, "No Params: " + s);
			else {
				Assert.AreEqual(1, pc.Method.Arguments.Length, "1 Param: " + s);
				Assert.AreEqual(param1, pc.Method.Arguments[0], "Param: " + s);
			}
		}

		[Test]
		public void TestParsingWithAccess()
		{
			string[] methods = new string[] { "method", "property" };
			string[] accesses = new string[] { "public", "protected", "*", "" };
			string[] types = new string[] { "*", "string" };
			string[] names = new string[] { "*", "Run.*", "Do_Something" };
			string[] param = new string[] { null, "*", "", "int" };

			foreach (string m in methods)
				foreach (string a in accesses)
					foreach (string t in types)
						foreach (string n in names)
							foreach (string p in param)
								DoParsing(m, a, t, n, p);
		}

		[Test]
		[ExpectedException(typeof(MismatchedTokenException))]
		public void ParsingInvalidPointcutDeclaration()
		{
			AspectParser parser = CreateParser(
				"aspect XPTO for MyNamespace.MyType \r\n" +
				" " + 
				" pointcut method|propertyread|property(*)" + 
				" end" + 
				" " + 
				"end");
			parser.Parse();
		}
	}
}
