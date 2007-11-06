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
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using Castle.Components.Common.EmailSender;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Test;

	public delegate void ContextInitializer(MockRailsEngineContext context);

	/// <summary>
	/// Base class that set ups the necessary infrastructure 
	/// to test controllers without the need
	/// for an ASP.Net Runtime. 
	/// </summary>
	/// 
	/// <example>
	/// The following code is an example of a controller test:
	/// 
	/// <code lang="cs">
	/// [TestFixture]
	/// public class LoginControllerTestCase : BaseControllerTest
	/// {
	///		private LoginController controller;
	/// 
	/// 	[SetUp]
	/// 	public void Init()
	/// 	{
	/// 		controller = new LoginController();
	/// 		PrepareController(controller);
	/// 	}
	/// 
	/// 	[Test]
	/// 	public void Authenticate_Should_Use_The_AuthenticationService()
	/// 	{
	///			// set up a mock authentication service before
	/// 
	/// 		controller.Authenticate("username", "my password", false);
	/// 
	/// 		Assert.AreEqual(3, controller.PropertyBag.Count);
	/// 		Assert.AreEqual("username", controller.PropertyBag["username"]);
	/// 		Assert.AreEqual("my password", controller.PropertyBag["password"]);
	/// 		Assert.AreEqual(false, controller.PropertyBag["autoLogin"]);
	/// 	}
	/// }
	/// </code>
	/// 
	/// <para>
	/// The following is a more sophisticate test for an action that sends emails.
	/// </para>
	/// 
	/// <code lang="cs">
	/// [Test]
	/// public void Register_Should_Add_Registration_Using_The_Repository()
	/// {
	/// 	Registration reg = new Registration("John Doe", "johndoe@gmail.com");
	/// 
	/// 	using(mockRepository.Record())
	/// 	{
	/// 		registrationRepositoryMock.Add(reg);
	/// 	}
	/// 
	///		using(mockRepository.Playback())
	///		{
	///			controller.Register(reg); // This action sends two emails
	/// 
	/// 		Assert.IsTrue(HasRenderedEmailTemplateNamed("emailToManager"));
	/// 		Assert.IsTrue(HasRenderedEmailTemplateNamed("emailToParticipant"));
	/// 
	/// 		Assert.AreEqual("manager@gmail.com", MessagesSent[0].To);
	/// 		Assert.AreEqual("johndoe@gmail.com", MessagesSent[1].To);
	/// 
	/// 		Assert.AreEqual("Registration\\Success", controller.SelectedViewName);
	/// 	}
	/// }
	/// </code>
	/// 
	/// </example>
	/// 
	/// <remarks>
	/// You must invoke <see cref="PrepareController(Controller)"/> -- or a different overload -
	/// before making invocations to the controller.
	/// </remarks>
	public abstract class BaseControllerTest
	{
		private readonly string domain;
		private readonly string domainPrefix;
		private readonly int port;
		protected string virtualDir = "";
		private MockRailsEngineContext context;
		private IMockRequest request;
		private IMockResponse response;
		private ITrace trace;
		private IDictionary cookies;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseControllerTest"/> class.
		/// </summary>
		protected BaseControllerTest() : this("app.com", "www", 80)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseControllerTest"/> class.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="domainPrefix">The domain prefix.</param>
		/// <param name="port">The port.</param>
		protected BaseControllerTest(string domain, string domainPrefix, int port)
		{
			this.domain = domain;
			this.domainPrefix = domainPrefix;
			this.port = port;
		}

		/// <summary>
		/// Override to perform any pre-test set up
		/// </summary>
		protected virtual void OnSetUp()
		{
		}

		/// <summary>
		/// Gets the cookies.
		/// </summary>
		/// <value>The cookies.</value>
		public IDictionary Cookies
		{
			get { return cookies; }
		}

		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		protected IRailsEngineContext Context
		{
			get { return context; }
		}

		/// <summary>
		/// Gets the request.
		/// </summary>
		/// <value>The request.</value>
		public IMockRequest Request
		{
			get { return request; }
		}

		/// <summary>
		/// Gets the response.
		/// </summary>
		/// <value>The response.</value>
		public IMockResponse Response
		{
			get { return response; }
		}

		/// <summary>
		/// Gets the trace.
		/// </summary>
		/// <value>The trace.</value>
		public ITrace Trace
		{
			get { return trace; }
		}

		/// <summary>
		/// Prepares the controller giving it mock implementations
		/// of the service it requires to function normally.
		/// </summary>
		/// <param name="controller">The controller.</param>
		protected void PrepareController(Controller controller)
		{
			PrepareController(controller, InitializeRailsEngineContext);
		}

		/// <summary>
		/// Prepares the controller giving it mock implementations
		/// of the service it requires to function normally.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="contextInitializer">The context initializer.</param>
		protected void PrepareController(Controller controller, ContextInitializer contextInitializer)
		{
			PrepareController(controller, "", "Controller", "Action", contextInitializer);
		}

		/// <summary>
		/// Prepares the controller giving it mock implementations
		/// of the service it requires to function normally.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		protected void PrepareController(Controller controller, string controllerName, string actionName)
		{
			PrepareController(controller, "", controllerName, actionName);
		}

		/// <summary>
		/// Prepares the controller giving it mock implementations
		/// of the service it requires to function normally.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="areaName">Name of the area (cannot be null).</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		protected void PrepareController(Controller controller, string areaName, string controllerName, string actionName)
		{
			PrepareController(controller, areaName, controllerName, actionName, InitializeRailsEngineContext);
		}

		/// <summary>
		/// Prepares the controller giving it mock implementations
		/// of the service it requires to function normally.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="areaName">Name of the area (cannot be null).</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		/// <param name="contextInitializer">The context initializer.</param>
		protected void PrepareController(Controller controller, string areaName, string controllerName, string actionName, ContextInitializer contextInitializer)
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
			if( contextInitializer == null) {
				throw new ArgumentNullException("contextInitializer");
			}

			cookies = new HybridDictionary(true);

			BuildRailsContext(areaName, controllerName, actionName, contextInitializer);
			controller.InitializeFieldsFromServiceProvider(context);
			controller.InitializeControllerState(areaName, controllerName, actionName);
			ControllerLifecycleExecutor executor = new ControllerLifecycleExecutor(controller, context);
			executor.Service(context);
			executor.InitializeController(controller.AreaName, controller.Name, controller.Action);
		}

		/// <summary>
		/// Constructs a mock context.
		/// </summary>
		/// <param name="areaName">Name of the area.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		protected void BuildRailsContext(string areaName, string controllerName, string actionName)
		{
			BuildRailsContext(areaName, controllerName, actionName, InitializeRailsEngineContext);
		}

		/// <summary>
		/// Constructs a mock context.
		/// </summary>
		/// <param name="areaName">Name of the area.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		/// <param name="contextInitializer">The context initializer.</param>
		protected void BuildRailsContext(string areaName, string controllerName, string actionName, ContextInitializer contextInitializer)
		{
			UrlInfo info = BuildUrlInfo(areaName, controllerName, actionName);
			request = BuildRequest();
			response = BuildResponse();
			trace = BuildTrace();
			context = BuildRailsEngineContext(request, response, trace, info);
			contextInitializer(context);
		}

		/// <summary>
		/// Builds the request.
		/// </summary>
		/// <returns></returns>
		protected virtual IMockRequest BuildRequest()
		{
			return new MockRequest(cookies);
		}

		/// <summary>
		/// Builds the response.
		/// </summary>
		/// <returns></returns>
		protected virtual IMockResponse BuildResponse()
		{
			return new MockResponse(cookies);
		}

		/// <summary>
		/// Builds the trace.
		/// </summary>
		/// <returns></returns>
		protected virtual ITrace BuildTrace()
		{
			return new MockTrace();
		}

		/// <summary>
		/// Builds the a mock context. You can override this method to 
		/// create a special configured mock context.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="response">The response.</param>
		/// <param name="trace">The trace.</param>
		/// <param name="urlInfo">The URL info.</param>
		/// <returns></returns>
		protected virtual MockRailsEngineContext BuildRailsEngineContext(IRequest request, IResponse response, ITrace trace,
		                                                                 UrlInfo urlInfo)
		{
			return new MockRailsEngineContext(request, response, trace, urlInfo);
		}

		/// <summary>
		/// Builds the URL info that represents the contextual Url.
		/// </summary>
		/// <param name="areaName">Name of the area.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="actionName">Name of the action.</param>
		/// <returns></returns>
		protected virtual UrlInfo BuildUrlInfo(string areaName, string controllerName, string actionName)
		{
			return new UrlInfo(domain, domainPrefix, virtualDir, "http", port,
			                   Path.Combine(Path.Combine(areaName, controllerName), actionName),
			                   areaName, controllerName, actionName, "rails", null);
		}

		/// <summary>
		/// Allows modifying of the engine context created by <see cref="BuildRailsEngineContext"/>
		/// </summary>
		/// <param name="mockRailsEngineContext">The engine context to modify</param>
		protected virtual void InitializeRailsEngineContext(MockRailsEngineContext mockRailsEngineContext)
		{}

		/// <summary>
		/// Determines whether a specified template was rendered -- to send an email.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <returns>
		/// 	<c>true</c> if was rendered; otherwise, <c>false</c>.
		/// </returns>
		protected bool HasRenderedEmailTemplateNamed(string templateName)
		{
			MockRailsEngineContext.RenderedEmailTemplate template = 
				context.RenderedEmailTemplates.Find(
					delegate(MockRailsEngineContext.RenderedEmailTemplate emailTemplate)
					{
						return templateName.Equals(emailTemplate.Name, StringComparison.OrdinalIgnoreCase);
					});
			
			return template != null;
		}

		/// <summary>
		/// Gets the fake email messages sent.
		/// </summary>
		/// <value>The messages sent.</value>
		protected Message[] MessagesSent
		{
			get { return context.MessagesSent.ToArray(); }
		}

		/// <summary>
		/// Gets the rendered email templates.
		/// </summary>
		/// <value>The rendered email templates.</value>
		protected MockRailsEngineContext.RenderedEmailTemplate[] RenderedEmailTemplates
		{
			get { return context.RenderedEmailTemplates.ToArray(); }
		}
	}
}
