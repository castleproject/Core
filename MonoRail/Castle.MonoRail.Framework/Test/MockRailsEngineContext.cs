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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Security.Principal;
	using System.Web;
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services;

	/// <summary>
	/// Represents a mock implementation of <see cref="IRailsEngineContext"/> for unit test purposes.
	/// </summary>
	public class MockRailsEngineContext : AbstractServiceContainer, IRailsEngineContext
	{
		private readonly string physicalPath = AppDomain.CurrentDomain.BaseDirectory;
		private readonly IRequest request;
		private readonly IResponse response;
		private readonly ITrace trace;
		private readonly UrlInfo urlInfo;
		private readonly Flash flash = new Flash();
		private readonly ICacheProvider cacheProvider = new MockCacheProvider();
		private readonly IServerUtility serverUtility = new MockServerUtility();
		private readonly IDictionary session = new HybridDictionary(true);
		private readonly IDictionary contextItems = new HybridDictionary(true);
		private readonly List<RenderedEmailTemplate> renderedEmailTemplates = new List<RenderedEmailTemplate>();
		private readonly List<Message> messagesSent = new List<Message>();
		private string urlReferrer;
		private IServiceProvider container;
		private IPrincipal currentUser = new GenericPrincipal(new GenericIdentity("user", "test"), new string[0]);
		private Exception lastException;
		private Controller currentController;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockRailsEngineContext"/> class.
		/// </summary>
		protected MockRailsEngineContext()
		{
			RegisterServices();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MockRailsEngineContext"/> class.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="response">The response.</param>
		/// <param name="trace">The trace.</param>
		/// <param name="urlInfo">The URL info.</param>
		public MockRailsEngineContext(IRequest request, IResponse response, ITrace trace, UrlInfo urlInfo) : this()
		{
			this.request = request;
			this.response = response;
			this.trace = trace;
			this.urlInfo = urlInfo; 
		}

		#region IRailsEngineContext Members

		/// <summary>
		/// Gets the request type (GET, POST, etc)
		/// </summary>
		/// <value></value>
		public virtual String RequestType
		{
			get { return request.HttpMethod; }
		}

		/// <summary>
		/// Gets the request URL.
		/// </summary>
		/// <value></value>
		public virtual string Url
		{
			get { return urlInfo.UrlRaw; }
		}

		/// <summary>
		/// Gets the referring URL.
		/// </summary>
		/// <value></value>
		public string UrlReferrer
		{
			get { return urlReferrer; }
			set { urlReferrer = value; }
		}

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
		/// Access the params (Query, Post, headers and Cookies)
		/// </summary>
		/// <value></value>
		public virtual NameValueCollection Params
		{
			get { return request.Params; }
		}

		/// <summary>
		/// Access the session objects.
		/// </summary>
		/// <value></value>
		public virtual IDictionary Session
		{
			get { return session; }
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
		}

		/// <summary>
		/// Access the Cache associated with this
		/// web execution context.
		/// </summary>
		/// <value></value>
		public virtual ICacheProvider Cache
		{
			get { return cacheProvider; }
		}

		/// <summary>
		/// Access a dictionary of volative items.
		/// </summary>
		/// <value></value>
		public virtual Flash Flash
		{
			get { return flash; }
		}

		/// <summary>
		/// Transfer the execution to another resource.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="preserveForm"></param>
		public virtual void Transfer(string path, bool preserveForm)
		{
			throw new NotImplementedException();
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
			get { return urlInfo.AppVirtualDir; }
		}

		/// <summary>
		/// Returns the physical application path.
		/// </summary>
		/// <value></value>
		public virtual string ApplicationPhysicalPath
		{
			get { return physicalPath; }
		}

		/// <summary>
		/// Returns the <see cref="UrlInfo"/> of the the current request.
		/// </summary>
		/// <value></value>
		public virtual UrlInfo UrlInfo
		{
			get { return urlInfo; }
		}

		/// <summary>
		/// Returns an <see cref="IServerUtility"/>.
		/// </summary>
		/// <value></value>
		public virtual IServerUtility Server
		{
			get { return serverUtility; }
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
		public virtual Controller CurrentController
		{
			get { return currentController; }
			set { currentController = value; }
		}

		/// <summary>
		/// If a container is available for the app, this
		/// property exposes its instance.
		/// </summary>
		/// <value></value>
		public IServiceProvider Container
		{
			get { return container; }
		}

		#endregion

		/// <summary>
		/// Sets the container.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public void SetContainer(IServiceProvider serviceProvider)
		{
			container = serviceProvider;
		}

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

		/// <summary>
		/// Registers the services.
		/// </summary>
		private void RegisterServices()
		{
			DefaultUrlBuilder urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = serverUtility;
			AddService(typeof(IUrlBuilder), urlBuilder);

			AddService(typeof(IValidatorRegistry), new CachedValidationRegistry());

			AddService(typeof(IEmailTemplateService), new MockEmailTemplateService(this));
			AddService(typeof(IEmailSender), new MockSmtpSender(this));

			AddService(typeof(IHelperDescriptorProvider), new DefaultHelperDescriptorProvider());
			AddService(typeof(IFilterDescriptorProvider), new DefaultFilterDescriptorProvider());
			AddService(typeof(ILayoutDescriptorProvider), new DefaultLayoutDescriptorProvider());
			AddService(typeof(IRescueDescriptorProvider), new DefaultRescueDescriptorProvider());
			AddService(typeof(IResourceDescriptorProvider), new DefaultResourceDescriptorProvider());
			AddService(typeof(ITransformFilterDescriptorProvider), new DefaultTransformFilterDescriptorProvider());

			DefaultControllerDescriptorProvider controllerDescProvider = new DefaultControllerDescriptorProvider();
			controllerDescProvider.Service(this);
			AddService(typeof(IControllerDescriptorProvider), controllerDescProvider);

			AddService(typeof(IViewEngineManager), new DefaultViewEngineManager());
			AddService(typeof(IScaffoldingSupport), new MockScaffoldingSupport());
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
