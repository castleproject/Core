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
	
	using Castle.MicroKernel;
	using Castle.Model.Configuration;
	
	using System.Runtime.Remoting;

	public class RemoteSubSystem : AbstractSubSystem, IRemoteSubSystem
	{
		private String _remoteUri;
		[NonSerialized()]
		private IKernel _kernel;
		[NonSerialized()]
		private IConfiguration _config;
		private IRemoteSubSystem _remote;

		public RemoteSubSystem(IConfiguration config)
		{
			_config = config;
			_remoteUri = config.Attributes["remote"];
		}

		#region IRemoteSubSystem Members

		public String RemoteUri
		{
			get { return _remoteUri; }
			set { _remoteUri = value; }
		}

		public IRemoteSubSystem Remote
		{
			get 
			{ 
				if(_remote==null && _remoteUri!=null)
					_remote = (IRemoteSubSystem)Activator.GetObject(typeof(IRemoteSubSystem),_remoteUri+"/"+KnownRemoteConsts.SubSystemRemoteConnection);
				return _remote; 
			}
		}

		public virtual void Connect()
		{
			_remote = null;
		}

		public void Connect( String remoteUri )
		{
			_remoteUri = remoteUri;
			Connect();
		}

		public ISubSystem GetSubSystem(String key)
		{
			if(Remote!=null)
				return Remote.GetSubSystem(key);
			else
				return _kernel.GetSubSystem(key);
		}

		#endregion

		#region ISubSystem Members

		public override void Init(IKernel kernel)
		{
			_kernel = kernel;
			RemotingServices.Marshal(this,KnownRemoteConsts.SubSystemRemoteConnection);
		}

		#endregion
	}
}
