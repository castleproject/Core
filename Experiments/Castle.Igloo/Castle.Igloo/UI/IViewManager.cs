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

using Castle.Igloo.Navigation;

namespace Castle.Igloo.UI
{
	/// <summary>
	/// Represents a view manager. 
	/// </summary>
	public interface IViewManager
	{

        /// <summary>
        /// Gets the next view.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>The view id</returns>
	    string GetNextView(NavigationContext navigationContext);
	    
        /// <summary>
        /// Activates a specified view.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        void ActivateView(INavigator navigator);
	}
}
