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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Model;

	/// <summary>
	/// Implements the basis of <see cref="IHandler"/>
	/// </summary>
	[Serializable]
	public abstract class AbstractHandler : MarshalByRefObject, IHandler, IExposeDependencyInfo, IDisposable
	{
		private IKernel kernel;
		private ComponentModel model;
		private HandlerState state;
		private ArrayList dependenciesByService; 
		private IDictionary dependenciesByKey; 

		protected ILifestyleManager lifestyleManager;
		
		public AbstractHandler(ComponentModel model)
		{
			this.model = model;
			this.state = HandlerState.Valid;
		}

		#region IHandler Members

		public virtual void Init(IKernel kernel)
		{
			this.kernel = kernel;
			this.kernel.AddedAsChildKernel += new EventHandler(OnAddedAsChildKernel);

			IComponentActivator activator = kernel.CreateComponentActivator(ComponentModel);
			
			lifestyleManager = CreateLifestyleManager(activator);

			EnsureDependenciesCanBeSatisfied();
		}

		public abstract object Resolve();

		public abstract void Release(object instance);

		public HandlerState CurrentState
		{
			get { return state; }
		}

		public ComponentModel ComponentModel
		{
			get { return model; }
		}
		
		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			lifestyleManager.Dispose();
		}

		#endregion

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		protected void SetNewState( HandlerState newState )
		{
			state = newState;
		}

		protected ArrayList DependenciesByService
		{
			get
			{
				if (dependenciesByService == null) 
				{
					dependenciesByService = new ArrayList();
				}
				return dependenciesByService;
			}
		}

		protected IDictionary DependenciesByKey
		{
			get
			{
				if (dependenciesByKey == null) 
				{
					dependenciesByKey = new HybridDictionary();
				}
				return dependenciesByKey;
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
			else if (ComponentModel.LifestyleType == LifestyleType.Pooled)
			{
				int initial = ExtendedPropertiesConstants.Pool_Default_InitialPoolSize;
				int maxSize = ExtendedPropertiesConstants.Pool_Default_MaxPoolSize;

				if (ComponentModel.ExtendedProperties.Contains(ExtendedPropertiesConstants.Pool_InitialPoolSize))
				{
					initial = (int) ComponentModel.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize];
				}
				if (ComponentModel.ExtendedProperties.Contains(ExtendedPropertiesConstants.Pool_MaxPoolSize))
				{
					maxSize = (int) ComponentModel.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize];
				}

				manager = new Lifestyle.PoolableLifestyleManager(initial, maxSize);
			}

			manager.Init( activator, Kernel );

			return manager;
		}

		/// <summary>
		/// Invoked by the kernel
		/// when one of registered dependencies were satisfied by 
		/// new components registered.
		/// </summary>
		/// <param name="handler"></param>
		protected virtual void DependencySatisfied(IHandler handler, ref bool stateChanged)
		{
			Type[] services = (Type[]) DependenciesByService.ToArray( typeof(Type) );

			foreach(Type service in services)
			{
				if (HasValidComponent(service))
				{
					DependenciesByService.Remove(service);
					AddGraphDependency(handler.ComponentModel);
				}
			}

			foreach(String compKey in DependenciesByKey.Keys)
			{
				if (HasValidComponent(compKey))
				{
					DependenciesByKey.Remove(compKey);
					AddGraphDependency(handler.ComponentModel);
				}
			}

			if (DependenciesByService.Count == 0 && DependenciesByKey.Count == 0)
			{
				SetNewState(HandlerState.Valid); stateChanged = true;
				Kernel.HandlerRegistered -= new HandlerDelegate(DependencySatisfied);

				// All components with dependencies (that happened to be 
				
				// We don't need these anymore
				dependenciesByKey = null;
				dependenciesByService = null;
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
				if (dependency.TargetType == typeof(IKernel)) return;
				if (HasValidComponent(dependency.TargetType))
				{
					AddGraphDependency(Kernel.GetHandler(dependency.TargetType).ComponentModel);
					return;
				}

				DependenciesByService.Add(dependency.TargetType);
			}
			else
			{
				if (HasValidComponent(dependency.DependencyKey))
				{
					AddGraphDependency(Kernel.GetHandler(dependency.DependencyKey).ComponentModel);
					return;
				}
				
				DependenciesByKey.Add(dependency.DependencyKey, String.Empty);
			}

			// This handler is considered invalid
			// until dependencies are satisfied
			SetNewState(HandlerState.WaitingDependency);

			// Register itself on the kernel
			// to be notified if the dependency is satified
			Kernel.HandlerRegistered += new HandlerDelegate(DependencySatisfied);
		}

		private bool HasValidComponent(Type service)
		{
			return IsValidHandlerState( kernel.GetHandler(service) );
		}

		private bool HasValidComponent(String key)
		{
			return IsValidHandlerState( kernel.GetHandler(key) );
		}

		private bool IsValidHandlerState(IHandler handler)
		{
			if (handler == null) return false;

			return handler.CurrentState == HandlerState.Valid;
		}

		protected void OnAddedAsChildKernel(object sender, EventArgs e)
		{
			if (DependenciesByKey.Count == 0 && DependenciesByService.Count == 0) return;

			bool stateChanged = false;

			Type[] services = new Type[ DependenciesByService.Count ];
			
			DependenciesByService.CopyTo( services, 0 );

			foreach(Type service in services)
			{
				if (Kernel.Parent.HasComponent(service))
				{
					IHandler handler = Kernel.Parent.GetHandler(service);
					DependencySatisfied(handler, ref stateChanged);
				}
			}

			String[] keys = new String[ DependenciesByKey.Count ];
			
			DependenciesByKey.Keys.CopyTo( services, 0 );

			foreach(String key in keys)
			{
				if (Kernel.Parent.HasComponent(key))
				{
					IHandler handler = Kernel.Parent.GetHandler(key);
					DependencySatisfied(handler, ref stateChanged);
				}
			}
		}

		private void AddGraphDependency(ComponentModel model)
		{
			ComponentModel.AddDependent( model );
		}

		/// <summary>
		/// Returns human readable list of dependencies 
		/// this handler is waiting for.
		/// </summary>
		/// <returns></returns>
		public String ObtainDependencyDetails()
		{
			if (this.CurrentState == HandlerState.Valid) return String.Empty;

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat( "\r\n{0} is waiting for the following dependencies: \r\n", ComponentModel.Name );

			if (DependenciesByService.Count != 0)
			{
				sb.Append( "\r\nServices: \r\n" );

				foreach(Type type in DependenciesByService)
				{
					IHandler handler = Kernel.GetHandler( type );

					if (handler == null)
					{
						sb.AppendFormat( "- {0} which was not registered. \r\n", type.FullName );
					}
					else
					{
						sb.AppendFormat( "- {0} which was registered but is also waiting for dependencies. \r\n", type.FullName );

						IExposeDependencyInfo info = handler as IExposeDependencyInfo;
						
						if (info != null)
						{
							sb.Append( info.ObtainDependencyDetails() );
						}
					}
				}
			}

			if (DependenciesByKey.Count != 0)
			{
				sb.Append( "\r\nKeys (components with specific keys)\r\n" );

				foreach(String key in DependenciesByKey)
				{
					IHandler handler = Kernel.GetHandler( key );

					if (handler == null)
					{
						sb.AppendFormat( "- {0} which was not registered. \r\n", key );
					}
					else
					{
						sb.AppendFormat( "- {0} which was registered but is also waiting for dependencies. \r\n", key );

						IExposeDependencyInfo info = handler as IExposeDependencyInfo;
						
						if (info != null)
						{
							sb.Append( info.ObtainDependencyDetails() );
						}
					}
				}
			}

			return sb.ToString();
		}
	}
}
