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

namespace Castle.Windsor.Configuration.AppDomain
{
	using System;
	using System.Xml;
	using System.Configuration;

	using Castle.Windsor.Configuration.Xml;

	/// <summary>
	/// Looks for a section 'castle' in the standard Configuration scheme.
	/// It then deserialize the nodes to <see cref="Castle.Model.Configuration.IConfiguration"/>
	/// implementation.
	/// 
	/// <code>
	/// &lt;configuration&gt;
	/// 
	///   &lt;configSections&gt;
	///     &lt;section name="castle"
	///       type="Castle.Windsor.Configuration.AppDomain.CastleSectionHandler, Castle.Windsor" /&gt;
	///   &lt;/configSections&gt;
	///   
	///   &lt;castle&gt;
	///     &lt;facilities&gt;
	///       &lt;facility id="myfacility"&gt;
	///     
	///       &lt;/facility&gt;
	///     &lt;/facilities&gt;
	///   
	///     &lt;components&gt;
	///       &lt;component id="component1"&gt;
	///     
	///       &lt;/component&gt;
	///     &lt;/components&gt;
	///   
	///   &lt;/castle&gt;
	///   
	/// &lt;/configuration&gt;
	/// </code>
	/// </summary>
	public class AppDomainConfigurationStore : XmlConfigurationStore
	{
		public AppDomainConfigurationStore()
		{
			XmlNode node = (XmlNode) ConfigurationSettings.GetConfig("castle");

//			Need to decide if this is going to be considered an error or not
//			if (node == null)
//			{
//				String message = String.Format(
//					"Could not find section 'castle' in the configuration file associated with this domain.");
//				throw new ConfigurationException(message);
//			}

			if (node == null) return;

			Deserialize( node );
		}
	}
}
