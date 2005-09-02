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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;

	using Castle.MonoRail.Engine;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Tests;
	
	using NUnit.Framework;

	[TestFixture]
	public class ComponentsTestCase
	{
		NVelocityViewEngine _viewEngine;
		ProcessEngine _engine;

		[TestFixtureSetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new NVelocityViewEngine();
			_viewEngine.ViewRootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
				"../Castle.MonoRail.Framework.Views.NVelocity.Tests/views");

			DefaultViewComponentFactory viewFactory = new DefaultViewComponentFactory();
			viewFactory.Inspect( "Castle.MonoRail.Framework.Views.NVelocity.Tests" );
			
			_engine = new ProcessEngine(factory, _viewEngine, viewFactory);

			viewFactory.ViewEngine = _viewEngine;

			_viewEngine.ViewComponentFactory = viewFactory;

			_viewEngine.Init();
		}

		[Test]
		public void InlineComponentUsingRenderText()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index1.rails");

			_engine.Process( context );

			AssertOutput( "static 1\r\nHello from SimpleInlineViewComponent static 2", context.Output );
		}

		[Test]
		public void InlineComponentUsingRender()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index2.rails");

			_engine.Process( context );

			AssertOutput( "static 1\r\nThis is a view used by a component static 2", context.Output );
		}

		[Test]
		public void InlineComponentNotOverridingRender()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index3.rails");

			_engine.Process( context );

			AssertOutput( "static 1\r\ndefault component view picked up automatically static 2", context.Output );
		}

		[Test]
		public void InlineComponentWithParam1()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index4.rails");

			_engine.Process( context );

			AssertOutput( "Done", context.Output );
		}

		[Test]
		public void BlockComp1()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index5.rails");

			_engine.Process( context );

			AssertOutput( "  item 0\r\n  item 1\r\n  item 2\r\n", context.Output );
		}

		[Test]
		public void BlockWithinForEach()
		{
			RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index8.rails");

			_engine.Process( context );

			AssertOutput( "inner content 1\r\ninner content 2\r\n", context.Output );
		}

		[Test]
		public void SeveralComponentsInvocation()
		{
			for(int i=0; i < 10; i++)
			{
				RailsEngineContextImpl context = new RailsEngineContextImpl("/usingcomponents/index9.rails");

				_engine.Process( context );

				// System.Diagnostics.Debug.WriteLine( context.Output );

				AssertOutput( "static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n", context.Output );
			}
		}

		void AssertOutput(string expected, object output)
		{
			Assert.AreEqual(NormalizeWhitespace(expected), NormalizeWhitespace(output.ToString()));
		}

		string NormalizeWhitespace(string s)
		{
			return s.Replace("\r\n", "\n");
		}
	}
}
