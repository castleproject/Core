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
	using System.Collections.Generic;
	
	/// <summary>
	/// Describes how to register a group of types.
	/// </summary>
	public class TypesDescriptor : IRegistration
	{
		private readonly Type basedOn;
		private readonly IEnumerable<Type> types;
		private readonly ServiceDescriptor service;
		private Action<ComponentRegistration> configurer;
		private Predicate<Type> unlessFilter;
		private Predicate<Type> ifFilter;
		
		/// <summary>
		/// Initializes a new instance of the TypesDescriptor.
		/// </summary>
		internal TypesDescriptor(Type basedOn, IEnumerable<Type> types)
		{
			this.basedOn = basedOn;
			this.types = types;
			service = new ServiceDescriptor(this);
		}

		/// <summary>
		/// Gets the type all types must be based on.
		/// </summary>
		public Type BasedOn
		{
			get { return basedOn; }
		}

		/// <summary>
		/// Assigns a conditional predication which must be satisfied.
		/// </summary>
		/// <param name="ifFilter">The predicate to satisfy.</param>
		/// <returns></returns>
		public TypesDescriptor If(Predicate<Type> ifFilter)
		{
			this.ifFilter = ifFilter;
			return this;
		}

		/// <summary>
		/// Assigns a conditional predication which must not be satisfied. 
		/// </summary>
		/// <param name="unlessFilter">The predicate not to satisify.</param>
		/// <returns></returns>
		public TypesDescriptor Unless( Predicate<Type> unlessFilter )
		{
			this.unlessFilter = unlessFilter;
			return this;
		}
		
		/// <summary>
		/// Gets the service descriptor.
		/// </summary>
		public ServiceDescriptor WithService
		{
			get { return service; }
		}
		
		/// <summary>
		/// Allows customized configurations of each matching type.
		/// </summary>
		/// <param name="configurer">The configuration action.</param>
		/// <returns></returns>
		public TypesDescriptor Configure(Action<ComponentRegistration> configurer)
		{
			this.configurer = configurer;
			return this;
		}
		
		#region IRegistration Members

		void IRegistration.Register(IKernel kernel)
		{
			foreach (Type type in types)
			{
				if (!IsTypeAccepted(type))
				{	
					continue;
				}
								
				Type serviceType = service.GetService(type);			
				ComponentRegistration registration = Component.For(serviceType);
				registration.ImplementedBy(type);

				if (configurer != null)
				{
					configurer(registration);
				}
				
				if (String.IsNullOrEmpty(registration.Name))
				{
					registration.Named(type.FullName);
				}
				
				if (!kernel.HasComponent(registration.Name))
				{	
					kernel.Register(registration);
				}
			}
		}

		#endregion
	
		private bool IsTypeAccepted(Type type)
		{
			return type.IsClass && !type.IsAbstract && basedOn.IsAssignableFrom(type)
				&& (ifFilter == null || ifFilter(type))
				&& (unlessFilter == null || !unlessFilter(type)
				);
		}
	}
}
