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
	public sealed class ComponentModel : GraphNode, IDeserializationCallback
	{
		#region Fields

		/// <summary>Name (key) of the component</summary>
		private String _name;

		/// <summary>Service exposed</summary>
		private Type _service;

		/// <summary>Implementation for the service</summary>
		private Type _implementation;

		/// <summary>Extended properties</summary>
		[NonSerialized]
		private IDictionary _extended;

		/// <summary>Lifestyle for the component</summary>
		private LifestyleType _lifestyleType;

		/// <summary>Custom lifestyle, if any</summary>
		private Type _customLifestyle;

		/// <summary>Custom activator, if any</summary>
		private Type _customComponentActivator;

		/// <summary>Dependencies the kernel must resolve</summary>
		private DependencyModelCollection _dependencies;
		
		/// <summary>All available constructors</summary>
		private ConstructorCandidateCollection _constructors;
		
		/// <summary>All potential properties that can be setted by the kernel</summary>
		private PropertySetCollection _properties;
		
		/// <summary>Steps of lifecycle</summary>
		private LifecycleStepCollection _lifecycleSteps;
		
		/// <summary>External parameters</summary>
		private ParameterModelCollection _parameters;
		
		/// <summary>Configuration node associated</summary>
		private IConfiguration _configuration;
		
		/// <summary>Interceptors associated</summary>
		private InterceptorReferenceCollection _interceptors;

		#endregion

		/// <summary>
		/// Constructs a ComponentModel
		/// </summary>
		public ComponentModel(String name, Type service, Type implementation)
		{
			_name = name;
			_service = service;
			_implementation = implementation;
			_lifestyleType = LifestyleType.Undefined;

			_extended = new Hashtable( CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default );
			_properties = new PropertySetCollection();
			_parameters = new ParameterModelCollection();
			_constructors = new ConstructorCandidateCollection();
			_interceptors = new InterceptorReferenceCollection();
			_lifecycleSteps = new LifecycleStepCollection();
			_dependencies = new DependencyModelCollection();
		}

		/// <summary>
		/// Sets or returns the component key
		/// </summary>
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Type Service
		{
			get { return _service; }
			set { _service = value; }
		}

		public Type Implementation
		{
			get { return _implementation; }
			set { _implementation = value; }
		}

		public IDictionary ExtendedProperties
		{
			get { return _extended; }
			set { _extended = value; }
		}

		public ConstructorCandidateCollection Constructors
		{
			get { return _constructors; }
		}

		public PropertySetCollection Properties
		{
			get { return _properties; }
		}

		public IConfiguration Configuration
		{
			get { return _configuration; }
			set { _configuration = value; }
		}

		public LifecycleStepCollection LifecycleSteps
		{
			get { return _lifecycleSteps; }
		}

		public LifestyleType LifestyleType
		{
			get { return _lifestyleType; }
			set { _lifestyleType = value; }
		}

		public Type CustomLifestyle
		{
			get { return _customLifestyle; }
			set { _customLifestyle = value; }
		}

		public Type CustomComponentActivator
		{
			get { return _customComponentActivator; }
			set { _customComponentActivator = value; }
		}

		public InterceptorReferenceCollection Interceptors
		{
			get { return _interceptors; }
		}

		public ParameterModelCollection Parameters
		{
			get { return _parameters; }
		}

		/// <summary>
		/// Dependencies are kept within constructors and
		/// properties. Others dependencies must be 
		/// registered here, so the kernel can check them
		/// </summary>
		public DependencyModelCollection Dependencies
		{
			get { return _dependencies; }
		}

		#region IDeserializationCallback

		public void OnDeserialization(object sender)
		{
			_extended = new Hashtable( CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default );
		}

		#endregion
	}
}
