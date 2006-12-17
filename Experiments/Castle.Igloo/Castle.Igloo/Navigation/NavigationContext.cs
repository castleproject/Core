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
using Castle.Igloo.Attributes;
using Castle.Igloo.Configuration;

namespace Castle.Igloo.Navigation
{
    /// <summary>
    /// Holds all the data to do the navigation
    /// </summary>
    [Scope(Scope = ScopeType.Request)]
    public sealed class NavigationContext
    {
        public const string NO_NAVIGATION = "\002NO_NAVIGATION\002";
        public const string NAVIGATION_CONTEXT = "_NAVIGATION_CONTEXT_";

        private string _currentView = string.Empty;
        private string _previousView = string.Empty;
        private string _nextView = string.Empty;
        private string _action = NO_NAVIGATION;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class.
        /// </summary>
        public NavigationContext()
        {
            HttpContext currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                _currentView = ConfigUtil.Settings.GetView(currentContext.Request.Path);
                if (currentContext.Request.UrlReferrer != null)
                {
                    _previousView = ConfigUtil.Settings.GetView(currentContext.Request.UrlReferrer.LocalPath);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the action value. This value determines 
        /// which view is the next view in the navigation graph.
        /// </summary>
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        /// <summary>
        /// Gets or sets the current view name.
        /// </summary>
        public string CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }

        /// <summary>
        /// Gets or sets the previous view.
        /// </summary>
        public string PreviousView
        {
            get { return _previousView; }
            set { _previousView = value; }
        }

        /// <summary>
        /// Gets or sets the next view.
        /// </summary>
        public string NextView
        {
            get { return _nextView; }
            set { _nextView = value; }
        }
    }
}
