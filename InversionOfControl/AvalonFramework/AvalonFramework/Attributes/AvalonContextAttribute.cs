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
	/// The context tag enables the declaration 
	/// of a custom context interface classname that a 
	/// component may safely cast a supplied context instance to.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)]
	public class AvalonContextAttribute : Attribute
	{
		private Type m_contextType;

		/// <summary>
		/// Constructor to initialize the context's type.
		/// </summary>
		/// <param name="context"></param>
		public AvalonContextAttribute(Type context)
		{
			if (!context.IsInterface)			{				throw new ArgumentException(					"The type passed in does not represent an interface",					"context" );			}			m_contextType = context;
		}

		///<summary>		///  The type of the context		///</summary>		public Type ContextType		{			get			{				return m_contextType;			}		}
	}
}
