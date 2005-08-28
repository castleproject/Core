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

	using NUnit.Framework;

	using Castle.MonoRail.Engine;

	using Castle.MonoRail.Framework.Tests;

	[TestFixture]
	public class ViewEngineTestCase
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
			_viewEngine.Init();
			
			_engine = new ProcessEngine(factory, _viewEngine);
		}

		[Test]
		public void SimpleRequest()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/home/index.rails");

			_engine.Process( context );

			Assert.AreEqual( "NVelocity is great!", context.Output );
		}

		[Test]
		public void SimpleRequestWithDifferentView()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/home/other.rails");

			_engine.Process( context );

			Assert.AreEqual( "MonoRail is great!\r\n", context.Output );
		}

		[Test]
		public void PropertyBag()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/home/bag.rails");

			_engine.Process( context );

			Assert.AreEqual( "hammett is the name\r\n  a\r\n  b\r\n  c\r\n", context.Output );
		}

		[Test]
		public void Layout()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/layout/index.rails");

			_engine.Process( context );

			Assert.AreEqual( "My layout\r\n\r\nindex contents\r\n\r\nFooter", context.Output );

			context = new RailsEngineContextImpl("/layout/save.rails");

			_engine.Process( context );

			Assert.AreEqual( "My printable layout\r\n\r\nsave contents\r\n\r\nFooter", context.Output );
		}

		[Test]
		public void Resource()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/home/resource.rails");

			_engine.Process( context );

			Assert.AreEqual( "current language = english\r\n", context.Output );
		}
	}
}
