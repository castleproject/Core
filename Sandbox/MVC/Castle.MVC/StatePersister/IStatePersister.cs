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

using System.Security.Principal;

using Castle.MVC.States;

namespace Castle.MVC.StatePersister
{
	/// <summary>
	/// This interface defines how State maintains.
	/// </summary>
	public interface IStatePersister
	{

		/// <summary>
		/// State factory
		/// </summary>
		IStateFactory StateFactory { get; set; }

		/// <summary>
		/// Save the state on the persistence medium.
		/// </summary>
		void Save(IState state);

		/// <summary>
		/// Loads the saved state.
		/// </summary>
		/// <returns>The saved state</returns>
		IState Load();


		/// <summary>
		/// Release a state
		/// </summary>
		/// <param name="state">The state to release.</param>
		void Release(IState state);

	}
}
