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
	/// This is an abstract <see cref="IConfiguration"/> implementation
	/// that deals with methods that can be abstracted away
	/// from underlying implementations.
	/// </summary>
	/// <remarks>
	/// <para><b>AbstractConfiguration</b> makes easier to implementers 
	/// to create a new version of <see cref="IConfiguration"/></para>
	/// </remarks>
	public abstract class AbstractConfiguration : IConfiguration
	{
		private bool readOnly;
		private string name;
		private string location;
		private string val;
		private string ns;
		private string prefix;
		private Hashtable attributes = new Hashtable();
		private ConfigurationCollection children = new ConfigurationCollection();

		/// <summary>
		/// Gets a value indicating whether the <see cref="IConfiguration"/> is read-only.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if the <see cref="IConfiguration"/> is read-only;
		/// otherwise, <see langword="false"/>.
		/// </value>
		public bool IsReadOnly
		{
			get
			{
				return readOnly;
			}
		}

		/// <summary>
		/// Gets the name of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The Name of the <see cref="IConfiguration"/>.
		/// </value>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				CheckReadOnly();

				name = value;
			}
		}

		/// <summary>
		/// Gets a string describing location of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// A String describing location of the <see cref="IConfiguration"/>.
		/// </value>
		public string Location
		{
			get
			{
				return location;
			}
			set
			{
				CheckReadOnly();

				location = value;
			}
		}

		/// <summary>
		/// Gets the value of <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The Value of the <see cref="IConfiguration"/>.
		/// </value>
		public string Value
		{
			get
			{
				return val;
			}
			set
			{
				CheckReadOnly();

				val = value;
			}
		}

		/// <summary>
		/// Gets the namespace of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The Namespace of the <see cref="IConfiguration"/>.
		/// </value>
		public string Namespace
		{
			get
			{
				return ns;
			}

			set
			{
				CheckReadOnly();

				ns = value;
			}
		}

		/// <summary>
		/// Gets the prefix of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The prefix of the <see cref="IConfiguration"/>.
		/// </value>
		public string Prefix
		{
			get
			{
				return prefix;
			}

			set
			{
				CheckReadOnly();

				prefix = value;
			}
		}


		/// <summary>
		/// Gets all child nodes.
		/// </summary>
		/// <value>The <see cref="ConfigurationCollection"/> of child nodes.</value>
		public ConfigurationCollection Children
		{
			get
			{
				if (children == null)
				{
					children = new ConfigurationCollection();
				}

				return children;
			}

			set
			{
				CheckReadOnly();

				children = value;
			}
		}

		/// <summary>
		/// Gets node attributes.
		/// </summary>
		/// <value>
		/// All attributes of the node.
		/// </value>
		public  IDictionary Attributes
		{
			get
			{
				if (attributes == null)
				{
					attributes = new Hashtable();
				}

				return attributes;
			}

			set
			{
				CheckReadOnly();

				attributes = new Hashtable(value);
			}
		}

		/// <summary>
		///	Gets a <see cref="IConfiguration"/> instance encapsulating the specified
		/// child node.
		/// </summary>
		/// <param name="child">The Name of the child node.</param>
		/// <returns>
		///	The <see cref="IConfiguration"/> instance encapsulating the specified
		///	child node.
		/// </returns>
		public IConfiguration GetChild(string child)
		{
			return GetChild(child, false);
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
		///	The <see cref="IConfiguration"/> instance encapsulating the specified
		///	child node.
		/// </returns>
		public abstract IConfiguration GetChild(string child, bool createNew);

		/// <summary>
		/// Return an <see cref="ConfigurationCollection"/> of <see cref="IConfiguration"/>
		/// elements containing all node children with the specified name.
		/// </summary>
		/// <param name="name">The Name of the children to get.</param>
		/// <returns>
		/// All node children with the specified name
		/// </returns>
		public abstract ConfigurationCollection GetChildren(string name);

		/// <summary>
		/// Gets the value of the node and converts it
		/// into specified <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/></param>
		/// <returns>The Value converted into the specified type.</returns>
		/// <exception cref="InvalidCastException">
		/// If the convertion fails, an exception will be thrown.
		/// </exception>
		public object GetValue(Type type)
		{
			return GetValue(type, null);
		}

		/// <summary>
		/// Gets the value of the node and converts it
		/// into specified <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/></param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value converted into the specified type.</returns>
		public object GetValue(Type type, object defaultValue)
		{

			return Converter.ChangeType(Value, type, defaultValue);
		}

		/// <summary>
		/// Gets the value of specified attribute and
		/// converts it into specified <see cref="System.Type"/>.
		/// </summary>
		/// <param name="name">The Name of the attribute you ask the value of.</param>
		/// <param name="type">The <see cref="System.Type"/></param>
		/// <returns>The Value converted into the specified type.</returns>
		/// <exception cref="InvalidCastException">
		/// If the convertion fails, an exception will be thrown.
		/// </exception>
		public object GetAttribute(string name, Type type)
		{
			return GetAttribute(name, type, null);
		}

		/// <summary>
		/// Gets the value of specified attribute
		/// </summary>
		/// <param name="name">The Name of the attribute you ask the value of.</param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value of the attribute.</returns>
		public object GetAttribute(string name, object defaultValue)
		{
			object value = Attributes[name];
			if (value == null)
			{
				value = defaultValue;
			}
			return value;
		}

		/// <summary>
		/// Gets the value of specified attribute and
		/// converts it into specified <see cref="System.Type"/>.
		/// </summary>
		/// <param name="name">The Name of the attribute you ask the value of.</param>
		/// <param name="type">The <see cref="System.Type"/></param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value converted into the specified type.</returns>
		public object GetAttribute(string name, Type type, object defaultValue)
		{
			return Converter.ChangeType(Attributes[name], type, defaultValue);
		}

		/// <summary>
		/// Make the configuration read only.
		/// </summary>
		public void MakeReadOnly()
		{
			readOnly = true;
		}

		/// <summary>
		/// Check whether this node is readonly or not.
		/// </summary>
		/// <exception cref="ConfigurationException">
		/// If this node is readonly then an exception will be thrown.
		/// </exception>
		protected void CheckReadOnly()
		{
			if( IsReadOnly )
			{
				throw new ConfigurationException( "Configuration is read only and can not be modified." );
			}
		}
	}
}
