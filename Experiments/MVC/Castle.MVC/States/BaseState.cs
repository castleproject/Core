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
using Castle.Core;
using Castle.MVC.StatePersister;

#endregion 

namespace Castle.MVC.States
{
	/// <summary>
	/// Represent the base State.
	/// You could inherit from this class when developing your state.
	/// </summary>
	[Serializable]
	[Transient]
	public abstract class BaseState : DictionaryBase, IState
	{

		/// <summary>
		/// Key used to retrieve the session.
		/// </summary>
		public const string SESSION_KEY = "_MVC_SESSION_STATE_";

		#region Fields

		private string _command = string.Empty;
		[NonSerialized]
		private HybridDictionary _items = new HybridDictionary();
		private string _currentView = string.Empty;
		private string _previousView = string.Empty;
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
			get{ return _previousView; }
			set{_previousView = value;}
		}

		/// <summary>
		/// Provides access to a dictionary of volative items.
		/// </summary>
		public IDictionary Items
		{
			get{ return _items; }
		}

		#endregion 

		#region Methods

		/// <summary>
		/// Gets or sets an element saved on the state with the specified key.
		/// </summary>
		public object this[ string key ]
		{
			set{this.Dictionary[ key ] = value;}
			get{ return this.Dictionary[ key ]; }
		}

		/// <summary>
		/// Reset state.
		/// </summary>
		public void Reset() 
		{
			_items.Clear();
		}

		/// <summary>
		/// Save the state
		/// </summary>
		public void Save()
		{
			_statePersister.Save(this);
		}
		#endregion 


	}
}
