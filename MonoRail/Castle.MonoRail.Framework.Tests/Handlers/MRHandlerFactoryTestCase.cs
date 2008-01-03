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

namespace Castle.MonoRail.Framework.Tests.Handlers
{
	using System;
	using System.IO;
	using System.Web;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using Container;
	using Descriptors;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class MRHandlerFactoryTestCase
	{
		private MockRepository mockRepository = new MockRepository();
		private MonoRailHttpHandlerFactory handlerFactory;
		private IServiceProviderLocator serviceProviderLocatorMock;
		private IMonoRailContainer container;
		private IControllerFactory controllerFactoryMock;
		private IController controllerMock;
		private IControllerDescriptorProvider controllerDescriptorProviderMock;
		private IControllerContextFactory controllerContextFactoryMock;

		[SetUp]
		public void Init()
		{
			container = mockRepository.CreateMock<IMonoRailContainer>();
			serviceProviderLocatorMock = mockRepository.CreateMock<IServiceProviderLocator>();
			controllerFactoryMock = mockRepository.CreateMock<IControllerFactory>();
			controllerMock = mockRepository.CreateMock<IController>();
			controllerDescriptorProviderMock = mockRepository.CreateMock<IControllerDescriptorProvider>();
			controllerContextFactoryMock = mockRepository.CreateMock<IControllerContextFactory>();

			SetupResult.For(container.UrlTokenizer).Return(new DefaultUrlTokenizer());
			SetupResult.For(container.UrlBuilder).Return(new DefaultUrlBuilder());
			SetupResult.For(container.EngineContextFactory).Return(new DefaultEngineContextFactory());
			SetupResult.For(container.ControllerFactory).Return(controllerFactoryMock);
			SetupResult.For(container.ControllerContextFactory).Return(controllerContextFactoryMock);
			SetupResult.For(container.ControllerDescriptorProvider).Return(controllerDescriptorProviderMock);
			SetupResult.For(container.StaticResourceRegistry).Return(new DefaultStaticResourceRegistry());

			handlerFactory = new MonoRailHttpHandlerFactory(serviceProviderLocatorMock);
			handlerFactory.ResetState();
			handlerFactory.Configuration = new MonoRailConfiguration();
			handlerFactory.Container = container;
		}

		[Test]
		public void Request_CreatesSessionfulHandler()
		{
			StringWriter writer = new StringWriter();
			
			HttpResponse res = new HttpResponse(writer);
			HttpRequest req = new HttpRequest(Path.Combine(
			                                  	AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt"),
			                                  "http://localhost:1333/home/something", "");
			RouteMatch routeMatch = new RouteMatch();
			HttpContext httpCtx = new HttpContext(req, res);
			httpCtx.Items[RouteMatch.RouteMatchKey] = routeMatch;

			using(mockRepository.Record())
			{
				ControllerMetaDescriptor controllerDesc = new ControllerMetaDescriptor();
				controllerDesc.ControllerDescriptor = new ControllerDescriptor(typeof(Controller), "home", "", false);

				Expect.Call(controllerFactoryMock.CreateController("", "home")).IgnoreArguments().Return(controllerMock);
				Expect.Call(controllerDescriptorProviderMock.BuildDescriptor(controllerMock)).Return(controllerDesc);
				Expect.Call(controllerContextFactoryMock.Create("", "home", "something", controllerDesc, routeMatch)).
					Return(new ControllerContext());
			}

			using(mockRepository.Playback())
			{
				IHttpHandler handler = handlerFactory.GetHandler(httpCtx, "GET", "", "");

				Assert.IsNotNull(handler);
				Assert.IsInstanceOfType(typeof(MonoRailHttpHandler), handler);
			}
		}

		[Test]
		public void Request_CreatesSessionlessHandler()
		{
			StringWriter writer = new StringWriter();

			HttpResponse res = new HttpResponse(writer);
			HttpRequest req = new HttpRequest(Path.Combine(
			                                  	AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt"),
			                                  "http://localhost:1333/home/something", "");
			RouteMatch routeMatch = new RouteMatch();
			HttpContext httpCtx = new HttpContext(req, res);
			httpCtx.Items[RouteMatch.RouteMatchKey] = routeMatch;

			using(mockRepository.Record())
			{
				ControllerMetaDescriptor controllerDesc = new ControllerMetaDescriptor();
				controllerDesc.ControllerDescriptor = new ControllerDescriptor(typeof(Controller), "home", "", true);

				Expect.Call(controllerFactoryMock.CreateController("", "home")).IgnoreArguments().Return(controllerMock);
				Expect.Call(controllerDescriptorProviderMock.BuildDescriptor(controllerMock)).Return(controllerDesc);
				Expect.Call(controllerContextFactoryMock.Create("", "home", "something", controllerDesc, routeMatch)).
					Return(new ControllerContext());
			}

			using(mockRepository.Playback())
			{
				IHttpHandler handler = handlerFactory.GetHandler(httpCtx, "GET", "", "");

				Assert.IsNotNull(handler);
				Assert.IsInstanceOfType(typeof(SessionlessMonoRailHttpHandler), handler);
			}
		}
	}
}