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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Configuration;
	using System.IO;

	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// Abstract base class for View Engines.
	/// </summary>
	public abstract class ViewEngineBase : IViewEngine, IServiceEnabledComponent
	{
		private bool xhtmlRendering;
		private IViewSourceLoader viewSourceLoader;
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// The service provider instance
		/// </summary>
		protected IServiceProvider serviceProvider;

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public virtual void Service(IServiceProvider provider)
		{
			serviceProvider = provider;

			viewSourceLoader = (IViewSourceLoader)provider.GetService(typeof(IViewSourceLoader));

			if (viewSourceLoader == null)
			{
				string message = "Could not obtain IViewSourceLoader";
				throw new ConfigurationErrorsException(message);
			}

			ILoggerFactory loggerFactory = (ILoggerFactory)provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(GetType());
			}
		}

		#endregion

		#region IViewEngine implementation

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// <c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public abstract bool SupportsJSGeneration { get; }

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public abstract string ViewFileExtension { get; }

		/// <summary>
		/// Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public abstract string JSGeneratorFileExtension { get; }

		/// <summary>
		/// Gets/sets whether rendering should aim 
		/// to be XHTML compliant, obtained from the configuration.
		/// </summary>
		public bool XHtmlRendering
		{
			get { return xhtmlRendering; }
			set { xhtmlRendering = value; }
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public virtual bool HasTemplate(String templateName)
		{
			return
				ViewSourceLoader.HasTemplate(ResolveTemplateName(templateName)) ||
				ViewSourceLoader.HasTemplate(ResolveJSTemplateName(templateName));
		}

		/// <summary>
		/// Evaluates whether the specified template can be used to generate js.
		/// </summary>
		/// <returns><c>true</c> if it exists and has the correct file extension</returns>
		public virtual bool IsTemplateForJSGeneration(String templateName)
		{
			string resolvedTemplateName = ResolveJSTemplateName(templateName);
			return 
				resolvedTemplateName.ToLowerInvariant().EndsWith(JSGeneratorFileExtension.ToLowerInvariant()) &&
				HasTemplate(resolvedTemplateName);
		}

		/// <summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		public abstract void Process(IRailsEngineContext context, IController controller, String templateName);

		///<summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template
		/// and writes the results to the System.IO.TextWriter.
		/// </summary>
		public abstract void Process(TextWriter output, IRailsEngineContext context, IController controller, String templateName);

		/// <summary>
		/// Should process the specified partial. The partial name must contains
		/// the path relative to the views folder.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="partialName">The partial name.</param>
		public abstract void ProcessPartial(TextWriter output, IRailsEngineContext context, IController controller, string partialName);

		/// <summary>
		/// Implementors should return a generator instance if
		/// the view engine supports JS generation.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <returns>A JS generator instance</returns>
		public abstract object CreateJSGenerator(IRailsEngineContext context);

		/// <summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the context to output the result.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template.</param>
		public virtual void GenerateJS(IRailsEngineContext context, IController controller, string templateName)
		{
			GenerateJS(context.Response.Output, context, controller, templateName);
		}

		/// <summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the specified <see cref="TextWriter"/>
		/// to output the result.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template.</param>
		public abstract void GenerateJS(TextWriter output, IRailsEngineContext context, IController controller, string templateName);

		/// <summary>
		/// Wraps the specified content in the layout using the 
		/// context to output the result.
		/// </summary>
		public abstract void ProcessContents(IRailsEngineContext context, IController controller, String contents);

		/// <summary>
		/// Resolves the template name into a file name with the proper file extension
		/// </summary>
		protected virtual string ResolveTemplateName(string templateName)
		{
			if (Path.HasExtension(templateName))
			{
				return templateName;
			}
			else
			{
				return templateName + ViewFileExtension;
			}
		}

		/// <summary>
		/// Resolves the template name into a JS generation file name with the proper file extension
		/// </summary>
		protected virtual string ResolveJSTemplateName(string templateName)
		{
			if (Path.HasExtension(templateName))
			{
				return templateName;
			}
			else
			{
				return templateName + JSGeneratorFileExtension;
			}
		}
		#endregion

		#region Pre/Post send view

		/// <summary>
		/// Invokes the <see cref="Controller.PreSendView"/>
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="view">The view argument.</param>
		protected virtual void PreSendView(IController controller, object view)
		{
			controller.PreSendView(view);
		}

		/// <summary>
		/// Invokes the <see cref="Controller.PostSendView"/>
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="view">The view argument.</param>
		protected virtual void PostSendView(IController controller, object view)
		{
			controller.PostSendView(view);
		}

		#endregion

		#region Useful properties

		/// <summary>
		/// Gets or sets the view source loader.
		/// </summary>
		/// <value>The view source loader.</value>
		protected IViewSourceLoader ViewSourceLoader
		{
			get { return viewSourceLoader; }
			set { viewSourceLoader = value; }
		}

		/// <summary>
		/// Gets the logger.
		/// </summary>
		/// <value>The logger.</value>
		protected ILogger Logger
		{
			get { return logger; }
		}

		#endregion

		#region Render Helpers

		/// <summary>
		/// Sets the HTTP Content-Type header appropriately.
		/// </summary>
		protected virtual void AdjustContentType(IRailsEngineContext context)
		{
			if (xhtmlRendering)
			{
				//Find out what they'll accept
				String httpAccept = context.Request.Headers["Accept"];

				//TODO: Evaluate the q-values of the Accept header

				//Do they accept application/xhtml+xml?
				if (httpAccept != null && httpAccept.IndexOf("application/xhtml+xml") != -1)
				{
					//Send them the proper content type
					context.Response.ContentType = "application/xhtml+xml";

					//TODO: Add the xml prolog for browsers that support it
					//response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				}
				else
				{
					//Fall back to text/html for older folk
					context.Response.ContentType = "text/html";
				}

				//Fix up the proxy
				context.Response.AppendHeader("Vary", "Accept");
			}
			else if (context.Response.ContentType == null)
			{
				//Just use HTML
				context.Response.ContentType = "text/html";
			}
		}

		/// <summary>
		/// Sets the HTTP Content-Type header to <c>text/javascript</c>
		/// </summary>
		protected void AdjustJavascriptContentType(IRailsEngineContext context)
		{
			context.Response.ContentType = "text/javascript";
		}

		#endregion
	}
}
