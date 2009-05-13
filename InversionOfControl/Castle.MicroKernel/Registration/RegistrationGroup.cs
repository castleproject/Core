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
	public abstract class RegistrationGroup<S>
	{
		private readonly ComponentRegistration<S> registration;

		public RegistrationGroup(ComponentRegistration<S> registration)
		{
			this.registration = registration;	
		}

		public ComponentRegistration<S> Registration
		{
			get { return registration; }
		}

		protected ComponentRegistration<S> AddAttributeDescriptor(string name, string value)
		{
			return registration.AddDescriptor(new AttributeDescriptor<S>(name, value));
		}

		protected ComponentRegistration<S> AddDescriptor(ComponentDescriptor<S> descriptor)
		{
			return registration.AddDescriptor(descriptor);
		}
	}
}
