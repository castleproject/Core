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

namespace Castle.MicroKernel
{
	using System;

	using Castle.Model;

	/// <summary>
	/// Implementors should use a strategy to obtain 
	/// valid references to properties and/or services 
	/// requested in the dependency model.
	/// </summary>
	public interface IDependencyResolver
	{
		/// <summary>
		/// Should return an instance of a service or property values as
		/// specified by the dependency model instance. 
		/// It is also the responsability of <see cref="IDependencyResolver"/>
		/// to throw an exception in the case a non-optional dependency 
		/// could not be resolved.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		object Resolve(ComponentModel model, DependencyModel dependency);

		/// <summary>
		/// Returns true if the resolver is able to satisfy this dependency.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		bool CanResolve(ComponentModel model, DependencyModel dependency);
	}
}
