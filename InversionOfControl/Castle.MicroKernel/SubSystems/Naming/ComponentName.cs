// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.Serialization;

	[Serializable]
	public class ComponentName : ISerializable
	{
		protected String _service;
		protected String _literalProperties = String.Empty;
		protected HybridDictionary _properties;
		protected bool _allProperties;

		/// <summary>
		/// Creates a ComponentName using a name pattern like
		/// "service:key=value,key2=value2"
		/// </summary>
		/// <param name="name">Complete name</param>
		public ComponentName(String name)
		{
			Setup(name);
		}

		/// <summary>
		/// Creates a ComponentName with specified service and 
		/// properties.
		/// </summary>
		/// <param name="service">Service name</param>
		/// <param name="properties">Property list.</param>
		public ComponentName(String service, String properties)
		{
			SetupService(service);
			SetupProperties(properties);
		}

		internal IDictionary Properties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Serialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ComponentName(SerializationInfo info, StreamingContext context)
		{
			String service = info.GetString("service");
			String props = info.GetString("props");
			SetupService(service);

			if (props != String.Empty)
			{
				SetupProperties(props);
			}
		}

		/// <summary>
		/// Parses the full name extracting the service and properties.
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
				String[] splitted = name.Split(new char[] {':'});

				SetupService(splitted[0]);
				SetupProperties(splitted[1]);
			}
			else
			{
				SetupService(name);
				SetupProperties(String.Empty);
			}
		}

		/// <summary>
		/// Sets up the service. Can be empty but can't be null.
		/// </summary>
		/// <param name="service"></param>
		protected virtual void SetupService(String service)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}

			_service = service;
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
				_literalProperties = "*";
				_allProperties = true;
				return;
			}
			if (properties == String.Empty)
			{
				_literalProperties = "";
				SetupProperties(new HybridDictionary(true));
				return;
			}

			String[] props = properties.Split(new char[] {','});

			HybridDictionary propsHash = new HybridDictionary(true);

			foreach (String chunk in props)
			{
				if (chunk.IndexOf('=') == -1)
				{
					throw new ArgumentException("Invalid properties.");
				}

				String[] keyvalue = chunk.Split(new char[] {'='});

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
		protected virtual void SetupProperties(IDictionary properties)
		{
			_properties = new HybridDictionary(true);

			StringBuilder sb = new StringBuilder();

			foreach (DictionaryEntry entry in properties)
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
				catch (InvalidCastException)
				{
					throw new ApplicationException("Key is not a String.");
				}

				String value = null;

				try
				{
					value = (String) entry.Value;
				}
				catch (InvalidCastException)
				{
					throw new ApplicationException("Value is not a String.");
				}

				sb.AppendFormat("{0}={1}", key, value);

				_properties[key] = value;
			}

			_literalProperties = sb.ToString();
		}

		public String Service
		{
			get { return _service; }
		}

		public String LiteralProperties
		{
			get { return _literalProperties; }
		}

		public String this[String key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}

				return (String) _properties[key];
			}
		}

		public override bool Equals(object obj)
		{
			ComponentName other = obj as ComponentName;

			if (other != null)
			{
				return other._service.Equals(_service) &&
					other._literalProperties.Equals(_literalProperties);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _service.GetHashCode() ^ _literalProperties.GetHashCode();
		}

		public override string ToString()
		{
			return
				String.Format("Service: {0} Properties: {1}",
				              _service, _literalProperties);
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("service", _service);
			info.AddValue("props", _literalProperties);
		}

		#endregion
	}
}