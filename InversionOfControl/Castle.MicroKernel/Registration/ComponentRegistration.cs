// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using Castle.Facilities.FactorySupport;

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities.OnCreate;
	using Castle.MicroKernel.Proxy;
	using Facilities;

	/// <summary>
	/// Delegate to filter component registration.
	/// </summary>
	/// <param name="kernel">The kernel.</param>
	/// <param name="model">The component model.</param>
	/// <returns>true if accepted.</returns>
	public delegate bool ComponentFilter(IKernel kernel, ComponentModel model);

    public delegate T Function<T>();

	/// <summary>
	/// Registration for a single type as a component with the kernel.
	/// <para />
	/// You can create a new registration with the <see cref="Component"/> factory.
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
	    private readonly List<IRegistration> additionalRegistrations;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class.
		/// </summary>
		public ComponentRegistration()
		{
			overwrite = false;
			registered = false;
			serviceType = typeof(S);
			descriptors = new List<ComponentDescriptor<S>>();
            additionalRegistrations = new List<IRegistration>();
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

		/// <summary>
		/// The name of the component. Will become the key for the component in the kernel.
		/// <para />
		/// To set the name, use <see cref="Named"/>.
		/// <para />
		/// If not set, the <see cref="Type.FullName"/> of the <see cref="Implementation"/>
		/// will be used as the key to register the component.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get { return name; }
		}

		/// <summary>
		/// The type of the service, the same as <typeparamref name="S"/>.
		/// <para />
		/// This is the first type passed to <see cref="Component.For(Type)"/>.
		/// </summary>
		/// <value>The type of the service.</value>
		public Type ServiceType
		{
			get { return serviceType; }
			protected set { serviceType = value; }	
		}

		/// <summary>
		/// Gets the forwarded service types on behalf of this component.
		/// <para />
		/// Add more types to forward using <see cref="Forward(Type[])"/>.
		/// </summary>
		/// <value>The types of the forwarded services.</value>
		public Type[] ForwardedTypes
		{
			get { return forwardedTypes.ToArray(); }
		}

		/// <summary>
		/// The concrete type that implements the service.
		/// <para />
		/// To set the implementation, use <see cref="ImplementedBy"/>.
		/// </summary>
		/// <value>The implementation of the service.</value>
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
		/// Change the name of this registration. 
		/// This will be the key for the component in the kernel.
		/// <para />
		/// If not set, the <see cref="Type.FullName"/> of the <see cref="Implementation"/>
		/// will be used as the key to register the component.
		/// </summary>
		/// <param name="name">The name of this registration.</param>
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

		/// <summary>
		/// Sets the concrete type that implements the service to <typeparamref name="C"/>.
		/// <para />
		/// If not set, the <see cref="ServiceType"/> will be used as the implementation for this component.
		/// </summary>
		/// <typeparam name="C">The type that is the implementation for the service.</typeparam>
		/// <returns></returns>
        public ComponentRegistration<S> ImplementedBy<C>() where C : S
        {
			return ImplementedBy(typeof(C));
		}

		/// <summary>
		/// Sets the concrete type that implements the service to <paramref name="type"/>.
		/// <para />
		/// If not set, the <see cref="ServiceType"/> will be used as the implementation for this component.
		/// </summary>
		/// <param name="type">The type that is the implementation for the service.</param>
		/// <returns></returns>
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
		/// Assigns an existing instance as the component for this registration.
		/// </summary>
		/// <param name="instance">The component instance.</param>
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
		/// Set proxy for this component.
		/// </summary>
		/// <value>The proxy.</value>
		public Proxy.ProxyGroup<S> Proxy
		{
			get { return new Proxy.ProxyGroup<S>(this); }
		}

		/// <summary>
		/// Set the lifestyle of this component.
		/// For example singleton and transient (also known as 'factory').
		/// </summary>
		/// <value>The with lifestyle.</value>
		public Lifestyle.LifestyleGroup<S> LifeStyle
		{
			get { return new Lifestyle.LifestyleGroup<S>(this); }
		}

		/// <summary>
		/// Set a custom <see cref="IComponentActivator"/> which creates and destroys the component.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> Activator<A>() where A : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(A).AssemblyQualifiedName);
		}

		/// <summary>
		/// Sets <see cref="ComponentModel.ExtendedProperties"/> for this component.
		/// </summary>
		/// <param name="properties">The extended properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(properties));
		}

		/// <summary>
		/// Sets <see cref="ComponentModel.ExtendedProperties"/> for this component.
		/// </summary>
		/// <param name="anonymous">The extendend properties as key/value pairs.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(anonymous));
		}

		/// <summary>
		/// Specify custom dependencies using <see cref="Property.ForKey"/>.
		/// <para />
		/// Use <see cref="ServiceOverrides(ServiceOverride[])"/> to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(params Property[] dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// Uses a dictionary of key/value pairs, to specify custom dependencies.
		/// <para />
		/// Use <see cref="ServiceOverrides(IDictionary)"/> to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// Uses an (anonymous) object as a dictionary, to specify custom dependencies.
		/// <para />
		/// Use <see cref="ServiceOverrides(object)"/> to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="anonymous">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> DependsOn(object anonymous)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(anonymous));
		}

		/// <summary>
		/// Obsolete, use <see cref="DependsOn(Property[])"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(params Property[] dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// Obsolete, use <see cref="DependsOn(IDictionary)"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(IDictionary dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// Obsolete, use <see cref="DependsOn(object)"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete]
		public ComponentRegistration<S> CustomDependencies(object dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// Override (some of) the services that this component needs.
		/// Use <see cref="ServiceOverride.ForKey"/> to create an override.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(Property[])"/>
		/// </summary>
		/// <param name="overrides">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// Override (some of) the services that this component needs, using a dictionary.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(IDictionary)"/>
		/// </summary>
		/// <param name="overrides">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// Override (some of) the services that this component needs, using an (anonymous) object as a dictionary.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(object)"/>
		/// </summary>
		/// <param name="anonymous">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(anonymous));
		}

		/// <summary>
		/// Set configuration parameters with string or <see cref="IConfiguration"/> values.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Parameters(params Parameter[] parameters)
		{
			return AddDescriptor(new ParametersDescriptor<S>(parameters));
		}

		/// <summary>
		/// Creates an attribute descriptor.
		/// </summary>
		/// <param name="key">The attribute key.</param>
		/// <returns></returns>
		public AttributeKeyDescriptor<S> Attribute(string key)
		{
			return new AttributeKeyDescriptor<S>(this, key);
		}

		/// <summary>
		/// Apply more complex configuration to this component registration.
		/// </summary>
		/// <param name="configNodes">The config nodes.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Configuration(params Node[] configNodes)
		{
			return AddDescriptor( new ConfigurationDescriptor<S>(configNodes));
		}

		/// <summary>
		/// Apply more complex configuration to this component registration.
		/// </summary>
		/// <param name="configuration">The configuration <see cref="MutableConfiguration"/>.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Configuration(IConfiguration configuration)
		{
			return AddDescriptor(new ConfigurationDescriptor<S>(configuration));
		}

		/// <summary>
		/// Set the interceptors for this component.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public Interceptor.InterceptorGroup<S> Interceptors(
				params InterceptorReference[] interceptors)
		{
			return new Interceptor.InterceptorGroup<S>(this, interceptors);
		}

		/// <summary>
		/// Sets the interceptor selector for this component.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ComponentRegistration<S> SelectInterceptorsWith(IInterceptorSelector selector)
		{
			return AddDescriptor(new Interceptor.InterceptorSelectorDescriptor<S>(selector));
		}

		/// <summary>
		/// Marks the component as startable.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Be sure that you first added the <see cref="Castle.Facilities.Startable.StartableFacility"/> 
		/// to the kernel, before registering this component.</remarks>
		public ComponentRegistration<S> Startable()
		{
			return AddAttributeDescriptor("startable", "true");	
		}

		/// <summary>
		/// Assigns the start method for the startable.
		/// </summary>
		/// <param name="startMethod">The start method.</param>
		/// <returns></returns>
		/// <remarks>Be sure that you first added the <see cref="Castle.Facilities.Startable.StartableFacility"/> 
		/// to the kernel, before registering this component.</remarks>
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
		/// <remarks>Be sure that you first added the <see cref="Castle.Facilities.Startable.StartableFacility"/> 
		/// to the kernel, before registering this component.</remarks>
		public ComponentRegistration<S> StopUsingMethod(string stopMethod)
		{
			return Startable()
				.AddAttributeDescriptor("stopMethod", stopMethod);
		}

		/// <summary>
		/// Stores a set of <see cref="OnCreateActionDelegate{T}"/> which will be invoked when the component
		/// is created
		/// </summary>
		/// <param name="actions">A setof actions</param>
		/// <remarks>Be sure that you first added the <see cref="OnCreateFacility"/> 
		/// to the kernel, before registering this component.</remarks>
		public ComponentRegistration<S> OnCreate(params OnCreateActionDelegate<S>[] actions)
		{ 
			this.AddDescriptor(new OnCreateComponentDescriptor<S>(actions));
			return this;
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
		/// <para />
		/// The component will only be registered into the kernel 
		/// if this predicate is satisfied (or not assigned at all).
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
		/// <para />
		/// The component will only be registered into the kernel 
		/// if this predicate is not satisfied (or not assigned at all).
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

				if (componentModel.Implementation.IsInterface && componentModel.Interceptors.Count > 0)
				{
					ProxyOptions options = ProxyUtil.ObtainProxyOptions(componentModel, true);
					options.OmitTarget = true;
				}

				if ((ifFilter == null || ifFilter(kernel, componentModel)) && 
					(unlessFilter == null || !unlessFilter(kernel, componentModel)))
				{
					kernel.AddCustomComponent(componentModel);

					foreach (Type type in forwardedTypes)
					{
						kernel.RegisterHandlerForwarding(type, name);
					}

                    foreach (IRegistration r in additionalRegistrations) 
                    {
                        r.Register(kernel);
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

        /// <summary>
        /// Uses a factory method to instantiate the component.
        /// Requires the <see cref="FactorySupportFacility"/> to be installed.
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="factoryMethod">Factory method</param>
        /// <returns></returns>
        public ComponentRegistration<S> UsingFactoryMethod<T>(Function<T> factoryMethod) where T: S
        {
            string factoryName = Guid.NewGuid().ToString();
            additionalRegistrations.Add(Component.For<GenericFactory<T>>().Named(factoryName)
                .Instance(new GenericFactory<T>(factoryMethod)));
            ConfigureFactoryWithId(factoryName);
            return this;
        }

        /// <summary>
        /// Uses a factory method to instantiate the component.
        /// Requires the <see cref="FactorySupportFacility"/> to be installed.
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="factoryMethod">Factory method</param>
        /// <returns></returns>
        public ComponentRegistration<S> UsingFactoryMethod<T>(Converter<IKernel, T> factoryMethod) where T : S 
        {
            string factoryName = Guid.NewGuid().ToString();
            string factoryMethodName = Guid.NewGuid().ToString();
            additionalRegistrations.Add(Component.For<KernelToT<T>>().Named(factoryMethodName)
                .Instance(new KernelToT<T>(factoryMethod)));
            additionalRegistrations.Add(Component.For<GenericFactoryWithKernel<T>>().Named(factoryName)
                .ServiceOverrides(ServiceOverride.ForKey("factoryMethod").Eq(factoryMethodName)));
            ConfigureFactoryWithId(factoryName);
            return this;
        }

		/// <summary>
		/// Uses a factory method to instantiate the component.
		/// Requires the <see cref="FactorySupportFacility"/> to be installed.
		/// </summary>
		/// <typeparam name="T">Implementation type</typeparam>
		/// <param name="factoryMethod">Factory method</param>
		/// <returns></returns>
		public ComponentRegistration<S> UsingFactoryMethod<T>(Func<IKernel, CreationContext, T> factoryMethod) where T : S
		{
			string factoryName = Guid.NewGuid().ToString();
			additionalRegistrations.Add(Component.For<GenericFactoryWithContext<T>>().Named(factoryName)
				.Instance(new GenericFactoryWithContext<T>(factoryMethod)));
			ConfigureFactoryWithId(factoryName);
			return this;
		}

        private void ConfigureFactoryWithId(string factoryId) 
        {
            Configuration(
                Attrib.ForName("factoryId").Eq(factoryId),
                Attrib.ForName("factoryCreate").Eq("Create")
                );
        }

        /// <summary>
        /// Uses a factory to instantiate the component
        /// </summary>
        /// <typeparam name="U">Factory type. This factory has to be registered in the kernel.</typeparam>
        /// <typeparam name="V">Implementation type.</typeparam>
        /// <param name="factory">Factory invocation</param>
        /// <returns></returns>
        public ComponentRegistration<S> UsingFactory<U, V>(Converter<U, V> factory) where V : S 
        {
            return UsingFactoryMethod(kernel => factory.Invoke(kernel.Resolve<U>()));
	    }
	}

    /// <summary>
    /// Helper wrapper around Converter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KernelToT<T> 
    {
        private readonly Converter<IKernel, T> fun;

        public KernelToT(Converter<IKernel, T> fun) 
        {
            this.fun = fun;
        }

        public T Call(IKernel kernel) 
        {
            return fun(kernel);
        }
    }

	/// <summary>
	/// Helper factory class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericFactory<T>
	{
		private readonly Function<T> factoryMethod;

		public GenericFactory(Function<T> factoryMethod)
		{
			this.factoryMethod = factoryMethod;
		}

		public T Create()
		{
			return factoryMethod();
		}
	}

	/// <summary>
	/// Helper factory class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericFactoryWithContext<T>
	{
		private readonly Func<IKernel, CreationContext, T> factoryMethod;

		public GenericFactoryWithContext(Func<IKernel, CreationContext, T> factoryMethod)
		{
			this.factoryMethod = factoryMethod;
		}

		public T Create(IKernel kernel, CreationContext context)
		{
			return factoryMethod(kernel, context);
		}
	}

    /// <summary>
    /// Helper factory class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericFactoryWithKernel<T>
    {
        private readonly KernelToT<T> factoryMethod;
        private readonly IKernel kernel;

        public GenericFactoryWithKernel(KernelToT<T> factoryMethod, IKernel kernel)
        {
            this.factoryMethod = factoryMethod;
            this.kernel = kernel;
        }

        public T Create()
        {
            return factoryMethod.Call(kernel);
        }
    }

	#region Nested Type: ComponentRegistration

	/// <summary>
	/// A non-generic <see cref="ComponentRegistration{S}"/>.
	/// <para />
	/// You can create a new registration with the <see cref="Component"/> factory.
	/// </summary>
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
