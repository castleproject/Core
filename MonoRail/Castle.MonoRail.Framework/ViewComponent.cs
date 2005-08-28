// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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


	public abstract class ViewComponent
	{
		private IViewComponentContext context;
		private IRailsEngineContext railsContext;

		#region "Internal" core methods

		public void Init(IRailsEngineContext railsContext, IViewComponentContext context)
		{
			this.railsContext = railsContext;
			this.context = context;

			Initialize();
		}

		#endregion

		#region Lifecycle methods (overridables)

		public virtual void Initialize()
		{
			
		}

		public virtual void Render()
		{
			RenderView("default");
		}

		#endregion

		#region Usefull properties

		public IViewComponentContext Context
		{
			get { return context; }
		}

		protected IRailsEngineContext RailsContext
		{
			get { return railsContext; }
		}

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
		protected IDictionary Flash
		{
			get { return railsContext.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return railsContext.UnderlyingContext as HttpContext; }
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
		/// Shortcut to Request.Params
		/// </summary>
		protected NameValueCollection Params
		{
			get { return Request.Params; }
		}

		#endregion

		#region Usefull operations

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
	}
}
