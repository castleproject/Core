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

using Castle.MVC.StatePersister;

namespace Castle.MVC.States
{
	/// <summary>
	/// Maintains user process state.
	/// </summary>
	public interface IState 
	{

		/// <summary>
		/// A state persister provider.
		/// </summary>
		IStatePersister StatePersister { get; set; }

		/// <summary>
		/// Gets or sets an element saved on the state with the specified key.
		/// </summary>
		object this[ string key ]{ get; set; }

		/// <summary>
		/// Gets or sets the command id value. This value determines 
		/// which view is the next view in the mapping graph.
		/// </summary>
		string Command { get; set; }

		/// <summary>
		/// Gets or sets the current view.
		/// </summary>
		string CurrentView { get; set; }

		/// <summary>
		/// Gets or sets the previous view.
		/// </summary>
		string PreviousView { get; set; }

		/// <summary>
		/// Provides access to a dictionary of volative items.
		/// </summary>
		IDictionary Items { get; }

		/// <summary>
		/// Reset volatile items and command.
		/// </summary>
		void Reset();

		/// <summary>
		/// Save the state
		/// </summary>
		void Save();
	}
}
