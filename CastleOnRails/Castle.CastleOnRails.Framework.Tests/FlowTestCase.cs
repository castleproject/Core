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

	/// <summary>
	/// Summary description for FlowTestCase.
	/// </summary>
	[TestFixture]
	public class FlowTestCase
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
		public void SimpleRequest()
		{
			_viewEngine.AddView("home", "index", "hello world!");

			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/home/index.rails");

			_engine.Process( context );

			Assert.AreEqual( "hello world!", context.Output );
		}

		[Test]
		public void SimpleRequestWithDifferentView()
		{
			_viewEngine.AddView("home", "index", "hello from index");
			_viewEngine.AddView("home", "display", "hello from display");

			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/home/other.rails");

			_engine.Process( context );

			Assert.AreEqual( "hello from display", context.Output );
		}
	}
}
