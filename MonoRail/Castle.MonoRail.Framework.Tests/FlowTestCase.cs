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
	// using Castle.MonoRail.TestSupport;

	/// <summary>
	/// Summary description for FlowTestCase.
	/// </summary>
	[TestFixture]
	public class FlowTestCase// : AbstractMRTestCase
	{
		private FakeViewEngine viewEngine;

//		public FlowTestCase()
//		{
//			Config.CustomEngineTypeName = "Castle.MonoRail.Framework.Tests.FakeViewEngine, Castle.MonoRail.Framework.Tests";
//
//			Config.ControllerAssemblies.Add( "Castle.MonoRail.Framework.Tests" );
//		}
//
//		protected override void CustomizeProcessEngine(ProcessEngine processEngine)
//		{
//			viewEngine = (FakeViewEngine) processEngine.ViewEngine;
//		}

//		[Test]
//		public void SimpleRequest()
//		{
//			viewEngine.AddView("home", "index", "hello world!");
//
//			DoGet("/home/index.rails");
//
//			AssertSuccess();
//
//			AssertReplyEqualsTo( "hello world!" );
//		}

//		[Test]
//		public void SimpleRequestWithDifferentView()
//		{
//			_viewEngine.AddView("home", "index", "hello from index");
//			_viewEngine.AddView("home", "display", "hello from display");
//
//			RailsEngineContextImpl context = new 
//				RailsEngineContextImpl("/home/other.rails");
//
//			_engine.Process( context );
//
//			Assert.AreEqual( "hello from display", context.Output );
//		}
	}
}
