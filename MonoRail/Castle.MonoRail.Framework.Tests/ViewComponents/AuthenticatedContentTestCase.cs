// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.ViewComponents
{
	using System.Collections;
	using System.IO;
	using System.Security.Principal;
	using Castle.MonoRail.Framework.ViewComponents;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class AuthenticatedContentTestCase
	{
		private AuthenticatedContent authComponent;
		private ViewEngineStub viewEngineStub;
		private StringWriter writer;
		private StubViewComponentContext componentContext;
		private StubEngineContext engine;

		[SetUp]
		public void Init()
		{
			writer = new StringWriter();
			viewEngineStub = new ViewEngineStub();
			componentContext = new StubViewComponentContext("name", writer, viewEngineStub);
			authComponent = new AuthenticatedContent();
			engine = new StubEngineContext(null, null, null, null);
		}

		[Test]
		public void RendersLoggedNestedSectionIfUserIsAuthenticated()
		{
			bool loggedRendered, notloggedRendered;

			loggedRendered = notloggedRendered = false;

			componentContext.SectionRender["logged"] = delegate { loggedRendered = true; };
			componentContext.SectionRender["notlogged"] = delegate { notloggedRendered = true; };

			engine.CurrentUser = new GenericPrincipal(new GenericIdentity("user", "test"), new string[0]);
			authComponent.Init(engine, componentContext);
			authComponent.Render();

			Assert.IsTrue(loggedRendered);
			Assert.IsFalse(notloggedRendered);
		}

		[Test]
		public void RendersNotLoggedNestedSectionIfUserIsNotAuthenticated()
		{
			bool loggedRendered, notloggedRendered;

			loggedRendered = notloggedRendered = false;

			componentContext.SectionRender["logged"] = delegate(IDictionary context, TextWriter writer) { loggedRendered = true; };
			componentContext.SectionRender["notlogged"] = delegate(IDictionary context, TextWriter writer) { notloggedRendered = true; };

			engine.CurrentUser = null;
			authComponent.Init(engine, componentContext);
			authComponent.Render();

			Assert.IsFalse(loggedRendered);
			Assert.IsTrue(notloggedRendered);
		}
	}
}
