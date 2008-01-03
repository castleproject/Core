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
	/// Depicts the contract used by the engine
	/// to obtain implementations of <see cref="IFilter"/>.
	/// </summary>
	public interface IFilterFactory
	{
		/// <summary>
		/// Creates the specified filter type.
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <returns></returns>
		IFilter Create(Type filterType);

		/// <summary>
		/// Releases the specified filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		void Release(IFilter filter);
	}
}