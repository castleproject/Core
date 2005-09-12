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

using Castle.MVC.Navigation;
using Castle.MVC.States;

#endregion 

namespace Castle.MVC.Controllers
{

	/// <summary>
	/// Interface controller.
	/// </summary>
	public interface IController
	{

		/// <summary>
		/// Provides access to the next view to display. 
		/// </summary>
		string NextView{get;set;}
		/// <summary>
		/// Provides access to the state associated with this controller.
		/// </summary>
		IState State{get;}
		/// <summary>
		/// Access the navigator which coordinates the interactions of views and controllers. 
		/// </summary>
		INavigator Navigator{get;set;}
	}
}
