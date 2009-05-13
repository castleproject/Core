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

namespace Castle.MicroKernel.Registration.Lifestyle
{
	using System;
	using Castle.Core;

	public class LifestyleGroup<S> : RegistrationGroup<S>
	{
		public LifestyleGroup(ComponentRegistration<S> registration)
			: base(registration)
		{
		}

		/// <summary>
		/// Sets the lifestyle to the specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Is(LifestyleType type)
		{
			return AddDescriptor(new LifestyleDescriptor<S>(type));
		}

		public ComponentRegistration<S> Transient
		{
			get { return AddDescriptor(new Transient<S>()); }
		}

		public ComponentRegistration<S> Singleton
		{
			get { return AddDescriptor(new Singleton<S>()); }
		}

		public ComponentRegistration<S> PerThread
		{
			get { return AddDescriptor(new PerThread<S>()); }
		}

		public ComponentRegistration<S> PerWebRequest
		{
			get { return AddDescriptor(new PerWebRequest<S>()); }
		}

		public ComponentRegistration<S> Pooled
		{
			get { return AddDescriptor(new Pooled<S>()); }
		}

		public ComponentRegistration<S> PooledWithSize(int? initialSize, int? maxSize)
		{
			return AddDescriptor(new Pooled<S>(initialSize, maxSize));			
		}

		/// <summary>
		/// Assign a custom lifestyle type, that implements <see cref="ILifestyleManager"/>.
		/// </summary>
		/// <param name="customLifestyleType">Type of the custom lifestyle.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Custom(Type customLifestyleType)
		{
			if (!typeof(ILifestyleManager).IsAssignableFrom(customLifestyleType))
			{
				throw new ComponentRegistrationException(String.Format(
					"The type {0} must implement ILifestyleManager to " +
					"be used as a custom lifestyle", customLifestyleType.FullName));
			}

			return AddDescriptor(new Custom<S>(customLifestyleType));
		}

		/// <summary>
		/// Assign a custom lifestyle type, that implements <see cref="ILifestyleManager"/>.
		/// </summary>
		/// <typeparam name="L">The type of the custom lifestyle</typeparam>
		/// <returns></returns>
		public ComponentRegistration<S> Custom<L>()
			where L : ILifestyleManager, new()
		{
			return Custom(typeof(L));
		}
	}
}
