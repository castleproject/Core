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
		private IList _dependenciesByService; 
		private IDictionary _dependenciesByKey; 

		protected ILifestyleManager _lifestyleManager;
		
		public AbstractHandler(ComponentModel model)
		{
			_model = model;
			_state = HandlerState.Valid;
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

		protected IList DependenciesByService
		{
			get
			{
				if (_dependenciesByService == null) 
				{
					_dependenciesByService = new ArrayList();
				}
				return _dependenciesByService;
			}
		}

		protected IDictionary DependenciesByKey
		{
			get
			{
				if (_dependenciesByKey == null) 
				{
					_dependenciesByKey = new SortedList( Comparer.Default );
				}
				return _dependenciesByKey;
			}
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

			DependenciesByService.Remove(service);
			DependenciesByKey.Remove(key);

			if (DependenciesByService.Count == 0 && DependenciesByKey.Count == 0)
			{
				SetNewState(HandlerState.Valid);

				Kernel.ComponentRegistered -= new ComponentDataDelegate(DependencySatisfied);
			}
		}

		protected virtual void EnsureDependenciesCanBeSatisfied()
		{
			// Custom activators should deal with this case
			if (ComponentModel.Constructors.Count == 0)
			{
				return;
			}

			// The following dependencies were added by - for example - 
			// facilities, for some reason, and we need to satisfy the non-optional

			foreach(DependencyModel dependency in ComponentModel.Dependencies)
			{
				if (!dependency.IsOptional && dependency.DependencyType == DependencyType.Service)
				{
					if (dependency.TargetType != null)
					{
						AddDependency(dependency);
					}
				}
			}

			// We need to satisfy at least the constructor 
			// with fewer arguments

			ConstructorCandidate candidate = ComponentModel.Constructors.FewerArgumentsCandidate;

			foreach(DependencyModel dependency in candidate.Dependencies)
			{
				if (dependency.DependencyType == DependencyType.Service)
				{
					AddDependency(dependency);
				}
			}
		}

		protected virtual void AddDependency(DependencyModel dependency)
		{
			if (dependency.TargetType != null)
			{
				if (Kernel.HasComponent(dependency.TargetType))
				{
					return;
				}

				DependenciesByService.Add(dependency.TargetType);
			}
			else
			{
				if (Kernel.HasComponent(dependency.DependencyKey))
				{
					return;
				}
				
				DependenciesByKey.Add(dependency.DependencyKey, String.Empty);
			}

			// This handler is considered invalid
			// until dependencies are satisfied
			SetNewState(HandlerState.WaitingDependency);

			// Register itself on the kernel
			// to be notified if the dependency is satified
			Kernel.ComponentRegistered += new ComponentDataDelegate(DependencySatisfied);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnAddedAsChildKernel(object sender, EventArgs e)
		{
			if (DependenciesByKey.Count == 0 && DependenciesByService.Count == 0) return;

			Type[] services = new Type[ DependenciesByService.Count ];
			
			DependenciesByService.CopyTo( services, 0 );

			foreach(Type service in services)
			{
				if (Kernel.Parent.HasComponent(service))
				{
					IHandler handler = Kernel.Parent.GetHandler(service);
					DependencySatisfied(handler.ComponentModel.Name, handler);
				}
			}

			String[] keys = new String[ DependenciesByKey.Count ];
			
			DependenciesByKey.Keys.CopyTo( services, 0 );

			foreach(String key in keys)
			{
				if (Kernel.Parent.HasComponent(key))
				{
					IHandler handler = Kernel.Parent.GetHandler(key);
					DependencySatisfied(handler.ComponentModel.Name, handler);
				}
			}
		}
	}
}
