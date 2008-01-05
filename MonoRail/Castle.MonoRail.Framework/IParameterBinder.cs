// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	/// <summary>
	/// Implemented by attributes that wants to perform 
	/// some conversion to populate a smart dispatcher 
	/// action argument.
	/// </summary>
	public interface IParameterBinder
	{
		/// <summary>
		/// Calculates the param points. Implementors should return value equals or greater than
		/// zero indicating whether the parameter can be bound successfully. The greater the value (points)
		/// the more successful the implementation indicates to the framework
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info.</param>
		/// <returns></returns>
		int CalculateParamPoints(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo);

		/// <summary>
		/// Binds the specified parameters for the action.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info.</param>
		/// <returns></returns>
		object Bind(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo);
	}
}
