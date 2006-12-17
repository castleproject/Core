#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
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

using System.Web;
using Castle.Igloo.Configuration;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.UI.Web
{
	/// <summary>
	/// Represent a web ViewManager to use in conjuntion with an application section configuration.
	/// </summary>
	public class XmlWebViewManager : IViewManager
	{   
		#region IViewManager members

	            /// <summary>
        /// Gets the next view.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>The view id</returns>
	    public string GetNextView(NavigationContext navigationContext)
        {
            return ConfigUtil.Settings.GetNextView(navigationContext.CurrentView, navigationContext.Action);
        }
	    
		/// <summary>
		/// Activates the specified view.
		/// </summary>
        public void ActivateView(INavigator navigator)
		{
            HttpContext.Current.Response.Redirect(ConfigUtil.Settings.GetUrl(navigator.NavigationContext.NextView), false);
		}

		#endregion
	}
}
