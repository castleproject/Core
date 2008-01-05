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
	/// Depicts the contract used by the view engine
	/// to obtain implementations of <see cref="ViewComponent"/>.
	/// </summary>
	public interface IViewComponentFactory
	{
		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		ViewComponent Create(String name);

		/// <summary>
		/// Gets the registry.
		/// </summary>
		/// <remarks>
		/// Exposing it here is a hack, and I really don't like the 
		/// design of the viewcomponent factory and its relation with the registry. 
		/// However, I can't refactor it now.
		/// </remarks>
		/// <value>The registry.</value>
		IViewComponentRegistry Registry { get; }
	}
}
