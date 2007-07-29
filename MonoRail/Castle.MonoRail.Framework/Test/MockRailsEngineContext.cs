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
		private IPrincipal currentUser = new GenericPrincipal(new GenericIdentity("user", "test"), new string[0]);
		private Exception lastException;
		private Controller currentController;

		protected MockRailsEngineContext()
		{
			RegisterServices();
		}

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
		public String RequestType
		{
			get { return request.HttpMethod; }
		}

		public virtual string Url
		{
			get { throw new NotImplementedException(); }
		}

		public string UrlReferrer
		{
			get { return urlReferrer; }
			set { urlReferrer = value; }
		}

		public HttpContext UnderlyingContext
		{
			get 
			{ 
				// new HttpContext(new HttpRequest(), new HttpResponse(writer));
				// TODO: Consider whether it's worthwhile to implement this one (definitely it's not easy)
				return null;
			}
		}

		public NameValueCollection Params
		{
			get { return request.Params; }
		}

		public IDictionary Session
		{
			get { return session; }
		}

		public IRequest Request
		{
			get { return request; }
		}

		public IResponse Response
		{
			get { return response; }
		}

		public ITrace Trace
		{
			get { return trace; }
		}

		public ICacheProvider Cache
		{
			get { return cacheProvider; }
		}

		public Flash Flash
		{
			get { return flash; }
		}

		public virtual void Transfer(string path, bool preserveForm)
		{
			throw new NotImplementedException();
		}

		public IPrincipal CurrentUser
		{
			get { return currentUser; }
			set { currentUser = value; }
		}

		public Exception LastException
		{
			get { return lastException; }
			set { lastException = value; }
		}

		/// <summary>
		/// I'm not sure about this one
		/// TODO: Review it
		/// </summary>
		public string ApplicationPath
		{
			get { return urlInfo.AppVirtualDir; }
		}

		public string ApplicationPhysicalPath
		{
			get { return physicalPath; }
		}

		public UrlInfo UrlInfo
		{
			get { return urlInfo; }
		}

		public IServerUtility Server
		{
			get { return serverUtility; }
		}

		public IDictionary Items
		{
			get { return contextItems; }
		}

		public Controller CurrentController
		{
			get { return currentController; }
			set { currentController = value; }
		}

		#endregion

		public List<RenderedEmailTemplate> RenderedEmailTemplates
		{
			get { return renderedEmailTemplates; }
		}

		public List<Message> MessagesSent
		{
			get { return messagesSent; }
		}

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

		public class RenderedEmailTemplate
		{
			private readonly string name;
			private readonly IDictionary parameters;

			public RenderedEmailTemplate(string name, IDictionary parameters)
			{
				this.name = name;
				this.parameters = parameters;
			}

			public string Name
			{
				get { return name; }
			}

			public IDictionary Parameters
			{
				get { return parameters; }
			}
		}
	}
}
