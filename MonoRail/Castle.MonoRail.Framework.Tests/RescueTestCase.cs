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

namespace Castle.MonoRail.Framework.Tests
{
	using NUnit.Framework;

	using Castle.MonoRail.Engine;

	[TestFixture]
	public class RescueTestCase
	{
		FakeViewEngine _viewEngine;
		ProcessEngine _engine;

		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new FakeViewEngine();
			_engine = new ProcessEngine(factory, _viewEngine);
		}

		[Test]
		public void GeneralAndSpecificRescue()
		{
			_viewEngine.AddView("exception", "index", "index view contents");
			_viewEngine.AddView("exception", "save", "save view contents");
			_viewEngine.AddView("exception", "update", "update view contents");
			_viewEngine.AddView("rescues", "generalrescue", "general error info");
			_viewEngine.AddView("rescues", "saverescue", "error on save");
			_viewEngine.AddView("rescues", "updaterescue", "error on update");

			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/section/exception/index.rails");
			_engine.Process( context );
			Assert.AreEqual( "general error info", context.Output );

			context = new RailsEngineContextImpl("/myvirdir/section/exception/save.rails");
			_engine.Process( context );
			Assert.AreEqual( "error on save", context.Output );

			context = new RailsEngineContextImpl("/myvirdir/section/exception/update.rails");
			_engine.Process( context );
			Assert.AreEqual( "error on update", context.Output );
		}
	}
}
