// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	/// Depicts the contract the engine has 
	/// to perform the creation and disposal of
	/// <see cref="IController"/> instances.
	/// </summary>
	public interface IControllerFactory
	{
		/// <summary>
		/// Implementors should perform their logic to
		/// return a instance of <see cref="IController"/>.
		/// If the <see cref="IController"/> can not be found,
		/// it should return <c>null</c>.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		IController CreateController(string area, string controller);

		/// <summary>
		/// Creates the controller.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <returns></returns>
		IController CreateController(Type controllerType);

		/// <summary>
		/// Implementors should perform their logic 
		/// to release the <see cref="IController"/> instance
		/// and its resources.
		/// </summary>
		/// <param name="controller"></param>
		void Release(IController controller);
	}
}
