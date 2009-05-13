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

namespace Castle.MicroKernel.Registration.Proxy
{
	using System;

	public class ProxyGroup<S> : RegistrationGroup<S>
	{
		public ProxyGroup(ComponentRegistration<S> registration)
			: base(registration)
		{
		}

		public ComponentRegistration<S> UsingSingleInterface
		{
			get
			{
				return AddAttributeDescriptor("useSingleInterfaceProxy", "true");
			}
		}

		public ComponentRegistration<S> AsMarshalByRefClass
		{
			get
			{
				return AddAttributeDescriptor("marshalByRefProxy", "true");
			}
		}

		public ComponentRegistration<S> AdditionalInterfaces(params Type[] interfaces)
		{
			AddDescriptor(new ProxyInterfaces<S>(interfaces));
			return Registration;
		}

		public ComponentRegistration<S> MixIns(params object[] mixIns)
		{
			AddDescriptor(new ProxyMixIns<S>(mixIns));
			return Registration;
		}
	}
}