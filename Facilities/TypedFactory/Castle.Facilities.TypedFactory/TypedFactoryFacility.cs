using System.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
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

namespace Castle.Facilities.TypedFactory
{
	using System;

	using Castle.Model;
	using Castle.Model.Configuration;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for TypedFactoryFacility.
	/// </summary>
	public class TypedFactoryFacility : IFacility
	{
		private IKernel _kernel;

		public void AddTypedFactoryEntry( FactoryEntry entry )
		{
			ComponentModel model = 
				new ComponentModel(entry.Id, entry.FactoryInterface, typeof(Empty));
			
			model.LifestyleType = LifestyleType.Singleton;
			model.ExtendedProperties["typed.fac.entry"] = entry;
			model.Interceptors.Add( new InterceptorReference( typeof(FactoryInterceptor) ) );

			_kernel.AddCustomComponent( model );
		}

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			_kernel = kernel;

			_kernel.AddComponent( "typed.fac.interceptor", typeof(FactoryInterceptor) );

			ITypeConverter converter = (ITypeConverter)
				_kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );

			AddFactories(facilityConfig, converter);
		}

		protected virtual void AddFactories(IConfiguration facilityConfig, ITypeConverter converter)
		{
			if (facilityConfig != null)
			{
				foreach(IConfiguration config in facilityConfig.Children["factories"].Children)
				{
					String id = config.Attributes["id"];
					String creation = config.Attributes["creation"];
					String destruction = config.Attributes["destruction"];
					Type factoryType = (Type)
						converter.PerformConversion( config.Attributes["interface"], typeof(Type) );

					try
					{
						AddTypedFactoryEntry( 
							new FactoryEntry(id, factoryType, creation, destruction) );
					}
					catch(Exception ex)
					{
						throw new ConfigurationException("Invalid factory entry in configuration", ex);
					}
				}
			}
		}

		public void Terminate()
		{
		}
	}

	public class Empty
	{
		
	}

	public class FactoryEntry
	{
		private String _id;
		private Type _factoryInterface;
		private String _creationMethod;
		private String _destructionMethod;

		public FactoryEntry(String id, Type factoryInterface, String creationMethod, String destructionMethod)
		{
			if (id == null || id.Length == 0) throw new ArgumentNullException("id");
			if (factoryInterface == null) throw new ArgumentNullException("factoryInterface");
			if (!factoryInterface.IsInterface) throw new ArgumentException("factoryInterface must be an interface");
			if (creationMethod == null || creationMethod.Length == 0) throw new ArgumentNullException("creationMethod");

			_id = id;
			_factoryInterface = factoryInterface;
			_creationMethod = creationMethod;
			_destructionMethod = destructionMethod;
		}

		public String Id
		{
			get { return _id; }
		}

		public Type FactoryInterface
		{
			get { return _factoryInterface; }
		}

		public String CreationMethod
		{
			get { return _creationMethod; }
		}

		public String DestructionMethod
		{
			get { return _destructionMethod; }
		}
	}
}
