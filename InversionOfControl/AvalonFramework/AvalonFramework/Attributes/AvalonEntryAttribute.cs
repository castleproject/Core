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
	/// The AvalonEntryAttribute Attribute declares a context entry 
	/// required by a component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)]
	public class AvalonEntryAttribute : Attribute
	{
		private String m_alias;
		private String m_key;
		private bool   m_volatile = false;
		private bool   m_optinal = false;
		private Type   m_type = typeof(String);

		/// <summary>
		/// Constructor to initialize Entry info.
		/// </summary>
		/// <param name="key"></param>
		public AvalonEntryAttribute(String key)
		{
			if (key == null || key.Length == 0)
			{
				throw new ArgumentNullException("key", "Entry's key can't be null");
			}

			m_key  = key;
		}

		/// <summary>
		/// Constructor to initialize Entry info.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type"></param>
		public AvalonEntryAttribute(String key, Type type) : this(key)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type", "Entry's type can't be null");
			}

			m_type = type;
		}

		/// <summary>
		/// Constructor to initialize Entry info.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type"></param>
		/// <param name="optional"></param>
		public AvalonEntryAttribute(String key, Type type, bool optional) : this(key, type)
		{
			m_optinal = optional;
		}

		/// <summary>
		/// The "official" key of the entry.
		/// </summary>
		public String Key
		{
			get
			{
				return m_key;
			}
			set
			{
				m_key = value;
			}
		}

		/// <summary>
		/// The alias that can be used by the component.
		/// </summary>
		public String Alias
		{
			get
			{
				return m_alias;
			}
			set
			{
				m_alias = value;
			}
		}

		/// <summary>
		/// The Type of the entry
		/// </summary>
		public Type EntryType
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		/// <summary>
		/// Is this entry optional? Defaults to false
		/// </summary>
		public bool Optional
		{
			get
			{
				return m_optinal;
			}
			set
			{
				m_optinal = value;
			}
		}

		/// <summary>
		/// Is this entry volatile? Defaults to false
		/// </summary>
		public bool Volatile
		{
			get
			{
				return m_volatile;
			}
			set
			{
				m_volatile = value;
			}
		}
	}
}
