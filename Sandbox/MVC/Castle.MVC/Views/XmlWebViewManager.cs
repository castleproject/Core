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
using System.Web;

using Castle.MVC.Configuration;
using Castle.MVC.Navigation;
#endregion 

namespace Castle.MVC.Views
{
	/// <summary>
	/// Represent a web ViewManager to use in conjuntion with an application section configuration.
	/// </summary>
	public class XmlWebViewManager : IViewManager
	{

		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public XmlWebViewManager()
		{}
		#endregion 

		#region IViewManager members

		/// <summary>
		/// Activates the specified view.
		/// </summary>
		/// <param name="navigator">A navigator to access the next view id.</param>
		public void ActivateView(INavigator navigator)
		{
			HttpContext.Current.Response.Redirect( ConfigUtil.Settings.GetUrl( navigator.CurrentState.CurrentView ), false );
		}

		#endregion
	}
}
