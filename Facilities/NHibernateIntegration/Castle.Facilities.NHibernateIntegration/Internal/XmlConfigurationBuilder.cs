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

#region

using Castle.Core.Configuration;
using NHibernate.Cfg;

#endregion

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	/// <summary>
	/// The configuration builder for NHibernate's own cfg.xml
	/// </summary>
	public class XmlConfigurationBuilder : IConfigurationBuilder
	{
		#region IConfigurationBuilder Members

		/// <summary>
		/// Returns the Configuration object for the given xml
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public Configuration GetConfiguration(IConfiguration config)
		{
			string cfgFile = config.Attributes["nhibernateConfigFile"];
			Configuration cfg = new Configuration();
			cfg.Configure(cfgFile);
			return cfg;
		}

		#endregion
	}
}
