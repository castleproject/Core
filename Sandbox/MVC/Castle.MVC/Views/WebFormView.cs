
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
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Castle.MVC.Configuration;
using Castle.MVC.Controllers;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.Windsor;

#endregion 

namespace Castle.MVC.Views
{
	/// <summary>
	/// Base class for user interaction in Web application. You can inherit from
	/// this class when developing your Web forms.
	/// </summary>
	public class WebFormView : Page, IView
	{

		#region Fields

		private IState _state = null;

		#endregion 

		#region Constructor

		/// <summary>
		/// Default cosntructor
		/// </summary>
		public WebFormView():base()
		{
			this.Init += new EventHandler(WebFormView_Init);
			this.Load += new EventHandler(WebFormView_Load);
		}
		#endregion 

		#region IView members

		/// <summary>
		/// Gets the current view name.
		/// </summary>
		public string View
		{
			get { return _state.CurrentView; }
		}

		/// <summary>
		/// Gets access to the user process state.
		/// </summary>
		public IState State
		{
			get { return _state; }
		}		
		#endregion

		#region Methods

		private void WebFormView_Init(object sender, EventArgs e)
		{
			IWindsorContainer container = ContainerWebAccessorUtil.ObtainContainer();

			// Get the State
			IStatePersister statePersister = (IStatePersister) container[typeof(IStatePersister)];
			_state = statePersister.Load();
			// Acquire current view
			_state.CurrentView = ConfigUtil.Settings.GetView(this.Request.Path);
			_state.Save();

			ControllerTree tree = (ControllerTree) container["mvc.controllerTree"];
			PropertyControllerCollection propertiesController = tree.GetControllers( this.GetType().BaseType );

			if (propertiesController!=null)
			{
				for(int i=0; i<propertiesController.Count; i++)
				{
					IController controller = container[propertiesController[i].ControllerType ] as IController;
					propertiesController[i].PropertyInfo.SetValue(this, controller, null);
				}				
			}
		}

		private void WebFormView_Load(object sender, EventArgs e)
		{
			// Try to set the command name on the state object
			Control control = null;
			bool findControl = false;
			foreach(string controllName in this.Request.Form)
			{
				control = this.FindControl(controllName);
				if (control is IPostBackEventHandler)
				{
					findControl =true;
					break;
				}
			}
			// Another try
			if (!findControl && this.Request.Form["__EVENTTARGET"]!=null)
			{
					control = this.FindControl(this.Request.Form["__EVENTTARGET"]);
			}

			// The Control.ViewState property is associated with each server control 
			// in your web form 
			// The commandName is in the control.ViewState["CommandName"]
			// wich is protected :-(
			if (control!=null)
			{
				PropertyInfo propertyInfo = typeof(Control).GetProperty("ViewState",BindingFlags.Instance 
					| BindingFlags.IgnoreReturn 
					| BindingFlags.Public 
					| BindingFlags.NonPublic); 
				StateBag statebag = propertyInfo.GetValue(control,null) as StateBag;
						
				_state.Command = statebag["CommandName"] as string;				
			}
			else
			{
				_state.Command = string.Empty;	
			}
			_state.Save();
		}


		/// <summary>
		/// Set the focus on a control
		/// </summary>
		/// <param name="clientID">The client id of the control.</param>
		public void SetFocus(string clientID ) 
		{
			StringBuilder stringBuilder = new StringBuilder();
		
			stringBuilder.Append("<script language='javascript'>"); 
			stringBuilder.Append(" document.getElementById('" + clientID + "').focus()");
			stringBuilder.Append("</script>");
			this.RegisterStartupScript("Onload", stringBuilder.ToString());			
		}

		/// <summary>
		/// Set the focus on a control
		/// </summary>
		/// <param name="control">The control</param>
		public void SetFocus(WebControl control ) 
		{
			StringBuilder stringBuilder = new StringBuilder();
		
			stringBuilder.Append("<script language='javascript'>"); 
			stringBuilder.Append(" document.getElementById('" + control.ClientID + "').focus()");
			stringBuilder.Append("</script>");
			this.RegisterStartupScript("Onload", stringBuilder.ToString());			
		}

		/// <summary>
		/// ToString Override
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _state.GetType().FullName + ":" + _state.CurrentView;
		}
		#endregion 

	}
}
