using System;
using Castle.MonoRail.Framework.Tests.Controllers;
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
	using System.Threading;
	using System.Globalization;

	using NUnit.Framework;

	using Castle.MonoRail.Engine;

	[TestFixture]
	public class ResourceTestCase
	{
		private CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );
		private CultureInfo nl = CultureInfo.CreateSpecificCulture( "nl" );
		private CultureInfo fr = CultureInfo.CreateSpecificCulture( "fr" );

		FakeViewEngine _viewEngine;
		ProcessEngine _engine;
		
		[SetUp]
		public void Init()
		{
			IControllerFactory factory = new FakeControllerFactory();

			_viewEngine = new FakeViewEngine();
			_engine = new ProcessEngine(factory, _viewEngine);

			_viewEngine.AddView( "resource", "resource", "$text.language" );

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			ResourceController.StaticPropertyBag = null;
		}

		[Test]
		public void GetResource()
		{
			IRailsEngineContext context = new RailsEngineContextImpl( "/resource/GetResource.rails");
			
			_engine.Process( context );

			Assert.AreEqual( "english", context.Response.Output.ToString() );
			Assert.AreEqual( CultureInfo.CurrentCulture, en );
			Assert.AreEqual( CultureInfo.CurrentUICulture, en );
		}

		[Test]
		public void GetResourceByCulture()
		{
			IRailsEngineContext context = new RailsEngineContextImpl( "/resource/GetResourceByCulture.rails");
			
			_engine.Process( context );

			Assert.AreEqual( "deutsch", context.Response.Output.ToString() );
			Assert.AreEqual( CultureInfo.CurrentCulture, en );
			Assert.AreEqual( CultureInfo.CurrentUICulture, en );
		}

		[Test]
		public void SetLocaleCulture()
		{
			IRailsEngineContext context = new RailsEngineContextImpl( "/resource/GetResource.rails");

			context.Request.QueryString.Add( "locale", "nl" );

			_engine.Process( context );

			Assert.AreEqual( "nederlands", context.Response.Output.ToString() );
			Assert.AreEqual( CultureInfo.CurrentCulture, nl );
			Assert.AreEqual( CultureInfo.CurrentUICulture, nl );
		}

		[Test]
		public void GetFallbackResource()
		{
			IRailsEngineContext context = new RailsEngineContextImpl( "/resource/GetResource.rails");

			context.Request.QueryString.Add( "locale", "fr" );

			_engine.Process( context );

			Assert.AreEqual( "english", context.Response.Output.ToString() );
			Assert.AreEqual( CultureInfo.CurrentCulture, fr );
			Assert.AreEqual( CultureInfo.CurrentUICulture, fr );
		}
	}
}
