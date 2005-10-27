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

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.Collections;

	using Castle.Model.Configuration;


	/// <summary>
	/// Usefull for test cases.
	/// </summary>
	public class InPlaceConfigurationSource : IConfigurationSource
	{
		private readonly IDictionary _type2Config = new Hashtable();
		private Type threadScopeInfoImplementation;

		public InPlaceConfigurationSource()
		{
		}

		public void Add(Type type, IDictionary properties)
		{
			Add(type, ConvertToConfiguration(properties));
		}

		public void Add(Type type, IConfiguration config)
		{
			_type2Config[type] = config;
		}

		protected void SetUpThreadInfoType(bool isWeb, String customType)
		{
			Type threadInfoType = null;

			if (isWeb)
			{
				threadInfoType = typeof(Castle.ActiveRecord.Framework.Scopes.WebThreadScopeInfo);
			}

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;
				
				threadInfoType = Type.GetType(typeName, false, false);

				if (threadInfoType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}
		}

		#region IConfigurationSource Members

		public Type ThreadScopeInfoImplementation
		{
			get { return threadScopeInfoImplementation; }
			set { threadScopeInfoImplementation = value; }
		}

		public IConfiguration GetConfiguration(Type type)
		{
			return _type2Config[type] as IConfiguration;
		}

		#endregion

		private IConfiguration ConvertToConfiguration(IDictionary properties)
		{
			MutableConfiguration conf = new MutableConfiguration("Config");

			foreach(DictionaryEntry entry in properties)
			{
				conf.Children.Add( new MutableConfiguration(entry.Key.ToString(), entry.Value.ToString()));
			}

			return conf;
		}
	}
}
