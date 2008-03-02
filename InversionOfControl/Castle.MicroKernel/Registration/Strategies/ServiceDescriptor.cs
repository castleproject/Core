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
	
	/// <summary>
	/// Describes how to select a types service.
	/// </summary>
	/// <typeparam name="T">The base type to match against.</typeparam>
	public class ServiceDescriptor<T>
	{
		public delegate Type ServiceSelector(Type type);
		
		private readonly TypesDescriptor<T> typesDescriptor;
		private ServiceSelector serviceSelector;
		
		internal ServiceDescriptor(TypesDescriptor<T> typesDescriptor)
		{
			this.typesDescriptor = typesDescriptor;
		}
		
		/// <summary>
		/// Uses the base type matched on.
		/// </summary>
		/// <returns></returns>
		public TypesDescriptor<T> Base()
		{
			return Select(delegate { return typeof(T); });
		}
				
		/// <summary>
		/// Uses the first interface of a type.
		/// </summary>
		/// <returns></returns>
		public TypesDescriptor<T> FirstInterface()
		{
			return Select(delegate(Type type)
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
				return first;
			});
		}
		
		/// <summary>
		/// Assigns a custom service selection strategy.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TypesDescriptor<T> Select(ServiceSelector selector)
		{
			serviceSelector = selector;
			return typesDescriptor;
		}
		
		internal Type GetService(Type type)
		{
			if (serviceSelector != null)
			{
				return serviceSelector(type) ?? type;
			}
			return type;
		}
	}
}
