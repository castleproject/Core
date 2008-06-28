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

namespace Castle.Core
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using Castle.Core.Configuration;
    using System.Collections.Generic;

	#region Enums

	/// <summary>
	/// Enumeration used to mark the component's lifestyle.
	/// </summary>
	public enum LifestyleType
	{
		/// <summary>
		/// No lifestyle specified.
		/// </summary>
		Undefined,
		/// <summary>
		/// Singleton components are instantiated once, and shared
		/// between all clients.
		/// </summary>
		Singleton,
		/// <summary>
		/// Thread components have a unique instance per thread.
		/// </summary>
		Thread,
		/// <summary>
		/// Transient components are created on demand.
		/// </summary>
		Transient,
		/// <summary>
		/// Optimization of transient components that keeps
		/// instance in a pool instead of always creating them.
		/// </summary>
		Pooled,
		/// <summary>
		/// Any other logic to create/release components.
		/// </summary>
		Custom,
		/// <summary>
		/// PerWebRequest components are created once per Http Request
		/// </summary>
		PerWebRequest
	}

	/// <summary>
	/// 
	/// </summary>
	public enum PropertiesInspectionBehavior
	{
		Undefined,
		None,
		All,
		DeclaredOnly
	}

	#endregion

	/// <summary>
	/// Represents the collection of information and
	/// meta information collected about a component.
	/// </summary>
	[DebuggerDisplay("{Implementation} / {Service}")]
#if !SILVERLIGHT
	[Serializable]
#endif
	public sealed class ComponentModel : GraphNode
	{
		public const string SkipRegistration = "skip.registration";

		#region Fields
		// Note the use of volatile for fields used in the double checked lock pattern.
		// This is necessary to ensure the pattern works correctly.

		/// <summary>Name (key) of the component</summary>
		private String name;

		/// <summary>Service exposed</summary>
		private Type service;

		/// <summary>Implementation for the service</summary>
		private Type implementation;

		/// <summary>Extended properties</summary>
#if !SILVERLIGHT
		[NonSerialized] 
#endif
		private volatile IDictionary extended;

		/// <summary>Lifestyle for the component</summary>
		private LifestyleType lifestyleType;

		private PropertiesInspectionBehavior inspectionBehavior;

		/// <summary>Custom lifestyle, if any</summary>
		private Type customLifestyle;

		/// <summary>Custom activator, if any</summary>
		private Type customComponentActivator;

		/// <summary>Dependencies the kernel must resolve</summary>
		private volatile DependencyModelCollection dependencies;

		/// <summary>All available constructors</summary>
		private volatile ConstructorCandidateCollection constructors;

		/// <summary>All potential properties that can be setted by the kernel</summary>
		private volatile PropertySetCollection properties;

		//private MethodMetaModelCollection methodMetaModels;

		/// <summary>Steps of lifecycle</summary>
		private volatile LifecycleStepCollection lifecycleSteps;

		/// <summary>External parameters</summary>
		private volatile ParameterModelCollection parameters;

		/// <summary>Configuration node associated</summary>
		private IConfiguration configuration;

		/// <summary>Interceptors associated</summary>
		private volatile InterceptorReferenceCollection interceptors;

		/// <summary>/// Custom dependencies/// </summary>
#if !SILVERLIGHT
		[NonSerialized] 
#endif
		private volatile IDictionary customDependencies;

		private bool requiresGenericArguments;

		private readonly object syncRoot = new object();

		#endregion

		/// <summary>
		/// Constructs a ComponentModel
		/// </summary>
		public ComponentModel(String name, Type service, Type implementation)
		{
			this.name = name;
			this.service = service;
			this.implementation = implementation;
			lifestyleType = LifestyleType.Undefined;
			inspectionBehavior = PropertiesInspectionBehavior.Undefined;
		}

		/// <summary>
		/// Sets or returns the component key
		/// </summary>
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the service exposed.
		/// </summary>
		/// <value>The service.</value>
		public Type Service
		{
			get { return service; }
			set { service = value; }
		}

		/// <summary>
		/// Gets or sets the component implementation.
		/// </summary>
		/// <value>The implementation.</value>
		public Type Implementation
		{
			get { return implementation; }
			set { implementation = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the component requires generic arguments.
		/// </summary>
		/// <value>
		/// <c>true</c> if generic arguments are required; otherwise, <c>false</c>.
		/// </value>
		public bool RequiresGenericArguments
		{
			get { return requiresGenericArguments; }
			set { requiresGenericArguments = value; }
		}

		/// <summary>
		/// Gets or sets the extended properties.
		/// </summary>
		/// <value>The extended properties.</value>
		public IDictionary ExtendedProperties
		{
			get
			{
				if (extended == null)
				{
					lock (syncRoot)
					{
						if (extended == null) extended = new Dictionary<object, object>();
					}
				}
				return extended;
			}
			set { extended = value; }
		}

		/// <summary>
		/// Gets the constructors candidates.
		/// </summary>
		/// <value>The constructors.</value>
		public ConstructorCandidateCollection Constructors
		{
			get
			{
				if (constructors == null)
				{
					lock (syncRoot)
					{
						if (constructors == null) constructors = new ConstructorCandidateCollection();
					}
				}
				return constructors;
			}
		}

		/// <summary>
		/// Gets the properties set.
		/// </summary>
		/// <value>The properties.</value>
		public PropertySetCollection Properties
		{
			get
			{
				if (properties == null)
				{
					lock (syncRoot)
					{
						if (properties == null) properties = new PropertySetCollection();
					}
				}
				return properties;
			}
		}

		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public IConfiguration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		}

		/// <summary>
		/// Gets the lifecycle steps.
		/// </summary>
		/// <value>The lifecycle steps.</value>
		public LifecycleStepCollection LifecycleSteps
		{
			get
			{
				if (lifecycleSteps == null)
				{
					lock (syncRoot)
					{
						if (lifecycleSteps == null) lifecycleSteps = new LifecycleStepCollection();
					}
				}
				return lifecycleSteps;
			}
		}

		/// <summary>
		/// Gets or sets the lifestyle type.
		/// </summary>
		/// <value>The type of the lifestyle.</value>
		public LifestyleType LifestyleType
		{
			get { return lifestyleType; }
			set { lifestyleType = value; }
		}

		/// <summary>
		/// Gets or sets the strategy for
		/// inspecting public properties 
		/// on the components
		/// </summary>
		public PropertiesInspectionBehavior InspectionBehavior
		{
			get { return inspectionBehavior; }
			set { inspectionBehavior = value; }
		}

		/// <summary>
		/// Gets or sets the custom lifestyle.
		/// </summary>
		/// <value>The custom lifestyle.</value>
		public Type CustomLifestyle
		{
			get { return customLifestyle; }
			set { customLifestyle = value; }
		}

		/// <summary>
		/// Gets or sets the custom component activator.
		/// </summary>
		/// <value>The custom component activator.</value>
		public Type CustomComponentActivator
		{
			get { return customComponentActivator; }
			set { customComponentActivator = value; }
		}

		/// <summary>
		/// Gets the interceptors.
		/// </summary>
		/// <value>The interceptors.</value>
		public InterceptorReferenceCollection Interceptors
		{
			get
			{
				if (interceptors == null)
				{
					lock (syncRoot)
					{
						if (interceptors == null) interceptors = new InterceptorReferenceCollection();
					}
				}
				return interceptors;
			}
		}

		/// <summary>
		/// Gets the parameter collection.
		/// </summary>
		/// <value>The parameters.</value>
		public ParameterModelCollection Parameters
		{
			get
			{
				if (parameters == null)
				{
					lock (syncRoot)
					{
						if (parameters == null) parameters = new ParameterModelCollection();
					}
				}
				return parameters;
			}
		}

		/// <summary>
		/// Dependencies are kept within constructors and
		/// properties. Others dependencies must be 
		/// registered here, so the kernel (as a matter 
		/// of fact the handler) can check them
		/// </summary>
		public DependencyModelCollection Dependencies
		{
			get
			{
				if (dependencies == null)
				{
					lock (syncRoot)
					{
						if (dependencies == null) dependencies = new DependencyModelCollection();
					}
				}
				return dependencies;
			}
		}

		/// <summary>
		/// Gets the custom dependencies.
		/// </summary>
		/// <value>The custom dependencies.</value>
		public IDictionary CustomDependencies
		{
			get
			{
				if (customDependencies == null)
				{
					lock (syncRoot)
					{
						if (customDependencies == null) customDependencies = new Dictionary<object, object>();
					}
				}
				return customDependencies;
			}
		}

		/// <summary>
		/// Requires the selected property dependencies.
		/// </summary>
		/// <param name="selectors">The property selector.</param>
		public void Requires(params Predicate<PropertySet>[] selectors)
		{
			foreach (PropertySet property in Properties)
			{
				foreach (Predicate<PropertySet> select in selectors)
				{
					if (select(property))
					{
						property.Dependency.IsOptional = false;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Requires the property dependencies of type <typeparamref name="D"/>.
		/// </summary>
		/// <typeparam name="D">The dependency type.</typeparam>
		public void Requires<D>() where D : class
		{
			Requires(delegate(PropertySet p)
			{
				return p.Dependency.TargetType == typeof(D);
			});
		}
	}
}
