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

using Castle.Model;
using Castle.MVC.Configuration;
using Castle.MVC.Navigation;
using Castle.MVC.States;

#endregion 

namespace Castle.MVC.Controllers
{
	/// <summary>
	/// Abstract base class to control the navigation between views.
	/// You must inherit from this class when developing yours controllers.
	/// </summary>
	public abstract class Controller : IController
	{

		#region Fields
		private INavigator _navigator = null;
		private string _nextViewToDisplay = null;
		#endregion 

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Controller()
		{
		}
		#endregion 

		#region IController Members


		/// <summary>
		/// The next view to display.
		/// </summary>
		public string NextView
		{
			get { return _nextViewToDisplay; }
			set {  _nextViewToDisplay = value; }
		}

		/// <summary>
		/// Get the user process state.
		/// </summary>
		public IState State
		{
			get { return _navigator.CurrentState; }
		}

		/// <summary>
		/// The navigator coordinates the interactions of views and controllers 
		/// </summary>
		public INavigator Navigator
		{
			get { return _navigator; }
			set { _navigator = value;}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Calls the Navigate method on the appropriate navigator
		/// and navigate to the next view according to the config file.
		/// </summary>
		public void Navigate()
		{
 			this.State.PreviousView = this.State.CurrentView;
			this.State.CurrentView = ConfigUtil.Settings.GetNextView(this.State.CurrentView, this.State.Command);
			_navigator.Navigate();
		}

		/// <summary>
		/// Calls the Navigate method on the appropriate navigator
		/// </summary>
		/// <param name="view">the Next View To Display</param>
		public void Navigate(string view)
		{
			this.State.PreviousView = this.State.CurrentView;
			this.State.CurrentView = view;
			_navigator.Navigate();
		}
		#endregion 

	}
}
