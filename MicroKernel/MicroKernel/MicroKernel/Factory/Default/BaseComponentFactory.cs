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
	using System.Collections;

	using Castle.MicroKernel.Assemble;
	using Castle.MicroKernel.Handler;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// Summary description for SimpleComponentFactory.
	/// </summary>
	public class BaseComponentFactory : IComponentFactory
	{
		private Hashtable m_instance2Burden = new Hashtable();
		protected IKernel m_kernel;
		protected IHandler m_handler;
		protected IComponentModel m_componentModel;
		protected Hashtable m_serv2handler;

		public BaseComponentFactory(IKernel kernel, IHandler handler,
		                            IComponentModel componentModel,
		                            Hashtable serv2handler)
		{
			AssertUtil.ArgumentNotNull(kernel, "kernel");
			AssertUtil.ArgumentNotNull(componentModel, "componentModel");
			AssertUtil.ArgumentNotNull(serv2handler, "serv2handler");

			m_kernel = kernel;
			m_handler = handler;
			m_componentModel = componentModel;
			m_serv2handler = serv2handler;
		}

		#region IComponentFactory Members

		public virtual Object Incarnate()
		{
			try
			{
				ComponentInstanceBurden burden = new ComponentInstanceBurden();

				object[] arguments = BuildArguments(burden);

				Object instance = Activator.CreateInstance(m_componentModel.ConstructionModel.Implementation, arguments);

				SetupProperties(instance, burden);
				AssociateBurden(instance, burden);
				instance = RaiseWrapEvent(instance);
				RaiseComponentReadyEvent(instance);

				return instance;
			}
			catch(Exception ex)
			{
				throw new HandlerException("Exception while attempting to instantiate type", ex);
			}
		}

		public virtual void Etherialize(object instance)
		{
			if (instance == null)
			{
				return;
			}

			RaiseComponentReleasedEvent(instance);
			instance = RaiseUnWrapEvent(instance);
			ReleaseBurden(instance);
		}

		#endregion

		private void AssociateBurden(object instance, ComponentInstanceBurden burden)
		{
			if (burden.HasBurden)
			{
				m_instance2Burden.Add(instance, burden);
			}
		}

		private void ReleaseBurden(object instance)
		{
			if (m_instance2Burden.ContainsKey(instance))
			{
				ComponentInstanceBurden burden =
					(ComponentInstanceBurden) m_instance2Burden[ instance ];

				burden.ReleaseBurden();

				m_instance2Burden.Remove(instance);
			}
		}

		protected virtual object[] BuildArguments(ComponentInstanceBurden burden)
		{
			return Assembler.BuildConstructorArguments(
				m_componentModel, burden, new ResolveTypeHandler(ResolveType));
		}

		protected virtual void SetupProperties(object instance, ComponentInstanceBurden burden)
		{
			Assembler.AssembleProperties(
				instance, m_componentModel, burden, new ResolveTypeHandler(ResolveType));
		}

		private void ResolveType(IComponentModel model, Type typeRequest,
		                         String argumentOrPropertyName, object key, out object value)
		{
			value = null;

			if (m_serv2handler.ContainsKey(typeRequest))
			{
				IHandler handler = (IHandler) m_serv2handler[ typeRequest ];
				value = handler.Resolve();

				ComponentInstanceBurden burden = (ComponentInstanceBurden) key;
				burden.AddBurden(value, handler);
			}
		}

		protected virtual object RaiseWrapEvent(object instance)
		{
			return m_kernel.RaiseWrapEvent(m_handler, instance);
		}

		protected virtual object RaiseUnWrapEvent(object instance)
		{
			return m_kernel.RaiseUnWrapEvent(m_handler, instance);
		}

		protected virtual void RaiseComponentReadyEvent(object instance)
		{
			m_kernel.RaiseComponentReadyEvent(m_handler, instance);
		}

		protected virtual void RaiseComponentReleasedEvent(object instance)
		{
			m_kernel.RaiseComponentReleasedEvent(m_handler, instance);
		}
	}
}