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

namespace Castle.MicroKernel.Registration.Lifestyle
{
	public class LifestyleGroup<S,T> : RegistrationGroup<S,T>
	{
		public LifestyleGroup(ComponentRegistration<S,T> registration)
			: base(registration)
		{
		}

		public ComponentRegistration<S,T> Transient
		{
			get { return AddDescriptor(new Transient<S,T>()); }
		}

		public ComponentRegistration<S,T> Singleton
		{
			get { return AddDescriptor(new Singleton<S,T>()); }
		}

		public ComponentRegistration<S,T> PerThread
		{
			get { return AddDescriptor(new PerThread<S,T>()); }
		}

		public ComponentRegistration<S,T> PerWebRequest
		{
			get { return AddDescriptor(new PerWebRequest<S,T>()); }
		}

		public ComponentRegistration<S,T> Pooled
		{
			get { return AddDescriptor(new Pooled<S,T>()); }
		}

		public ComponentRegistration<S,T> PooledWithSize(int initialSize, int maxSize)
		{
			return AddDescriptor(new Pooled<S,T>(initialSize, maxSize));			
		}

		public ComponentRegistration<S,T> Custom<L>()
			where L : ILifestyleManager, new()
		{
			return AddDescriptor(new Custom<S,T,L>());
		}
	}
}
