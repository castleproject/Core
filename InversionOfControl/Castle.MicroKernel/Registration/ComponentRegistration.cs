// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.Core.Configuration;
	using MicroKernel;

	/// <summary>
	/// Delegate to filter component registration.
	/// </summary>
	/// <param name="kernel">The kernel.</param>
	/// <param name="model">The component model.</param>
	/// <returns>true if accepted.</returns>
	public delegate bool ComponentFilter(IKernel kernel, ComponentModel model);

	/// <summary>
	/// Registration for a single component with the kernel.
	/// </summary>
	/// <typeparam name="S">The service type</typeparam>
	public class ComponentRegistration<S> : IRegistration
	{
		private String name;
		private bool overwrite;
		private bool isInstanceRegistration;
		private Type serviceType;
		private Type implementation;
		private List<Type> forwardedTypes = new List<Type>();
		private ComponentFilter unlessFilter;
		private ComponentFilter ifFilter;
		private readonly List<ComponentDescriptor<S>> descriptors;
		private ComponentModel componentModel;
		private bool registered;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class.
		/// </summary>
		public ComponentRegistration()
		{
			overwrite = false;
			registered = false;
			serviceType = typeof(S);
			descriptors = new List<ComponentDescriptor<S>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class
		/// with an existing <see cref="ComponentModel"/>.
		/// </summary>
		protected ComponentRegistration(ComponentModel componentModel) : this()
		{
			if (componentModel == null)
			{
				throw new ArgumentNullException("componentModel");
			}
			
			this.componentModel = componentModel;
			name = componentModel.Name;
			serviceType = componentModel.Service;
			implementation = componentModel.Implementation;			
		}
		
		public String Name
		{
			get { return name; }
		}
		
		public Type ServiceType
		{
			get { return serviceType; }
			protected set { serviceType = value; }	
		}

		public Type[] ForwardedTypes
		{
			get { return forwardedTypes.ToArray(); }
		}

		public Type Implementation
		{
			get { return implementation; }
		}
		
		internal bool IsOverWrite
		{
			get { return overwrite; }
		}
		
		/// <summary>
		/// With the overwrite.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> OverWrite()
		{
			overwrite = true;
			return this;
		}

		/// <summary>
		/// With the name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Named(String name)
		{
			if (this.name != null)
			{
				String message = String.Format("This component has " +
					"already been assigned name '{0}'", this.name);

				throw new ComponentRegistrationException(message);					
			}

			this.name = name;
			return this;
		}

		public ComponentRegistration<S> ImplementedBy<C>() where C : S
		{
			return ImplementedBy(typeof(C));
		}

		public ComponentRegistration<S> ImplementedBy(Type type)
		{
			if (implementation != null)
			{
				String message = String.Format("This component has " +
					"already been assigned implementation {0}", implementation.FullName);
				throw new ComponentRegistrationException(message);					
			}

			implementation = type;
			return this;
		}

		/// <summary>
		/// With the instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Instance(S instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			isInstanceRegistration = true;
			ImplementedBy(instance.GetType());
			return AddDescriptor(new ComponentInstanceDescriptior<S>(instance));
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <param name="types">The types to forward.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Forward(params Type[] types)
		{
			return Forward((IEnumerable<Type>)types);
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="F">The forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<S> Forward<F>()
		{
			return Forward(new Type[] { typeof(F) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<S> Forward<F1, F2>()
		{
			return Forward(new Type[] { typeof(F1), typeof(F2) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <typeparam name="F3">The third forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<S> Forward<F1, F2, F3>()
		{
			return Forward(new Type[] { typeof(F1), typeof(F2), typeof(F3) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <typeparam name="F3">The third forwarded type.</typeparam>
		/// <typeparam name="F4">The fourth forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<S> Forward<F1, F2, F3, F4>()
		{
			return Forward(new Type[] { typeof(F1), typeof(F2), typeof(F3), typeof(F4) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <param name="types">The types to forward.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Forward(IEnumerable<Type> types)
		{
			foreach (Type type in types)
			{
				if (!forwardedTypes.Contains(type))
				{
					forwardedTypes.Add(type);
				}
			}
			return this;
		}

		/// <summary>
		/// Gets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		public Proxy.ProxyGroup<S> Proxy
		{
			get { return new Proxy.ProxyGroup<S>(this); }
		}

		/// <summary>
		/// Gets the with lifestyle.
		/// </summary>
		/// <value>The with lifestyle.</value>
		public Lifestyle.LifestyleGroup<S> LifeStyle
		{
			get { return new Lifestyle.LifestyleGroup<S>(this); }
		}

		/// <summary>
		/// With the activator.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> Activator<A>() where A : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(A).AssemblyQualifiedName);
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(properties));
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="anonymous">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(params Property[] dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="anonymous">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(object anonymous)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(params Property[] dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(IDictionary dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(object dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="anonymous">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the configuration parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Parameters(params Parameter[] parameters)
		{
			return AddDescriptor(new ParametersDescriptor<S>(parameters));
		}

		/// <summary>
		/// With the configuration.
		/// </summary>
		/// <param name="configNodes">The config nodes.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Configuration(params Node[] configNodes)
		{
			return AddDescriptor( new ConfigurationDescriptor<S>(configNodes));
		}

		/// <summary>
		/// With the configuration.
		/// </summary>
		/// <param name="configuration">The configuration <see cref="MutableConfiguration"/>.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Configuration(IConfiguration configuration)
		{
			return AddDescriptor(new ConfigurationDescriptor<S>(configuration));
		}

		/// <summary>
		/// With the interceptors.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public Interceptor.InterceptorGroup<S> Interceptors(
				params InterceptorReference[] interceptors)
		{
			return new Interceptor.InterceptorGroup<S>(this, interceptors);
		}

		/// <summary>
		/// Makrs component as startable.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> Startable()
		{
			return AddAttributeDescriptor("startable", "true");	
		}

		/// <summary>
		/// Assigns the start method for the startable.
		/// </summary>
		/// <param name="startMethod">The start method.</param>
		/// <returns></returns>
		public ComponentRegistration<S> StartUsingMethod(string startMethod)
		{
			return Startable()
				.AddAttributeDescriptor("startMethod", startMethod);	
		}

		/// <summary>
		/// Assigns the stop method for the startable.
		/// </summary>
		/// <param name="stopMethod">The stop method.</param>
		/// <returns></returns>
		public ComponentRegistration<S> StopUsingMethod(string stopMethod)
		{
			return Startable()
				.AddAttributeDescriptor("stopMethod", stopMethod);
		}

		/// <summary>
		/// Marks the components with one or more actors.
		/// </summary>
		/// <param name="actors">The component actors.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ActAs(params object[] actors)
		{
			foreach (object actor in actors)
			{
				if (actor != null)
				{
					DependsOn(Property.ForKey(Guid.NewGuid().ToString()).Eq(actor));
				}
			}
			return this;
		}

		/// <summary>
		/// Assigns a conditional predication which must be satisfied.
		/// </summary>
		/// <param name="ifFilter">The predicate to satisfy.</param>
		/// <returns></returns>
		public ComponentRegistration<S> If(ComponentFilter ifFilter)
		{
			this.ifFilter = ifFilter;
			return this;
		}

		/// <summary>
		/// Assigns a conditional predication which must not be satisfied. 
		/// </summary>
		/// <param name="unlessFilter">The predicate not to satisify.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Unless(ComponentFilter unlessFilter)
		{
			this.unlessFilter = unlessFilter;
			return this;
		}

		/// <summary>
		/// Registers this component with the <see cref="IKernel"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		void IRegistration.Register(IKernel kernel)
		{
			if (!registered)
			{
				registered = true;
				InitializeDefaults();

				IConfiguration configuration = EnsureComponentConfiguration(kernel);

				foreach(ComponentDescriptor<S> descriptor in descriptors)
				{
					descriptor.ApplyToConfiguration(kernel, configuration);
				}

				if (componentModel == null && isInstanceRegistration == false)
				{
                    componentModel = kernel.ComponentModelBuilder.BuildModel(name, serviceType, implementation, null);
				} 
				else if (componentModel == null && isInstanceRegistration)
				{
					componentModel = new ComponentModel(name, serviceType, implementation);
				}
                
				foreach (ComponentDescriptor<S> descriptor in descriptors)
				{
					descriptor.ApplyToModel(kernel, componentModel);
				}

				if ((ifFilter == null || ifFilter(kernel, componentModel)) && 
					(unlessFilter == null || !unlessFilter(kernel, componentModel)))
				{
					kernel.AddCustomComponent(componentModel);

					foreach (Type type in forwardedTypes)
					{
						kernel.RegisterHandlerForwarding(type, name);
					}
				}	
			}
		}

		/// <summary>
		/// Adds the attribute descriptor.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public ComponentRegistration<S> AddAttributeDescriptor(string key, string value)
		{
			AddDescriptor(new AttributeDescriptor<S>(key, value));
			return this;
		}

		/// <summary>
		/// Adds the descriptor.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public ComponentRegistration<S> AddDescriptor(ComponentDescriptor<S> descriptor)
		{
			descriptor.Registration = this;
			descriptors.Add(descriptor);
			return this;
		}

		internal void AddParameter(IKernel kernel, ComponentModel model, String key, String value)
		{
			IConfiguration parameters = EnsureParametersConfiguration(kernel);
			MutableConfiguration parameter = new MutableConfiguration(key, value);
			parameters.Children.Add(parameter);
			model.Parameters.Add(key, value);
		}

		internal void AddParameter(IKernel kernel, ComponentModel model, String key, IConfiguration value)
		{
			IConfiguration parameters = EnsureParametersConfiguration(kernel);
			MutableConfiguration parameter = new MutableConfiguration(key);
			parameter.Children.Add(value);
			parameters.Children.Add(parameter);
			model.Parameters.Add(key, value);
		}

		private void InitializeDefaults()
		{
			if (implementation == null)
			{
				implementation = serviceType;	
			}

			if (String.IsNullOrEmpty(name))
			{
				name = implementation.FullName;
			}
		}

		private IConfiguration EnsureParametersConfiguration(IKernel kernel)
		{
			IConfiguration configuration = EnsureComponentConfiguration(kernel);
			IConfiguration parameters = configuration.Children["parameters"];
			if (parameters == null)
			{
				parameters = new MutableConfiguration("parameters");
				configuration.Children.Add(parameters);
			}
			return parameters;
		}

		private IConfiguration EnsureComponentConfiguration(IKernel kernel)
		{
			IConfiguration configuration = kernel.ConfigurationStore.GetComponentConfiguration(name);
			if (configuration == null)
			{
				configuration = new MutableConfiguration("component");
				kernel.ConfigurationStore.AddComponentConfiguration(name, configuration);
			}
			return configuration;
		}
	}

	#region Nested Type: ComponentRegistration
	
	public class ComponentRegistration : ComponentRegistration<object>
	{
		public ComponentRegistration()
		{
		}

		public ComponentRegistration(Type serviceType, params Type[] forwaredTypes)
		{
			ServiceType = serviceType;
			Forward(forwaredTypes);
		}

		public ComponentRegistration(ComponentModel componentModel)
			: base(componentModel)
		{
		}

		public ComponentRegistration For(Type serviceType, params Type[] forwaredTypes)
		{
			if (ServiceType != null)
			{
				String message = String.Format("This component has " +
					"already been assigned service type {0}", ServiceType.FullName);
				throw new ComponentRegistrationException(message);
			}

			ServiceType = serviceType;
			Forward(forwaredTypes);
			return this;
		}
	}
	
	#endregion
}
