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

	///<summary>	/// Attribute used to mark the services that a component implements	///</summary>		[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]	public sealed class AvalonServiceAttribute : Attribute	{		private Type m_type;		///<summary>		///  Constructor to initialize the service's name.		///</summary>		///<param name="type">The type for the service</param>		///<exception cref="ArgumentException">If the "type" value is not an interface</exception>		public AvalonServiceAttribute(Type type)		{			if (!type.IsInterface)			{				throw new ArgumentException(					"The type passed in does not represent an interface",					"type" );			}			m_type = type;		}		///<summary>		///  The Type of the service		///</summary>		public Type ServiceType		{			get			{				return m_type;			}		}	}
}
