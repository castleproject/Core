// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for IConfiguration.
	/// </summary>
	/// <summary>
	/// <see cref="IConfiguration"/> is a interface encapsulating a configuration node
	///	used to retrieve configuration values.
	/// </summary>
	public interface IConfiguration
	{
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		/// <value>
		/// The Name of the node.
		/// </value> 
		String Name { get; }

		/// <summary>
		/// Gets the value of the node.
		/// </summary>
		/// <value>
		/// The Value of the node.
		/// </value> 
		String Value { get; }

		/// <summary>
		/// Gets an <see cref="ConfigurationCollection"/> of <see cref="IConfiguration"/>
		/// elements containing all node children.
		/// </summary>
		/// <value>The Collection of child nodes.</value>
		ConfigurationCollection Children { get; }

		/// <summary>
		/// Gets an <see cref="IDictionary"/> of the configuration attributes.
		/// </summary>
		NameValueCollection Attributes { get; }

		/// <summary>
		/// Gets the value of the node and converts it 
		/// into specified <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/></param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value converted into the specified type.</returns>
		object GetValue(Type type, object defaultValue);
	}
}