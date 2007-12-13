// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration.Proxy
{
	using System;

	public class ProxyGroup<S,T> : RegistrationGroup<S,T>
	{
		public ProxyGroup(ComponentRegistration<S,T> registration)
			: base(registration)
		{
		}

		public ComponentRegistration<S,T> UsingSingleInterface
		{
			get
			{
				return AddAttributeDescriptor("useSingleInterfaceProxy", "true");
			}
		}

		public ComponentRegistration<S,T> AsMarshalByRefClass
		{
			get
			{
				return AddAttributeDescriptor("marshalByRefProxy", "true");
			}
		}

		public ComponentRegistration<S,T> AdditionalInterfaces(params Type[] interfaces)
		{
			AddDescriptor(new ProxyInterfaces<S,T>(interfaces));
			return Registration;
		}
	}
}