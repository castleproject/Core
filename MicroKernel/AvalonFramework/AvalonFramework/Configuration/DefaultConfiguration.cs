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
	using System.Runtime.Serialization;	
	
	/// <summary>
	/// This is the default <see cref="IConfiguration"/> implementation.
	/// </summary>
	/// 
	[Serializable]
	public class DefaultConfiguration: AbstractConfiguration, ISerializable
	{
		private static readonly DefaultConfiguration empty = new DefaultConfiguration();

		private static readonly string SERIAL_NAME_NAME = "name";
		private static readonly string SERIAL_NAME_LOCATION = "location";
		private static readonly string SERIAL_NAME_NAMESPACE = "namespace";
		private static readonly string SERIAL_NAME_PREFIX = "prefix";
		private static readonly string SERIAL_NAME_VALUE = "value";
		private static readonly string SERIAL_NAME_READ_ONLY = "read-only";
		private static readonly string SERIAL_NAME_ATTRIBUTE_KEYS = "attribute-keys";
		private static readonly string SERIAL_NAME_ATTRIBUTE_VALUES = "attribute-values";
		private static readonly string SERIAL_NAME_CHILDREN = "children";

		/// <summary>
		/// Creates a new <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		public DefaultConfiguration()
		{
		}

		/// <summary>
		/// Creates a new <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="name">The Name of the node.</param>
		/// <param name="location">The Location of the node.</param>
		public DefaultConfiguration(string name, string location )
		{
			Name = name;
			Location = location;
		}

		/// <summary>
		/// Creates a new <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="name">The Name of the node.</param>
		/// <param name="location">The Location of the node.</param>
		/// <param name="ns">The Namespace of the node.</param>
		/// <param name="prefix">The Prefix of the node.</param>
		public DefaultConfiguration(string name, string location, string ns, string prefix): this(name, location)
		{
			Namespace = ns;
			Prefix = prefix;
		}

		/// <summary>
		/// Creates a new <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		public DefaultConfiguration(SerializationInfo info, StreamingContext context)
		{
			Name = info.GetString(SERIAL_NAME_NAME);
			Location = info.GetString(SERIAL_NAME_LOCATION);
			Namespace = info.GetString(SERIAL_NAME_NAMESPACE);
			Prefix = info.GetString(SERIAL_NAME_PREFIX);

			Value = info.GetString(SERIAL_NAME_VALUE);

			Attributes = new Hashtable(RuntimeSerializer.DeserializeIDictionary(info,
				SERIAL_NAME_ATTRIBUTE_KEYS, SERIAL_NAME_ATTRIBUTE_VALUES));

			foreach (IConfiguration config in RuntimeSerializer.DeserializeArray(info, SERIAL_NAME_CHILDREN))
			{
				Children.Add((IConfiguration) config);
			}

			bool readOnlyMode = info.GetBoolean(SERIAL_NAME_READ_ONLY);

			if (readOnlyMode)
			{
				MakeReadOnly();
			}
		}

		/// <summary>
		///	Gets an <see cref="IConfiguration"/> instance encapsulating the specified
		/// child node.
		/// </summary>
		/// <param name="name">The Name of the child node.</param>
		/// <param name="createNew">
		///	If <see langword="true"/>, a new <see cref="IConfiguration"/>
		/// will be created and returned if the specified child does not exist.
		/// If <see langword="true"/>, <see langword="null"/> will be returned when the specified
		/// child doesn't exist.
		/// </param>
		/// <returns>
		/// The <see cref="IConfiguration"/> instance encapsulating the specified child node.
		/// </returns>
		public override IConfiguration GetChild(string name, bool createNew )
		{
			IConfiguration result = null;

			if (Children.Count != 0)
			{
				foreach (IConfiguration configuration in Children)
				{
					if (string.Compare(configuration.Name, name) == 0)
					{
						result = configuration;
						break;
					}
				}
			}
			
			if ( result == null )
			{
				if( createNew )
				{
					result = new DefaultConfiguration( name, string.Empty );
					Children.Add( result );
				}
			}

			return result;
		}

		/// <summary>
		/// Return a collection of <see cref="IConfiguration"/>
		/// elements containing all node children with the specified name.
		/// </summary>
		/// <param name="name">The Name of the children to get.</param>
		/// <returns>
		/// The collection of <see cref="IConfiguration"/> children of
		/// this associated with the given name.
		/// </returns>
		public override ConfigurationCollection GetChildren(string name)
		{
			ConfigurationCollection result = new ConfigurationCollection();

			foreach (IConfiguration configuration in Children)
			{
				if (string.Compare(configuration.Name, name) == 0)
				{
					result.Add(configuration);
				}
			}

			return result;
		}

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed
		/// to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(SERIAL_NAME_NAME, Name);
			info.AddValue(SERIAL_NAME_LOCATION, Location);
			info.AddValue(SERIAL_NAME_NAMESPACE, Namespace);
			info.AddValue(SERIAL_NAME_PREFIX, Prefix);

			info.AddValue(SERIAL_NAME_VALUE, Value);

			info.AddValue(SERIAL_NAME_READ_ONLY, IsReadOnly);

			RuntimeSerializer.SerializeIDictionary(info, Attributes,
				SERIAL_NAME_ATTRIBUTE_KEYS, SERIAL_NAME_ATTRIBUTE_VALUES);

			RuntimeSerializer.SerializeICollection(info, Children, SERIAL_NAME_CHILDREN);
		}

		/// <summary>
		/// Returns a Empty instance of <see cref="DefaultConfiguration"/>.
		/// </summary>
		public static DefaultConfiguration EmptyConfiguration
		{
			get
			{
				return empty;
			}
		}
	}
}
