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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// The view engine manager sits between MonoRail and all the registered 
	/// view engines. It is used to identify the view engine that should handle a 
	/// render request and delegates such requests properly. 
	/// </summary>
	public class DefaultViewEngineManager : IViewEngineManager, IServiceEnabledComponent, IInitializable
	{
		private MonoRailConfiguration config;
		private IServiceProvider provider;
		private IDictionary ext2ViewEngine;
		private IDictionary viewEnginesFastLookup;
		private IDictionary jsgFastLookup;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultViewEngineManager"/> class.
		/// </summary>
		public DefaultViewEngineManager()
		{
			ext2ViewEngine = new HybridDictionary(true);
			jsgFastLookup = new HybridDictionary(true);
			viewEnginesFastLookup = new Hashtable();
		}

		#region IInitializable

		/// <summary>
		/// Implementors should perform any initialization logic.
		/// </summary>
		public void Initialize()
		{
			foreach (ViewEngineInfo info in config.ViewEngineConfig.ViewEngines)
			{
				try
				{
					IViewEngine engine = (IViewEngine)Activator.CreateInstance(info.Engine);

					RegisterEngineForView(engine);

					RegisterEngineForExtesionLookup(engine);

					engine.XHtmlRendering = info.XhtmlRendering;

					IServiceEnabledComponent serviceEnabled = engine as IServiceEnabledComponent;

					if (serviceEnabled != null)
					{
						serviceEnabled.Service(provider);
					}

					IInitializable initializable = engine as IInitializable;

					if (initializable != null)
					{
						initializable.Initialize();
					}
				}
				catch (InvalidCastException)
				{
					throw new RailsException("Type " + info.Engine.FullName + " does not implement IViewEngine");
				}
				catch (Exception ex)
				{
					throw new RailsException("Could not create view engine instance: " + info.Engine, ex);
				}
			}

			config = null;
		}

		private void RegisterEngineForExtesionLookup(IViewEngine engine)
		{
			viewEnginesFastLookup.Add(engine, null);
		}

		#endregion

		#region IServiceEnabledComponent

		/// <summary>
		/// Services the specified service provider.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public void Service(IServiceProvider serviceProvider)
		{
			provider = serviceProvider;

			config = (MonoRailConfiguration)provider.GetService(typeof(MonoRailConfiguration));
		}

		#endregion

		#region IViewEngineManager

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName">View template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(String templateName)
		{
			IViewEngine engine = ResolveEngine(templateName, false);
			if (engine == null)
				return false;
			return engine.HasTemplate(templateName);
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public void Process(IRailsEngineContext context, IController controller, string templateName)
		{
			IViewEngine engine = ResolveEngine(templateName);

			ContextualizeViewEngine(engine);

			if (engine.SupportsJSGeneration && engine.IsTemplateForJSGeneration(templateName))
			{
				engine.GenerateJS(context, controller, templateName);
				return;
			}

			engine.Process(context, controller, templateName);
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// <para>
		/// Please note that no layout is applied
		/// </para>
		/// </summary>
		/// <param name="output"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public void Process(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			IViewEngine engine = ResolveEngine(templateName);

			ContextualizeViewEngine(engine);

			if (engine.SupportsJSGeneration && engine.IsTemplateForJSGeneration(templateName))
			{
				engine.GenerateJS(output, context, controller, templateName);
				return;
			}

			engine.Process(output, context, controller, templateName);
		}

		/// <summary>
		/// Processes a partial view = using the partialName
		/// to obtain the correct template and writes the
		/// results to the System.TextWriter.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="partialName">The partial name.</param>
		public void ProcessPartial(TextWriter output, IRailsEngineContext context, IController controller, string partialName)
		{

			IViewEngine engine = ResolveEngine(partialName);

			engine.ProcessPartial(output, context, controller, partialName);
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void ProcessContents(IRailsEngineContext context, IController controller, String contents)
		{
			if (controller.LayoutName == null)
			{
				throw new RailsException("ProcessContents can only work with a layout");
			}

			String templateName = Path.Combine("layouts", controller.LayoutName);

			IViewEngine engine = ResolveEngine(templateName);

			engine.ProcessContents(context, controller, contents);
		}

		#endregion

		/// <summary>
		/// Contextualizes the view engine.
		/// </summary>
		/// <param name="engine">The engine.</param>
		private void ContextualizeViewEngine(IViewEngine engine)
		{
			MonoRailHttpHandler.CurrentContext.AddService(typeof(IViewEngine), engine);
		}

		/// <summary>
		/// The view can be informed with an extension. If so, we use it
		/// to discover the extension. Otherwise, we ask the configured 
		/// view engines to find out which (if any) can handle the requested
		/// view. If no suitable view engine is found, an exception would be thrown
		/// </summary>
		/// <param name="templateName">View name</param>
		/// <returns>A view engine instance</returns>
		private IViewEngine ResolveEngine(String templateName)
		{
			return ResolveEngine(templateName, true);
		}

		/// <summary>
		/// The view can be informed with an extension. If so, we use it
		/// to discover the extension. Otherwise, we ask the configured 
		/// view engines to find out which (if any) can handle the requested
		/// view.
		/// </summary>
		/// <param name="throwIfNotFound">If true and no suitable view engine is found, an exception would be thrown</param>
		/// <param name="templateName">View name</param>
		/// <returns>A view engine instance</returns>
		private IViewEngine ResolveEngine(String templateName, bool throwIfNotFound)
		{
			if (Path.HasExtension(templateName))
			{
				String extension = Path.GetExtension(templateName);
				IViewEngine engine = ext2ViewEngine[extension] as IViewEngine;
				return engine;
			}

			foreach (IViewEngine engine in viewEnginesFastLookup.Keys)
				if (engine.HasTemplate(templateName))
					return engine;
					
			if (throwIfNotFound)
				throw new RailsException(string.Format(
@"MonoRail could not have resolved a view engine instance for the template '{0}'
There are two possible explanations: that the template does not exist, or that the relevant view engine has not been configured correctly in web.config.", templateName));
			
			return null;
		}

		/// <summary>
		/// Associates extensions with the view engine instance.
		/// </summary>
		/// <param name="engine">The view engine instance</param>
		private void RegisterEngineForView(IViewEngine engine)
		{
			if (ext2ViewEngine.Contains(engine.ViewFileExtension))
			{
				IViewEngine existing = (IViewEngine)ext2ViewEngine[engine.ViewFileExtension];

				throw new RailsException(
					"At least two view engines are handling the same file extension. " +
					"This isn't going to work. Extension: " + engine.ViewFileExtension +
					" View Engine 1: " + existing.GetType() +
					" View Engine 2: " + engine.GetType());
			}

			String extension = engine.ViewFileExtension.StartsWith(".")
								? engine.ViewFileExtension
								: "." + engine.ViewFileExtension;

			ext2ViewEngine[extension] = engine;

			if (engine.SupportsJSGeneration && ext2ViewEngine.Contains(engine.JSGeneratorFileExtension))
			{
				IViewEngine existing = (IViewEngine)ext2ViewEngine[engine.JSGeneratorFileExtension];

				throw new RailsException(
					"At least two view engines are handling the same file extension. " +
					"This isn't going to work. Extension: " + engine.JSGeneratorFileExtension +
					" View Engine 1: " + existing.GetType() +
					" View Engine 2: " + engine.GetType());
			}

			if (engine.SupportsJSGeneration)
			{
				extension = engine.JSGeneratorFileExtension.StartsWith(".")
								? engine.JSGeneratorFileExtension
								: "." + engine.JSGeneratorFileExtension;

				ext2ViewEngine[extension] = engine;
				jsgFastLookup[extension] = engine;
			}
		}
	}
}
