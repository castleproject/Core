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

	/// <summary>
	/// Summary description for Controller.
	/// </summary>
	public abstract class Controller
	{
		private IViewEngine _viewEngine;
		private IRailsEngineContext _context;
		private String _areaName;
		private String _controllerName;
		private String _selectedViewName;
		private IDictionary _bag;

		public Controller()
		{
			_bag = new HybridDictionary();
		}

		public String Name
		{
			get { return _controllerName; }
		}

		protected void RenderView( String name )
		{
			String basePath = _controllerName;

			if (_areaName != null)
			{
				basePath = Path.Combine( _areaName, _controllerName );
			}

			_selectedViewName = Path.Combine( basePath, name );
		}

		protected void RenderView( String controller, String name )
		{
			_selectedViewName = Path.Combine( controller, name );
		}

		protected void Redirect( String controller, String action )
		{
			// Cancel the view processing
			_selectedViewName = null;
			_context.Response.Redirect( 
				String.Format("../{0}/{1}.rails", controller, action), true );
		}

		public void Process( IRailsEngineContext context, 
			String areaName, String controllerName, String actionName, IViewEngine viewEngine )
		{
			_areaName = areaName;
			_controllerName = controllerName;
			_viewEngine = viewEngine;
			_context = context;

			Send( actionName );
		}

		protected virtual void InvokeMethod(MethodInfo method, IRequest request)
		{
			method.Invoke( this, new object[0] );
		}

		protected virtual MethodInfo SelectMethod(String action, IRequest request)
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

		public IDictionary PropertyBag
		{
			get { return _bag; }
		}

		protected HttpContext Context
		{
			get { return _context.UnderlyingContext as HttpContext; }
		}

		public virtual void Send( String action )
		{
			// Specifies the default view for this area/controller/action
			RenderView( action );

			MethodInfo method = SelectMethod(action, _context.Request);

			InvokeMethod(method);

			ProcessView();
		}

		private void InvokeMethod(MethodInfo method)
		{
			try
			{
				InvokeMethod(method, _context.Request);
			}
			catch(Exception ex)
			{
				// TODO: Implement a rescue feature
				throw ex;
			}
		}

		private void ProcessView()
		{
			if (_selectedViewName != null)
			{
				_viewEngine.Process( _context, this, _selectedViewName );
			}
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
