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

namespace Castle.MicroKernel.Factory.Default
{
	using System;

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Concerns;

	/// <summary>
	/// Sits on the place of a standard <see cref="IComponentFactory"/>
	/// but makes the process of construction and destruction pass through 
	/// a Concern Chain. 
	/// <para>The actual construction should happen when the CreationConcern
	/// invokes the delegate factory. The same for the destruction phase.</para>
	/// </summary>
	/// <remarks>
	/// It is important to invoke the delegate factory for creation and destruction 
	/// of a component instance as it will fire the proper events in the correct order.
	/// </remarks>
	public class ConcernChainComponentFactory : IComponentFactory
	{
		private IConcern m_commissionChain;
		private IConcern m_decomissionChain;
		private IComponentModel m_model;
		private IComponentFactory m_delegateFactory;

		public ConcernChainComponentFactory(
			IConcern commissionChain, IConcern decomissionChain, 
			IComponentModel model, IComponentFactory delegateFactory)
		{
			AssertUtil.ArgumentNotNull( commissionChain, "commissionChain" );
			AssertUtil.ArgumentNotNull( decomissionChain, "decomissionChain" );
			AssertUtil.ArgumentNotNull( model, "model" );
			AssertUtil.ArgumentNotNull( delegateFactory, "delegateFactory" );

			m_commissionChain = commissionChain;
			m_decomissionChain = decomissionChain;
			m_model = model;
			m_delegateFactory = delegateFactory;
		}

		#region IComponentFactory Members

		public Object Incarnate()
		{
			ICreationConcern creationConcern = (ICreationConcern) m_commissionChain;

			object instance = creationConcern.Apply( m_model, m_delegateFactory );

			creationConcern.Apply( m_model, instance );

			return instance;
		}

		public void Etherialize( object instance )
		{
			m_decomissionChain.Apply( m_model, instance );

			IDecommissionConcern concern = (IDecommissionConcern) m_decomissionChain;

			while( true )
			{
				if (concern is IDestructionConcern)
				{
					(concern as IDestructionConcern).Apply( m_model, m_delegateFactory, instance );
					break;
				}
				
				concern = concern.Next as IDecommissionConcern;

				if (concern == null)
				{
					// IDestructionConcern not found?
					break;
				}
			}
		}

		#endregion
	}
}
