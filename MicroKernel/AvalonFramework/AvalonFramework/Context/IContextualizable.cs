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
	/// This inteface should be implemented by components that need
	/// a Context to work. Context contains runtime generated object
	/// provided by the Container to this Component.
	/// </summary>
	public interface IContextualizable
	{
		/// <summary>
		/// Pass the Context to the component.
		/// This method is called after the ILogEnabled.EnableLogging() (if present)
		/// method and before any other method.
		/// </summary>
		/// <param name="context">the context. Must not be <code>null</code>.</param>
		/// <exception cref="ContextException">if context is invalid</exception>
		void Contextualize( IContext context );
	}
}
