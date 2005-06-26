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
	using System.Reflection;

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
			_viewEngine.AddView("databind", "databind", "databinding view");
		}

		[Test]
		public void BuildSimpleArgs()
		{
			_engine.Process( GetContext("ToStr", "str", "strv") );
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
			_engine.Process( GetContext("ToInt32Array", "i", new string[] { "1", "2", "3" }) );
			AssertResponse();

			_engine.Process( GetContext("ToStringArray", "str", new string[] { "a", "b", "c" } ) );
			AssertResponse();

			_engine.Process( GetContext("ToNullStringArray", "str", (string[])null) );
			AssertResponse();

			_engine.Process( GetContext("ToNullInt32Array", "i", (string[]) null) );
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

		[Test]
		public void DataBindNoPrefix()
		{
			IRailsEngineContext ctx = GetDataBindContext( "MapNoPrefix" );

			ctx.Params.Add( "value", DataBindController.Value.ToString() );
			ctx.Params.Add( "internal.text", DataBindController.Text );
			ctx.Params.Add( "internal.date", DataBindController.Date.ToString() );

			_engine.Process( ctx );

			AssertResponse();
		}

		[Test]
		public void DataBindWithPrefix()
		{
			IRailsEngineContext ctx = GetDataBindContext( "MapWithPrefix" );

			ctx.Params.Add( DataBindController.FormPrefix + ".value", DataBindController.Value.ToString() );
			ctx.Params.Add( DataBindController.FormPrefix + ".internal.text", DataBindController.Text );
			ctx.Params.Add( DataBindController.FormPrefix + ".internal.date", DataBindController.Date.ToString() );

			_engine.Process( ctx );

			AssertResponse();
		}

		[Test]
		public void DataBindWithErrors()
		{
			IRailsEngineContext ctx = GetDataBindContext( "MapWithErrors" );

			ctx.Params.Add( "value", "thisisnotanint" );
			ctx.Params.Add( "internal.text", DataBindController.Text );
			ctx.Params.Add( "internal.date", "thisisnotadate" );

			_engine.Process( ctx );

			AssertResponse();
		}

		[Test]
		public void DataBindFromQueryString()
		{
			IRailsEngineContext ctx = GetDataBindContext( "MapFromQueryGood" );

			ctx.Request.QueryString.Add( "value", DataBindController.Value.ToString() );
			ctx.Request.QueryString.Add( "internal.text", DataBindController.Text );
			ctx.Request.QueryString.Add( "internal.date", DataBindController.Date.ToString() );
			ctx.Request.Form.Add( "value", (DataBindController.Value + 15).ToString() );
			ctx.Request.Form.Add( "internal.text", DataBindController.Text + "jkj" );
			ctx.Request.Form.Add( "internal.date", DataBindController.Date.AddYears( 15 ).ToString() );

			_engine.Process( ctx );

			AssertResponse();
		}

		[Test]
		[ExpectedException(typeof(TargetInvocationException))]
		public void DataBindFromForm()
		{
			IRailsEngineContext ctx = GetDataBindContext( "MapFromQueryBad" );

			ctx.Request.QueryString.Add( "value", DataBindController.Value.ToString() );
			ctx.Request.QueryString.Add( "internal.text", DataBindController.Text );
			ctx.Request.QueryString.Add( "internal.date", DataBindController.Date.ToString() );
			ctx.Request.Form.Add( "value", (DataBindController.Value + 15).ToString() );
			ctx.Request.Form.Add( "internal.text", DataBindController.Text + "jkj" );
			ctx.Request.Form.Add( "internal.date", DataBindController.Date.AddYears( 15 ).ToString() );

			_engine.Process( ctx );

			AssertResponse();
		}

		private void AssertResponse()
		{
			Assert.AreEqual("ok", _context.Response.Output.ToString());
		}

		private RailsEngineContextImpl GetDataBindContext( string action )
		{
			_context = new RailsEngineContextImpl("/databind/"+ action +".rails");	

			Assert.AreEqual("", _context.Response.Output.ToString());

			return _context;
		}

		private RailsEngineContextImpl GetContext(string action, string name, string value)
		{
			return GetContext( action, name, new string[] { value } );
		}

		private RailsEngineContextImpl GetContext(string action, string name, string[] values)
		{
			_context = new RailsEngineContextImpl("/smart/"+ action +".rails");
	
			if ( values != null )
			{
				foreach ( string s in values )
					_context.Request.Params.Add( name, s );
			}
			else
				_context.Request.Params.Add( name, null );

			Assert.AreEqual("", _context.Response.Output.ToString());

			return _context;
		}
	}
}
