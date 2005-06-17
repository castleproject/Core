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
using System.Configuration;

using Castle.Model;
using Castle.MVC;
using Castle.MVC.Configuration;
using Castle.MVC.Views;
using Castle.MVC.States;
using Castle.MVC.StatePersister;

namespace Castle.MVC.Navigation
{
	/// <summary>
	/// Description résumée de WebNavigator.
	/// </summary>
	[Transient]
	public class DefaultNavigator : INavigator
	{

		#region Fields

		private IState _state = null;
		private IViewManager _viewManager = null;
		private IStatePersister _statePersister = null;
		#endregion 

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewManager">A view manager</param>
		/// <param name="statePersister">A state persister</param>
		public DefaultNavigator(IViewManager viewManager,IStatePersister statePersister)
		{
			_viewManager = viewManager;
			_statePersister = statePersister;
			_state = _statePersister.Load();
		}
		#endregion 

		#region INavigator members

		/// <summary>
		///  The current state.
		/// </summary>
		public IState CurrentState
		{
			get { return _state; }
		}

		/// <summary>
		/// Natvigate to the next view.
		/// </summary>
		public void Navigate()
		{
			_statePersister.Save(_state);
			_viewManager.ActivateView(this);
		}

		#endregion
	}
}
