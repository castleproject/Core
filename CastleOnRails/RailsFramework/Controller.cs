// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework
{
	using System;
	using System.IO;
	using System.Web;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.CastleOnRails.Framework.Views;

	/// <summary>
	/// Summary description for Controller.
	/// </summary>
	public abstract class Controller
	{
		private String __controller;
		private String __view;
		private String __url;
		private String __viewPhysicalPath;
		private IViewEngine __viewEngine;
		private HttpContext __context;
		private IDictionary __bag;

		public Controller()
		{
			__bag = new HybridDictionary();
		}

		public String Name
		{
			get { return __controller; }
		}

		public IDictionary Bag
		{
			get { return __bag; }
		}

		protected void RenderView( String name )
		{
			__view = Path.Combine( __controller, name );
		}

		protected void RenderView( String controller, String name )
		{
			__view = Path.Combine( controller, name );
		}

		public virtual void __Process( String url, String viewPath, String controller, String action, 
			IViewEngine viewEngine, HttpContext context )
		{
			__url = url;
			__viewPhysicalPath = viewPath;
			__controller = controller;
			__viewEngine = viewEngine;
			__context = context;

			Send( action );
		}

		protected virtual void InvokeMethod(MethodInfo method, HttpRequest request)
		{
			method.Invoke( this, new object[0] );
		}

		protected virtual MethodInfo SelectMethod(String action, HttpRequest request)
		{
			Type type = this.GetType();

			MethodInfo method = type.GetMethod( action, 
				BindingFlags.IgnoreCase|BindingFlags.Public|BindingFlags.Instance,
				null, CallingConventions.Standard, new Type[0], new ParameterModifier[0]);
	
			if (method == null)
			{
				throw new ControllerException( String.Format("No action for '{0}' found", action) );
			}

			return method;
		}

		protected HttpContext Context
		{
			get { return __context; }
		}

		public virtual void Send( String action )
		{
			// Specifies the default action for this action
			RenderView( action );

			// Try to dispatch the action
			MethodInfo method = SelectMethod(action, Context.Request);

			try
			{
				InvokeMethod(method, Context.Request);
			}
			catch(Exception ex)
			{
				// TODO: If rescue defined...

				throw ex;
			}

			// Send view 
			__viewEngine.Process( this, __url, __viewPhysicalPath, __view, Context );
		}

		public virtual void PreSendView(object view)
		{
			if ( view is IControllerAware )
			{
				(view as IControllerAware).SetController(this);
			}
		}

		public virtual void PostSendView(object view)
		{
		}
	}
}
