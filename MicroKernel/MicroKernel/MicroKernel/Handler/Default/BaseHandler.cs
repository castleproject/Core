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

namespace Castle.MicroKernel.Handler.Default
{
	using System;

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Factory.Default;

	/// <summary>
	/// Provides a fully functional component handler.
	/// </summary>
	public class BaseHandler : AbstractHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public BaseHandler(IComponentModel model) : base(model)
		{
		}

		#region IHandler Members

		public override void Init(IKernel kernel)
		{
			base.Init(kernel);

			// Now we check with the kernel if 
			// we have the necessary implementations 
			// for the services requested by the constructor

			EnsureDependenciesCanBeSatisfied();
			CreateComponentFactoryAndLifestyleManager();
		}

		#endregion

		protected virtual void EnsureDependenciesCanBeSatisfied()
		{
			foreach(IDependencyModel dependency in ComponentModel.Dependencies)
			{
				AddDependency(dependency.Service);
			}
		}

		protected virtual void AddDependency(Type service)
		{
			if (Kernel.HasService(service))
			{
				m_serv2handler[ service ] = Kernel.GetHandlerForService(service);
			}
			else
			{
				// This handler is considered invalid
				// until dependencies are satisfied
				SetNewState(State.WaitingDependency);
				Dependencies.Add(service);

				// Register ourself in the kernel
				// to be notified if the dependency is satified
				Kernel.AddDependencyListener(
					service,
					new DependencyListenerDelegate(DependencySatisfied));
			}
		}

		/// <summary>
		/// Delegate implementation invoked by kernel
		/// when one of registered dependencies were satisfied by 
		/// new components registered.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="handler"></param>
		private void DependencySatisfied(Type service, IHandler handler)
		{
			m_serv2handler[ service ] = handler;

			Dependencies.Remove(service);

			if (Dependencies.Count == 0)
			{
				SetNewState(State.Valid);
			}
		}

		protected virtual void CreateComponentFactoryAndLifestyleManager()
		{
			IComponentFactory factory = new BaseComponentFactory(
				Kernel, this, ComponentModel, m_serv2handler);

			m_lifestyleManager = Kernel.LifestyleManagerFactory.Create(
				factory, ComponentModel);
		}
	}
}