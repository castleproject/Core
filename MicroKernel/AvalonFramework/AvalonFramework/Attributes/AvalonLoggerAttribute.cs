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

	///<summary>	///  Attribute to declare the name of a logging channel that is consumed 	///  by a component.	///</summary>	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=false)]	public sealed class AvalonLoggerAttribute : Attribute	{		private string m_name;		/// <summary>		/// Marks a class with the logger channel name.		/// </summary>		/// <param name="name">The name of a subsidiary logging channel</param>		/// <param name="lifestyle">The lifestyle used for the component</param>		public AvalonLoggerAttribute( string name )		{			m_name = name;		}		/// <summary>		/// The name of a subsidiary logging channel 		/// relative to the logging channel that is 		/// supplied to the component by the container.		/// </summary>		public string Name		{			get			{				return m_name;			}		}	}
}
