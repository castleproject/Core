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

using System;
using System.Web.UI.WebControls;

using Castle.MVC.States;
using Castle.MVC.Controllers;

namespace Castle.MVC.Views
{
	/// <summary>
	/// Description résumée de WebUserControlView.
	/// </summary>
	public class WebUserControlView : System.Web.UI.UserControl
	{

		/// <summary>
		/// Gets the user process state.
		/// </summary>
		public IState State
		{
			get
			{
				return (this.Page as WebFormView).State;
			}
		}

		/// <summary>
		/// Gets access to the controller.
		/// </summary>
		public IController ControllerBase
		{
			get
			{
				return (this.Page as WebFormView).ControllerBase;
			}
		}
	}
}
