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

namespace Castle.ManagementExtensions.Remote
{
	using System;

	/// <summary>
	/// Summary description for MServerProxy.
	/// </summary>
	public class MServerProxy : MarshalByRefObject, MServer
	{
		protected MServer server;

		public MServerProxy(MServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}

			this.server = server;
		}

		#region MServer Members

		public Object Instantiate(String typeName)
		{
			return server.Instantiate(typeName);
		}

		public Object Instantiate(String assemblyName, String typeName)
		{
			return server.Instantiate(assemblyName, typeName);
		}

		public ManagedInstance CreateManagedObject(String typeName, ManagedObjectName name)
		{
			return server.CreateManagedObject(typeName, name);
		}

		public ManagedInstance CreateManagedObject(String assemblyName, String typeName, ManagedObjectName name)
		{
			return server.CreateManagedObject(assemblyName, typeName, name);
		}

		public ManagedInstance RegisterManagedObject(Object instance, ManagedObjectName name)
		{
			return server.RegisterManagedObject(instance, name);
		}

		public ManagedInstance GetManagedInstance(ManagedObjectName name)
		{
			return server.GetManagedInstance(name);
		}

		public void UnregisterManagedObject(ManagedObjectName name)
		{
			server.UnregisterManagedObject(name);
		}

		public Object Invoke(ManagedObjectName name, String action, Object[] args, Type[] signature)
		{
			return server.Invoke(name, action, args, signature);
		}

		public ManagementInfo GetManagementInfo(ManagedObjectName name)
		{
			return server.GetManagementInfo(name);
		}

		public Object GetAttribute(ManagedObjectName name, String attributeName)
		{
			return server.GetAttribute(name, attributeName);
		}

		public void SetAttribute(ManagedObjectName name, String attributeName, Object attributeValue)
		{
			server.SetAttribute(name, attributeName, attributeValue);
		}

		public ManagedObjectName[] Query(ManagedObjectName query)
		{
			return server.Query(query);
		}

		#endregion

		#region MServerConnection Members

		public String[] GetDomains()
		{
			return server.GetDomains();
		}

		#endregion
	}
}
