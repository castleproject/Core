// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Web;

	/// <summary>
	/// Base class for UI Components
	/// </summary>
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

		#region "Internal" core methods

		/// <summary>
		/// Invoked by the framework.
		/// </summary>
		/// <param name="railsContext"></param>
		/// <param name="context"></param>
		public void Init(IRailsEngineContext railsContext, IViewComponentContext context)
		{
			this.railsContext = railsContext;
			this.context = context;

			Initialize();
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

		/// <summary>
		/// Implementor should return true only if the 
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns><see langword="true"/> if section is supported</returns>
		public virtual bool SupportsSection(string name)
		{
			return false;
		}
	}
}
