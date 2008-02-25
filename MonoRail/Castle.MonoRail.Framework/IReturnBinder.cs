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
	using System;

	/// <summary>
	/// Allows an attribute to be a binder for the return type
	/// </summary>
	public interface IReturnBinder
	{
		/// <summary>
		/// Binds the specified parameters for the action.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="returnType">Type being returned.</param>
		/// <param name="returnValue">The return value.</param>
		/// <returns></returns>
		void Bind(IEngineContext context, IController controller, IControllerContext controllerContext,
		          Type returnType, object returnValue);
	}
}