// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.DynamicDispatching;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using NUnit.Framework;

	[TestFixture]
	public class JSGeneratorDispatcherBaseTestCase
	{
		private JSGeneratorDispatcher dispatcher;
		private JSCodeGenerator codeGen;

		[SetUp]
		public void Init()
		{
			codeGen = new JSCodeGenerator();
			dispatcher = new JSGeneratorDispatcher(codeGen, new PrototypeGenerator(codeGen), new object[0], new object[0]);
		}

		[Test]
		public void SimpleCallGeneratesDelegateToMethodInvocation()
		{
			dispatcher.Invoke("InsertHtml", new object[] { "Top", "id", "something here" });

			Assert.AreEqual("new Insertion.Top(\"id\",\"something here\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void ElCallGeneratesAccessToElement()
		{
			dispatcher.Invoke("el", new object[] { "id" });

			Assert.AreEqual("$('id');\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void ElementCanChainOperations()
		{
			JSElementGeneratorDispatcher elementDispatcher = (JSElementGeneratorDispatcher) dispatcher.Invoke("el", new object[] { "id" });

			elementDispatcher.Invoke("replace", "something");

			Assert.AreEqual("$('id').replace(something);\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void SetOnElementGeneratesAssignment()
		{
			JSElementGeneratorDispatcher elementDispatcher = (JSElementGeneratorDispatcher) dispatcher.Invoke("el", new object[] { "id" });

			elementDispatcher.Invoke("set", "something");

			Assert.AreEqual("$('id') = something;\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void GetOnElementGeneratesNestedIdentifierAccess()
		{
			JSElementGeneratorDispatcher elementDispatcher = (JSElementGeneratorDispatcher) dispatcher.Invoke("el", new object[] { "id" });

			elementDispatcher.Get("style");
			elementDispatcher.Get("color");
			elementDispatcher.Invoke("set", "'white'");

			Assert.AreEqual("$('id').style.color = 'white';\r\n", codeGen.Lines.ToString());
		}
		
		internal class JSGeneratorDispatcher : JSGeneratorDispatcherBase
		{
			public JSGeneratorDispatcher(IJSCodeGenerator codeGen, IJSGenerator generator, object[] extensions,
			                             object[] elementExtensions) : base(codeGen, generator, extensions, elementExtensions)
			{
			}

			public object Invoke(string method, params object[] args)
			{
				return InternalInvoke(method, args);
			}

			protected override object CreateNullGenerator()
			{
				throw new NotImplementedException();
			}

			protected override object CreateJSElementGeneratorProxy(IJSCodeGenerator codeGen,
			                                                        IJSElementGenerator elementGenerator,
			                                                        object[] elementExtensions)
			{
				return new JSElementGeneratorDispatcher(codeGen, elementGenerator, elementExtensions);
			}
		}

		internal class JSElementGeneratorDispatcher : JSElementGeneratorDispatcherBase
		{
			public JSElementGeneratorDispatcher(IJSCodeGenerator codeGen,
			                                    IJSElementGenerator elementGenerator, params object[] extensions) :
			                                    	base(codeGen, elementGenerator, extensions)
			{
			}

			public void Get(string propName)
			{
				InternalGet(propName);
			}

			public object Invoke(string method, params object[] args)
			{
				return InternalInvoke(method, args);
			}
		}
	}
}