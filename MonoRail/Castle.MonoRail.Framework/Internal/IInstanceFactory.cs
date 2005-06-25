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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Factory used to create instances of a specific object type
	/// in a specific application request context.
	/// </summary>
	public interface IInstanceFactory
	{
		/// <summary>
		/// Returns an instance of the requested type.
		/// </summary>
		object GetInstance( Type instanceType, IRailsEngineContext context );

		/// <summary>
		/// Releases an instance in the specified context.
		/// </summary>
		void Release( object instance, IRailsEngineContext context );
	}
}
