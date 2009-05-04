// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents a ManagedObject's Name. 
	/// TODO: Supports query semantic.
	/// </summary>
	[Serializable]
	public class ManagedObjectName : ISerializable
	{
		protected String domain;
		protected String literalProperties = String.Empty;
		protected Hashtable properties;
		protected bool allProperties;

		/// <summary>
		/// Creates a ManagedObjectName using a name pattern like
		/// "domain:key=value,key2=value2"
		/// </summary>
		/// <param name="name">Complete name</param>
		public ManagedObjectName(String name)
		{
			Setup(name);
		}

		/// <summary>
		/// Creates a ManagedObjectName with specified domain and 
		/// properties.
		/// </summary>
		/// <param name="domain">Domain name</param>
		/// <param name="properties">Property list.</param>
		public ManagedObjectName(String domain, String properties)
		{
			SetupDomain(domain);
			SetupProperties(properties);
		}

		/// <summary>
		/// Creates a ManagedObjectName with specified domain and 
		/// properties.
		/// </summary>
		/// <param name="domain">Domain name</param>
		/// <param name="properties">Property list.</param>
		public ManagedObjectName(String domain, Hashtable properties)
		{
			SetupDomain(domain);
			SetupProperties(properties);
		}

		/// <summary>
		/// Serialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ManagedObjectName(SerializationInfo info, StreamingContext context)
		{
			String domain = info.GetString("domain");
			String props  = info.GetString("props");
			SetupDomain(domain);

			if (props != String.Empty)
			{
				SetupProperties(props);
			}
		}

		/// <summary>
		/// Parses the full name extracting the domain and properties.
		/// </summary>
		/// <param name="name">Full name.</param>
		protected virtual void Setup(String name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			if (name.IndexOf(':') != -1)
			{
				String[] splitted = name.Split(new char[] { ':' });
				
				SetupDomain(splitted[0]);
				SetupProperties(splitted[1]);
			}
			else
			{
				SetupDomain(name);
			}
		}

		/// <summary>
		/// Sets up the domain. Can be empty but can't be null.
		/// </summary>
		/// <param name="domain"></param>
		protected virtual void SetupDomain(String domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}

			this.domain = domain;
		}

		/// <summary>
		/// Parses and validate a properties list string like 
		/// "key=value,key2=value2" and so on.
		/// </summary>
		/// <param name="properties">Property list.</param>
		protected virtual void SetupProperties(String properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (properties.Equals("*"))
			{
				literalProperties = "*";
				allProperties = true;
				return;
			}

			String [] props = properties.Split( new char[] { ',' } );

			Hashtable propsHash = new Hashtable(
				CaseInsensitiveHashCodeProvider.Default, 
				CaseInsensitiveComparer.Default);

			foreach(String chunk in props)
			{
				if (chunk.IndexOf('=') == -1)
				{
					throw new InvalidManagedObjectName("Invalid properties.");
				}

				String[] keyvalue = chunk.Split( new char[] { '=' } );

				String key = keyvalue[0];
				String value = keyvalue[1];

				propsHash.Add(key, value);
			}

			SetupProperties(propsHash);
		}

		/// <summary>
		/// Validates a properties Hashtable.
		/// </summary>
		/// <param name="properties">Property list.</param>
		protected virtual void SetupProperties(Hashtable properties)
		{
			StringBuilder sb = new StringBuilder();

			foreach(DictionaryEntry entry in properties)
			{
				if (sb.Length != 0)
				{
					sb.Append(",");
				}

				String key = null;

				try
				{
					key = (String) entry.Key;
				}
				catch(InvalidCastException)
				{
					throw new InvalidManagedObjectName("Key is not a String.");
				}

				String value = null;

				try
				{
					value = (String) entry.Value;
				}
				catch(InvalidCastException)
				{
					throw new InvalidManagedObjectName("Value is not a String.");
				}

				sb.AppendFormat("{0}={1}", key, value);
			}

			this.literalProperties = sb.ToString();
			this.properties = new Hashtable(properties);
		}

		public String Domain
		{
			get
			{
				return domain;
			}
		}

		public String LiteralProperties
		{
			get
			{
				return literalProperties;
			}
		}

		public String this[ String key ]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}

				return (String) this.properties[key];
			}
		}

		public override bool Equals(object obj)
		{
			ManagedObjectName other = obj as ManagedObjectName;

			if (other != null)
			{
				return other.domain.Equals(domain) && 
					other.literalProperties.Equals(literalProperties);
			}

			return false;
		}
	
		public override int GetHashCode()
		{
			return domain.GetHashCode() ^ literalProperties.GetHashCode();
		}
	
		public override string ToString()
		{
			return 
				String.Format("Domain: {0} Properties: {1}", 
					domain, literalProperties);
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("domain", domain);
			info.AddValue("props", literalProperties);
		}

		#endregion
	}
}
