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

using System;

namespace Castle.MVC.Controllers
{
	/// <summary>
	/// Decorates a page which a controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ControllerAttribute : Attribute
	{

		#region Fields

		private Type _controllerType = null;

		#endregion 

		#region Properties

		/// <summary>
		/// The controller's Type
		/// </summary>
		public Type ControllerType
		{
			get { return _controllerType; }
		}
		#endregion 

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="controllerType">A Controller Type</param>
		public ControllerAttribute(Type controllerType)
		{
			_controllerType = controllerType;
		}
		#endregion 

	}
}
