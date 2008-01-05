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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Security.Principal;
	using System.Web;
	using Castle.Components.Common.EmailSender;
	using Container;

	/// <summary>
	/// Represents a mock implementation of <see cref="IEngineContext"/> for unit test purposes.
	/// </summary>
	public class MockEngineContext : AbstractServiceContainer, IEngineContext
	{
//		private readonly string physicalPath = AppDomain.CurrentDomain.BaseDirectory;
		private readonly IRequest request;
		private readonly IResponse response;
		private Flash flash = new Flash();
		private IServerUtility serverUtility = new MockServerUtility();
		private readonly IDictionary contextItems = new HybridDictionary(true);
		private readonly List<RenderedEmailTemplate> renderedEmailTemplates = new List<RenderedEmailTemplate>();
		private readonly List<Message> messagesSent = new List<Message>();
		private UrlInfo urlInfo;
		private ITrace trace;
		private IPrincipal currentUser;
		private Exception lastException;
		private IDictionary session = new HybridDictionary(true);
		private IController currentController;
		private IControllerContext currentControllerContext;
		private IMonoRailServices services;
//
//		/// <summary>
//		/// Initializes a new instance of the <see cref="MockRailsEngineContext"/> class.
//		/// </summary>
//		protected MockRailsEngineContext()
//		{
//			RegisterServices();
//		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MockEngineContext"/> class.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="response">The response.</param>
		/// <param name="services">The services.</param>
		/// <param name="urlInfo">The URL info.</param>
		public MockEngineContext(IRequest request, IResponse response, IMonoRailServices services, UrlInfo urlInfo)
		{
			this.request = request;
			this.response = response;
			this.services = services;
			this.urlInfo = urlInfo;
		}

//		#region IEngineContext Members
//
//		/// <summary>
//		/// Gets the request type (GET, POST, etc)
//		/// </summary>
//		/// <value></value>
//		public virtual String RequestType
//		{
//			get { return request.HttpMethod; }
//		}
//
//		/// <summary>
//		/// Gets the request URL.
//		/// </summary>
//		/// <value></value>
//		public virtual string Url
//		{
//			get { return urlInfo.UrlRaw; }
//		}

		/// <summary>
		/// Gets the underlying context of the API being used.
		/// </summary>
		/// <value></value>
		public virtual HttpContext UnderlyingContext
		{
			get
			{
				// new HttpContext(new HttpRequest(), new HttpResponse(writer));
				// TODO: Consider whether it's worthwhile to implement this one (definitely it's not easy)
				return null;
			}
		}

		/// <summary>
		/// Access the session objects.
		/// </summary>
		/// <value></value>
		public virtual IDictionary Session
		{
			get { return session; }
			set { session = value; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		/// <value></value>
		public virtual IRequest Request
		{
			get { return request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		/// <value></value>
		public virtual IResponse Response
		{
			get { return response; }
		}

		/// <summary>
		/// Gets the trace object.
		/// </summary>
		/// <value></value>
		public virtual ITrace Trace
		{
			get { return trace; }
			set { trace = value; }
		}

		/// <summary>
		/// Access a dictionary of volative items.
		/// </summary>
		/// <value></value>
		public virtual Flash Flash
		{
			get { return flash; }
			set { flash = value; }
		}

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		/// <value></value>
		public IPrincipal CurrentUser
		{
			get { return currentUser; }
			set { currentUser = value; }
		}

		/// <summary>
		/// Gets the last exception raised during
		/// the execution of an action.
		/// </summary>
		/// <value></value>
		public Exception LastException
		{
			get { return lastException; }
			set { lastException = value; }
		}

		/// <summary>
		/// Returns the application path.
		/// </summary>
		/// <value></value>
		public virtual string ApplicationPath
		{
			get { return urlInfo.AppVirtualDir ?? "/"; }
		}

//		/// <summary>
//		/// Transfer the execution to another resource.
//		/// </summary>
//		/// <param name="path"></param>
//		/// <param name="preserveForm"></param>
//		public virtual void Transfer(string path, bool preserveForm)
//		{
//		}

//		/// <summary>
//		/// Returns the physical application path.
//		/// </summary>
//		/// <value></value>
//		public virtual string ApplicationPhysicalPath
//		{
//			get { return physicalPath; }
//		}

		/// <summary>
		/// Returns the <see cref="UrlInfo"/> of the the current request.
		/// </summary>
		/// <value></value>
		public virtual UrlInfo UrlInfo
		{
			get { return urlInfo; }
			set { urlInfo = value; }
		}

		/// <summary>
		/// Returns an <see cref="IServerUtility"/>.
		/// </summary>
		/// <value></value>
		public virtual IServerUtility Server
		{
			get { return serverUtility; }
			set { serverUtility = value; }
		}

		/// <summary>
		/// Returns the Items collection from the current HttpContext.
		/// </summary>
		/// <value></value>
		public virtual IDictionary Items
		{
			get { return contextItems; }
		}

		/// <summary>
		/// Gets or sets the current controller.
		/// </summary>
		/// <value>The current controller.</value>
		public virtual IController CurrentController
		{
			get { return currentController; }
			set { currentController = value; }
		}

		/// <summary>
		/// Gets or sets the current controller context.
		/// </summary>
		/// <value>The current controller context.</value>
		public IControllerContext CurrentControllerContext
		{
			get { return currentControllerContext; }
			set { currentControllerContext = value; }
		}

		/// <summary>
		/// Gets a reference to the MonoRail services.
		/// </summary>
		/// <value>The services.</value>
		public IMonoRailServices Services
		{
			get { return services; }
			set { services = value; }
		}

//		#endregion
//
//		/// <summary>
//		/// Sets the container.
//		/// </summary>
//		/// <param name="serviceProvider">The service provider.</param>
//		public void SetContainer(IServiceProvider serviceProvider)
//		{
//			container = serviceProvider;
//		}

		/// <summary>
		/// Gets the rendered email templates.
		/// </summary>
		/// <value>The rendered email templates.</value>
		public virtual List<RenderedEmailTemplate> RenderedEmailTemplates
		{
			get { return renderedEmailTemplates; }
		}

		/// <summary>
		/// Gets the messages sent.
		/// </summary>
		/// <value>The messages sent.</value>
		public virtual List<Message> MessagesSent
		{
			get { return messagesSent; }
		}

		internal void AddMailTemplateRendered(string templateName, IDictionary parameters)
		{
			renderedEmailTemplates.Add(new RenderedEmailTemplate(templateName, parameters));
		}

		internal void AddEmailMessageSent(Message message)
		{
			messagesSent.Add(message);
		}

		/// <summary>
		/// Represents an email template for unit test purposes
		/// </summary>
		public class RenderedEmailTemplate
		{
			private readonly string name;
			private readonly IDictionary parameters;

			/// <summary>
			/// Initializes a new instance of the <see cref="RenderedEmailTemplate"/> class.
			/// </summary>
			/// <param name="name">The name.</param>
			/// <param name="parameters">The parameters.</param>
			public RenderedEmailTemplate(string name, IDictionary parameters)
			{
				this.name = name;
				this.parameters = parameters;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public string Name
			{
				get { return name; }
			}

			/// <summary>
			/// Gets the parameters.
			/// </summary>
			/// <value>The parameters.</value>
			public IDictionary Parameters
			{
				get { return parameters; }
			}
		}
	}
}