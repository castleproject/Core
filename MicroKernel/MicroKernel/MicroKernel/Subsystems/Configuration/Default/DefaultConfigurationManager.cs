// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Subsystems.Configuration.Default
{
	using System;
	using System.Collections;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Subsystems;

	/// <summary>
	/// The default implementation of <see cref="IConfigurationManager"/>
	/// simply associates a configuration with a component name.
	/// </summary>
	public class DefaultConfigurationManager : AbstractSubsystem, IConfigurationManager
	{
		private Hashtable m_name2Config;

		public DefaultConfigurationManager()
		{
			m_name2Config = Hashtable.Synchronized( 
				new Hashtable(
					CaseInsensitiveHashCodeProvider.Default, 
					CaseInsensitiveComparer.Default) );
		}
	
		protected virtual IDictionary ConfigurationDictionary
		{
			get { return m_name2Config; }
		}

		#region IConfigurationManager Members

		/// <summary>
		/// Implementation should return a configuration for 
		/// the component.
		/// </summary>
		/// <param name="componentName"></param>
		/// <returns></returns>
		public virtual IConfiguration GetConfiguration(String componentName)
		{
			AssertUtil.ArgumentNotNull( componentName, "componentName" );

			IConfiguration config = (IConfiguration) m_name2Config[ componentName ];

			if ( config == null )
			{
				config = DefaultConfiguration.EmptyConfiguration;
			}

			return config;
		}

		/// <summary>
		/// Implementation should associate a configuration for
		/// the component name.
		/// </summary>
		/// <param name="componentName"></param>
		/// <param name="configuration"></param>
		public virtual void Add(String componentName, IConfiguration configuration)
		{
			AssertUtil.ArgumentNotNull( componentName, "componentName" );
			AssertUtil.ArgumentNotNull( configuration, "configuration" );

			m_name2Config[ componentName ] = configuration;
		}

		/// <summary>
		/// Returns configurations available.
		/// </summary>
		public virtual IConfiguration[] Configurations
		{
			get
			{
				ArrayList list = new ArrayList( m_name2Config.Values );
				return (IConfiguration[]) list.ToArray( typeof(IConfiguration) );
			}
		}

		#endregion
	}
}