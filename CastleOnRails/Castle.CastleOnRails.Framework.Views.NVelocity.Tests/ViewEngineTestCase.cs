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

namespace Castle.CastleOnRails.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine;

	using Castle.CastleOnRails.Framework.Tests;

	[TestFixture]
	public class ViewEngineTestCase
	{
		NVelocityViewEngine _viewEngine;
		ProcessEngine _engine;

		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new NVelocityViewEngine();
			_viewEngine.ViewRootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
				"../Castle.CastleOnRails.Framework.Views.NVelocity.Tests/views");
			
			_engine = new ProcessEngine("myvirdir", factory, _viewEngine);
		}

		[Test]
		public void SimpleRequest()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/home/index.rails");

			_engine.Process( context );

			Assert.AreEqual( "NVelocity is great!", context.Output );
		}

		[Test]
		public void SimpleRequestWithDifferentView()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/home/other.rails");

			_engine.Process( context );

			Assert.AreEqual( "Castle on rails is great!\r\n", context.Output );
		}

		[Test]
		public void PropertyBag()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/home/bag.rails");

			_engine.Process( context );

			Assert.AreEqual( "hammett is the name\r\n  a\r\n  b\r\n  c\r\n", context.Output );
		}

		[Test]
		public void Layout()
		{
			RailsEngineContextImpl context = new 
				RailsEngineContextImpl("/myvirdir/layout/index.rails");

			_engine.Process( context );

			Assert.AreEqual( "My layout\r\n\r\nindex contents\r\n\r\nFooter", context.Output );

			context = new RailsEngineContextImpl("/myvirdir/layout/save.rails");

			_engine.Process( context );

			Assert.AreEqual( "My printable layout\r\n\r\nsave contents\r\n\r\nFooter", context.Output );
		}
	}
}
