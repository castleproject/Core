#region Copyright
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
#endregion

namespace Castle.Windsor.Adapters.ComponentModel
{
	using System;
	using System.ComponentModel;
	
	using Castle.MicroKernel;

	internal class ContainerAdapterSite : IContainerAdapterSite
	{
		private string _name;
		private string _effectiveName;
		private IComponent _component;
		private ContainerAdapter _container;

		public ContainerAdapterSite(IComponent component, ContainerAdapter container, string name)
		{
			_component = component;
			_container = container;
			_name = name;

			if ((_effectiveName = _name) == null)
			{
				_effectiveName = Guid.NewGuid().ToString();
			}
		}

		#region ISite Members

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool DesignMode
		{
			get { return false; }
		}
 
		public IComponent Component
		{
			get { return _component; }
		}
 
		public IContainer Container
		{
			get { return _container; }
		}

		public string EffectiveName
		{
			get { return _effectiveName; }
		}

		public object GetService(Type service)
		{
			if (service == typeof(ISite) || service == typeof(IContainerAdapterSite))
			{
				return this;
			}
			else if (service == typeof(IHandler))
			{
				return _container.Kernel.GetHandler(_effectiveName);
			}
			else
			{
				return _container.GetService(service);
			}
		}

		#endregion
	}
}
