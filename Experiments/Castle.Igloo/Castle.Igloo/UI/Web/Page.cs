
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

using System;
using Castle.Igloo.Attributes;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.UI.Web
{
	/// <summary>
    /// Serves as the base class for controls that represent a page in Web application. 
	/// You must inherit from this class when developing your Web forms.
    /// It gives you access to the current navigation context.
    /// </summary>
    [Scope(Scope=ScopeType.Request)]
    public class Page : System.Web.UI.Page, IView
	{
	    private NavigationState _navigationState = null;
        private FlashMessages _messages = null;

		#region Constructor

		/// <summary>
		/// Default cosntructor
		/// </summary>
		public Page():base()
		{
            PreInit += new EventHandler(CommonWebUI.WebUI_InjectComponent);
            PreLoad += new EventHandler(CommonWebUI.WebUI_RetrieveAction);
		}
		#endregion 
	    
		#region IView members

        /// <summary>
        /// Gets the current NavigationContext.
        /// </summary>
        public NavigationState NavigationState
        {
            get { return _navigationState; }
            set { _navigationState = value; }
        }

        /// <summary>
        /// Holds context request flashMessages.
        /// </summary>
        /// <value></value>
        [Inject(Name = FlashMessages.FLASH_MESSAGES, Scope = ScopeType.Request, Create = true)]
        public FlashMessages FlashMessages
        {
            get { return _messages; }
            set { _messages = value; }
        }
	    	
		#endregion

	}
}
