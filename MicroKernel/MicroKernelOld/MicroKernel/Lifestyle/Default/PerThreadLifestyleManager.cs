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

namespace Castle.MicroKernel.Lifestyle.Default
{
	using System;
	using System.Collections;
	using System.Threading;

	/// <summary>
	/// Summary description for PerThreadLifestyleManager.
	/// </summary>
	public class PerThreadLifestyleManager : AbstractLifestyleManager
	{
		private static LocalDataStoreSlot m_slot = Thread.AllocateNamedDataSlot("CastlePerThread");

		private static IList m_instances = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentFactory"></param>
		public PerThreadLifestyleManager(IComponentFactory componentFactory) : base(componentFactory)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Dispose()
		{
			foreach( object instance in m_instances )
			{
				base.Release( instance );
			}

			m_instances.Clear();

			Thread.FreeNamedDataSlot( "CastlePerThread" );
		}

		#region IResolver Members

		public override object Resolve()
		{
			lock(m_slot)
			{
				Hashtable map = (Hashtable) Thread.GetData( m_slot );

				if (map == null)
				{
					map = new Hashtable();

					Thread.SetData( m_slot, map );
				}

				Object instance = map[ m_componentFactory ];

				if ( instance == null )
				{
					instance = base.Resolve();
					map.Add( m_componentFactory, instance );
					m_instances.Add( instance );
				}

				return instance;
			}
		}

		public override void Release( object instance )
		{
			// Do nothing.
		}

		#endregion
	}
}
