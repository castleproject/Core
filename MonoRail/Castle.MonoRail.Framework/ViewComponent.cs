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
	using System.Collections.Specialized;
	using System.IO;
	using System.Collections;
	using System.Reflection;
	using System.Web;
	using Castle.Components.Binder;

	/// <summary>
	/// Base class for reusable UI Components. 
	/// <para>
	/// Implementors should override <see cref="ViewComponent.Initialize"/>
	/// for implement proper initialization (if necessary). 
	/// Also implement <see cref="ViewComponent.Render"/> as by default it 
	/// will render a <c>default</c> view on <c>[ViewFolderRoot]/components/[componentname]</c>.
	/// </para>
	/// <para>
	/// You can also override <see cref="ViewComponent.SupportsSection"/> if your component supports 
	/// neste sections (ie templates provided on the view that uses the view component.
	/// </para>
	/// </summary>
	/// <example>
	/// A very simplist view component that renders the time.
	/// <code>
	/// public class ShowTime : ViewComponent
	/// {
	///		public override void Initialize()
	///		{
	///		}
	/// 
	///		public override void Render()
	///		{
	///			RenderText("Time: " + DateTime.Now.ToString());
	///		}
	/// }
	/// </code>
	/// <para>
	/// This can be used from the view using the following syntax (NVelocity view engine)
	/// </para>
	/// <code>
	/// #component(ShowTime)
	/// </code>
	/// </example>
	public abstract class ViewComponent
	{
		/// <summary>
		/// Holds the component context
		/// </summary>
		private IViewComponentContext context;

		/// <summary>
		/// Holds the <see cref="IRailsEngineContext"/> associated
		/// to the request lifetime.
		/// </summary>
		private IRailsEngineContext railsContext;

		private string[] sectionsFromAttribute;

		#region "Internal" core methods

		/// <summary>
		/// Invoked by the framework.
		/// </summary>
		/// <param name="engineContext">Request context</param>
		/// <param name="componentContext">ViewComponent context</param>
		public void Init(IRailsEngineContext engineContext, IViewComponentContext componentContext)
		{
			railsContext = engineContext;
			context = componentContext;

			BindComponentParameters();

			Initialize();
		}

		/// <summary>
		/// Binds the component parameters.
		/// </summary>
		private void BindComponentParameters()
		{
			IConverter converter = new DefaultConverter();

			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		
			foreach(PropertyInfo property in properties)
			{
				if (!property.CanWrite) continue;

				object[] attributes = property.GetCustomAttributes(typeof(ViewComponentParamAttribute), true);
			
				if (attributes.Length == 1)
				{
					BindParameter((ViewComponentParamAttribute) attributes[0], property, converter);
				}
			}
		}

		private void BindParameter(ViewComponentParamAttribute paramAtt, PropertyInfo property, IConverter converter)
		{
			string compParamKey = string.IsNullOrEmpty(paramAtt.ParamName) ? property.Name : paramAtt.ParamName;

			object value = ComponentParams[compParamKey];

			if (value == null)
			{
				if (paramAtt.Required && 
					(property.PropertyType.IsValueType || property.GetValue(this, null) == null))
				{
					throw new ViewComponentException(string.Format("The parameter '{0}' is required by " +
						"the ViewComponent {1} but was not passed or had a null value", compParamKey, GetType().Name));
				}
			}
			else
			{
				try
				{
					bool succeeded;

					object converted = converter.Convert(property.PropertyType, value.GetType(), value, out succeeded);

					if (succeeded)
					{
						property.SetValue(this, converted, null);
					}
					else
					{
						throw new Exception("Could not convert '" + value + "' to type " + property.PropertyType);
					}
				}
				catch(Exception ex)
				{
					throw new ViewComponentException(string.Format("Error trying to set value for parameter '{0}' " +
						"on ViewComponent {1}: {2}", compParamKey, GetType().Name, ex.Message), ex);
				}
			}
		}

		#endregion

		#region Lifecycle methods (overridables)

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public virtual void Render()
		{
			RenderView("default");
		}

		/// <summary>
		/// Implementor should return true only if the 
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns><see langword="true"/> if section is supported</returns>
		public virtual bool SupportsSection(string name)
		{
			// TODO: We need to cache this

			if (sectionsFromAttribute == null)
			{
				object[] attributes = GetType().GetCustomAttributes(typeof(ViewComponentDetailsAttribute), true);

				if (attributes.Length != 0)
				{
					ViewComponentDetailsAttribute detailsAtt = (ViewComponentDetailsAttribute) attributes[0];

					if (!string.IsNullOrEmpty(detailsAtt.Sections))
					{
						sectionsFromAttribute = detailsAtt.Sections.Split(',');
					}
				}

				if (sectionsFromAttribute == null)
				{
					sectionsFromAttribute = new string[0];
				}
			}

			return Array.Find(sectionsFromAttribute, 
				delegate(string item)
					{ return string.Equals(item, StringComparer.InvariantCultureIgnoreCase); }) != null;
		}

		#endregion

		#region Usefull properties

		/// <summary>
		/// Gets the Component Context
		/// </summary>
		public IViewComponentContext Context
		{
			get { return context; }
		}

		/// <summary>
		/// Gets the <see cref="IRailsEngineContext"/>
		/// associated with the current request
		/// </summary>
		protected IRailsEngineContext RailsContext
		{
			get { return railsContext; }
		}

		/// <summary>
		/// Gets the component parameters
		/// </summary>
		protected IDictionary ComponentParams
		{
			get { return context.ComponentParameters; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return railsContext.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		protected Flash Flash
		{
			get { return railsContext.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return railsContext.UnderlyingContext; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		protected IRequest Request
		{
			get { return railsContext.Request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		protected IResponse Response
		{
			get { return railsContext.Response; }
		}

		/// <summary>
		/// Provides a way to make data available
		/// to the view that the component uses
		/// </summary>
		protected IDictionary PropertyBag
		{
			get { return context.ContextVars; }
		}

		/// <summary>
		/// Shortcut to Request.Params
		/// </summary>
		protected NameValueCollection Params
		{
			get { return Request.Params; }
		}

		#endregion

		#region Useful operations

		/// <summary>
		/// Specifies the view to be processed after the component has finished its processing. 
		/// </summary>
		protected void RenderView(String name)
		{
			context.ViewToRender = Path.Combine(GetBaseViewPath(), name);
		}

		/// <summary>
		/// Specifies the view to be processed after the component has finished its processing. 
		/// </summary>
		protected void RenderView(String component, String name)
		{
			context.ViewToRender = Path.Combine(GetBaseViewPath(component), name);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the component has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		protected void RenderSharedView(String name)
		{
			context.ViewToRender = name;
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		protected void CancelView()
		{
			context.ViewToRender = null;
		}

		protected void RenderText(String content)
		{
			context.Writer.Write(content);
		}

		/// <summary>
		/// Determines whether the current component declaration on the view
		/// has the specified section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns>
		/// <c>true</c> if the specified section exists; otherwise, <c>false</c>.
		/// </returns>
		protected bool HasSection(String sectionName)
		{
			return context.HasSection(sectionName);
		}

		/// <summary>
		/// Renders the component body.
		/// </summary>
		protected void RenderBody()
		{
			context.RenderBody();
		}

		/// <summary>
		/// Renders the body into the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer.</param>
		protected void RenderBody(TextWriter writer)
		{
			context.RenderBody(writer);
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		protected void RenderSection(String sectionName)
		{
			context.RenderSection(sectionName);
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="writer">The writer.</param>
		protected void RenderSection(String sectionName, TextWriter writer)
		{
			context.RenderSection(sectionName, writer);
		}

		#endregion

		#region private helper methods

		private String GetBaseViewPath()
		{
			return GetBaseViewPath(context.ComponentName);
		}

		private String GetBaseViewPath(String componentName)
		{
			return String.Format("components/{0}", componentName);
		}

		#endregion
	}
}
