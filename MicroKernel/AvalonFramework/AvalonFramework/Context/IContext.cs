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
	/// The context is the interface through which the component and its
	/// container communicate.
	/// </summary>
	public interface IContext
	{
		/// <summary>
		/// Retrieve an object from Context.
		/// </summary>
		/// <param name="key">the key into context</param>
		/// <returns>the object</returns>
		/// <exception cref="ContextException">if object not found. Note that this
		/// means that either Component is asking for invalid entry
		/// or the Container is not living up to contract.</exception>
		object this[ object key ] 
		{
			get;
		}
	}
}
