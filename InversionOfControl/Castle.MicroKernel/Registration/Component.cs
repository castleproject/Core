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
	using Castle.Core;

	public static class Component
	{
		/// <summary>
		/// Creates a component registration for the <paramref name="serviceType"/>
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <returns>The component registration.</returns>
		public static ComponentRegistration For(Type serviceType)
		{
			return new ComponentRegistration(serviceType);
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
		public static bool ServiceAlreadyRegistred(IKernel kernel, ComponentModel model)
		{
			return kernel.HasComponent(model.Service);
		}
	}
}
