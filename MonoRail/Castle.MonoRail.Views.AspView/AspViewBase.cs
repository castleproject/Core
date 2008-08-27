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

namespace Castle.MonoRail.Views.AspView
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Web;
	using Components.DictionaryAdapter;
	using Framework;
	using Internal;

	public abstract class AspViewBase : IViewBaseInternal
	{
		protected IDictionaryAdapterFactory dictionaryAdapterFactory;
		private IViewBaseInternal contentView;
		private IEngineContext context;
		private IController controller;
		private IControllerContext controllerContext;
		private IHelpersAccesor helpers;
		private bool initialized;
		private TextWriter outputWriter;

		/// <summary>
		/// Stack of writers, used as buffers for viewfilters
		/// </summary>
		private Stack<TextWriter> outputWriters;

		private IViewBase parentView;
		private IDictionary properties;
		private string viewContents;
		private AspViewEngine viewEngine;

		/// <summary>
		/// Maintains the currently active view filters
		/// </summary>
		private Stack<IViewFilter> viewFilters;

		/// <summary>
		/// Gets the Application's full virtual root, including protocol, host and port
		/// </summary>
		protected string fullSiteRoot
		{
			get { return (string) Properties["fullSiteRoot"]; }
		}

		/// <summary>
		/// Gets the builtin helpers
		/// </summary>
		protected IHelpersAccesor Helpers
		{
			get
			{
				if (helpers == null)
					helpers = dictionaryAdapterFactory.GetAdapter<IHelpersAccesor>(Properties);
				return helpers;
			}
		}

		/// <summary>
		/// Gets the Application's virtual root
		/// </summary>
		protected string siteRoot
		{
			get { return (string) Properties["siteRoot"]; }
		}

		protected abstract string ViewDirectory { get; }
		protected abstract string ViewName { get; }

		private bool HasContentView
		{
			get { return contentView != null; }
		}

		/// <summary>
		/// Signals the current view to apply the last view filter that was started on the buffered output
		/// </summary>
		protected void EndFiltering()
		{
			string original = outputWriter.ToString();
			IViewFilter filter = viewFilters.Pop();
			string filtered = filter.ApplyOn(original);
			outputWriter.Dispose();
			outputWriter = outputWriters.Pop();
			outputWriter.Write(filtered);
		}

		/// <summary>
		/// Gathers properties marked with ".@bubbleUp" from an other view
		/// Should be used with CaptureFor components and the likes
		/// </summary>
		/// <param name="otherView">The view to gather the bubbling properties from</param>
		protected void GatherBubblingPropertiesFrom(IViewBase otherView)
		{
			foreach (string key in otherView.Properties.Keys)
			{
				if (!otherView.Properties.Contains(key + ".@bubbleUp"))
					continue;
				properties[key] = otherView.Properties[key];
				properties[key + ".@bubbleUp"] = true;
			}
		}

		/// <summary>
		/// Gets a parameter's value from the view's propery containers.
		/// will throw exception if the parameter is not found
		/// </summary>
		/// <param name="parameterName">Parameter's name to look for</param>
		/// <returns>The parametr's value</returns>
		protected object GetParameter(string parameterName)
		{
			object value;
			if (!TryGetParameter(parameterName, out value, null))
				throw new AspViewException("Parameter '" + parameterName + "' was not found!");
			return value;
		}

		/// <summary>
		/// Gets a parameter's value from the view's propery containers.
		/// will return a default value if the parameter is not found
		/// </summary>
		/// <param name="parameterName">Parameter's name to look for</param>
		/// <param name="defaultValue">The value to use if the parameter will not be found</param>
		/// <returns>The parametr's value</returns>
		protected object GetParameter(string parameterName, object defaultValue)
		{
			object value;
			TryGetParameter(parameterName, out value, defaultValue);
			return value;
		}

		/// <summary>
		/// Invokes a parameterless, bodyless, sectionless view component
		/// </summary>
		/// <param name="componentName">The view component name</param>
		protected void InvokeViewComponent(string componentName)
		{
			InvokeViewComponent(componentName, new ParametersDictionary(), null, null);
		}

		/// <summary>
		/// Invokes a bodyless, sectionless view component
		/// </summary>
		/// <param name="componentName">The view component name</param>
		/// <param name="parameters">The parameters to be passed to the component</param>
		protected void InvokeViewComponent(string componentName, IDictionary parameters)
		{
			InvokeViewComponent(componentName, parameters, null, null);
		}

		/// <summary>
		/// Invokes a parameterless view component, and registeres section handlers
		/// </summary>
		/// <param name="componentName">The view component name</param>
		/// <param name="bodyHandler">Delegate to render the component's body. null if the component does not have a body</param>
		/// <param name="sectionHandlers">Delegates to render the component's sections, by the delegate names</param>
		protected void InvokeViewComponent(
			string componentName,
			ViewComponentSectionRendereDelegate bodyHandler,
			IEnumerable<KeyValuePair<string, ViewComponentSectionRendereDelegate>> sectionHandlers)
		{
			InvokeViewComponent(componentName, new ParametersDictionary(), bodyHandler, sectionHandlers);
		}
	
		/// <summary>
		/// Invokes a view component, and registeres section handlers
		/// </summary>
		/// <param name="componentName">The view component name</param>
		/// <param name="parameters">The parameters to be passed to the component</param>
		/// <param name="bodyHandler">Delegate to render the component's body. null if the component does not have a body</param>
		/// <param name="sectionHandlers">Delegates to render the component's sections, by the delegate names</param>
		protected void InvokeViewComponent(
			string componentName, 
			IDictionary parameters,
			ViewComponentSectionRendereDelegate bodyHandler,
            IEnumerable<KeyValuePair<string, ViewComponentSectionRendereDelegate>> sectionHandlers)
		{
			ViewComponentContext viewComponentContext = new ViewComponentContext(
				this, bodyHandler,
				componentName, parameters);

			if (sectionHandlers != null)
				foreach (KeyValuePair<string, ViewComponentSectionRendereDelegate> pair in sectionHandlers)
					viewComponentContext.RegisterSection(pair.Key, pair.Value);
			ViewComponent viewComponent =
				((IViewComponentFactory) Context.GetService(typeof (IViewComponentFactory))).Create(componentName);
			viewComponent.Init(Context, viewComponentContext);
			viewComponent.Render();
			if (viewComponentContext.ViewToRender != null)
				OutputSubView("\\" + viewComponentContext.ViewToRender, viewComponentContext.ContextVars);
		}

		/// <summary>
		/// Output a string to the current output writer, Html Encoded
		/// </summary>
		/// <remarks>
		/// This method is supposed to be used internally. One should be aware
		/// that this method does not html-encode the string, ans use it wisely
		/// </remarks>
		/// <param name="markup">Message to output</param>
		protected void Output(string markup)
		{
			OutputWriter.Write(markup);
		}

		/// <summary>
		/// Output an object's 'ToString()' to the current output writer, Html Encoded
		/// </summary>
		/// <remarks>
		/// This method is supposed to be used internally. One should be aware
		/// that this method does not html-encode the string, ans use it wisely
		/// </remarks>
		/// <param name="markup">object to output</param>
		protected void Output(object markup)
		{
			OutputWriter.Write(markup);
		}

		/// <summary>
		/// Output a fragment to the current output writer, Html Encoded
		/// </summary>
		/// <param name="fragment">string to output</param>
		protected void OutputEncoded(string fragment)
		{
			if (string.IsNullOrEmpty(fragment))
				return;
			OutputWriter.Write(HttpUtility.HtmlEncode(fragment).Replace("'", "&#39;"));
		}

		/// <summary>
		/// Output an object's ToString() to the current output writer, Html Encoded
		/// </summary>
		/// <param name="fragment">object to output</param>
		protected void OutputEncoded(object fragment)
		{
			if (fragment == null)
				return;
			OutputEncoded(fragment.ToString());
		}

		/// <summary>
		/// Creates a MonoRailDictionary with a single entry
		/// </summary>
		/// <param name="name">The entry's name</param>
		/// <param name="value">The entry's value</param>
		/// <returns>The new dictionary</returns>
		protected ParametersDictionary N(string name, object value)
		{
			return new ParametersDictionary().N(name, value);
		}

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		protected void OutputSubView(string subViewName)
		{
			OutputSubView(subViewName, outputWriter, new ParametersDictionary());
		}

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
		protected void OutputSubView(string subViewName, IDictionary parameters)
		{
			OutputSubView(subViewName, outputWriter, parameters);
		}

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
		/// <param name="writer">The writer that will be used for the sub view's output</param>
		protected void OutputSubView(string subViewName, TextWriter writer, IDictionary parameters)
		{
			AspViewBase subView = viewEngine.GetView(GetRootedSubViewTemplate(subViewName), writer, context, controller, controllerContext);
			if (parameters != null)
				foreach (string key in parameters.Keys)
					if (parameters[key] != null)
						subView.Properties[key] = parameters[key];
			subView.Render();
			GatherBubblingPropertiesFrom(subView);
		}

		/// <summary>
		/// Signaling the current view to start bufferring the following writes, filtering it later
		/// </summary>
		/// <param name="filterName">The filter's type name to apply</param>
		protected void StartFiltering(string filterName)
		{
			Type filterType = GetFilterType(filterName);
			IViewFilter filter = (IViewFilter) Activator.CreateInstance(filterType);
			StartFiltering(filter);
		}

		/// <summary>
		/// Signaling the current view to start bufferring the following writes, filtering it later
		/// </summary>
		/// <param name="filter">The filter to apply</param>
		protected void StartFiltering(IViewFilter filter)
		{
			outputWriters.Push(outputWriter);
			outputWriter = new StringWriter();
			viewFilters.Push(filter);
		}

		/// <summary>
		/// Actually looking in the property containers for a parameter's value given it's name
		/// </summary>
		/// <param name="parameterName">The parameter's name</param>
		/// <param name="parameter">The parameter's value</param>
		/// <param name="defaultValue">The value to use if <paramref name="parameterName"/> wasn't found in the controller's properties</param>
		/// <returns>True if the property is found, False elsewhere</returns>
		protected bool TryGetParameter(string parameterName, out object parameter, object defaultValue)
		{
			if (properties.Contains(parameterName))
			{
				parameter = properties[parameterName];
				return true;
			}

			parameter = defaultValue;
			return false;
		}

		/// <summary>
		/// Sets a view's parent view. Used in layouts
		/// </summary>
		/// <param name="view">The view's parent</param>
		internal void SetParent(AspViewBase view)
		{
			parentView = view;
		}

		private string GetContentViewContent()
		{
			StringWriter contentWriter = new StringWriter();
			string rendered;
			using (contentView.SetDisposeableOutputWriter(contentWriter))
			{
				contentView.Process();
				rendered = contentWriter.GetStringBuilder().ToString();
			}
			GatherBubblingPropertiesFrom(contentView);
			return rendered;
		}

		/// <summary>
		/// Gets a qualified template name
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <returns>Relative or absolute path and filename to the sub view</returns>
		private string GetRootedSubViewTemplate(string subViewName)
		{
			if (subViewName[0] == '/' || subViewName[0] == '\\')
				return subViewName;

            return Path.Combine(ViewDirectory, subViewName);
		}

		private void InitProperties()
		{
			properties = new ParametersDictionary();
			if (context != null)
			{
				properties.Add("context", context);
				properties.Add("request", context.Request);
				properties.Add("response", context.Response);
				properties.Add("session", context.Session);
			}
			properties.Add("controller", controller);
			if (controllerContext.Resources != null)
				foreach (string key in controllerContext.Resources.Keys)
					if (key != null)
						properties[key] = controllerContext.Resources[key];
			if (controllerContext.Helpers != null)
				foreach (string key in controllerContext.Helpers.Keys)
					if (key != null)
						properties[key] = controllerContext.Helpers[key];
			if (context != null && context.Request.Params != null)
				foreach (string key in context.Request.Params.Keys)
					if (key != null)
						properties[key] = context.Request.Params[key];
			if (context != null && context.Flash != null)
				foreach (DictionaryEntry entry in context.Flash)
					properties[entry.Key.ToString()] = entry.Value;
			if (controllerContext.PropertyBag != null)
				foreach (DictionaryEntry entry in controllerContext.PropertyBag)
					properties[entry.Key.ToString()] = entry.Value;
			if (context != null)
			{
				properties["siteRoot"] = context.ApplicationPath ?? string.Empty;
				properties["fullSiteRoot"] = context.Request.Uri != null
												?
													context.Request.Uri.GetLeftPart(UriPartial.Authority) + context.ApplicationPath
												:
													string.Empty;
			}
		}

		/// <summary>
		/// Gets the output writer for the current view rendering
		/// </summary>
		public TextWriter OutputWriter
		{
			get { return outputWriter; }
		}

		/// <summary>
		/// Used only in layouts. Gets the view contents
		/// </summary>
		public string ViewContents
		{
			get { return viewContents; }
		}

		/// <summary>
		/// Gets the properties container. Based on current property containers that was sent from the controller, such us PropertyBag, Flash, etc.
		/// </summary>
		public IDictionary Properties
		{
			get { return properties; }
		}

		/// <summary>
		/// Gets the current Rails context
		/// </summary>
		public IEngineContext Context
		{
			get { return context; }
		}

		/// <summary>
		/// Gets the calling controller
		/// </summary>
		public IController Controller
		{
			get { return controller; }
		}

		/// <summary>
		/// Gets the view engine instance
		/// </summary>
		public IViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		/// <summary>
		/// Gets a reference to the view's parent view
		/// </summary>
		public IViewBase ParentView
		{
			get { return parentView; }
		}

		public virtual void Initialize(AspViewEngine newViewEngine, TextWriter output, IEngineContext newContext,
		                               IController newController, IControllerContext newControllerContext)
		{
			if (initialized)
				throw new ApplicationException("Sorry, but a view instance cannot be initialized twice");
			initialized = true;
			viewEngine = newViewEngine;
			outputWriter = output;
			context = newContext;
			controller = newController;
			controllerContext = newControllerContext;
			InitProperties();
			dictionaryAdapterFactory = new DictionaryAdapterFactory();
			outputWriters = new Stack<TextWriter>();
			viewFilters = new Stack<IViewFilter>();
		}

		public void Process()
		{
			if (HasContentView && viewContents == null)
				viewContents = GetContentViewContent();

			Render();
		}

		/// <summary>
		/// When overriden in a concrete view class, renders the view content to the output writer
		/// </summary>
		public abstract void Render();

		/// <summary>
		/// This is required because we may want to replace the output stream and get the correct
		/// behavior from components call RenderText() or RenderSection()
		/// </summary>	
		IDisposable IViewBaseInternal.SetDisposeableOutputWriter(TextWriter newWriter)
		{
			ReturnOutputStreamToInitialWriter disposable = new ReturnOutputStreamToInitialWriter(OutputWriter, this);
			outputWriter = newWriter;
			return disposable;
		}

		void IViewBaseInternal.SetOutputWriter(TextWriter newWriter)
		{
			outputWriter = newWriter;
		}

		void IViewBaseInternal.SetContent(string content)
		{
			viewContents = content;
		}

		IViewBaseInternal IViewBaseInternal.ContentView
		{
			get { return contentView; }
			set { contentView = value; }
		}

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		void IViewBaseInternal.OutputSubView(string subViewName)
		{
			OutputSubView(subViewName, new Dictionary<string, object>());
		}

		/// <summary>
		/// Searching a view filter given it's type's name
		/// </summary>
		/// <param name="filterName">the filter's typeName</param>
		/// <returns>System.Type of the filter</returns>
		private static Type GetFilterType(string filterName)
		{
			Type filterType = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name.Equals(filterName, StringComparison.CurrentCultureIgnoreCase) &&
					    type.GetInterface("IViewFilter") != null)
					{
						filterType = type;
						break;
					}
				}
			}
			if (filterType == null)
				throw new AspViewException("Cannot find a viewfilter [{0}]", filterName);
			return filterType;
		}

		private class ReturnOutputStreamToInitialWriter : IDisposable
		{
			private readonly TextWriter initialWriter;
			private readonly IViewBaseInternal view;

			public ReturnOutputStreamToInitialWriter(TextWriter initialWriter, IViewBaseInternal view)
			{
				this.initialWriter = initialWriter;
				this.view = view;
			}

			public void Dispose()
			{
				view.SetOutputWriter(initialWriter);
			}
		}
	}
}
