// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.RemoteIntegration
{

	using System;
	using System.Collections;
	using System.Runtime.Remoting;

	using System.Collections.Specialized;
	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Naming;

	public class RemoteNamingSubSystem : DefaultNamingSubSystem
	{
		[NonSerialized()]
		private IKernel _kernel;

		public RemoteNamingSubSystem( IDictionary key2Handler, IDictionary service2Handler)
		{
			this.key2Handler = key2Handler;
			this.service2Handler = service2Handler;
		}

		public override IHandler GetHandler(Type service)
		{
			IHandler handler = base.GetHandler(service);
			if(handler == null && Remote!=null)
				handler = ((INamingSubSystem)Remote.GetSubSystem(SubSystemConstants.NamingKey)).GetHandler(service);
			return handler;
		}

		public override IHandler GetHandler(String key)
		{
			IHandler handler = base.GetHandler(key);
			if(handler == null && Remote!=null)
				handler = ((INamingSubSystem)Remote.GetSubSystem(SubSystemConstants.NamingKey)).GetHandler(key);
			return handler;
		}

		public override void Init(IKernel kernel)
		{
			_kernel = kernel;
		}
		
		private IRemoteSubSystem Remote
		{
			get
			{
				ISubSystem remote = _kernel.GetSubSystem(KnownRemoteConsts.SubSystemRemoteConnection);
				if(remote!=null)
					return (IRemoteSubSystem)remote;
				else
					return null;
			}
		}
	}
}
