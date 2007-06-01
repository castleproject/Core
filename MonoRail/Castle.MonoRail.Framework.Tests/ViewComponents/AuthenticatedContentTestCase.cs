// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;

	[TestFixture]
	public class AuthenticatedContentTestCase : BaseViewComponentTest
	{
		private AuthenticatedContent authComponent;

		[SetUp]
		public void Init()
		{
			authComponent = new AuthenticatedContent();
		}

		[TearDown]
		public void Terminate()
		{
			CleanUp();
		}

		[Test]
		public void RendersLoggedNestedSectionIfUserIsAuthenticated()
		{
			bool loggedRendered, notloggedRendered;

			loggedRendered = notloggedRendered = false;

			SectionRender["logged"] = delegate(IDictionary context, TextWriter writer) { loggedRendered = true; };
			SectionRender["notlogged"] = delegate(IDictionary context, TextWriter writer) { notloggedRendered = true; };

			BuildRailsContext("", "controller", "index");
			Context.CurrentUser = new GenericPrincipal(new GenericIdentity("user", "test"), new string[0]);
			PrepareViewComponent(authComponent);
			authComponent.Render();

			Assert.IsTrue(loggedRendered);
			Assert.IsFalse(notloggedRendered);
		}

		[Test]
		public void RendersNotLoggedNestedSectionIfUserIsNotAuthenticated()
		{
			bool loggedRendered, notloggedRendered;

			loggedRendered = notloggedRendered = false;

			SectionRender["logged"] = delegate(IDictionary context, TextWriter writer) { loggedRendered = true; };
			SectionRender["notlogged"] = delegate(IDictionary context, TextWriter writer) { notloggedRendered = true; };

			BuildRailsContext("", "controller", "index");
			Context.CurrentUser = null;
			PrepareViewComponent(authComponent);
			authComponent.Render();

			Assert.IsFalse(loggedRendered);
			Assert.IsTrue(notloggedRendered);
		}
	}
}
