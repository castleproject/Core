// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="S">The service type</typeparam>
	/// <typeparam name="T">The chaining type</typeparam>
	public class ComponentRegistration<S,T>
	{
		private String name;
		private bool overwrite;
		private readonly IKernel kernel;
		private Type classType;
		private readonly T chain;
		private readonly List<ComponentDescriptor<S,T>> descriptors;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S,T}"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="chain">The chain</param>
		public ComponentRegistration(IKernel kernel, T chain)
		{
			overwrite = false;
			this.kernel = kernel;
			this.chain = chain;
			descriptors = new List<ComponentDescriptor<S,T>>();
		}

		internal bool Overwrite
		{
			get { return overwrite; }	
		}

		/// <summary>
		/// With the overwrite.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithOverwrite()
		{
			overwrite = true;
			return this;
		}

		/// <summary>
		/// With the name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithName(String name)
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

		public ComponentRegistration<S, T> WithImplementation<C>()
		{
			if (classType != null)
			{
				String message = String.Format("This component has " +
					"already been assigned implementation {0}", classType.FullName);

				throw new ComponentRegistrationException(message);					
			}

			classType = typeof(C);
			return this;
		}

		/// <summary>
		/// With the instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithInstance(S instance)
		{
			return AddDescriptor(new ComponentInstanceDescriptior<S,T>(instance));
		}

		/// <summary>
		/// Gets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		public Proxy.ProxyGroup<S,T> Proxy
		{
			get { return new Proxy.ProxyGroup<S,T>(this); }
		}

		/// <summary>
		/// Gets the with lifestyle.
		/// </summary>
		/// <value>The with lifestyle.</value>
		public Lifestyle.LifestyleGroup<S,T> WithLifestyle
		{
			get { return new Lifestyle.LifestyleGroup<S,T>(this); }
		}

		/// <summary>
		/// With the activator.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithActivator<A>() where A : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(A).AssemblyQualifiedName);
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S,T>(properties));
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="anonymous">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S, T> WithExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S, T>(anonymous));
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithCustomDependencies(params Property[] dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S,T>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithCustomDependencies(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S,T>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="anonymous">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S, T> WithCustomDependencies(object anonymous)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S, T>(anonymous));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S,T>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> WithServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S,T>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="anonymous">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S, T> WithServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S, T>(anonymous));
		}

		/// <summary>
		/// With the interceptors.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public Interceptor.InterceptorGroup<S,T> WithInterceptors(
				params InterceptorReference[] interceptors)
		{
			return new Interceptor.InterceptorGroup<S,T>(this, interceptors);
		}

		/// <summary>
		/// Ases the startable.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S,T> AsStartable()
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S,T>(
			                     	Property.WithKey("startable").Eq(true)));
		}

		/// <summary>
		/// Registers this instance.
		/// </summary>
		/// <returns></returns>
		public T Register()
		{
			InitializeDefaults();
			ComponentModel model = BuildComponentModel();
			kernel.AddCustomComponent(model);
			return chain;
		}

		/// <summary>
		/// Builds the component model.
		/// </summary>
		/// <returns></returns>
		private ComponentModel BuildComponentModel()
		{
			IConfiguration configuration = EnsureComponentConfiguration();
			foreach(ComponentDescriptor<S,T> descriptor in descriptors)
			{
				descriptor.ApplyToConfiguration(configuration);
			}

			ComponentModel model = kernel.ComponentModelBuilder.BuildModel(
				name, typeof(S), classType, null);
			foreach(ComponentDescriptor<S,T> descriptor in descriptors)
			{
				descriptor.ApplyToModel(model);
			}

			return model;
		}

		/// <summary>
		/// Adds the attribute descriptor.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> AddAttributeDescriptor(string name, string value)
		{
			AddDescriptor(new AttributeDescriptor<S,T>(name, value));
			return this;
		}

		/// <summary>
		/// Adds the descriptor.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public ComponentRegistration<S,T> AddDescriptor(ComponentDescriptor<S,T> descriptor)
		{
			descriptor.Registration = this;
			descriptors.Add(descriptor);
			return this;
		}

		internal void AddParameter(ComponentModel model, String name, String value)
		{
			IConfiguration configuration = EnsureComponentConfiguration();
			IConfiguration parameters = configuration.Children["parameters"];
			if (parameters == null)
			{
				parameters = new MutableConfiguration("component");
				configuration.Children.Add(parameters);
			}

			MutableConfiguration reference = new MutableConfiguration(name, value);
			parameters.Children.Add(reference);
			model.Parameters.Add(name, value);
		}

		private void InitializeDefaults()
		{
			if (classType == null)
			{
				classType = typeof(S);	
			}

			if (String.IsNullOrEmpty(name))
			{
				name = classType.FullName;
			}
		}

		private IConfiguration EnsureComponentConfiguration()
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
}
