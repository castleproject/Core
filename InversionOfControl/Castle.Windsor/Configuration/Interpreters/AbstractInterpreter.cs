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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Configuration;

	using Castle.MicroKernel;

	using Castle.Model.Configuration;

	using Castle.Windsor.Configuration.Sources;
	using Castle.Windsor.Configuration.Interpreters.CastleLanguage;

	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractInterpreter : IConfigurationInterpreter
	{
		protected static readonly String FacilitiesNodeName = "facilities";
		protected static readonly String FacilityNodeName = "facility";
		protected static readonly String ComponentsNodeName = "components";
		protected static readonly String ComponentNodeName = "component";

		private IConfigurationSource _source;
		private ImportDirectiveCollection _imports = new ImportDirectiveCollection();

		public AbstractInterpreter(IConfigurationSource source)
		{
			if (source == null) throw new ArgumentNullException("source", "IConfigurationSource is null");

			_source = source;
		} 

		public AbstractInterpreter(String filename) : this(new ExternalFileSource(filename))
		{
		} 

		public AbstractInterpreter() : this(new AppDomainConfigSource())
		{
		}

		public IConfigurationSource Source
		{
			get { return _source; }
		}

		public ImportDirectiveCollection Imports
		{
			get { return _imports; }
			set { _imports = value; }
		}

		protected void AddFacilityConfig(IConfiguration facility, IConfigurationStore store)
		{
			AddFacilityConfig( facility.Attributes["id"], facility, store );
		}

		protected void AddComponentConfig(IConfiguration component, IConfigurationStore store)
		{
			AddComponentConfig( component.Attributes["id"], component, store );
		}

		protected void AddFacilityConfig(String id, IConfiguration facility, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type attribute (if it exists)

			store.AddFacilityConfiguration( id, facility );
		}

		protected void AddComponentConfig(String id, IConfiguration component, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type and service attribute (if they exist)
			
			store.AddComponentConfiguration( id, component );
		}

		private void AssertValidId(string id)
		{
			if (id == null || id.Length == 0)
			{
				throw new ConfigurationException("Component or Facility was declared without a proper 'id' attribute");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="store"></param>
		public abstract void Process(IConfigurationStore store);
	}
}
