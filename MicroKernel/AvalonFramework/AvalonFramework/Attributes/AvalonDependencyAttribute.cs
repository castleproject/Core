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

	/// <summary>	/// An enumeration used to mark a dependency as optional or not.	/// </summary>	public enum Optional	{		/// <summary>		/// Use "True" if the dependency is not required for the component		/// to run properly.		/// </summary>		True,		/// <summary>		/// Use "False" if the component will not work without the dependnecy.		/// </summary>		False	}	///<summary>	///  Attribute to mark the dependencies for a component.	///</summary>	[AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]	public sealed class AvalonDependencyAttribute : Attribute	{		private Type m_type;		private bool m_optional;		private string m_name;		///<summary>		///  Constructor to initialize the dependency's name.		///</summary>		///<param name="type">The type for the dependency</param>		///<param name="key">The dependency's lookup key</param>		///<param name="optional">Whether or not the dependency is optional</param>		///<exception cref="ArgumentException">If the "type" value is not an interface</exception>		public AvalonDependencyAttribute(Type type, string key, Optional optional)		{			if (!type.IsInterface)			{				throw new ArgumentException(					"The type passed in does not represent an interface",					"type" );			}			m_name = (null == key) ? type.Name : key;			m_optional = (optional == Optional.True);			m_type = type;		}		///<summary>		///  The lookup name of the dependency		///</summary>		public string Key		{			get			{				return m_name;			}		}		///<summary>		///  Is this dependency optional?		///</summary>		public bool IsOptional		{			get			{				return m_optional;			}		}		/// <summary>		///   The dependency type		/// </summary>		public Type DependencyType		{			get			{				return m_type;			}		}	}
}
