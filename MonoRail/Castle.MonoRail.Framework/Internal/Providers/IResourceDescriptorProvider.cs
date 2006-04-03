// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System.Reflection;

	/// <summary>
	/// Defines the contract to an implementation 
	/// that wish to create <see cref="ResourceDescriptor"/>.
	/// </summary>
	/// <remarks>
	/// The default implementation creates the descriptors
	/// based on <see cref="ResourceAttribute"/> associated
	/// with the controller
	/// </remarks>
	public interface IResourceDescriptorProvider : IProvider
	{
		/// <summary>
		/// Implementors should collect the resource information
		/// and return descriptors instances, or an empty array if none 
		/// was found.
		/// </summary>
		/// <param name="member">The controller or action (MethodInfo)</param>
		/// <returns>An array of <see cref="ResourceDescriptor"/></returns>
		ResourceDescriptor[] CollectResources(MemberInfo member);
	}
}
