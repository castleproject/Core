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
using System.Configuration;

using Castle.MVC.Configuration;
using Castle.MVC.Navigation;
#endregion 

namespace Castle.MVC.Views
{
	/// <summary>
	/// Description résumée de XmlViewManager.
	/// </summary>
	public class XmlWebViewManager : IViewManager
	{

		#region Fields

		private MVCConfigSettings _setting = null;

		#endregion 

		#region Constructor

		public XmlWebViewManager()
		{
			_setting = ConfigurationSettings.GetConfig("castle/mvc") as MVCConfigSettings; 
		}
		#endregion 

		#region IViewManager members

		public void ActivateView(INavigator navigator)
		{
			string url = _setting.GetPath( navigator.CurrentState.CurrentView );

			HttpContext.Current.Response.Redirect( url, false );
		}

		#endregion
	}
}
