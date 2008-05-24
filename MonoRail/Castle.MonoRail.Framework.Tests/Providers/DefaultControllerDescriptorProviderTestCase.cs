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

namespace Castle.MonoRail.Framework.Tests.Providers
{
	using System;
	using System.Reflection;
	using Castle.MonoRail.Framework.Providers;
	using Castle.MonoRail.Framework.TransformFilters;
	using Descriptors;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TransformFilters;

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
		private IReturnBinderDescriptorProvider returnTypeDescProviderMock;
		private IDynamicActionProviderDescriptorProvider dynamicActionProviderDescProviderMock;

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
			Assert.AreEqual(1, metaDesc.DynamicActionProviders.Length);
			Assert.AreEqual(typeof(DummyDynActionProvider), metaDesc.DynamicActionProviders[0].DynamicActionProviderType);
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
		public void CollectsTransformFilterForAction()
		{
			BuildDescriptor();

			Type controllerType = typeof(TransformFilterController);

			ControllerMetaDescriptor metaDesc = provider.BuildDescriptor(controllerType);
			Assert.IsNotNull(metaDesc);
			MethodInfo actionMethod = controllerType.GetMethod("Action1");
			ActionMetaDescriptor actionMetaDesc = metaDesc.GetAction(actionMethod);
			Assert.IsNotNull(actionMetaDesc);
			Assert.AreEqual(1, actionMetaDesc.TransformFilters.Length);
			Assert.AreEqual(typeof(UpperCaseTransformFilter), actionMetaDesc.TransformFilters[0].TransformFilterType);
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
			helperDescProviderMock = mockRepository.DynamicMock<IHelperDescriptorProvider>();
			filterDescProviderMock = mockRepository.DynamicMock<IFilterDescriptorProvider>();
			layoutDescProviderMock = mockRepository.DynamicMock<ILayoutDescriptorProvider>();
			rescueDescProviderMock = mockRepository.DynamicMock<IRescueDescriptorProvider>();
			resourceProviderMock = mockRepository.DynamicMock<IResourceDescriptorProvider>();
			transformDescProviderMock = mockRepository.DynamicMock<ITransformFilterDescriptorProvider>();
			returnTypeDescProviderMock = mockRepository.DynamicMock<IReturnBinderDescriptorProvider>();
			dynamicActionProviderDescProviderMock = mockRepository.DynamicMock<IDynamicActionProviderDescriptorProvider>();

			provider = new DefaultControllerDescriptorProvider(helperDescProviderMock,
															   filterDescProviderMock,
															   layoutDescProviderMock,
															   rescueDescProviderMock,
															   resourceProviderMock,
															   transformDescProviderMock, returnTypeDescProviderMock,
															   dynamicActionProviderDescProviderMock);

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
				Expect.Call(returnTypeDescProviderMock.Collect(actionMethod)).Return(null);
				Expect.Call(dynamicActionProviderDescProviderMock.CollectProviders(controllerType)).Return(
					new DynamicActionProviderDescriptor[0]);
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

		public class TransformFilterController : Controller
		{
			[TransformFilter(typeof(UpperCaseTransformFilter))]
			public void Action1()
			{
				RenderText("test");
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
															   new DefaultTransformFilterDescriptorProvider(), new DefaultReturnBinderDescriptorProvider(), 
															   new DefaultDynamicActionProviderDescriptorProvider());
		}
	}
}
