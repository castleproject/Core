// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.Serialization;

	using Castle.Model.Configuration;

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
		Custom
	}

	/// <summary>
	/// Represents the collection of information and
	/// meta information collected about a component.
	/// </summary>
	[Serializable]
	public sealed class ComponentModel : GraphNode
	{
		#region Fields

		/// <summary>Name (key) of the component</summary>
		private String name;

		/// <summary>Service exposed</summary>
		private Type service;

		/// <summary>Implementation for the service</summary>
		private Type implementation;

		/// <summary>Extended properties</summary>
		[NonSerialized]
		private IDictionary extended;

		/// <summary>Lifestyle for the component</summary>
		private LifestyleType lifestyleType;

		/// <summary>Custom lifestyle, if any</summary>
		private Type customLifestyle;

		/// <summary>Custom activator, if any</summary>
		private Type customComponentActivator;

		/// <summary>Dependencies the kernel must resolve</summary>
		private DependencyModelCollection dependencies;
		
		/// <summary>All available constructors</summary>
		private ConstructorCandidateCollection constructors;
		
		/// <summary>All potential properties that can be setted by the kernel</summary>
		private PropertySetCollection properties;

		private MethodMetaModelCollection methodMetaModels;
		
		/// <summary>Steps of lifecycle</summary>
		private LifecycleStepCollection lifecycleSteps;
		
		/// <summary>External parameters</summary>
		private ParameterModelCollection parameters;
		
		/// <summary>Configuration node associated</summary>
		private IConfiguration configuration;
		
		/// <summary>Interceptors associated</summary>
		private InterceptorReferenceCollection interceptors;

		#endregion

		/// <summary>
		/// Constructs a ComponentModel
		/// </summary>
		public ComponentModel(String name, Type service, Type implementation)
		{
			this.name = name;
			this.service = service;
			this.implementation = implementation;
			this.lifestyleType = LifestyleType.Undefined;
		}

		/// <summary>
		/// Sets or returns the component key
		/// </summary>
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public Type Service
		{
			get { return service; }
			set { service = value; }
		}

		public Type Implementation
		{
			get { return implementation; }
			set { implementation = value; }
		}

		public IDictionary ExtendedProperties
		{
			get
			{
				lock(this)
				{
					if (extended == null) extended = new HybridDictionary();
				}
				return extended;
			}
			set { extended = value; }
		}

		public ConstructorCandidateCollection Constructors
		{
			get
			{
				lock(this)
				{
					if (constructors == null) constructors = new ConstructorCandidateCollection();
				}
				return constructors;
			}
		}

		public PropertySetCollection Properties
		{
			get
			{
				lock(this)
				{
					if (properties == null) properties = new PropertySetCollection();
				}
				return properties;
			}
		}

		public MethodMetaModelCollection MethodMetaModels
		{
			get
			{
				lock(this)
				{
					if (methodMetaModels == null) methodMetaModels = new MethodMetaModelCollection();
				}
				return methodMetaModels;
			}
		}

		public IConfiguration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		}

		public LifecycleStepCollection LifecycleSteps
		{
			get
			{
				lock(this)
				{
					if (lifecycleSteps == null) lifecycleSteps = new LifecycleStepCollection();
				}
				return lifecycleSteps;
			}
		}

		public LifestyleType LifestyleType
		{
			get { return lifestyleType; }
			set { lifestyleType = value; }
		}

		public Type CustomLifestyle
		{
			get { return customLifestyle; }
			set { customLifestyle = value; }
		}

		public Type CustomComponentActivator
		{
			get { return customComponentActivator; }
			set { customComponentActivator = value; }
		}

		public InterceptorReferenceCollection Interceptors
		{
			get
			{
				lock(this)
				{
					if (interceptors == null) interceptors = new InterceptorReferenceCollection();
				}
				return interceptors;
			}
		}

		public ParameterModelCollection Parameters
		{
			get
			{
				lock(this)
				{
					if (parameters == null) parameters = new ParameterModelCollection();
				}
				return parameters;
			}
		}

		/// <summary>
		/// Dependencies are kept within constructors and
		/// properties. Others dependencies must be 
		/// registered here, so the kernel can check them
		/// </summary>
		public DependencyModelCollection Dependencies
		{
			get
			{
				lock(this)
				{
					if (dependencies == null) dependencies = new DependencyModelCollection();
				}
				return dependencies;
			}
		}
	}
}
