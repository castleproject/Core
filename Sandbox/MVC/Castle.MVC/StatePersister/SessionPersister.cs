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
using System.Web;
using System.Collections;

using System.Security.Principal;

using Castle.Model; // Transient attribute
using Castle.MVC.States;


namespace Castle.MVC.StatePersister
{
	/// <summary>
	/// This class provides simple session-based state persister for Web applications. 
	/// </summary>
	public class SessionPersister : IStatePersister
	{

		private IStateFactory _stateFactory = null;

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public SessionPersister()
		{}
		#endregion 

		#region IStatePersister Members

		/// <summary>
		/// State factory
		/// </summary>
		public IStateFactory StateFactory
		{
			get{ return _stateFactory;}
			set{ _stateFactory = value; }
		}

		/// <summary>
		/// Saves state to the Session object.
		/// </summary>
		/// <param name="state">The state to save.</param>
		public void Save(IState state)
		{
			//  put State object directly into Session
			HttpContext.Current.Session[ BaseState.SESSION_KEY ] = state;
		}

		/// <summary>
		/// Loads the saved state.
		/// </summary>
		/// <returns>The saved state</returns>
		public IState Load()
		{
			//  pull State object directly out of Session
			IState state = HttpContext.Current.Session[ BaseState.SESSION_KEY ] as IState;
			if (state==null)
			{
				state = _stateFactory.Create();
				HttpContext.Current.Session[ BaseState.SESSION_KEY ] = state;
			}
			state.Reset();
			return state;
		}
		#endregion
	}
}
