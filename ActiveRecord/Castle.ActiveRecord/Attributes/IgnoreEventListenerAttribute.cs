// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Attributes
{
	using System;

	///<summary>
	/// Instructs <see cref="ActiveRecordStarter"/> to ignore the event listeners 
	/// of the specified type or assembly. 
	///</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class IgnoreEventListenerAttribute : EventListenerAssemblyAttribute
	{
		/// <summary>
		/// Adds an assembly which event listeners should be ignored when added per <see cref="EventListenerAttribute"/>.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly. A partial name is sufficient.</param>
		public IgnoreEventListenerAttribute(string assemblyName)
		{
			ParseStringExpression(assemblyName);
		}

		/// <summary>
		/// Ignores the specified event listener.
		/// </summary>
		/// <param name="type">A type that implements at least one event listener interface.</param>
		public IgnoreEventListenerAttribute(Type type)
		{
			Type = type;
		}		
	}
}