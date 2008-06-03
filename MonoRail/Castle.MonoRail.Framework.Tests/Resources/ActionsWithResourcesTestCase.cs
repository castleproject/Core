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

namespace Castle.MonoRail.Framework.Tests.Resources
{
	using System.Collections;
	using Castle.MonoRail.Framework.Resources;
	using Descriptors;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Test;

	[TestFixture]
	public class ActionsWithResourcesTestCase
	{
		private MockRepository mockRepository = new MockRepository();
		private StubEngineContext engineContext;
		private StubViewEngineManager engStubViewEngineManager;
		private StubMonoRailServices services;
		private IResourceFactory resourceFactoryMock;

		[SetUp]
		public void Init()
		{
			resourceFactoryMock = mockRepository.DynamicMock<IResourceFactory>();

			StubRequest request = new StubRequest();
			StubResponse response = new StubResponse();
			services = new StubMonoRailServices();
			engStubViewEngineManager = new StubViewEngineManager();
			services.ViewEngineManager = engStubViewEngineManager;
			services.ResourceFactory = resourceFactoryMock;
			engineContext = new StubEngineContext(request, response, services, null);
		}

		[Test]
		public void CreatesResourcesSpecifiedThroughAttributesOnAction()
		{
			ControllerWithResource controller = new ControllerWithResource();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			using(mockRepository.Record())
			{
				Expect.Call(resourceFactoryMock.Create(
				            	new ResourceDescriptor(null, "key", "Castle.MonoRail.Framework.Tests.Resources.Language", "neutral",
				            	                       "Castle.MonoRail.Framework.Tests"),
				            	typeof(ControllerWithResourcesTestCase).Assembly)).Return(new DummyResource());
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);

				Assert.AreEqual(1, context.Resources.Count);
				Assert.IsNotNull(context.Resources["key"]);
			}
		}

		#region Controllers

		private class ControllerWithResource : Controller
		{
			[Resource("key", "Castle.MonoRail.Framework.Tests.Resources.Language", CultureName = "neutral",
				AssemblyName = "Castle.MonoRail.Framework.Tests")]
			public void Index()
			{
			}
		}

		#endregion

		private class DummyResource : IResource
		{
			public object this[string key]
			{
				get { throw new System.NotImplementedException(); }
			}

			public string GetString(string key)
			{
				throw new System.NotImplementedException();
			}

			public object GetObject(string key)
			{
				throw new System.NotImplementedException();
			}

			public IEnumerator GetEnumerator()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}