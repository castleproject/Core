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
	/// The optional configuration attribute allows 
	/// the declaration of a validation schema with a component type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method,AllowMultiple=false,Inherited=true)]
	public class AvalonConfigurationAttribute : Attribute
	{
		private string m_schema;

		/// <summary>
		/// Constructor to initialize Configuration Schema info.
		/// </summary>
		/// <param name="schema">The schema name</param>
		/// <exception cref="ArgumentNullException">If schema is null.</exception>
		public AvalonConfigurationAttribute(String schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException( "schema", "Schema can't be null" );
			}

			m_schema = schema;
		}

		/// <summary>		/// The schema name used to validate the configuration.		/// </summary>		public string Schema		{			get			{				return m_schema;			}		}
	}
}
