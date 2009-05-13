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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections.Generic;
	using Castle.Core;

	/// <summary>
	/// Factory for creating <see cref="ComponentRegistration"/> objects.
	/// </summary>
	public static class Component
	{
		/// <summary>
		/// Creates a component registration for the <paramref name="serviceType"/>
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration For(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException("serviceType",
				                                "The argument was null. Check that the assembly " 
												+ "is referenced and the type available to your application.");

			return new ComponentRegistration(serviceType);
		}

		/// <summary>
		/// Creates a component registration for the <paramref name="serviceTypes"/>
		/// </summary>
		/// <param name="serviceTypes">Types of the service.</param>
		/// <returns>The component registration.</returns>B
		public static ComponentRegistration For(params Type[] serviceTypes)
		{
			if (serviceTypes.Length == 0)
			{
				throw new ArgumentException("At least one service type must be supplied");
			}

			Type[] forwardTypes = new Type[serviceTypes.Length - 1];
			Array.Copy(serviceTypes, 1, forwardTypes, 0, serviceTypes.Length - 1);

			ComponentRegistration registration = For(serviceTypes[0]);
			registration.Forward(forwardTypes);
			return registration;
		}

		/// <summary>
		/// Creates a component registration for the <paramref name="serviceTypes"/>
		/// </summary>
		/// <param name="serviceTypes">Types of the service.</param>
		/// <returns>The component registration.</returns>B
		public static ComponentRegistration For(IEnumerable<Type> serviceTypes)
		{
			ComponentRegistration registration = null;

			foreach (Type serviceType in serviceTypes)
			{
				if (registration == null)
				{
					registration = For(serviceType);
				}
				else
				{
					registration.Forward(serviceType);
				}
			}

			if (registration == null)
			{
				throw new ArgumentException("At least one service type must be supplied");
			}

			return registration;
		}

		/// <summary>
		/// Creates a component registration for the service type.
		/// </summary>
		/// <typeparam name="S">The service type.</typeparam>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration<S> For<S>()
		{
			return new ComponentRegistration<S>();
		}

		#region Forwarded Service Types

		/// <summary>
		/// Creates a component registration for the service types.
		/// </summary>
		/// <typeparam name="S">The primary service type.</typeparam>
		/// <typeparam name="F">The forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration<S> For<S, F>()
		{
			return new ComponentRegistration<S>().Forward<F>();
		}

		/// <summary>
		/// Creates a component registration for the service types.
		/// </summary>
		/// <typeparam name="S">The primary service type.</typeparam>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration<S> For<S, F1, F2>()
		{
			return new ComponentRegistration<S>().Forward<F1, F2>();
		}

		/// <summary>
		/// Creates a component registration for the service types.
		/// </summary>
		/// <typeparam name="S">The primary service type.</typeparam>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <typeparam name="F3">The third forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration<S> For<S, F1, F2, F3>()
		{
			return new ComponentRegistration<S>().Forward<F1, F2, F3>();
		}

		/// <summary>
		/// Creates a component registration for the service types.
		/// </summary>
		/// <typeparam name="S">The primary service type.</typeparam>
		/// <typeparam name="F1">The first forwarded type.</typeparam>
		/// <typeparam name="F2">The second forwarded type.</typeparam>
		/// <typeparam name="F3">The third forwarded type.</typeparam>
		/// <typeparam name="F4">The fourth forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration<S> For<S, F1, F2, F3, F4>()
		{
			return new ComponentRegistration<S>().Forward<F1, F2, F3, F4>();
		}

		#endregion

		/// <summary>
		/// Create a component registration for an exisiting <see cref="ComponentModel"/>
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration For(ComponentModel model)
		{
			return new ComponentRegistration(model);
		}

		/// <summary>
		/// Determines if the component service is already registered.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="model">The component model.</param>
		/// <returns>true if the service is already registered.</returns>
		public static bool ServiceAlreadyRegistered(IKernel kernel, ComponentModel model)
		{
			return kernel.HasComponent(model.Service);
		}

		/// <summary>
		/// Creates a predicate to check if a component is in a namespace.
		/// </summary>
		/// <param name="namespace">The namespace.</param>
		/// <returns>true if the component type is in the namespace.</returns>
		public static Predicate<Type> IsInNamespace(string @namespace)
		{
			return delegate(Type type) { return type.Namespace == @namespace; };
		}

		/// <summary>
		/// Creates a predicate to check if a component shares a namespace with another.
		/// </summary>
		/// <param name="type">The component type to test namespace against.</param>
		/// <returns>true if the component is in the same namespace.</returns>
		public static Predicate<Type> IsInSameNamespaceAs(Type type)
		{
			return IsInNamespace(type.Namespace);
		}

		/// <summary>
		/// Creates a predicate to check if a component shares a namespace with another.
		/// </summary>
		/// <typeparam name="T">The component type to test namespace against.</typeparam>
		/// <returns>true if the component is in the same namespace.</returns>
		public static Predicate<Type> IsInSameNamespaceAs<T>() where T : class
		{
			return IsInSameNamespaceAs(typeof(T));
		}
	}
}
