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

namespace Castle.MicroKernel.Factory.Default
{
	using System;
	using System.Collections;

	/// <summary>
	/// ComponentInstanceBurden is responsible for keep track 
	/// of dependencies assembled by the container - not by 
	/// the component code - and release them on owner disposal
	/// </summary>
	public class ComponentInstanceBurden
	{
		private IList m_list = new ArrayList();

		public ComponentInstanceBurden()
		{
		}

		public void AddBurden(Object instance, IHandler handler)
		{
			m_list.Add(new BurdenData(instance, handler));
		}

		public bool HasBurden
		{
			get { return m_list.Count != 0; }
		}

		public void ReleaseBurden()
		{
			foreach(BurdenData data in m_list)
			{
				data.Handler.Release(data.Instance);
			}

			m_list.Clear();
		}
	}

	internal class BurdenData
	{
		private object m_instance;
		private IHandler m_handler;

		public BurdenData(object instance, IHandler handler)
		{
			AssertUtil.ArgumentNotNull(instance, "instance");
			AssertUtil.ArgumentNotNull(handler, "handler");

			m_instance = instance;
			m_handler = handler;
		}

		public object Instance
		{
			get { return m_instance; }
		}

		public IHandler Handler
		{
			get { return m_handler; }
		}
	}
}