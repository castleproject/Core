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

namespace Castle.MicroKernel.Concerns
{
	using System;
	using System.Collections;

	using Castle.MicroKernel.Concerns.Default;

	/// <summary>
	/// Summary description for ConcernManager.
	/// </summary>
	public class ConcernManager
	{
		private Type m_creationConcern;

		private Type m_destructionConcern;

		private IList m_commissionConcerns = new ArrayList();

		private IList m_decommissionConcerns = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		public ConcernManager()
		{
			// Configure the default behavior
			Add( typeof(CreationConcern) );
			Add( typeof(EnableLoggerConcern) );
			Add( typeof(ContextConcern) );
			Add( typeof(EnableLookupConcern) );
			Add( typeof(ConfigureConcern) );
			Add( typeof(InitializeConcern) );
			Add( typeof(StartConcern) );
			Add( typeof(ShutdownConcern) );
			Add( typeof(DestructionConcern) );
		}

		public void Add( Type concern )
		{
			AssertUtil.ArgumentNotNull( concern, "concern" );

			if ( typeof(ICreationConcern).IsAssignableFrom(concern) )
			{
				m_creationConcern = concern;
			}
			else if ( typeof(IDestructionConcern).IsAssignableFrom(concern) )
			{
				m_destructionConcern = concern;
			}
			else if ( typeof(ICommissionConcern).IsAssignableFrom(concern) )
			{
				m_commissionConcerns.Add( concern );
			}
			else if ( typeof(IDecommissionConcern).IsAssignableFrom(concern) )
			{
				m_decommissionConcerns.Add( concern );
			}
			else
			{
				throw new ArgumentException(
					"Concern implementation must implement " + 
					"ICreationConcern, ICommissionConcern or IDecommissionConcern", 
					"concern");
			}
		}

		public IList CommissionConcerns
		{
			get
			{
				return m_commissionConcerns;
			}
		}

		public IList DecommissionConcerns
		{
			get
			{
				return m_decommissionConcerns;
			}
		}

		public IConcern GetCommissionChain( IKernel kernel )
		{
			ArrayList concerns = new ArrayList( m_commissionConcerns );
			concerns.Insert( 0, m_creationConcern );
			Type[] concernTypes = (Type[]) concerns.ToArray( typeof(Type) );
			Array.Reverse( concernTypes );

			IConcern next = null;
			foreach( Type concernType in concernTypes )
			{
				next = (IConcern) 
					Activator.CreateInstance( concernType, new object[] { next } );
				next.Init( kernel );
			}

			return next;
		}

		public IConcern GetDecommissionChain( IKernel kernel )
		{
			ArrayList concerns = new ArrayList( m_decommissionConcerns );
			concerns.Add( m_destructionConcern );
			Type[] concernTypes = (Type[]) concerns.ToArray( typeof(Type) );
			Array.Reverse( concernTypes );

			IConcern next = null;
			foreach( Type concernType in concernTypes )
			{
				next = (IConcern) 
					Activator.CreateInstance( concernType, new object[] { next } );
				next.Init( kernel );
			}

			return next;
		}
	}
}
