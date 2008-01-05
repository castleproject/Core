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
	public abstract class RegistrationGroup<S,T>
	{
		private readonly ComponentRegistration<S,T> registration;

		public RegistrationGroup(ComponentRegistration<S,T> registration)
		{
			this.registration = registration;	
		}

		public ComponentRegistration<S,T> Registration
		{
			get { return registration; }
		}

		protected ComponentRegistration<S,T> AddAttributeDescriptor(string name, string value)
		{
			return registration.AddDescriptor(new AttributeDescriptor<S,T>(name, value));
		}

		protected ComponentRegistration<S,T> AddDescriptor(ComponentDescriptor<S,T> descriptor)
		{
			return registration.AddDescriptor(descriptor);
		}
	}
}
