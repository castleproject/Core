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
	/// that wish to create <see cref="LayoutDescriptor"/>.
	/// </summary>
	/// <remarks>
	/// The default implementation creates the descriptors
	/// based on <see cref="LayoutAttribute"/> associated
	/// with the controller
	/// </remarks>
	public interface ILayoutDescriptorProvider : IProvider
	{
		/// <summary>
		/// Implementors should collect the layout information
		/// and return a descriptor instance, or null if none 
		/// was found.
		/// </summary>
		/// <param name="memberInfo">The controller type or action (MethodInfo)</param>
		/// <returns>An <see cref="LayoutDescriptor"/> instance</returns>
		LayoutDescriptor CollectLayout(MemberInfo memberInfo);
	}
}
