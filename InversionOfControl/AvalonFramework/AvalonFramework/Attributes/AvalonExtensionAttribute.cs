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
	/// The AvalonExtensionAttribute allows a component to exposed
	/// itself as an Lifecycle Extension.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
	public class AvalonExtensionAttribute : Attribute
	{
		String m_id;

		/// <summary>
		/// Constructor to initialize extension's info.
		/// </summary>
		/// <param name="id">The extension's name.</param>
		public AvalonExtensionAttribute(String id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id", "Extension's id can't be null");
			}

			m_id = id;
		}

		/// <summary>
		/// Returns the extension name.
		/// </summary>
		public String ID
		{
			get
			{
				return m_id;
			}
		}
	}
}
