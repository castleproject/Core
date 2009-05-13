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

namespace Castle.MicroKernel.Registration.Interceptor
{
	using System;
	using Castle.Core;

	public class InterceptorDescriptor<S> : ComponentDescriptor<S>
	{
		private readonly Where where;
		private readonly int insertIndex;
		private readonly InterceptorReference[] interceptors;

		public enum Where
		{
			First,
			Last,
			Insert,
			Default
		}

		public InterceptorDescriptor(InterceptorReference[] interceptors, Where where)
		{
			this.interceptors = interceptors;
			this.where = where;
		}

		public InterceptorDescriptor(InterceptorReference[] interceptors, int insertIndex)
			: this(interceptors, Where.Insert)
		{
			if (insertIndex < 0)
			{
				throw new ArgumentOutOfRangeException("insertIndex", "insertIndex must be >= 0");
			}

			this.insertIndex = insertIndex;
		}

		public InterceptorDescriptor(InterceptorReference[] interceptors)
		{
			where = Where.Default;
			this.interceptors = interceptors;
		}

		protected internal override void ApplyToModel(IKernel kernel, ComponentModel model)
		{
			foreach(InterceptorReference interceptor in interceptors)
			{
				switch (where)
				{
					case Where.First:
						model.Interceptors.AddFirst(interceptor);
						break;

					case Where.Last:
						model.Interceptors.AddLast(interceptor);
						break;

					case Where.Insert:
						model.Interceptors.Insert(insertIndex, interceptor);
						break;

					default:
						model.Interceptors.Add(interceptor);
						break;
				}

				AddDependencyModel(interceptor, model);
			}
		}

		private static void AddDependencyModel(InterceptorReference interceptor, ComponentModel model)
		{
			DependencyModel dependency = new DependencyModel(
				DependencyType.Service, interceptor.ComponentKey,
				interceptor.ServiceType, false);
			model.Dependencies.Add(dependency);
		}
	}
}