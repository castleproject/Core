// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	
	using Castle.Core.Configuration;

	/// <summary>
	/// Usefull for test cases.
	/// </summary>
	public class InPlaceConfigurationSource : IConfigurationSource
	{
		private readonly IDictionary _type2Config = new Hashtable();
		private Type threadScopeInfoImplementation;
		private Type sessionFactoryHolderImplementation;
        private Type namingStrategyImplementation;
		private bool debug = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="InPlaceConfigurationSource"/> class.
		/// </summary>
		public InPlaceConfigurationSource()
		{
		}

		#region IConfigurationSource Members

		/// <summary>
		/// Return a type that implements
		/// the interface <see cref="IThreadScopeInfo"/>
		/// </summary>
		/// <value></value>
		public Type ThreadScopeInfoImplementation
		{
			get { return threadScopeInfoImplementation; }
			set { threadScopeInfoImplementation = value; }
		}

		/// <summary>
		/// Return a type that implements
		/// the interface <see cref="ISessionFactoryHolder"/>
		/// </summary>
		/// <value></value>
		public Type SessionFactoryHolderImplementation
		{
			get { return sessionFactoryHolderImplementation; }
			set { sessionFactoryHolderImplementation = value; }
		}

		/// <summary>
		/// Return a type that implements
		/// the interface NHibernate.Cfg.INamingStrategy
		/// </summary>
		/// <value></value>
		public Type NamingStrategyImplementation
		{
			get { return namingStrategyImplementation; }
			set { namingStrategyImplementation = value; }
		}

		/// <summary>
		/// Return an <see cref="IConfiguration"/> for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IConfiguration GetConfiguration(Type type)
		{
			return _type2Config[type] as IConfiguration;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IConfigurationSource"/> produce debug information
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		public bool Debug
		{
			get { return debug; }
		}

		#endregion

		/// <summary>
		/// Adds the specified type with the properties
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="properties">The properties.</param>
		public void Add(Type type, IDictionary properties)
		{
			Add(type, ConvertToConfiguration(properties));
		}

		/// <summary>
		/// Adds the specified type with configuration
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="config">The config.</param>
		public void Add(Type type, IConfiguration config)
		{
			_type2Config[type] = config;
		}

		/// <summary>
		/// Sets the type of the thread info.
		/// </summary>
		/// <param name="isWeb">if we run in a web context or not</param>
		/// <param name="customType">Type of the custom implementation</param>
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

			ThreadScopeInfoImplementation = threadInfoType;
		}

		/// <summary>
		/// Sets the type of the session factory holder.
		/// </summary>
		/// <param name="customType">Custom implementation</param>
		protected void SetUpSessionFactoryHolderType(String customType)
		{
			Type sessionFactoryHolderType = typeof(SessionFactoryHolder);

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				sessionFactoryHolderType = Type.GetType(typeName, false, false);

				if (sessionFactoryHolderType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}

			SessionFactoryHolderImplementation = sessionFactoryHolderType;
		}

		/// <summary>
		/// Sets the type of the naming strategy.
		/// </summary>
		/// <param name="customType">Custom implementation type name</param>
		protected void SetUpNamingStrategyType(String customType) 
		{
			if (customType != null && customType != String.Empty) 
			{
				String typeName = customType;

				Type namingStrategyType = Type.GetType(typeName, false, false);

				if (namingStrategyType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}

				NamingStrategyImplementation = namingStrategyType;
			}
		}

		/// <summary>
		/// Sets the debug flag.
		/// </summary>
		/// <param name="isDebug">if set to <c>true</c> Active Record will produce debug information.</param>
		protected void SetDebugFlag(bool isDebug)
		{
			debug = isDebug;
		}

		private IConfiguration ConvertToConfiguration(IDictionary properties)
		{
			MutableConfiguration conf = new MutableConfiguration("Config");

			foreach(DictionaryEntry entry in properties)
			{
				conf.Children.Add(new MutableConfiguration(entry.Key.ToString(), entry.Value.ToString()));
			}

			return conf;
		}
	}
}
