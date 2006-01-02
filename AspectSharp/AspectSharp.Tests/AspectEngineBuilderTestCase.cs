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

namespace AspectSharp.Tests
{
	using System;

	using AspectSharp.Builder;
	using AspectSharp.Lang.AST;
	using AspectSharp.Tests.Classes;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for AspectEngineBuilderTestCase.
	/// </summary>
	[TestFixture]
	public class AspectEngineBuilderTestCase
	{
		[Test]
		public void BuildUsingLanguage()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" interceptors [" + 
				" \"key\" : DummyInterceptor " + 
				" ]" + 
				" mixins [" + 
				" \"key\" : DummyMixin " + 
				" ]" + 
				" " + 
				" aspect McBrother for DummyCustomer " + 
				"   include \"key\"" + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(\"key\")" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);

			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void BuildUsingLanguageWithDifferingKeys()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" interceptors [" + 
				" \"interceptor\" : DummyInterceptor " + 
				" ]" + 
				" mixins [" + 
				" \"mixin\" : DummyMixin " + 
				" ]" + 
				" " + 
				" aspect McBrother for DummyCustomer " + 
				"   include \"mixin\"" + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(\"interceptor\")" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);

			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void BuildUsingXmlWithLanguageInCData()
		{
			String xmlContents = "<configuration>" + 
				" <![CDATA[" +
				" import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" interceptors [" + 
				" \"key\" : DummyInterceptor " + 
				" ]" + 
				" mixins [" + 
				" \"key\" : DummyMixin " + 
				" ]" + 
				" " + 
				" aspect McBrother for DummyCustomer " + 
				"   include \"key\"" + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(\"key\")" + 
				"   end" + 
				"   " + 
				" end " +
				" ]]>" +
				"</configuration>";
			XmlEngineBuilder builder = new XmlEngineBuilder(xmlContents);
			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void BuildUsingXmlWithLanguageInConfigurationNode()
		{
			String xmlContents = "<configuration>" + 
				" import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" interceptors [" + 
				" \"key\" : DummyInterceptor " + 
				" ]" + 
				" mixins [" + 
				" \"key\" : DummyMixin " + 
				" ]" + 
				" " + 
				" aspect McBrother for DummyCustomer " + 
				"   include \"key\"" + 
				"   " + 
				"   pointcut method(*)" + 
				"     advice(\"key\")" + 
				"   end" + 
				"   " + 
				" end " +
				"</configuration>";
			XmlEngineBuilder builder = new XmlEngineBuilder(xmlContents);
			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void BuildUsingAppDomainConfiguration()
		{
			AppDomainConfigurationBuilder builder = new AppDomainConfigurationBuilder();
			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void BuildUsingXml()
		{
			String xmlContents = "<configuration>" + 
				"<import namespace=\"AspectSharp.Tests.Classes\" assembly=\"AspectSharp.Tests\" />" + 
				"<mixins>" + 
				"<mixin key=\"key\" type=\"DummyMixin\" refTypeEnum=\"Type\" />" +
				"</mixins><interceptors>" +
				"<interceptor key=\"key\" type=\"DummyInterceptor\" refTypeEnum=\"Type\" />" +
				"</interceptors>" +
				"<aspect name=\"McBrother\"><for>" +
				"<singletype type=\"DummyCustomer\" refTypeEnum=\"Type\" />" +
				"</for>" +
				"<mixin type=\"key\" refTypeEnum=\"Link\" />" +
				"<pointcut symbol=\"Method\"><signature>(*)</signature>" +
				"<interceptor type=\"key\" refTypeEnum=\"Link\" />" +
				"</pointcut>" +
				"</aspect>" +
				"</configuration>";
			XmlEngineBuilder builder = new XmlEngineBuilder(xmlContents);
			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		[Test]
		public void XmlWithMoreComplexMethodSignature()
		{
			String xmlContents = "<configuration>" + 
				"<import namespace=\"AspectSharp.Tests.Classes\" assembly=\"AspectSharp.Tests\" />" + 
				"<mixins>" + 
				"<mixin key=\"key\" type=\"DummyMixin\" refTypeEnum=\"Type\" />" +
				"</mixins><interceptors>" +
				"<interceptor key=\"key\" type=\"DummyInterceptor\" refTypeEnum=\"Type\" />" +
				"</interceptors>" +
				"<aspect name=\"McBrother\"><for>" +
				"<singletype type=\"DummyCustomer\" refTypeEnum=\"Type\" />" +
				"</for>" +
				"<mixin type=\"key\" refTypeEnum=\"Link\" />" +
				"<pointcut symbol=\"Method\"><signature>(void Name(*))</signature>" +
				"<interceptor type=\"key\" refTypeEnum=\"Link\" />" +
				"</pointcut>" +
				"</aspect>" +
				"</configuration>";
			XmlEngineBuilder builder = new XmlEngineBuilder(xmlContents);
			AspectEngine engine = builder.Build();

			Assert.IsNotNull(engine);
			Assert.IsNotNull(engine.Configuration);

			AspectDefinition aspect = engine.Configuration.Aspects[0];
			Assert.AreEqual(1, aspect.Mixins.Count);

			PointCutDefinition pointcut = aspect.PointCuts[0];
			Assert.IsTrue( pointcut.Method.AllArguments );
			Assert.AreEqual( "void", pointcut.Method.RetType );
			Assert.AreEqual("Name", pointcut.Method.MethodName );

			Assert.AreEqual(1, pointcut.Advices.Count);
			InterceptorDefinition advice = pointcut.Advices[0];
			Assert.AreEqual( typeof(DummyInterceptor), advice.TypeReference.ResolvedType );
		}

		[Test]
		public void BuildUsingCode()
		{
			CodeEngineBuilder builder = new CodeEngineBuilder();
			EngineConfiguration conf = builder.GetConfiguration();
			
			ImportDirective import = new ImportDirective(LexicalInfo.Empty, "AspectSharp.Tests.Classes");
			import.AssemblyReference = new AssemblyReference(LexicalInfo.Empty, "AspectSharp.Tests");
			conf.Imports.Add( import );

			conf.Mixins.Add( "key", LexicalInfo.Empty ).TypeReference = new TypeReference(LexicalInfo.Empty, "DummyMixin"); 
			conf.Interceptors.Add( "key", LexicalInfo.Empty ).TypeReference = new TypeReference(LexicalInfo.Empty, "DummyInterceptor"); 

			AspectDefinition aspect = new AspectDefinition(LexicalInfo.Empty, "McBrother");
			aspect.TargetType = new TargetTypeDefinition();
			aspect.TargetType.SingleType = new TypeReference(LexicalInfo.Empty, "DummyCustomer");
			conf.Aspects.Add(aspect);
			
			MixinDefinition mixin = new MixinDefinition(LexicalInfo.Empty);
			mixin.TypeReference = new TypeReference(LexicalInfo.Empty, "key", TargetTypeEnum.Link);
			aspect.Mixins.Add( mixin );
			
			PointCutDefinition pointcut = new PointCutDefinition(LexicalInfo.Empty, PointCutFlags.Method);
			pointcut.Method = AllMethodSignature.Instance;

			InterceptorDefinition interceptor = new InterceptorDefinition(LexicalInfo.Empty);
			interceptor.TypeReference = new TypeReference(LexicalInfo.Empty, "key", TargetTypeEnum.Link);
			pointcut.Advices.Add(interceptor);

			aspect.PointCuts.Add(pointcut);

			AspectEngine engine = builder.Build();
			AssertEngineConfiguration(engine);
		}

		private static void AssertEngineConfiguration(AspectEngine engine)
		{
			Assert.IsNotNull(engine);
			Assert.IsNotNull(engine.Configuration);
			Assert.AreEqual(1, engine.Configuration.Imports.Count);
			Assert.AreEqual(1, engine.Configuration.Mixins.Count);
			Assert.AreEqual(1, engine.Configuration.Interceptors.Count);
			Assert.AreEqual(1, engine.Configuration.Aspects.Count);

			AspectDefinition aspect = engine.Configuration.Aspects[0];
			Assert.AreEqual("McBrother", aspect.Name);
			Assert.AreEqual( typeof(DummyCustomer), aspect.TargetType.SingleType.ResolvedType );
			
			Assert.AreEqual(1, aspect.Mixins.Count);
			MixinDefinition mixin = aspect.Mixins[0];
			Assert.AreEqual( typeof(DummyMixin), mixin.TypeReference.ResolvedType );

			Assert.AreEqual(1, aspect.PointCuts.Count);
			PointCutDefinition pointcut = aspect.PointCuts[0];
			Assert.AreEqual(AllMethodSignature.Instance, pointcut.Method );

			Assert.AreEqual(1, pointcut.Advices.Count);
			InterceptorDefinition advice = pointcut.Advices[0];
			Assert.AreEqual( typeof(DummyInterceptor), advice.TypeReference.ResolvedType );
		}
	}
}
