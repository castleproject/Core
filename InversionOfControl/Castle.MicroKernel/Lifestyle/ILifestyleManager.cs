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

	/// <summary>
	/// The <c>ILifestyleManager</c> implements 
	/// a strategy for a given lifestyle, like singleton, perthread
	/// and transient.
	/// </summary>
	/// <remarks>
	/// The responsability of <c>ILifestyleManager</c>
	/// is only the management of lifestyle. It should rely on
	/// <see cref="IComponentActivator"/> to obtain a new component instance
	/// </remarks>
	public interface ILifestyleManager : IDisposable
	{
		/// <summary>
		/// Initializes the <c>ILifestyleManager</c> with the 
		/// <see cref="IComponentActivator"/>
		/// </summary>
		/// <param name="componentActivator"></param>
		/// <param name="kernel"></param>
		void Init(IComponentActivator componentActivator, IKernel kernel);

		/// <summary>
		/// Implementors should return the component instance based 
		/// on the lifestyle semantic.
		/// </summary>
		/// <returns></returns>
		object Resolve();

		/// <summary>
		/// Implementors should release the component instance based
		/// on the lifestyle semantic, for example, singleton components
		/// should not be released on a call for release, instead they should
		/// release them when disposed is invoked.
		/// </summary>
		/// <param name="instance"></param>
		void Release(object instance);
	}
}
