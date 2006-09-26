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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections;

	/// <summary>
	/// 
	/// </summary>
	public class ProxyGenerationOptions
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private IProxyGenerationHook hook;
		private IInterceptorSelector selector;
		private ArrayList mixins;
		private Type baseTypeForInterfaceProxy = typeof(object);
		private bool useSelector;

		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			this.hook = hook;
			useSelector = false;
		}

		public ProxyGenerationOptions() : this(new AllMethodsHook())
		{
		}

		public IProxyGenerationHook Hook
		{
			get { return hook; }
			set { hook = value; }
		}

		public IInterceptorSelector Selector
		{
			get { return selector; }
			set { selector = value; }
		}

		public bool UseSelector
		{
			get { return useSelector; }
			set { useSelector = value; }
		}

		public void AddMixinInstance(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			if (mixins == null)
			{
				mixins = new ArrayList();
			}
			
			mixins.Add(instance);
		}

		public object[] MixinsAsArray()
		{
			if (mixins == null) return new object[0];
			
			return mixins.ToArray();
		}

		public bool HasMixins
		{
			get { return mixins == null ? false : mixins.Count != 0; }
		}

		public Type BaseTypeForInterfaceProxy
		{
			get { return baseTypeForInterfaceProxy; }
			set { baseTypeForInterfaceProxy = value; }
		}
	}
}
