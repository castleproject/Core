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

namespace Castle.MicroKernel.Concerns.Default
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// Summary description for EnableLookupConcern.
	/// </summary>
	public class EnableLookupConcern : AbstractConcern, ICommissionConcern
	{
		public EnableLookupConcern(IConcern next) : base(next)
		{
		}

		public override void Apply(IComponentModel model, object component)
		{
			if (component is ILookupEnabled)
			{
				IDependencyModel[] dependencies = GetLookupDependencies(model);

				LookupManager manager = new LookupManager( m_kernel, dependencies );

				ContainerUtil.Service( component, manager );
			}

			base.Apply( model, component );
		}

		protected virtual IDependencyModel[] GetLookupDependencies(IComponentModel model)
		{
			IDependencyModel[] dependencies = model.Dependencies;
			ArrayList lookupDependencies = new ArrayList();

			foreach(IDependencyModel dependency in dependencies)
			{
				if (dependency.LookupKey == null || dependency.LookupKey == String.Empty)
				{
					continue;
				}

				lookupDependencies.Add( dependency );
			}

			return (IDependencyModel[]) lookupDependencies.ToArray( 
				typeof(IDependencyModel) );
		}

		/// <summary>
		/// 
		/// </summary>
		public class LookupManager : ILookupManager
		{
			IKernel m_kernel;
			HybridDictionary m_key2handler = new HybridDictionary();

			public LookupManager( IKernel kernel, IDependencyModel[] dependencies )
			{
				m_kernel = kernel;
				
				foreach(IDependencyModel dependency in dependencies )
				{
					if (!kernel.HasService( dependency.Service ))
					{
						if (!dependency.Optional)
						{
							throw new LookupException( dependency.LookupKey, "Kernel can't supply specified service.");
						}
						else
						{
							continue;
						}
					}

					m_key2handler[ dependency.LookupKey ] = 
						kernel.GetHandlerForService( dependency.Service );
				}
			}

			#region ILookupManager Members

			public object this[string role]
			{
				get
				{
					if (!Contains( role ))
					{
						throw new LookupException( role, "Key not found." );
					}

					IHandler handler = 
						(IHandler) m_key2handler[ role ];

					object instance = handler.Resolve();

					return instance;
				}
			}

			public void Release(object instance)
			{
				if ( instance == null )
				{
					return;
				}

				foreach(IHandler handler in m_key2handler.Values)
				{
					if (handler.IsOwner( instance ))
					{
						handler.Release( instance );
						return;
					}
				}

				throw new LookupException( 
					"The specified instance was not created by this LookupManager" );
			}

			public bool Contains(string role)
			{
				return m_key2handler.Contains( role );
			}

			public object LookUp(string role, object criteria)
			{
				return null;
				// return m_key2handler.Contains( role );
			}

			#endregion
		}
	}
}
