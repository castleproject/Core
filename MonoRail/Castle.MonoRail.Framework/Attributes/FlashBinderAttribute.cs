// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Another implementation of a parameter binding. This
	/// one allow the programmer to say that the parameter value comes 
	/// from an entry in the <see cref="Controller.Flash"/>.
	/// </summary>
	[Serializable]
	public class FlashBinderAttribute : Attribute, IParameterBinder
	{
		private String flashKey;

		/// <summary>
		/// The flash entry to use. If none 
		/// is provided, the target parameter name is used
		/// </summary>
		public String FlashKey
		{
			get { return flashKey; }
			set { flashKey = value; }
		}

		/// <summary>
		/// Implementation of <see cref="IParameterBinder.Bind"/>
		/// and it is used to read the data available and construct the
		/// parameter type accordingly.
		/// </summary>
		/// <param name="controller">The controller instance</param>
		/// <param name="parameterInfo">The parameter info</param>
		/// <returns>The bound instance</returns>
		public object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			String key = ObtainKey(parameterInfo);

			return controller.Context.Flash[key];
		}

		/// <summary>
		/// Implementation of <see cref="IParameterBinder.CalculateParamPoints"/>
		/// and it is used to give the method a weight when overloads are available.
		/// </summary>
		/// <param name="controller">The controller instance</param>
		/// <param name="parameterInfo">The parameter info</param>
		/// <returns>Positive value if the parameter can be bound</returns>
		public int CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			String key = ObtainKey(parameterInfo);

			return controller.Context.Flash[key] != null ? 10 : 0;
		}

		private String ObtainKey(ParameterInfo parameterInfo)
		{
			return flashKey != null ? flashKey : parameterInfo.Name;
		}
	}
}
