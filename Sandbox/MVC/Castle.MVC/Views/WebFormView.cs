#region Apache Notice
/*****************************************************************************
 * 
 * Castle.MVC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

#region Autors

/************************************************
* Gilles Bayon
*************************************************/
#endregion 

#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Castle.Windsor;
using Castle.MVC.Controllers;
using Castle.MVC.States;
using Castle.MVC.Configuration;
#endregion 

namespace Castle.MVC.Views
{
	/// <summary>
	/// Base class for user interaction in Web application. You can inherit from
	/// this class when developing your Web forms.
	/// </summary>
	public class WebFormView : System.Web.UI.Page, IView
	{

		#region Fields

		private IController _controller = null;

		#endregion 

		#region Constructor

		/// <summary>
		/// Default cosntructor
		/// </summary>
		public WebFormView():base()
		{
			this.Load += new EventHandler(WebFormView_Load);
		}
		#endregion 

		#region IView members

		/// <summary>
		/// Gets the current view name.
		/// </summary>
		public string View
		{
			get
			{
				return _controller.State.CurrentView;
			}
		}

		/// <summary>
		/// Provides access to the controller.
		/// </summary>
		public IController ControllerBase
		{
			get
			{
				return _controller;
			}
		}

		/// <summary>
		/// Gets access to the user process state.
		/// </summary>
		public IState State
		{
			get
			{
				return _controller.State;
			}
		}		
		#endregion

		#region Methods


		private void WebFormView_Load(object sender, System.EventArgs e)
		{
			IWindsorContainer container = ContainerWebAccessorUtil.ObtainContainer();

			// Retrieve the controller name from the attribute
			ControllerAttribute attribute = null;
			object[] attrs = this.GetType().GetCustomAttributes( typeof(ControllerAttribute), true );
			if (attrs.Length > 0)
			{
				attribute = (ControllerAttribute) attrs[0];
				if (attribute != null)
				{
					_controller = container[ attribute.ControllerType ] as IController;
				}
			}
			// Set the command name on the state object
			foreach(string controllName in this.Request.Form)
			{
				Control control = this.FindControl(controllName);
				if (control is IPostBackEventHandler)
				{
					// The commandName is in the control.ViewState["CommandName"]
					// wich is protected !!! Thanks Microsoft :-(
					// so we will use the control id for the command Name
					this.State.Command = control.ID;
				}
			}

			// Acquire current view
			this.State.CurrentView = ConfigUtil.Settings.GetView(this.Request.Path);
		}


		/// <summary>
		/// ToString Override
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this._controller.Navigator.GetType().FullName + ":" + this._controller.State.GetType().FullName + ":" + this._controller.State.CurrentView;
		}
		#endregion 

	}
}
