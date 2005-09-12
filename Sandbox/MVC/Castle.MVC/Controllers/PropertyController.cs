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
using System.Reflection;
#endregion 



namespace Castle.MVC.Controllers
{
	/// <summary>
	/// PropertyController.
	/// </summary>
	public class PropertyController
	{
		private PropertyInfo _propertyInfo = null;
		private Type _controllerType = null;

		/// <summary>
		/// 
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get {return _propertyInfo;}	
		}

		/// <summary>
		/// 
		/// </summary>
		public Type ControllerType
		{
			get {return _controllerType;}	
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="controllerType"></param>
		public PropertyController(PropertyInfo info, Type controllerType)
		{
			_propertyInfo = info;
			_controllerType= controllerType;
		}
	}
}
