// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Windsor
{
	using System;

	using Castle.MicroKernel;
	using Castle.Windsor.Configuration.AppDomain;

	/// <summary>
	/// Summary description for Windsor.
	/// </summary>
	public class WindsorContainer : IWindsorContainer
	{
		private IKernel _kernel;
		private IWindsorContainer _parent;

		public WindsorContainer() : this(new AppDomainConfigurationStore())
		{
			
		}

		public WindsorContainer(IConfigurationStore store) : this(new DefaultKernel())
		{
			Kernel.ConfigurationStore = store;
		}

		public WindsorContainer(IKernel kernel)
		{
			_kernel = kernel;
			_kernel.ProxyFactory = new Proxy.DefaultProxyFactory();
		}

		#region IWindsorContainer Members

		public IKernel Kernel
		{
			get { return _kernel; }
		}

		public IWindsorContainer Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public void AddFacility(String key, IFacility facility)
		{
			_kernel.AddFacility(key, facility);
		}

		public void AddComponent(String key, Type classType)
		{
			Kernel.AddComponent(key, classType);
		}

		public void AddComponent(String key, Type serviceType, Type classType)
		{
			Kernel.AddComponent(key, serviceType, classType);
		}

		public object Resolve(String key)
		{
			return Kernel[key];
		}

		public object Resolve(Type service)
		{
			return Kernel[service];
		}

		public void Release(object instance)
		{
			Kernel.ReleaseComponent(instance);
		}

		public void AddChildContainer(IWindsorContainer childContainer)
		{
			childContainer.Parent = this;
			Kernel.AddChildKernel(childContainer.Kernel);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_kernel.Dispose();
		}

		#endregion
	}
}