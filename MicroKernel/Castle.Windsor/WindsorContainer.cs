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

	/// <summary>
	/// Summary description for Windsor.
	/// </summary>
	public class WindsorContainer : IWindsorContainer
	{
		private IKernel _kernel;

		public WindsorContainer() : this(new DefaultKernel())
		{
		}

		public WindsorContainer(IKernel kernel)
		{
			_kernel = kernel;
		}

		#region IWindsorContainer Members

		public IKernel Kernel
		{
			get
			{
				// TODO:  Add WindsorContainer.Kernel getter implementation
				return null;
			}
		}

		public IWindsorContainer Parent
		{
			get
			{
				// TODO:  Add WindsorContainer.Parent getter implementation
				return null;
			}
		}

		public void AddFacility( IFacility facility )
		{
			_kernel.AddFacility( facility );
		}

		public void AddComponent(String key, Type classType)
		{
			// TODO:  Add WindsorContainer.AddComponent implementation
		}

		public void AddComponent(String key, Type serviceType, Type classType)
		{
			// TODO:  Add WindsorContainer.Castle.Windsor.IWindsorContainer.AddComponent implementation
		}

		public object Resolve(String key)
		{
			// TODO:  Add WindsorContainer.Resolve implementation
			return null;
		}

		public object Resolve(Type service)
		{
			// TODO:  Add WindsorContainer.Castle.Windsor.IWindsorContainer.Resolve implementation
			return null;
		}

		public void Release(object instance)
		{
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
