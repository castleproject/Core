// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine;

	[TestFixture]
	public class LayoutTestCase 
	{
		FakeViewEngine _viewEngine;
		ProcessEngine _engine;

		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new FakeViewEngineWithLayoutSupport();
			_engine = new ProcessEngine(factory, _viewEngine);
		}

		[Test]
		public void LayoutUsage()
		{
			_viewEngine.AddView("layout", "index", "index view contents");
			_viewEngine.AddView("layout", "save", "save view contents");
			_viewEngine.AddView("layout", "update", "update view contents");
			_viewEngine.AddView("layouts", "general", "My layout! follows the contents {0}");

			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/section/layout/index.rails");
			_engine.Process( context );
			Assert.AreEqual( "My layout! follows the contents index view contents", context.Output );

			context = new RailsEngineContextImpl("/myvirdir/section/layout/save.rails");
			_engine.Process( context );
			Assert.AreEqual( "My layout! follows the contents save view contents", context.Output );

			context = new RailsEngineContextImpl("/myvirdir/section/layout/update.rails");
			_engine.Process( context );
			Assert.AreEqual( "My layout! follows the contents update view contents", context.Output );
		}
	}
}
