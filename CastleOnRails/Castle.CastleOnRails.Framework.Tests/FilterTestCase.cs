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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine;

	[TestFixture]
	public class FilterTestCase
	{
		FakeViewEngine _viewEngine;
		ProcessEngine _engine;

		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new FakeViewEngine();
			_engine = new ProcessEngine("myvirdir", factory, _viewEngine);
		}

		[Test]
		public void FilteringAndSkippingFiltering()
		{
			_viewEngine.AddView("filtered", "index", "index view contents");
			_viewEngine.AddView("filtered", "save", "save view contents");
			_viewEngine.AddView("filtered", "update", "update view contents");

			// Non filtered action
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/filtered/index.rails");
			_engine.Process( context );
			Assert.AreEqual( "index view contents", context.Output );

			// Filtered action
			context = new RailsEngineContextImpl("/myvirdir/filtered/save.rails");
			_engine.Process( context );
			Assert.AreEqual( "(before)(after)save view contents", context.Output );

			// Filtered action
			context = new RailsEngineContextImpl("/myvirdir/filtered/update.rails");
			_engine.Process( context );
			Assert.AreEqual( "(before)(after)update view contents", context.Output );
		}

		[Test]
		public void BeforeFilters()
		{
			_viewEngine.AddView("filtered2", "index", "index view contents");
			_viewEngine.AddView("filtered2", "save", "save view contents");
			_viewEngine.AddView("filtered2", "update", "update view contents");

			// Non filtered action
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/filtered2/index.rails");
			_engine.Process( context );
			Assert.AreEqual( "index view contents", context.Output );

			// Filtered action
			context = new RailsEngineContextImpl("/myvirdir/filtered2/save.rails");
			_engine.Process( context );
			Assert.AreEqual( "(before)save view contents", context.Output );

			// Filtered action
			context = new RailsEngineContextImpl("/myvirdir/filtered2/update.rails");
			_engine.Process( context );
			Assert.AreEqual( "(before)update view contents", context.Output );
		}
	}
}
