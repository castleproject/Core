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

using Castle.MVC.Controllers;
using Castle.MVC.States;

namespace Castle.MVC.Views
{
	/// <summary>
	///  Represents a view used in Web or Windows applications.
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// Provides access to the current view controller.
		/// </summary>
		IController ControllerBase { get; }

		/// <summary>
		/// Gets the view id.
		/// </summary>
		string View { get; }

		/// <summary>
		/// Provides access to the associated state .
		/// </summary>
		IState State{get;}
	}
}
