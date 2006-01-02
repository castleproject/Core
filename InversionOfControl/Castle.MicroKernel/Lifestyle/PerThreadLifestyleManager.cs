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

namespace Castle.MicroKernel.Lifestyle
{
	using System;
	using System.Collections;
	using System.Threading;
	using System.Runtime.Serialization;

	/// <summary>
	/// Summary description for PerThreadLifestyleManager.
	/// </summary>
	[Serializable]
	public class PerThreadLifestyleManager : AbstractLifestyleManager, IDeserializationCallback
	{
		[NonSerialized]
		private static LocalDataStoreSlot slot = Thread.AllocateNamedDataSlot("CastlePerThread");

		[NonSerialized]
		private IList instances = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		public override void Dispose()
		{
			foreach( object instance in instances )
			{
				base.Release( instance );
			}

			instances.Clear();

			Thread.FreeNamedDataSlot( "CastlePerThread" );
		}

		#region IResolver Members

		public override object Resolve()
		{
			lock(slot)
			{
				Hashtable map = (Hashtable) Thread.GetData( slot );

				if (map == null)
				{
					map = new Hashtable();

					Thread.SetData( slot, map );
				}

				Object instance = map[ ComponentActivator ];

				if ( instance == null )
				{
					instance = base.Resolve();
					map.Add( ComponentActivator, instance );
					instances.Add( instance );
				}

				return instance;
			}
		}

		public override void Release( object instance )
		{
			// Do nothing.
		}

		#endregion

		public void OnDeserialization(object sender)
		{
			slot = Thread.AllocateNamedDataSlot("CastlePerThread");
			instances = new ArrayList();
		}
	}
}
