// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Remote.Server
{
	using System;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;

	/// <summary>
	/// Summary description for MConnectorServer.
	/// </summary>
	[ManagedComponent]
	public class MConnectorServer : MarshalByRefObject, MRegistrationListener, IDisposable
	{
		public static readonly ManagedObjectName DEFAULT_NAME = new ManagedObjectName("connector.server");

		protected MServer server;
		protected MServerProxy serverProxy;
		protected String objectUri;
		protected ManagedObjectName name;

		private bool initDone = false;

		public MConnectorServer()
		{
		}

		public MConnectorServer(String objectUri)
		{
			this.objectUri = objectUri;
		}

		public MConnectorServer(MServer server, String objectUri) : this(objectUri)
		{
			if (IsProxy(server))
			{
				throw new ArgumentException("Argument can't be transparent proxy", "server");
			}

			this.server = server;

			RegisterServer();
		}

		~MConnectorServer()
		{
			DeregisterServer();
		}

		#region MRegistrationListener Members

		public void BeforeRegister(MServer server, ManagedObjectName name)
		{
			this.server = server;
			this.name = name;

			RegisterServer();
		}

		public void AfterRegister()
		{
		}

		public void BeforeDeregister()
		{
		}

		public void AfterDeregister()
		{
			DeregisterServer();
		}

		#endregion

		[ManagedAttribute]
		public ManagedObjectName ManagedObjectName
		{
			get
			{
				return name;
			}
		}

		[ManagedAttribute]
		public String ServerUri
		{
			get
			{
				return objectUri;
			}
			set
			{
				objectUri = value;
			}
		}

		[ManagedAttribute]
		public MServer Server
		{
			get
			{
				return server;
			}
			set
			{
				if (IsProxy(value))
				{
					throw new ArgumentException("Argument can't be transparent proxy", "server");
				}
				server = value;
			}
		}

		private void RegisterServer()
		{
			if (initDone)
			{
				return;
			}

			if (serverProxy == null)
			{
				serverProxy = new MServerProxy(server);
			}

			ObjRef objref = RemotingServices.Marshal(
				serverProxy, ServerUri, typeof(MServerProxy) );

			initDone = true;
		}

		private void DeregisterServer()
		{
			if (initDone)
			{
				if (!RemotingServices.IsTransparentProxy( serverProxy ))
				{
					RemotingServices.Disconnect( serverProxy );
				}
				initDone = false;
			}
		}

		private bool IsProxy(object obj)
		{
			return RemotingServices.IsTransparentProxy( obj );
		}

		#region IDisposable Members

		public void Dispose()
		{
			DeregisterServer();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
