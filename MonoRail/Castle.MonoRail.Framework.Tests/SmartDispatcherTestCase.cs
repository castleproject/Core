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
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Engine;
	using Castle.MonoRail.Framework.Tests.Controllers;

	[TestFixture]
	public class SmartDispatcherTestCase
	{
		FakeViewEngine _viewEngine;
		ProcessEngine _engine;
		RailsEngineContextImpl _context;

		public SmartDispatcherTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new FakeViewEngine();
			_engine = new ProcessEngine(factory, _viewEngine);

			_viewEngine.AddView("smart", "smart", "it's smarty!");
		}

		[Test]
		public void BuildSimpleArgs()
		{
			_engine.Process( GetContext("ToString", "str", "strv") );
			AssertResponse();

			_engine.Process( GetContext("ToGuid", "gd", Guid.Empty.ToString()) );
			AssertResponse();

			_engine.Process( GetContext("ToGuid", "", "") );
			AssertResponse();

			_engine.Process( GetContext("ToUInt16", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToUInt32", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToUInt64", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToInt16", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToInt32", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToInt64", "i", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToByte", "b", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToSByte", "b", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToSingle", "s", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToDouble", "d", "1") );
			AssertResponse();

			_engine.Process( GetContext("ToDateTime", "dt", "12/12/2000") );
			AssertResponse();

			_engine.Process( GetContext("ToBoolean", "bol", "true") );
			AssertResponse();
		}

		[Test]
		public void BuildArrayArgs()
		{
			_engine.Process( GetContext("ToInt32Array", "i", "1,2,3") );
			AssertResponse();

			_engine.Process( GetContext("ToStringArray", "str", "a,b,c") );
			AssertResponse();

			_engine.Process( GetContext("ToNullStringArray", "str", null) );
			AssertResponse();

			_engine.Process( GetContext("ToNullInt32Array", "i", null) );
			AssertResponse();
		}

		[Test]
		public void InvokeOverloaded()
		{
			_engine.Process( GetContext("Overloaded", "i", "1") );
			Assert.IsTrue(SmartController.IntInvoked);
			Assert.IsFalse(SmartController.StrInvoked);
			AssertResponse();

			_engine.Process( GetContext("Overloaded", "str", "a") );
			Assert.IsTrue(SmartController.StrInvoked);
			AssertResponse();
		}

		private void AssertResponse()
		{
			Assert.AreEqual("ok", _context.Response.Output.ToString());
		}

		private RailsEngineContextImpl GetContext(string action, string name, string value)
		{
			_context = new RailsEngineContextImpl("/smart/"+ action +".rails");
	
			_context.Request.Params[name] = value;

			Assert.AreEqual("", _context.Response.Output.ToString());

			return _context;
		}
	}
}
