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
	/// Pendent
	/// </summary>
	public class DefaultViewEngineManager : IViewEngineManager, IServiceEnabledComponent, IInitializable
	{
		private MonoRailConfiguration config;
		private IServiceProvider provider;
		private IDictionary ext2ViewEngine;
		private IViewSourceLoader viewSourceLoader;
		private IDictionary jsgFastLookup;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultViewEngineManager"/> class.
		/// </summary>
		public DefaultViewEngineManager()
		{
			ext2ViewEngine = new HybridDictionary(true);
			jsgFastLookup = new HybridDictionary(true);
		}

		#region IInitializable

		public void Initialize()
		{
			foreach(ViewEngineInfo info in config.ViewEngineConfig.ViewEngines)
			{
				try
				{
					IViewEngine engine = (IViewEngine) Activator.CreateInstance(info.Engine);

					RegisterEngineForView(engine);

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
				catch(InvalidCastException)
				{
					throw new RailsException("Type " + info.Engine.FullName + " does not implement IViewEngine");
				}
				catch(Exception ex)
				{
					throw new RailsException("Could not create view engine instance: " + info.Engine, ex);
				}
			}

			config = null;
		}

		#endregion

		#region IServiceEnabledComponent

		public void Service(IServiceProvider serviceProvider)
		{
			provider = serviceProvider;

			config = (MonoRailConfiguration) provider.GetService(typeof(MonoRailConfiguration));
			viewSourceLoader = (IViewSourceLoader) provider.GetService(typeof(IViewSourceLoader));
		}

		#endregion

		#region IViewEngineManager

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName">View template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			return FindExistingTemplate(templateName) != null;
		}

		public void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			String resolvedTemplateName = FindExistingTemplate(templateName);

			AssertTemplateExists(resolvedTemplateName, templateName);

			IViewEngine engine = ResolveEngine(resolvedTemplateName);

			ContextualizeViewEngine(engine);

			if (jsgFastLookup.Contains(Path.GetExtension(resolvedTemplateName)))
			{
				engine.GenerateJS(context, controller, resolvedTemplateName);
			}
			else
			{
				engine.Process(context, controller, templateName);
			}
		}

		public void Process(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			String resolvedTemplateName = FindExistingTemplate(templateName);

			AssertTemplateExists(resolvedTemplateName, templateName);

			IViewEngine engine = ResolveEngine(resolvedTemplateName);

			ContextualizeViewEngine(engine);

			if (jsgFastLookup.Contains(Path.GetExtension(resolvedTemplateName)))
			{
				engine.GenerateJS(output, context, controller, resolvedTemplateName);
			}
			else
			{
				engine.Process(output, context, controller, templateName);
			}
		}

		public void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller, string partialName)
		{
			String resolvedTemplateName = FindExistingTemplate(partialName);

			AssertTemplateExists(resolvedTemplateName, partialName);

			IViewEngine engine = ResolveEngine(resolvedTemplateName);

			engine.ProcessPartial(output, context, controller, resolvedTemplateName);
		}

		/// <summary>
		/// Wraps the specified content in the layout using
		/// the context to output the result.
		/// </summary>
		public void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			if (controller.LayoutName == null)
			{
				throw new RailsException("ProcessContents can only work with a layout");
			}

			String templateName = Path.Combine("layouts", controller.LayoutName);
			String resolvedTemplateName = FindExistingTemplate(templateName);

			AssertTemplateExists(resolvedTemplateName, templateName);

			IViewEngine engine = ResolveEngine(resolvedTemplateName);

			engine.ProcessContents(context, controller, contents);
		}

		#endregion

		private String FindExistingTemplate(string templateName)
		{
			if (Path.HasExtension(templateName))
			{
				return viewSourceLoader.HasTemplate(templateName) ? templateName : null;
			}
			else
			{
				foreach(String ext in ext2ViewEngine.Keys)
				{
					String tempTemplateName = templateName + ext;

					if (viewSourceLoader.HasTemplate(tempTemplateName))
					{
						return tempTemplateName;
					}
				}

				return null;
			}
		}

		private void ContextualizeViewEngine(IViewEngine engine)
		{
			MonoRailHttpHandler.CurrentContext.AddService(typeof(IViewEngine), engine);
		}

		/// <summary>
		/// The view can be informed with an extension. If so, we use it
		/// to discover the extension. Otherwise, we use the view source
		/// to find out the file that exists there, and hence the view 
		/// engine instance
		/// </summary>
		/// <param name="templateName">View name</param>
		/// <returns>A view engine instance</returns>
		private IViewEngine ResolveEngine(string templateName)
		{
			if (!Path.HasExtension(templateName))
			{
				throw new ArgumentException("ResolveEngine only works with a template " +
				                            "name with extension. templateName " + templateName);
			}

			String extension = Path.GetExtension(templateName);

			IViewEngine engine = (IViewEngine) ext2ViewEngine[extension];

			if (engine != null) return engine;

			throw new RailsException(
				"Could not find a suitable View engine to " +
				"handle view template with extension " + extension + ". View template: " + templateName);
		}

		/// <summary>
		/// Associates extensions with the view engine instance.
		/// </summary>
		/// <param name="engine">The view engine instance</param>
		private void RegisterEngineForView(IViewEngine engine)
		{
			if (ext2ViewEngine.Contains(engine.ViewFileExtension))
			{
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.ViewFileExtension];

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
				IViewEngine existing = (IViewEngine) ext2ViewEngine[engine.JSGeneratorFileExtension];

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

		private void AssertTemplateExists(string resolvedTemplateName, string templateName)
		{
			if (resolvedTemplateName == null)
			{
				throw new RailsException("Could not find view template: " + templateName);
			}
		}
	}
}