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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using JSGeneration;

	/// <summary>
	/// The view engine manager sits between MonoRail and all the registered 
	/// view engines. It is used to identify the view engine that should handle a 
	/// render request and delegates such requests properly. 
	/// </summary>
	public class DefaultViewEngineManager : IViewEngineManager, IServiceEnabledComponent, IInitializable
	{
		private IMonoRailConfiguration config;
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
			foreach(ViewEngineInfo info in config.ViewEngineConfig.ViewEngines)
			{
				IViewEngine engine;

				try
				{
					engine = (IViewEngine) Activator.CreateInstance(info.Engine);
				}
				catch(InvalidCastException)
				{
					throw new MonoRailException("Type " + info.Engine.FullName + " does not implement IViewEngine");
				}
				catch(Exception ex)
				{
					throw new MonoRailException("Could not create view engine instance: " + info.Engine, ex);
				}

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
		}

		/// <summary>
		/// Registers the engine for extesion lookup.
		/// </summary>
		/// <param name="engine">The engine.</param>
		public void RegisterEngineForExtesionLookup(IViewEngine engine)
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

			config = (IMonoRailConfiguration) provider.GetService(typeof(IMonoRailConfiguration));
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

			if (engine == null) return false;

			return engine.HasTemplate(templateName);
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
		/// <param name="controllerContext"></param>
		/// <param name="templateName"></param>
		public void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		                    IControllerContext controllerContext)
		{
			IViewEngine engine = ResolveEngine(templateName);

			ContextualizeViewEngine(engine);

			if (engine.SupportsJSGeneration && engine.IsTemplateForJSGeneration(templateName))
			{
				engine.GenerateJS(templateName, output, CreateJSCodeGeneratorInfo(context, controller, controllerContext), context, controller, controllerContext);
			}
			else
			{
				engine.Process(templateName, output, context, controller, controllerContext);
			}
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.TextWriter.
		/// </summary>
		public void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
		{
			IViewEngine engine = ResolveEngine(templateName);

			ContextualizeViewEngine(engine);

			engine.Process(templateName, layoutName, output, parameters);
		}

		/// <summary>
		/// Processes a partial view = using the partialName
		/// to obtain the correct template and writes the
		/// results to the System.TextWriter.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="partialName">The partial name.</param>
		public void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller,
		                           IControllerContext controllerContext)
		{
			IViewEngine engine = ResolveEngine(partialName);

			ContextualizeViewEngine(engine);

			engine.ProcessPartial(partialName, output, context, controller, controllerContext);
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
		                                     IControllerContext controllerContext)
		{
			if (controllerContext.LayoutNames == null)
			{
				throw new MonoRailException("RenderStaticWithinLayout can only work with a layout");
			}

			String templateName = Path.Combine("layouts", controllerContext.LayoutNames[0]);

			IViewEngine engine = ResolveEngine(templateName);

			ContextualizeViewEngine(engine);

			engine.RenderStaticWithinLayout(contents, context, controller, controllerContext);
		}

		/// <summary>
		/// Creates the JS code generator info. Temporarily on IViewEngineManager
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns></returns>
		public JSCodeGeneratorInfo CreateJSCodeGeneratorInfo(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			JSGeneratorConfiguration jsConfig = config.JSGeneratorConfiguration;

			if (jsConfig.DefaultLibrary == null)
			{
				throw new MonoRailException("No default JS Generator library configured. By default MonoRail configures " +
					"itself to use the Prototype JS library. If you have configured other, make sure you set it as default.");
			}

			JSCodeGenerator codeGenerator =
				new JSCodeGenerator(engineContext.Server, this,
					engineContext, controller, controllerContext, engineContext.Services.UrlBuilder);

			IJSGenerator jsGen = (IJSGenerator)
				Activator.CreateInstance(jsConfig.DefaultLibrary.MainGenerator, new object[] { codeGenerator });

			codeGenerator.JSGenerator = jsGen;

			object[] extensions = CreateExtensions(codeGenerator, jsConfig.DefaultLibrary.MainExtensions);
			object[] elementExtension = CreateExtensions(codeGenerator, jsConfig.DefaultLibrary.ElementExtension);

			return new JSCodeGeneratorInfo(codeGenerator, jsGen, extensions, elementExtension);
		}

		#endregion

		/// <summary>
		/// Contextualizes the view engine.
		/// </summary>
		/// <param name="engine">The engine.</param>
		private void ContextualizeViewEngine(IViewEngine engine)
		{
			if (MonoRailHttpHandlerFactory.CurrentEngineContext != null)//required for tests
			{
				MonoRailHttpHandlerFactory.CurrentEngineContext.AddService(typeof(IViewEngine), engine);
			}
		}

		private static object[] CreateExtensions(IJSCodeGenerator generator, List<Type> extensions)
		{
			int index = 0;
			object[] list = new object[extensions.Count];

			foreach(Type extensionType in extensions)
			{
				object extension = Activator.CreateInstance(extensionType, generator);

				list[index++] = extension;

				generator.Extensions.Add(extensionType.Name, extension);
			}

			return list;
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

			foreach(IViewEngine engine in viewEnginesFastLookup.Keys)
			{
				if (engine.HasTemplate(templateName)) return engine;
			}

			if (throwIfNotFound)
			{
				throw new MonoRailException(string.Format(
				                            	@"MonoRail could not resolve a view engine instance for the template '{0}'
There are two possible reasons: either the template does not exist, or the view engine " +
				                            	"that handles an specific file extension has not been configured correctly web.config (section monorail, node viewEngines).",
				                            	templateName));
			}

			return null;
		}

		/// <summary>
		/// Associates extensions with the view engine instance.
		/// </summary>
		/// <param name="engine">The view engine instance</param>
		public void RegisterEngineForView(IViewEngine engine)
		{
			if (ext2ViewEngine.Contains(engine.ViewFileExtension))
			{
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.ViewFileExtension];

				throw new MonoRailException(
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
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.JSGeneratorFileExtension];

				throw new MonoRailException(
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
