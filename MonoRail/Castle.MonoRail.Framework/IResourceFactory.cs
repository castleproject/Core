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
	using System.Reflection;
	
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Depicts the contract used by the engine
	/// to obtain implementations of <see cref="IResource"/>.
	/// </summary>
	public interface IResourceFactory
	{
		/// <summary>
		/// Creates the specified descriptor.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="appAssembly">The app assembly.</param>
		/// <returns></returns>
		IResource Create(ResourceDescriptor descriptor, Assembly appAssembly);

		/// <summary>
		/// Releases the specified resource.
		/// </summary>
		/// <param name="resource">The resource.</param>
		void Release(IResource resource);
	}
}
