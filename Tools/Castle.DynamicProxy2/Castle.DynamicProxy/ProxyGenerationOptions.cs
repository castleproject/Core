// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Runtime.Serialization;

#if SILVERLIGHT
	public class ProxyGenerationOptions
#else
	[Serializable]
	public class ProxyGenerationOptions : ISerializable
#endif
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private IProxyGenerationHook hook;
		private IInterceptorSelector selector;
		private List<object> mixins;
		private Type baseTypeForInterfaceProxy = typeof(object);
		private bool useSingleInterfaceProxy;

#if SILVERLIGHT
#else
		[NonSerialized]
#endif
		private MixinData mixinData; // this is calculated dynamically on proxy type creation

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		/// <param name="hook">The hook.</param>
		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			this.hook = hook;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		public ProxyGenerationOptions()
			: this(new AllMethodsHook())
		{
		}

#if SILVERLIGHT
#warning What to do?
#else
		private ProxyGenerationOptions(SerializationInfo info, StreamingContext context)
		{
			hook = (IProxyGenerationHook)info.GetValue("hook", typeof(IProxyGenerationHook));
			selector = (IInterceptorSelector)info.GetValue("selector", typeof(IInterceptorSelector));
			mixins = (List<object>)info.GetValue("mixins", typeof(List<object>));
			baseTypeForInterfaceProxy = Type.GetType(info.GetString("baseTypeForInterfaceProxy.AssemblyQualifiedName"));
			useSingleInterfaceProxy = info.GetBoolean("useSingleInterfaceProxy");
		}
#endif

		public void Initialize()
		{
			if (mixinData == null)
				mixinData = new MixinData(mixins);
		}

#if SILVERLIGHT
#warning What to do?
#else
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("hook", hook);
			info.AddValue("selector", selector);
			info.AddValue("mixins", mixins);
			info.AddValue("baseTypeForInterfaceProxy.AssemblyQualifiedName", baseTypeForInterfaceProxy.AssemblyQualifiedName);
			info.AddValue("useSingleInterfaceProxy", useSingleInterfaceProxy);
		}
#endif

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

		public bool UseSingleInterfaceProxy
		{
			get { return useSingleInterfaceProxy; }
			set { useSingleInterfaceProxy = value; }
		}

		public MixinData MixinData
		{
			get
			{
				if (mixinData == null)
					throw new InvalidOperationException("Call Initialize before accessing the MixinData property.");
				return mixinData;
			}
		}

		public void AddMixinInstance(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			if (mixins == null)
			{
				mixins = new List<object>();
			}

			mixins.Add(instance);
			mixinData = null;
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

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;

			ProxyGenerationOptions proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (ReferenceEquals(proxyGenerationOptions, null)) return false;

			// ensure initialization before accessing MixinData
			Initialize();
			proxyGenerationOptions.Initialize();

			if (!Equals(hook, proxyGenerationOptions.hook)) return false;
			if (!Equals(selector, proxyGenerationOptions.selector)) return false;
			if (!Equals(MixinData, proxyGenerationOptions.MixinData)) return false;
			if (!Equals(baseTypeForInterfaceProxy, proxyGenerationOptions.baseTypeForInterfaceProxy)) return false;
			if (!Equals(useSingleInterfaceProxy, proxyGenerationOptions.useSingleInterfaceProxy)) return false;

			return true;
		}

		public override int GetHashCode()
		{
			// ensure initialization before accessing MixinData
			Initialize();

			int result = hook != null ? hook.GetType().GetHashCode() : 0;
			result = 29 * result + (selector != null ? selector.GetHashCode() : 0);
			result = 29 * result + MixinData.GetHashCode();
			result = 29 * result + (baseTypeForInterfaceProxy != null ? baseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29 * result + useSingleInterfaceProxy.GetHashCode();
			return result;
		}
	}
}
