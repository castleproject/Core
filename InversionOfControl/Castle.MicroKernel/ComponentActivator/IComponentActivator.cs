// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	/// Implements the instance creation logic. The default
	/// implementation should rely on an ordinary call to 
	/// Activator.CreateInstance(). 
	/// </summary>
	/// <remarks>
	/// This interface is provided in order to allow custom components
	/// to be created using a different logic, such as using a specific factory
	/// or builder.
	/// <seealso cref="ComponentActivator.AbstractComponentActivator"/>
	/// <seealso cref="ComponentActivator.DefaultComponentActivator"/>
	/// </remarks>
	public interface IComponentActivator
	{
		/// <summary>
		/// Should return a new component instance.
		/// </summary>
		/// <returns></returns>
		object Create();

		/// <summary>
		/// Should perform all necessary work to dispose the instance
		/// and/or any resource related to it.
		/// </summary>
		/// <param name="instance"></param>
		void Destroy(object instance);
	}
}
