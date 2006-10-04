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

	using Castle.Core;
	using Castle.MicroKernel;
	
	/// <summary>
	/// Implements the basis of <see cref="IHandler"/>
    /// </summary>
#if DOTNET2
    [System.Diagnostics.DebuggerDisplay("Model: {ComponentModel.Service} / {ComponentModel.Implementation} ")]
#endif
    [Serializable]
	public abstract class AbstractHandler : MarshalByRefObject, IHandler, IExposeDependencyInfo, IDisposable
	{
		private readonly ComponentModel model;
		private IKernel kernel;
		private HandlerState state;
		/// <summary>
		/// Dictionary of Type to <see cref="DependencyModel"/>
		/// </summary>
    	private IDictionary dependenciesByService;
		/// <summary>
		/// Dictionary of key (string) to <see cref="DependencyModel"/>
		/// </summary>
		private IDictionary dependenciesByKey;
    	/// <summary>
    	/// Custom dependencies values associated with the handler
    	/// </summary>
		private IDictionary customParameters;
    	/// <summary>
    	/// Lifestyle manager instance
    	/// </summary>
		protected ILifestyleManager lifestyleManager;

    	/// <summary>
    	/// Constructs and initializes the handler
    	/// </summary>
    	/// <param name="model"></param>
    	public AbstractHandler(ComponentModel model)
		{
			this.model = model;
			state = HandlerState.Valid;
			customParameters = new HybridDictionary(true);
		}

		#region IHandler Members

    	/// <summary>
    	/// Saves the kernel instance, subscribes to 
		/// <see cref="IKernelEvents.AddedAsChildKernel"/> event,
		/// creates the lifestyle manager instance and computes
		/// the handler state.
		/// </summary>
    	/// <param name="kernel"></param>
		public virtual void Init(IKernel kernel)
		{
			this.kernel = kernel;
			this.kernel.AddedAsChildKernel += new EventHandler(OnAddedAsChildKernel);

			IComponentActivator activator = kernel.CreateComponentActivator(ComponentModel);
			
			lifestyleManager = CreateLifestyleManager(activator);

			EnsureDependenciesCanBeSatisfied();
		}

    	/// <summary>
    	/// Should be implemented by derived classes: 
    	/// returns an instance of the component this handler
    	/// is responsible for
    	/// </summary>
    	/// <param name="context"></param>
    	/// <returns></returns>
		public abstract object Resolve(CreationContext context);

    	/// <summary>
		/// Should be implemented by derived classes: 
		/// disposes the component instance (or recycle it)
    	/// </summary>
    	/// <param name="instance"></param>
		public abstract void Release(object instance);

    	/// <summary>
    	/// Gets the handler state.
    	/// </summary>
		public HandlerState CurrentState
		{
			get { return state; }
		}

    	/// <summary>
    	/// Gets the component model.
    	/// </summary>
		public ComponentModel ComponentModel
		{
			get { return model; }
		}

    	/// <summary>
    	/// TODO: Pendent
    	/// </summary>
    	/// <param name="key"></param>
    	/// <param name="value"></param>
    	public void AddCustomDependencyValue(string key, object value)
    	{
			if (customParameters == null)
			{
				customParameters = new HybridDictionary(true);
			}

			customParameters[key] = value;
			RaiseHandlerStateChanged();
    	}

		/// <summary>
		/// TODO: Pendent
		/// </summary>
		/// <param name="key"></param>
    	/// <returns></returns>
    	public bool HasCustomParameter(string key)
    	{
			if (key == null) return false;
			
    		if (customParameters != null)
    		{
				return customParameters.Contains(key);
    		}

			return false;
    	}

		/// <summary>
		/// TODO: Pendent
		/// </summary>
		/// <param name="key"></param>
    	public void RemoveCustomDependencyValue(string key)
    	{
			if (customParameters != null)
			{
				customParameters.Remove(key);
				RaiseHandlerStateChanged();
			}
		}

    	/// <summary>
    	/// 
    	/// </summary>
		public event HandlerStateDelegate OnHandlerStateChanged;

    	#endregion

		#region ISubDependencyResolver Members

		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
    	{
			return customParameters[dependency.DependencyKey];
    	}

		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
    	{
			if (dependency.DependencyKey == null) return false;
			
			return HasCustomParameter(dependency.DependencyKey);
    	}

    	#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			lifestyleManager.Dispose();
		}

		#endregion

		#region IExposeDependencyInfo Members

		/// <summary>
		/// Returns human readable list of dependencies 
		/// this handler is waiting for.
		/// </summary>
		/// <returns></returns>
		public String ObtainDependencyDetails(IList dependenciesChecked)
		{
			if (CurrentState == HandlerState.Valid) return String.Empty;

			if (dependenciesChecked.Contains(this)) return String.Empty;

			dependenciesChecked.Add(this);

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("\r\n{0} is waiting for the following dependencies: \r\n", ComponentModel.Name);

			if (DependenciesByService.Count != 0)
			{
				sb.Append("\r\nServices: \r\n");

				foreach(Type type in DependenciesByService.Keys)
				{
					IHandler handler = Kernel.GetHandler(type);

					if (handler == null)
					{
						sb.AppendFormat("- {0} which was not registered. \r\n", type.FullName);
					}
					else if (handler == this)
					{
						sb.AppendFormat("- {0}. \r\n  A dependency cannot be satisfied by itself, " +
							"did you forget to add a parameter name to differentiate between the " +
							"two dependencies? \r\n", type.FullName);
					}
					else
					{
						sb.AppendFormat("- {0} which was registered but is also waiting for " +
							"dependencies. \r\n", type.FullName);

						IExposeDependencyInfo info = handler as IExposeDependencyInfo;

						if (info != null)
						{
							sb.Append(info.ObtainDependencyDetails(dependenciesChecked));
						}
					}
				}
			}

			if (DependenciesByKey.Count != 0)
			{
				sb.Append("\r\nKeys (components with specific keys)\r\n");

				foreach(DictionaryEntry entry in DependenciesByKey)
				{
					String key = entry.Key.ToString();

					IHandler handler = Kernel.GetHandler(key);

					if (handler == null)
					{
						sb.AppendFormat("- {0} which was not registered. \r\n", key);
					}
					else
					{
						sb.AppendFormat("- {0} which was registered but is also " +
							"waiting for dependencies. \r\n", key);

						IExposeDependencyInfo info = handler as IExposeDependencyInfo;

						if (info != null)
						{
							sb.Append(info.ObtainDependencyDetails(dependenciesChecked));
						}
					}
				}
			}

			return sb.ToString();
		}

		#endregion

    	/// <summary>
		/// Creates an implementation of <see cref="ILifestyleManager"/> based
		/// on <see cref="LifestyleType"/> and invokes <see cref="ILifestyleManager.Init"/>
		/// to initialize the newly created manager.
    	/// </summary>
    	/// <param name="activator"></param>
    	/// <returns></returns>
		protected virtual ILifestyleManager CreateLifestyleManager(IComponentActivator activator)
		{
			ILifestyleManager manager = null;

			LifestyleType type = ComponentModel.LifestyleType;

			if (type == LifestyleType.Undefined	|| type == LifestyleType.Singleton)
			{
				manager = new Lifestyle.SingletonLifestyleManager();
			}
			else if (type == LifestyleType.Thread)
			{
				manager = new Lifestyle.PerThreadLifestyleManager();
			}
			else if (type == LifestyleType.Transient)
			{
				manager = new Lifestyle.TransientLifestyleManager();
			}
			else if (type == LifestyleType.Custom)
			{
				manager = (ILifestyleManager) 
					Activator.CreateInstance( ComponentModel.CustomLifestyle );
			}
			else if (type == LifestyleType.Pooled)
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
			else if (type == LifestyleType.PerWebRequest)
			{
				manager = new Lifestyle.PerWebRequestLifestyleManager();
			}

			manager.Init(activator, Kernel);

			return manager;
		}

    	/// <summary>
    	/// Checks if the handler is able to, at very least, satisfy
    	/// the dependencies for the constructor with less parameters
    	/// </summary>
    	/// <remarks>
    	/// For each non*optional dependency, the implementation will invoke 
		/// <see cref="AddDependency"/>
    	/// </remarks>
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
				if (!dependency.IsOptional && 
				    (dependency.DependencyType == DependencyType.Service || 
				     dependency.DependencyType == DependencyType.ServiceOverride))
				{
					AddDependency(dependency);
				}
			}

			// We need to satisfy at least the constructor 
			// with fewer arguments

			ConstructorCandidate candidate = ComponentModel.Constructors.FewerArgumentsCandidate;

			foreach(DependencyModel dependency in candidate.Dependencies)
			{
				if (dependency.DependencyType == DependencyType.Service || 
					dependency.DependencyType == DependencyType.ServiceOverride)
				{
					AddDependency(dependency);
				}
			}
		}

    	/// <summary>
		/// Invoked by <see cref="EnsureDependenciesCanBeSatisfied"/>
		/// in order to check if a dependency can be satisfied.
		/// If not, the handler is set to a 'waiting dependency' state.
    	/// </summary>
    	/// <remarks>
    	/// This method registers the dependencies within the correct collection 
		/// or dictionary and changes the handler state to 
		/// <see cref="HandlerState.WaitingDependency"/>
    	/// </remarks>
    	/// <param name="dependency"></param>
		protected void AddDependency(DependencyModel dependency)
		{
			if (Kernel.Resolver.CanResolve(null, this, model, dependency))
    		{
				if (dependency.DependencyType == DependencyType.Service && dependency.TargetType != null)
				{
					IHandler depHandler = Kernel.GetHandler(dependency.TargetType);

					if (depHandler != null)
					{
						AddGraphDependency(depHandler.ComponentModel);
					}
				}
    			else
				{
					IHandler depHandler = Kernel.GetHandler(dependency.DependencyKey);

					if (depHandler != null)
					{
						AddGraphDependency(depHandler.ComponentModel);
					}
				}
    			
				return;
    		}
    		
			if (dependency.DependencyType == DependencyType.Service && dependency.TargetType != null)
			{
				DependenciesByService.Add(dependency.TargetType, dependency);
			}
			else
			{
				DependenciesByKey.Add(dependency.DependencyKey, dependency);
			}

			if (state != HandlerState.WaitingDependency)
			{
				// This handler is considered invalid
				// until dependencies are satisfied
				SetNewState(HandlerState.WaitingDependency);

				// Register itself on the kernel
				// to be notified if the dependency is satified
				Kernel.HandlerRegistered += new HandlerDelegate(DependencySatisfied);

				// We also gonna pay attention for state
				// changed within this very handler. The 
				// state can be changed by AddCustomDependencyValue and RemoveCustomDependencyValue
				OnHandlerStateChanged += new HandlerStateDelegate(HandlerStateChanged);
			}
		}

		/// <summary>
		/// Invoked by the kernel
		/// when one of registered dependencies were satisfied by 
		/// new components registered.
		/// </summary>
		/// <remarks>
		/// Handler for the event <see cref="IKernelEvents.HandlerRegistered"/>
		/// </remarks>
		/// <param name="handler"></param>
		/// <param name="stateChanged"></param>
		protected void DependencySatisfied(IHandler handler, ref bool stateChanged)
		{
			// Check within the handler
			
			if (customParameters != null && customParameters.Count != 0)
			{
				DependencyModel[] dependencies = Union(DependenciesByService.Values, DependenciesByKey.Values);

				foreach(DependencyModel dependency in dependencies)
				{
					if (!HasCustomParameter(dependency.DependencyKey)) continue;
					
					if (dependency.DependencyType == DependencyType.Service)
					{
						DependenciesByService.Remove(dependency.TargetType);
					}
					else
					{
						DependenciesByKey.Remove(dependency.DependencyKey);
					}
				}
			}
			
			// Check within the Kernel
			
			Type[] services = new Type[DependenciesByService.Count];
			DependenciesByService.Keys.CopyTo(services, 0);

			foreach(Type service in services)
			{
				if (HasValidComponent(service))
				{
					DependenciesByService.Remove(service);
					AddGraphDependency(kernel.GetHandler(service).ComponentModel);
				}
			}

			String[] keys = new String[DependenciesByKey.Keys.Count];
			DependenciesByKey.Keys.CopyTo(keys, 0);

			foreach(String compKey in keys)
			{
				if (HasValidComponent(compKey) || HasCustomParameter(compKey))
				{
					DependenciesByKey.Remove(compKey);
					AddGraphDependency(kernel.GetHandler(compKey).ComponentModel);
				}
			}

			if (DependenciesByService.Count == 0 && DependenciesByKey.Count == 0)
			{
				SetNewState(HandlerState.Valid); stateChanged = true;
				Kernel.HandlerRegistered -= new HandlerDelegate(DependencySatisfied);

				// We don't need these anymore
				dependenciesByKey = null;
				dependenciesByService = null;
			}
		}

    	/// <summary>
    	/// Invoked whe the container receives a parent container reference.
    	/// </summary>
    	/// <remarks>
    	/// This method implementation checks whether the parent container
    	/// is able to supply the dependencies for this handler.
    	/// </remarks>
    	/// <param name="sender"></param>
    	/// <param name="e"></param>
		protected void OnAddedAsChildKernel(object sender, EventArgs e)
		{
			if (DependenciesByKey.Count == 0 && DependenciesByService.Count == 0) return;

			bool stateChanged = false;

			Type[] services = new Type[DependenciesByService.Count];
			
			DependenciesByService.Keys.CopyTo(services, 0);

			foreach(Type service in services)
			{
				if (Kernel.Parent.HasComponent(service))
				{
					IHandler handler = Kernel.Parent.GetHandler(service);
					DependencySatisfied(handler, ref stateChanged);
				}
			}

			String[] keys = new String[DependenciesByKey.Count];
			
			DependenciesByKey.Keys.CopyTo(services, 0);

			foreach(String key in keys)
			{
				if (Kernel.Parent.HasComponent(key))
				{
					IHandler handler = Kernel.Parent.GetHandler(key);
					DependencySatisfied(handler, ref stateChanged);
				}
			}
		}

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		protected void SetNewState(HandlerState newState)
		{
			state = newState;
		}

		protected IDictionary DependenciesByService
		{
			get
			{
				if (dependenciesByService == null)
				{
					dependenciesByService = new HybridDictionary();
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

		private bool HasValidComponent(Type service)
		{
			foreach(IHandler handler in kernel.GetHandlers(service))
			{
				if (IsValidHandlerState(handler))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasValidComponent(String key)
		{
			return IsValidHandlerState(kernel.GetHandler(key));
		}

		private bool IsValidHandlerState(IHandler handler)
		{
			if (handler == null) return false;

			return handler.CurrentState == HandlerState.Valid;
		}

		private void AddGraphDependency(ComponentModel model)
		{
			ComponentModel.AddDependent(model);
		}

		private DependencyModel[] Union(ICollection firstset, ICollection secondset)
		{
			DependencyModel[] result = new DependencyModel[firstset.Count + secondset.Count];

			firstset.CopyTo(result, 0);
			secondset.CopyTo(result, firstset.Count);

			return result;
		}

    	/// <summary>
		/// Handler for the event <see cref="OnHandlerStateChanged"/>
    	/// </summary>
    	/// <param name="source"></param>
    	/// <param name="args"></param>
		private void HandlerStateChanged(object source, EventArgs args)
		{
			bool stateChanged = false;
			DependencySatisfied(null, ref stateChanged);
		}

		private void RaiseHandlerStateChanged()
		{
			if (OnHandlerStateChanged != null)
			{
				OnHandlerStateChanged(this, EventArgs.Empty);
			}
		}
	}
}
