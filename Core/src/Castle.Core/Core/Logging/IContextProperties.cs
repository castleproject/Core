// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Logging
{
	/// <summary>
	///   Interface for Context Properties implementations
	/// </summary>
	/// <remarks>
	///   <para>
	///     This interface defines a basic property get set accessor.
	///   </para>
	///   <para>
	///     Based on the ContextPropertiesBase of log4net, by Nicko Cadell.
	///   </para>
	/// </remarks>
	public interface IContextProperties
	{
		/// <summary>
		///   Gets or sets the value of a property
		/// </summary>
		/// <value>
		///   The value for the property with the specified key
		/// </value>
		/// <remarks>
		///   <para>
		///     Gets or sets the value of a property
		///   </para>
		/// </remarks>
		object this[string key] { get; set; }
	}
}