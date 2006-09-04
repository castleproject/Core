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
using System.Reflection;
using System.Windows.Forms;
using Castle.MVC.Configuration;
using Castle.MVC.Controllers;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.Windsor;

namespace Castle.MVC.Views
{
	/// <summary>
	/// Base class for user interaction in windows application. You can inherit from
	/// this class when developing your windows forms.
	/// </summary>
	public class WinFormView : Form, IView 
	{
		private IState _state = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public WinFormView()
		{
			 this.Load += new EventHandler(WinFormViewOnLoad);
		}

		private void WinFormViewOnLoad( object source, EventArgs e )
		{
			//To Do
		}

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
	}
}
