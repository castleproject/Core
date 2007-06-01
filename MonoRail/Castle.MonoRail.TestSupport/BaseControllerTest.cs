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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.IO;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Test;

	public delegate void ContextInitializer(MockRailsEngineContext context);

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class BaseControllerTest
	{
		private readonly string domain;
		private readonly string domainPrefix;
		private readonly int port;
		private string virtualDir = "/";
		private MockRailsEngineContext context;
		private MockRequest request;
		private MockResponse response;
		private MockTrace trace;

		protected BaseControllerTest() : this("app.com", "www", 80)
		{
		}

		protected BaseControllerTest(string domain, string domainPrefix, int port)
		{
			this.domain = domain;
			this.domainPrefix = domainPrefix;
			this.port = port;
		}

		protected MockRailsEngineContext Context
		{
			get { return context; }
		}

		public MockRequest Request
		{
			get { return request; }
		}

		public MockResponse Response
		{
			get { return response; }
		}

		public MockTrace Trace
		{
			get { return trace; }
		}

		protected void PrepareController(Controller controller)
		{
			PrepareController(controller, null);
		}

		protected void PrepareController(Controller controller, ContextInitializer contextInitializer)
		{
			PrepareController(controller, "", "Controller", "Action");
		}

		protected void PrepareController(Controller controller, string controllerName, string actionName)
		{
			PrepareController(controller, "", controllerName, actionName);
		}

		protected void PrepareController(Controller controller, string areaName, string controllerName, string actionName)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller", "'controller' cannot be null");
			}
			if (areaName == null)
			{
				throw new ArgumentNullException("areaName");
			}
			if (controllerName == null)
			{
				throw new ArgumentNullException("controllerName");
			}
			if (actionName == null)
			{
				throw new ArgumentNullException("actionName");
			}

			BuildRailsContext(areaName, controllerName, actionName);
			controller.InitializeFieldsFromServiceProvider(context);
			controller.InitializeControllerState(areaName, controllerName, actionName);
			ControllerLifecycleExecutor executor = new ControllerLifecycleExecutor(controller, context);
			executor.Service(context);
			executor.InitializeController(controller.AreaName, controller.Name, controller.Action);
		}

		protected void BuildRailsContext(string areaName, string controllerName, string actionName)
		{
			UrlInfo info = BuildUrlInfo(areaName, controllerName, actionName);
			request = BuildRequest();
			response = BuildResponse();
			trace = BuildTrace();
			context = BuildRailsEngineContext(request, response, trace, info);
		}

		protected virtual MockRequest BuildRequest()
		{
			return new MockRequest();
		}

		protected virtual MockResponse BuildResponse()
		{
			return new MockResponse();
		}

		protected virtual MockTrace BuildTrace()
		{
			return new MockTrace();
		}

		protected virtual MockRailsEngineContext BuildRailsEngineContext(IRequest request, IResponse response, ITrace trace, UrlInfo urlInfo)
		{
			return new MockRailsEngineContext(request, response, trace, urlInfo);
		}

		protected virtual UrlInfo BuildUrlInfo(string areaName, string controllerName, string actionName)
		{
			return new UrlInfo(domain, domainPrefix, virtualDir, "http", port,
				Path.Combine(Path.Combine(areaName, controllerName), actionName), areaName, controllerName, actionName, "rails");
		}
	}
}
