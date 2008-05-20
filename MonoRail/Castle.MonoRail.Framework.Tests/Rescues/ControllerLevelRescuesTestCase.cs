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

namespace Castle.MonoRail.Framework.Tests.Rescues
{
	using System;
	using System.Data;
	using System.Reflection;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class ControllerLevelRescuesTestCase
	{
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;
		private MockResponse response;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void ControllerRescueIsUsed()
		{
			ControllerWithRescue controller = new ControllerWithRescue();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual(500, response.StatusCode);
			Assert.AreEqual("Error processing action", response.StatusDescription);
			Assert.AreEqual("rescues\\generalerror", viewEngStub.TemplateRendered);
		}

		[Test]
		public void BestRescueIsSelectedBasedOnTheExceptionTypeInheritance()
		{
			ControllerWithMultipleRescues controller = new ControllerWithMultipleRescues();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "index1", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual(500, response.StatusCode);
			Assert.AreEqual("Error processing action", response.StatusDescription);
			Assert.AreEqual("rescues\\sysException", viewEngStub.TemplateRendered);
		}

		[Test]
		public void BestRescueIsSelectedBasedOnTheExactExceptionType()
		{
			ControllerWithMultipleRescues controller = new ControllerWithMultipleRescues();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "index3", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual(500, response.StatusCode);
			Assert.AreEqual("Error processing action", response.StatusDescription);
			Assert.AreEqual("rescues\\sqlexception", viewEngStub.TemplateRendered);
		}

		[Test]
		public void FallsBackToGeneralIfNothingMatches()
		{
			ControllerWithMultipleRescues controller = new ControllerWithMultipleRescues();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "index2", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual(500, response.StatusCode);
			Assert.AreEqual("Error processing action", response.StatusDescription);
			Assert.AreEqual("rescues\\generalerror", viewEngStub.TemplateRendered);
		}

		[Test, ExpectedException(typeof(TargetInvocationException), ExpectedMessage = "Exception has been thrown by the target of an invocation.")]
		public void FallsBackToExceptionIfNothingMatches()
		{
			ControllerWithSpecializedRescuesOnly controller = new ControllerWithSpecializedRescuesOnly();

			IControllerContext context = new DefaultControllerContextFactory().
				Create("", "home", "index1", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			try
			{
				controller.Process(engineContext, context);
			}
			catch(Exception ex)
			{
				Assert.AreEqual(500, response.StatusCode);
				Assert.AreEqual("Error processing action", response.StatusDescription);
				Assert.AreEqual("Testing", ex.InnerException.Message);

				throw;
			}
		}

		#region Controllers

		[Rescue("generalerror")]
		class ControllerWithRescue : Controller
		{
			public void Index()
			{
				throw new InvalidOperationException();
			}
		}

		[Rescue("generalerror")]
		[Rescue("sysException", typeof(SystemException))]
		[Rescue("sqlexception", typeof(ConstraintException))]
		class ControllerWithMultipleRescues : Controller
		{
			public void Index1()
			{
				throw new InvalidOperationException();
			}

			public void Index2()
			{
				throw new MonoRailException("Testing");
			}

			public void Index3()
			{
				throw new ConstraintException("Testing");
			}
		}

		[Rescue("sysException", typeof(SystemException))]
		[Rescue("sqlexception", typeof(ConstraintException))]
		class ControllerWithSpecializedRescuesOnly : Controller
		{
			public void Index1()
			{
				throw new MonoRailException("Testing");
			}
		}

		#endregion
	}
}
