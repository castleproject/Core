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
	using System.Collections;

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
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets a string describing location of <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// A String describing location of <see cref="IConfiguration"/>.
		/// </value> 
		string Location
		{
			get;
		}

		/// <summary>
		/// Gets the value of the node.
		/// </summary>
		/// <value>
		/// The Value of the node.
		/// </value> 
		string Value
		{
			get;
		}

		/// <summary>
		/// Gets the namespace of the node.
		/// </summary>
		/// <value>
		/// The Namespace of the node.
		/// </value>
		string Namespace
		{
			get;
		}

		/// <summary>
		/// Gets the prefix of the node.
		/// </summary>
		/// <value>
		/// The Prefix of the node.
		/// </value>
		string Prefix
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="IConfiguration"/> is read-only.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if the <see cref="IConfiguration"/> is read-only; otherwise, <see langword="false"/>.
		/// </value> 
		bool IsReadOnly
		{
			get;
		}
		
		/// <summary>
		/// Gets an <see cref="ConfigurationCollection"/> of <see cref="IConfiguration"/>
		/// elements containing all node children.
		/// </summary>
		/// <value>The Collection of child nodes.</value>
		ConfigurationCollection Children
		{
			get;
		}

		/// <summary>
		/// Gets an <see cref="IDictionary"/> of the configuration attributes.
		/// </summary>
		IDictionary Attributes
		{
			get;
		}
		
		/// <summary>
		///	Gets a <see cref="IConfiguration"/> instance encapsulating the specified
		/// child node.
		/// </summary>
		/// <param name="child">The Name of the child node.</param> 
		/// <param name="createNew">
		///	If <see langword="true"/>, a new <see cref="IConfiguration"/>
		/// will be created and returned if the specified child does not exist.
		/// If <see langword="false"/>, <see langword="null"/> will be returned when the specified
		/// child doesn't exist.
		/// </param>
		/// <returns>
		/// The <see cref="IConfiguration"/> instance encapsulating the specified child node.
		/// </returns>  
		IConfiguration GetChild(string child, bool createNew);

		/// <summary>
		/// Return an <see cref="ConfigurationCollection"/> of <see cref="IConfiguration"/>
		/// elements containing all node children with the specified name.
		/// </summary>
		/// <param name="name">The Name of the children to get.</param> 
		/// <returns>
		/// The <see cref="ConfigurationCollection"/> of
		/// <see cref="IConfiguration"/> children of 
		/// this associated with the given name.
		/// </returns> 
		ConfigurationCollection GetChildren(string name);

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
		
		/// <summary>
		/// Gets the value of specified attribute and
		/// converts it into specified <see cref="Type"/>. 
		/// </summary>
		/// <param name="name">The Name of the attribute you ask the value of.</param>
		/// <param name="type">The <see cref="Type"/></param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value of the attribute.</returns>
		object GetAttribute(string name, Type type, object defaultValue);

		/// <summary>
		/// Gets the value of specified attribute
		/// </summary>
		/// <param name="name">The Name of the attribute you ask the value of.</param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value of the attribute.</returns>
		object GetAttribute(string name, object defaultValue);
	}
}
