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

namespace Castle.MicroKernel.Proxy
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents options to configure proxies.
	/// </summary>
	public class ProxyOptions
	{
		private IProxyHook hook;
		private ArrayList interfaceList;
		private bool useSingleInterfaceProxy;
		private bool useMarshalByRefAsBaseClass;
		private bool omitTarget;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyOptions"/> class.
		/// </summary>
		public ProxyOptions()
		{
			useSingleInterfaceProxy = false;
			omitTarget = false;
		}

		/// <summary>
		/// Gets or sets the proxy hook.
		/// </summary>
		public IProxyHook Hook
		{
			get { return hook; }
			set { hook = value; }
		}

		/// <summary>
		/// Determines if the proxied component uses a target.
		/// </summary>
		public bool OmitTarget
		{
			get { return omitTarget; }
			set { omitTarget = value; }
		}

		/// <summary>
		/// Determines if the proxied component should only include
		/// the service interface.
		/// </summary>
		public bool UseSingleInterfaceProxy
		{
			get { return useSingleInterfaceProxy; }
			set { useSingleInterfaceProxy = value; }
		}

		/// <summary>
		/// Determines if the interface proxied component should inherit 
		/// from <see cref="MarshalByRefObject"/>
		/// </summary>
		public bool UseMarshalByRefAsBaseClass
		{
			get { return useMarshalByRefAsBaseClass; }
			set { useMarshalByRefAsBaseClass = value; }
		}

		/// <summary>
		/// Gets the additional interfaces to proxy.
		/// </summary>
		/// <value>The interfaces.</value>
		public Type[] AdditionalInterfaces
		{
			get
			{
				if (interfaceList != null)
				{
					return (Type[]) interfaceList.ToArray(typeof(Type));
				}

				return new Type[0];
			}
		}

		/// <summary>
		/// Adds the additional interfaces to proxy.
		/// </summary>
		/// <param name="interfaces">The interfaces.</param>
		public void AddAdditionalInterfaces(params Type[] interfaces)
		{
			if (interfaces == null)
			{
				throw new ArgumentNullException("interfaces");
			}

			if (interfaceList == null)
			{
				interfaceList = new ArrayList();
			}

			interfaceList.AddRange(interfaces);
		}

		/// <summary>
		/// Equalses the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>true if equal.</returns>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ProxyOptions proxyOptions = obj as ProxyOptions;
			if (proxyOptions == null) return false;
			if (!Equals(hook, proxyOptions.hook)) return false;
			if (!Equals(useSingleInterfaceProxy, proxyOptions.useSingleInterfaceProxy)) return false;
			if (!Equals(omitTarget, proxyOptions.omitTarget)) return false;
			return AdditionalInterfacesAreEquals(proxyOptions);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return 29 * base.GetHashCode() + GetAdditionalInterfacesHashCode();
		}

		private bool AdditionalInterfacesAreEquals(ProxyOptions proxyOptions)
		{
			if (!Equals(interfaceList == null, proxyOptions.interfaceList == null)) return false;
			if (interfaceList == null) return true; //both are null, nothing more to check
			if (interfaceList.Count != proxyOptions.interfaceList.Count) return false;
			for(int i = 0; i < interfaceList.Count; ++i)
			{
				if (!proxyOptions.interfaceList.Contains(interfaceList[0])) return false;
			}
			return true;
		}

		private int GetAdditionalInterfacesHashCode()
		{
			int result = 0;

			if (interfaceList == null) return result;

			foreach(object type in interfaceList)
			{
				result = 29 * result + type.GetHashCode();
			}

			return result;
		}
	}
}