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

namespace AspectSharp.Lang.Tests.Visitors
{
	using System;
	// using System.Xml;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.AST.Visitors;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for XmlTreeVisitorTestCase.
	/// </summary>
	[TestFixture]
	public class XmlTreeVisitorTestCase : ParserTestCaseBase
	{
		[Test]
		public void GenerateImports()
		{
			AspectParser parser = CreateParser(
				"import XPTO " + 
				"import XY in My.Assembly ");
			EngineConfiguration conf = parser.Parse();
			
			XmlTreeVisitor visitor = new XmlTreeVisitor();
			visitor.Visit( conf );
			String content = visitor.Document.InnerXml;
			Assert.AreEqual( "<configuration>" + 
			 	"<import namespace=\"XPTO\" />" + 
			 	"<import namespace=\"XY\" assembly=\"My.Assembly\" />" + 
			 	"</configuration>", content );
		}

		[Test]
		public void GenerateGlobalMixinsAndInterceptors()
		{
			AspectParser parser = CreateParser(
				"import XPTO " + 
				"import XY in My.Assembly " + 
				" " + 
				" interceptors [ \"key\" : MyType in MyAssembly; \"key2\" : MyTypeThatMustBeResolved ] " + 
				" mixins [ \"key\" : MyType in MyAssembly; \"key2\" : MyTypeThatMustBeResolved ] " + 
				" " + 
				" " + 
				" ");
			EngineConfiguration conf = parser.Parse();
			
			XmlTreeVisitor visitor = new XmlTreeVisitor();
			visitor.Visit( conf );
			String content = visitor.Document.InnerXml;
			Assert.AreEqual( "<configuration>" + 
				"<import namespace=\"XPTO\" />" + 
				"<import namespace=\"XY\" assembly=\"My.Assembly\" />" +
				"<mixins><mixin key=\"key\" type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" +
				"<mixin key=\"key2\" type=\"MyTypeThatMustBeResolved\" refTypeEnum=\"Type\" />" +
				"</mixins><interceptors>" +
				"<interceptor key=\"key\" type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" +
				"<interceptor key=\"key2\" type=\"MyTypeThatMustBeResolved\" refTypeEnum=\"Type\" />" + 
				"</interceptors></configuration>", content );
		}

		[Test]
		public void GenerateAspect()
		{
			AspectParser parser = CreateParser(
				"import XPTO " + 
				"import XY in My.Assembly " + 
				" " + 
				" aspect McBrother for MyType in MyAssembly " + 
				" end" + 
				" ");
			EngineConfiguration conf = parser.Parse();
			
			XmlTreeVisitor visitor = new XmlTreeVisitor();
			visitor.Visit( conf );
			String content = visitor.Document.InnerXml;
			Assert.AreEqual( "<configuration>" + 
				"<import namespace=\"XPTO\" />" + 
				"<import namespace=\"XY\" assembly=\"My.Assembly\" />" + 
				"<aspect name=\"McBrother\"><for>" + 
				"<singletype type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" + 
				"</for></aspect></configuration>", content );
		}

		[Test]
		public void GenerateAspectWithIncludes()
		{
			AspectParser parser = CreateParser(
				"import XPTO " + 
				"import XY in My.Assembly " + 
				" " + 
				" aspect McBrother for MyType in MyAssembly " + 
				"   include \"key\"" + 
				"   include MyType in MyAssembly" + 
				" end" + 
				" ");
			EngineConfiguration conf = parser.Parse();
			
			XmlTreeVisitor visitor = new XmlTreeVisitor();
			visitor.Visit( conf );
			String content = visitor.Document.InnerXml;
			Assert.AreEqual( "<configuration>" +
				"<import namespace=\"XPTO\" />" +
				"<import namespace=\"XY\" assembly=\"My.Assembly\" />" +
				"<aspect name=\"McBrother\"><for>" +
				"<singletype type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" +
				"</for>" +
				"<mixin type=\"key\" refTypeEnum=\"Link\" />" +
				"<mixin type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" +
				"</aspect></configuration>", content );
		}

		[Test]
		public void GenerateAspectWithPointcuts()
		{
			AspectParser parser = CreateParser(
				"import XPTO " + 
				"import XY in My.Assembly " + 
				" " + 
				" aspect McBrother for MyType in MyAssembly " + 
				" " + 
				"   pointcut method(*)" + 
				"     advice(MyType)" + 
				"   end" + 
				" " + 
				" end" + 
				" ");
			EngineConfiguration conf = parser.Parse();
			
			XmlTreeVisitor visitor = new XmlTreeVisitor();
			visitor.Visit( conf );
			String content = visitor.Document.InnerXml;
			Assert.AreEqual( "<configuration>" +
				"<import namespace=\"XPTO\" />" +
				"<import namespace=\"XY\" assembly=\"My.Assembly\" />" +
				"<aspect name=\"McBrother\">" +
				"<for><singletype type=\"MyType\" refTypeEnum=\"Type\" assembly=\"MyAssembly\" />" +
				"</for>" +
				"<pointcut symbol=\"Method\"><signature>(*)</signature>" +
				"<interceptor type=\"MyType\" refTypeEnum=\"Type\" />" +
				"</pointcut></aspect></configuration>", content );
		}
	}
}
