// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.LifecycleConcerns
{
	using Castle.Core;

	/// <summary>
	/// Represents a concern that will be applied to a component instance
	/// during commission or decomission phase. 
	/// </summary>
	public interface ILifecycleConcern
	{
		/// <summary>
		/// Implementors should act on the instance in response to 
		/// a decomission or commission phase. 
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="component">The component.</param>
		void Apply(ComponentModel model, object component);
	}
}