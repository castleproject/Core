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

using Castle.Igloo.Attributes;
using Castle.Igloo.UI;
using Castle.Igloo.Util;

namespace Castle.Igloo.Navigation
{
	/// <summary>
    /// DefaultNavigator.
	/// </summary>
    [Scope(Scope = ScopeType.Request)]
	public class DefaultNavigator : INavigator
	{
		private IViewManager _viewManager = null;
        private NavigationContext _navigationContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewManager">A view manager</param>
        public DefaultNavigator( IViewManager viewManager )
		{
            AssertUtils.ArgumentNotNull(viewManager, "viewManager");
		    
			_viewManager = viewManager;
		}

		#region INavigator members


        /// <summary>
        /// The current Navigation Context.
        /// </summary>
        /// <value></value>
        [Inject(Name = NavigationContext.NAVIGATION_CONTEXT, Scope = ScopeType.Request)]
        public NavigationContext NavigationContext
        {
            get { return _navigationContext; }
            set { _navigationContext = value; }
        }

		/// <summary>
		/// Natvigate to the next view.
		/// </summary>
		public void Navigate()
		{
            _navigationContext.PreviousView = NavigationContext.CurrentView;

            if (_navigationContext.Action != NavigationContext.NO_NAVIGATION)
            {
                _navigationContext.NextView = _viewManager.GetNextView(NavigationContext);
			    _viewManager.ActivateView(this);                
            }
		    else
		    {
                _navigationContext.NextView = _navigationContext.CurrentView;
		    }
		}

		#endregion

	}
}
