// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;

	/// <summary>
	/// This interface is used to indicate objects that need to be
	/// resolved in some particular context.
	/// </summary>
	public interface IResolvable
	{
		/// <summary>
		/// Resolve a object to a value.
		/// </summary>
		/// <param name="context">the contextwith respect which to resolve</param>
		/// <returns>the resolved object</returns>
		/// <exception cref="ContextException">if an error occurs</exception>
		object Resolve( IContext context );
	}
}
