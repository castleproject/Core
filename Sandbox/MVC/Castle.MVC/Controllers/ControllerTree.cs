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

#region Using

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
#endregion 



namespace Castle.MVC.Controllers
{
	/// <summary>
	/// Description résumée de ControllerTree.
	/// </summary>
	public class ControllerTree
	{
		private IDictionary _pageController = new HybridDictionary();

		/// <summary>
		/// Constructor
		/// </summary>
		public ControllerTree()
		{
		}

		/// <summary>
		/// Add a controller
		/// </summary>
		/// <param name="viewType"></param>
		/// <param name="propertyInfo"></param>
		/// <param name="controllerType"></param>
		public void AddController(Type viewType, PropertyInfo propertyInfo, Type controllerType)
		{
			if (viewType == null) throw new ArgumentNullException("pageType");
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
			if (controllerType == null) throw new ArgumentNullException("controllerType");

			if ( !_pageController.Contains(viewType) )
			{
				_pageController[viewType] = new PropertyControllerCollection();
			}

			PropertyController pair = new PropertyController(propertyInfo, controllerType);
			(_pageController[viewType] as PropertyControllerCollection).Add(pair);
		}

		/// <summary>
		/// Get the list controllers fot this view
		/// </summary>
		/// <param name="viewType"></param>
		/// <returns></returns>
		public PropertyControllerCollection GetControllers(Type viewType)
		{
			return (_pageController[viewType] as PropertyControllerCollection);
		}
	}
}
