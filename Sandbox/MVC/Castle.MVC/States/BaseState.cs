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
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Runtime.Serialization;

using Castle.MVC.StatePersister;
using Castle.Model;

namespace Castle.MVC.States
{
	/// <summary>
	/// Summary description for State.
	/// </summary>
	[Serializable]
	public class BaseState : DictionaryBase, IState
	{

		public const string SESSION_KEY = "_MVC_SESSION_STATE_";

		#region Fields

		private string _command = string.Empty;
		[NonSerialized]
		private HybridDictionary _items = new HybridDictionary();
		private string _currentView = string.Empty;
		private string _previousViewas = string.Empty;
		IStatePersister _statePersister = null;
		#endregion 

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public BaseState()
		{
		}
		#endregion 


		/// <summary>
		/// Gets or sets an element saved on the state with the specified key.
		/// </summary>
		public object this[ string key ]
		{
			set{this.Dictionary[ key ] = value;}
			get{ return this.Dictionary[ key ]; }
		}

		#region IState members

		/// <summary>
		/// A state persister provider.
		/// </summary>
		public IStatePersister StatePersister
		{
			get
			{
				return _statePersister;
			}
			set
			{
				_statePersister = value;
			}
		}

		/// <summary>
		/// Gets or sets the command id value. This value determines 
		/// which view is the next view in the navigation graph.
		/// </summary>
		public string Command
		{
			get{ return _command;}
			set{ _command = value; }
		}

		/// <summary>
		/// Gets or sets the current view name.
		/// </summary>
		public string CurrentView
		{
			get{ return _currentView; }
			set{_currentView = value;}
		}

		/// <summary>
		/// Gets or sets the previous view.
		/// </summary>
		public string PreviousView
		{
			get{ return _previousViewas; }
			set{_previousViewas = value;}
		}

		/// <summary>
		/// Provides access to a dictionary of volative items.
		/// </summary>
		public IDictionary Items
		{
			get{ return _items; }
		}

		#endregion 


		/// <summary>
		/// Reset state.
		/// </summary>
		public void Reset() 
		{
			_command = string.Empty;
			_items.Clear();
		}


		/// <summary>
		/// Save the state
		/// </summary>
		public void Save()
		{
			_statePersister.Save(this);
		}
	}
}
