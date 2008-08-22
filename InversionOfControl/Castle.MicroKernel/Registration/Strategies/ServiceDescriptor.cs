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
	/// Describes how to select a types service.
	/// </summary>
	public class ServiceDescriptor
	{
		public delegate IEnumerable<Type> ServiceSelector(Type type, Type baseType);

		private bool useBaseType;
		private readonly BasedOnDescriptor basedOnDescriptor;
		private ServiceSelector serviceSelector;
		
		internal ServiceDescriptor(BasedOnDescriptor basedOnDescriptor)
		{
			this.basedOnDescriptor = basedOnDescriptor;
		}
		
		/// <summary>
		/// Uses the base type matched on.
		/// </summary>
		/// <returns></returns>
		public BasedOnDescriptor Base()
		{
			useBaseType = true;
			return basedOnDescriptor;
		}
				
		/// <summary>
		/// Uses the first interface of a type.
		/// </summary>
		/// <returns></returns>
		public BasedOnDescriptor FirstInterface()
		{
			useBaseType = false;
			return Select(delegate(Type type, Type baseType)
			{
				Type first = null;
				Type[] interfaces = type.GetInterfaces();
				if (interfaces.Length > 0)
				{
					first = interfaces[0];

					// This is a workaround for a CLR bug in
					// which GetInterfaces() returns interfaces
					// with no implementations.
					if (first.IsGenericType && first.ReflectedType == null)
					{
						first = first.GetGenericTypeDefinition();
					}
				}
				return new Type[] { first };
			});
		}
		
		/// <summary>
		/// Assigns a custom service selection strategy.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public BasedOnDescriptor Select(ServiceSelector selector)
		{
			useBaseType = false;
			serviceSelector = selector;
			return basedOnDescriptor;
		}
		
		/// <summary>
		/// Assigns the supplied service types.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public BasedOnDescriptor Select(IEnumerable<Type> types)
		{
			return Select(delegate { return types; });
		}

		internal IEnumerable<Type> GetServices(Type type, Type baseType)
		{
			IEnumerable<Type> services = null;

			if (useBaseType)
			{
				type = baseType;
			}
			else if (serviceSelector != null)
			{
				services = serviceSelector(type, baseType);
			}

			return services ?? new Type[] { type };
		}
	}
}
