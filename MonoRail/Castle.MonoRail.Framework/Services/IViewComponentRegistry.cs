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
	/// Depicts a contract for viewcomponent registry implementations
	/// </summary>
	public interface IViewComponentRegistry
	{
		/// <summary>
		/// Adds the view component.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		void AddViewComponent(string name, Type type);

		/// <summary>
		/// Gets the view component.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Type GetViewComponent(string name);

		/// <summary>
		/// Checks if the view component is registered.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>True if registered or false if GetViewComponent would throw an exception.</returns>
		bool HasViewComponent(string name);
	}
}
