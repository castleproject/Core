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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections;

	using Castle.Model;


	/// <summary>
	/// Summary description for AbstractHandler.
	/// </summary>
	public abstract class AbstractHandler : IHandler, IDisposable
	{
		private IKernel _kernel;
		private ComponentModel _model;
		private HandlerState _state;
		private IList _dependencies; 

		protected ILifestyleManager _lifestyleManager;

		
		public AbstractHandler(ComponentModel model)
		{
			_model = model;
			_state = HandlerState.Valid;
			_dependencies = new ArrayList();
		}

		#region IHandler Members

		public virtual void Init(IKernel kernel)
		{
			_kernel = kernel;
			_kernel.AddedAsChildKernel += new EventHandler(OnAddedAsChildKernel);

			IComponentActivator activator = _kernel.CreateComponentActivator(ComponentModel);
			
			_lifestyleManager = CreateLifestyleManager(activator);

			EnsureDependenciesCanBeSatisfied();
		}

		public abstract object Resolve();

		public abstract void Release(object instance);

		public HandlerState CurrentState
		{
			get { return _state; }
		}

		public ComponentModel ComponentModel
		{
			get { return _model; }
		}
		
		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			_lifestyleManager.Dispose();
		}

		#endregion

		protected IKernel Kernel
		{
			get { return _kernel; }
		}

		protected void SetNewState( HandlerState newState )
		{
			_state = newState;

			// RaiseChangeStateEvent();
		}

		protected IList Dependencies
		{
			get { return _dependencies; }
		}

		protected virtual ILifestyleManager CreateLifestyleManager(IComponentActivator activator)
		{
			ILifestyleManager manager = null;

			if (ComponentModel.LifestyleType == LifestyleType.Undefined
				|| ComponentModel.LifestyleType == LifestyleType.Singleton)
			{
				manager = new Lifestyle.SingletonLifestyleManager();
			}
			else if (ComponentModel.LifestyleType == LifestyleType.Thread)
			{
				manager = new Lifestyle.PerThreadLifestyleManager();
			}
			else if (ComponentModel.LifestyleType == LifestyleType.Transient)
			{
				manager = new Lifestyle.TransientLifestyleManager();
			}
			else if (ComponentModel.LifestyleType == LifestyleType.Custom)
			{
				manager = (ILifestyleManager) 
					Activator.CreateInstance( ComponentModel.CustomLifestyle );
			}

			manager.Init( activator );

			return manager;
		}

		/// <summary>
		/// Invoked by the kernel
		/// when one of registered dependencies were satisfied by 
		/// new components registered.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="handler"></param>
		protected virtual void DependencySatisfied(String key, IHandler handler)
		{
			Type service = handler.ComponentModel.Service;

			Dependencies.Remove(service);

			if (Dependencies.Count == 0)
			{
				SetNewState(HandlerState.Valid);

				Kernel.ComponentRegistered -= new ComponentDataDelegate(DependencySatisfied);
			}
		}

		protected virtual void EnsureDependenciesCanBeSatisfied()
		{
			// We need to satisfy at least the constructor 
			// with fewer arguments

			ConstructorCandidate candidate = ComponentModel.Constructors.FewerArgumentsCandidate;

			foreach(DependencyModel dependency in candidate.Dependencies)
			{
				if (dependency.DependencyType == DependencyType.Service)
				{
					AddDependency(dependency.Service);
				}
			}
		}

		protected virtual void AddDependency(Type service)
		{
			if (!Kernel.HasComponent(service))
			{
				// This handler is considered invalid
				// until dependencies are satisfied
				SetNewState(HandlerState.WaitingDependency);
				Dependencies.Add(service);

				// Register itself on the kernel
				// to be notified if the dependency is satified
				Kernel.ComponentRegistered += new ComponentDataDelegate(DependencySatisfied);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnAddedAsChildKernel(object sender, EventArgs e)
		{
			if (Dependencies.Count == 0) return;

			Type[] services = new Type[ Dependencies.Count ];
			
			Dependencies.CopyTo( services, 0 );

			foreach(Type service in services)
			{
				if (Kernel.Parent.HasComponent(service))
				{
					IHandler handler = Kernel.Parent.GetHandler(service);
					DependencySatisfied(handler.ComponentModel.Name, handler);
				}
			}
		}
	}
}
