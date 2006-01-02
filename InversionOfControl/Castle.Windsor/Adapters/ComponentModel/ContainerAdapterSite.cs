// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Adapters.ComponentModel
{
	using System;
	using System.ComponentModel;
	
	using Castle.MicroKernel;

	internal class ContainerAdapterSite : IContainerAdapterSite
	{
		private string name;
		private string effectiveName;
		private IComponent component;
		private IContainerAdapter container;

		public ContainerAdapterSite(IComponent component, IContainerAdapter container, string name)
		{
			this.component = component;
			this.container = container;
			this.name = name;

			if ((effectiveName = name) == null)
			{
				effectiveName = Guid.NewGuid().ToString();
			}
		}

		#region ISite Members

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool DesignMode
		{
			get { return false; }
		}
 
		public IComponent Component
		{
			get { return component; }
		}
 
		public IContainer Container
		{
			get { return container; }
		}

		public string EffectiveName
		{
			get { return effectiveName; }
		}

		public object GetService(Type service)
		{
			if (service == typeof(ISite) || service == typeof(IContainerAdapterSite))
			{
				return this;
			}
			else if (service == typeof(IHandler))
			{
				return container.Container.Kernel.GetHandler(effectiveName);
			}
			else
			{
				return container.GetService(service);
			}
		}

		#endregion
	}
}
