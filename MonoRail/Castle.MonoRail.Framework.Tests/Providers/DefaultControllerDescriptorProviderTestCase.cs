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

namespace Castle.MonoRail.Framework.Tests.Providers
{
	using System;
	using System.Reflection;
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class DefaultControllerDescriptorProviderTestCase
	{
		private MockRepository mockRepository = new MockRepository();
		private DefaultControllerDescriptorProvider provider;
		private IHelperDescriptorProvider helperDescProviderMock;
		private IFilterDescriptorProvider filterDescProviderMock;
		private ILayoutDescriptorProvider layoutDescProviderMock;
		private IRescueDescriptorProvider rescueDescProviderMock;
		private IResourceDescriptorProvider resourceProviderMock;
		private ITransformFilterDescriptorProvider transformDescProviderMock;

		[Test]
		public void CollectsSkipRescueForAction()
		{
			BuildDescriptor();

			Type controllerType = typeof(SkipRescueController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			MethodInfo actionMethod = controllerType.GetMethod("Action1");
			ActionMetaDescriptor actionMetaDesc = metaDesc.GetAction(actionMethod);
			Assert.IsNotNull(actionMetaDesc);
			Assert.IsNotNull(actionMetaDesc.SkipRescue);
		}

		[Test]
		public void CollectsDefaultActionForController()
		{
			BuildDescriptor();

			Type controllerType = typeof(DefActionController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			Assert.IsNotNull(metaDesc.DefaultAction);
			Assert.AreEqual("action", metaDesc.DefaultAction.DefaultAction);
		}

		[Test]
		public void CollectsDynamicActionsForController()
		{
			BuildDescriptor();

			Type controllerType = typeof(DynActionController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			Assert.AreEqual(1, metaDesc.ActionProviders.Count);
			Assert.AreEqual(typeof(DummyDynActionProvider), metaDesc.ActionProviders[0]);
		}

		[Test]
		public void CollectsScaffoldingForController()
		{
			BuildDescriptor();

			Type controllerType = typeof(ScaffoldController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			Assert.AreEqual(1, metaDesc.Scaffoldings.Count);
			Assert.AreEqual(typeof(DummyScaffoldEntity), metaDesc.Scaffoldings[0].Model);
		}

		[Test]
		public void CollectsSkipFilterForAction()
		{
			BuildDescriptor();

			Type controllerType = typeof(SkipFilterController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			MethodInfo actionMethod = controllerType.GetMethod("Action1");
			ActionMetaDescriptor actionMetaDesc = metaDesc.GetAction(actionMethod);
			Assert.IsNotNull(actionMetaDesc);
			Assert.IsNotNull(actionMetaDesc.SkipFilters);
		}

		[Test]
		public void CollectsAccessibleThroughForAction()
		{
			BuildDescriptor();

			Type controllerType = typeof(AccThrController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			MethodInfo actionMethod = controllerType.GetMethod("Action1");
			ActionMetaDescriptor actionMetaDesc = metaDesc.GetAction(actionMethod);
			Assert.IsNotNull(actionMetaDesc);
			Assert.IsNotNull(actionMetaDesc.AccessibleThrough);
		}

		[Test]
		public void DescriptorIsCreatedForControllerAndAction()
		{
			helperDescProviderMock = mockRepository.CreateMock<IHelperDescriptorProvider>();
			filterDescProviderMock = mockRepository.CreateMock<IFilterDescriptorProvider>();
			layoutDescProviderMock = mockRepository.CreateMock<ILayoutDescriptorProvider>();
			rescueDescProviderMock = mockRepository.CreateMock<IRescueDescriptorProvider>();
			resourceProviderMock = mockRepository.CreateMock<IResourceDescriptorProvider>();
			transformDescProviderMock = mockRepository.CreateMock<ITransformFilterDescriptorProvider>();

			provider = new DefaultControllerDescriptorProvider(helperDescProviderMock,
															   filterDescProviderMock,
															   layoutDescProviderMock,
															   rescueDescProviderMock,
															   resourceProviderMock,
															   transformDescProviderMock);

			Type controllerType = typeof(SingleActionController);
			MethodInfo actionMethod = controllerType.GetMethod("Action1");

			using(mockRepository.Record())
			{
				// Controller level
				Expect.Call(helperDescProviderMock.CollectHelpers(controllerType)).Return(new HelperDescriptor[0]);
				Expect.Call(resourceProviderMock.CollectResources(controllerType)).Return(new ResourceDescriptor[0]);
				Expect.Call(filterDescProviderMock.CollectFilters(controllerType)).Return(new FilterDescriptor[0]);
				Expect.Call(layoutDescProviderMock.CollectLayout(controllerType)).Return(null);
				Expect.Call(rescueDescProviderMock.CollectRescues(controllerType)).Return(new RescueDescriptor[0]);
			
				// Action level
				Expect.Call(resourceProviderMock.CollectResources(actionMethod)).Return(new ResourceDescriptor[0]);
				Expect.Call(rescueDescProviderMock.CollectRescues(actionMethod)).Return(new RescueDescriptor[0]);
				Expect.Call(layoutDescProviderMock.CollectLayout(actionMethod)).Return(null);
				Expect.Call(transformDescProviderMock.CollectFilters(actionMethod)).Return(new TransformFilterDescriptor[0]);
			}

			using(mockRepository.Playback())
			{
				ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
				Assert.IsNotNull(metaDesc);
				ActionMetaDescriptor actionMetaDesc = metaDesc.GetAction(actionMethod);
				Assert.IsNotNull(actionMetaDesc);
				Assert.IsNull(actionMetaDesc.AccessibleThrough);
			}
		}

		#region Controllers

		public class SingleActionController : Controller
		{
			public void Action1()
			{
			}
		}

		public class AccThrController : Controller
		{
			[AccessibleThrough(Verb.Post)]
			public void Action1()
			{
			}
		}

		public class SkipRescueController : Controller
		{
			[SkipRescue]
			public void Action1()
			{
			}
		}

		public class SkipFilterController : Controller
		{
			[SkipFilter]
			public void Action1()
			{
			}
		}

		[DynamicActionProvider(typeof(DummyDynActionProvider))]
		public class DynActionController : Controller
		{
		}

		public class DummyDynActionProvider : IDynamicActionProvider
		{
			public void IncludeActions(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
			{
			}
		}

		[Scaffolding(typeof(DummyScaffoldEntity))]
		public class ScaffoldController : Controller
		{
			
		}

		public class DummyScaffoldEntity
		{
		}

		[DefaultAction("action")]
		public class DefActionController : Controller
		{
		}

		#endregion

		private void BuildDescriptor()
		{
			provider = new DefaultControllerDescriptorProvider(new DefaultHelperDescriptorProvider(),
															   new DefaultFilterDescriptorProvider(), new DefaultLayoutDescriptorProvider(),
															   new DefaultRescueDescriptorProvider(), new DefaultResourceDescriptorProvider(),
															   new DefaultTransformFilterDescriptorProvider());
		}
	}
}