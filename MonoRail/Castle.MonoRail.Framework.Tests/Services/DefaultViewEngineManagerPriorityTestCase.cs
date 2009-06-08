// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultViewEngineManagerPriorityTestCase
	{
		DefaultViewEngineManager viewEngineManager;
		IViewEngine firstEngine;
		IViewEngine secondEngine;
		
		[SetUp]
		public void Before_each_test()
		{
			viewEngineManager = new DefaultViewEngineManager();
			firstEngine = new FirstEngine();
			viewEngineManager.RegisterEngineForExtesionLookup(firstEngine);
			secondEngine = new SecondEngine();
			viewEngineManager.RegisterEngineForExtesionLookup(secondEngine);
		}

		[Test]
		public void When_resolving_engine_should_lookup_by_registering_order()
		{
			var resolvedEngine = viewEngineManager.ResolveEngine("templateName", true);
			Assert.IsNotNull(resolvedEngine);
			Assert.IsInstanceOf(typeof(FirstEngine), resolvedEngine);
		}
	}

	#region First Engine

	internal class FirstEngine : ViewEngineBase
	{
		public override string ViewFileExtension
		{
			get { return "first"; }
		}

		public override bool HasTemplate(string templateName)
		{
			return true;
		}

		public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
		{
		}

		public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		public override string JSGeneratorFileExtension
		{
			get { return "firstjs"; }
		}

		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}
	}

	#endregion

	#region Second Engine

	internal class SecondEngine : ViewEngineBase
	{
		public override string ViewFileExtension
		{
			get { return "Second"; }
		}

		public override bool HasTemplate(string templateName)
		{
			return true;
		}

		public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
		{
		}

		public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}

		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		public override string JSGeneratorFileExtension
		{
			get { return "secondjs"; }
		}

		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
		}
	}

	#endregion
}
