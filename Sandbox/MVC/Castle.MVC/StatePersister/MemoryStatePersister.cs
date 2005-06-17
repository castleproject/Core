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
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;

using Castle.MVC.States;
#endregion 

namespace Castle.MVC.StatePersister
{
	/// <summary>
	/// This class provides simple memory-based state persister for Windows Forms applications. 
	/// It is a singleton build by Castle IOC framework.
	/// </summary>
	public class MemoryStatePersister : IStatePersister
	{

		#region Fields

		private IState _state = null;
		private IStateFactory _stateFactory = null;

		#endregion 

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public MemoryStatePersister()
		{
		}
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
		/// Saves the State object in memory.
		/// </summary>
		/// <param name="state">A valid State object.</param>
		public void Save(IState state)
		{
			_state = state;
		}

		/// <summary>
		/// Loads the saved state.
		/// </summary>
		/// <returns>The saved state</returns>
		public IState Load()
		{
			if (_state==null)
			{
				_state = _stateFactory.Create();
			}
			return _state;	
		}

		/// <summary>
		/// Release a state
		/// </summary>
		/// <param name="state">The state to release.</param>
		public void Release(IState state)
		{
			_stateFactory.Release(state);
			_state = null;
		}
		#endregion
	}
}
