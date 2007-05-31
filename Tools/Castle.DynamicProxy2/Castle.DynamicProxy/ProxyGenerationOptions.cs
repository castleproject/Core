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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// The proxy generation options, note that this is a statefull class with regard to mixin.
	/// </summary>
	[Serializable]
	public class ProxyGenerationOptions : ISerializable
	{
		/// <summary>
		/// Gets the default options
		/// </summary>
		/// <value>The default.</value>
		public static ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private IProxyGenerationHook hook;
		private IInterceptorSelector selector;
		private ArrayList mixins;
		private ArrayList mixinsImpl;
		private List<Type> mixinsTypes;
		private Dictionary<Type, int> mixinPositions;
		private Type baseTypeForInterfaceProxy = typeof(object);
		private bool useSelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		/// <param name="hook">The hook.</param>
		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			this.hook = hook;
			useSelector = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		public ProxyGenerationOptions()
			: this(new AllMethodsHook())
		{
		}

		private ProxyGenerationOptions(SerializationInfo info, StreamingContext context)
		{
			hook = (IProxyGenerationHook) info.GetValue("hook", typeof(IProxyGenerationHook));
			selector = (IInterceptorSelector) info.GetValue("selector", typeof(IInterceptorSelector));
			useSelector = info.GetBoolean("useSelector");
			mixins = (ArrayList) info.GetValue("mixins", typeof(ArrayList));

			string baseTypeName = info.GetString("baseTypeForInterfaceProxy");
			baseTypeForInterfaceProxy = Type.GetType(baseTypeName);

			Initialize();
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

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ProxyGenerationOptions proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (proxyGenerationOptions == null) return false;
			if (!Equals(hook.GetType(), proxyGenerationOptions.hook.GetType())) return false;
			if (!Equals(selector, proxyGenerationOptions.selector)) return false;
			if (!MixinTypesAreEquals(proxyGenerationOptions)) return false;
			if (!Equals(baseTypeForInterfaceProxy, proxyGenerationOptions.baseTypeForInterfaceProxy)) return false;
			if (!Equals(useSelector, proxyGenerationOptions.useSelector)) return false;
			return true;
		}

		private bool MixinTypesAreEquals(ProxyGenerationOptions proxyGenerationOptions)
		{
			if (!Equals(mixinsTypes == null, proxyGenerationOptions.mixinsTypes == null)) return false;
			if (mixins == null) return true; //both are null, nothing more to check
			if (mixinsTypes.Count != proxyGenerationOptions.mixinsTypes.Count) return false;
			for(int i = 0; i < mixinsTypes.Count; i++)
			{
				if (!Equals(mixinsTypes[i], proxyGenerationOptions.mixinsTypes[i])) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int result = hook != null ? hook.GetType().GetHashCode() : 0;
			result = 29 * result + (selector != null ? selector.GetHashCode() : 0);
			result = 29 * result + (GetMixinHashCode());
			result = 29 * result + (baseTypeForInterfaceProxy != null ? baseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29 * result + useSelector.GetHashCode();
			return result;
		}

		private int GetMixinHashCode()
		{
			int result = 0;
			if (mixinsTypes == null)
				return result;
			foreach(object mixin in mixinsTypes)
			{
				result = 29 * result + mixin.GetHashCode();
			}
			return result;
		}

		/// <summary>
		/// This is required because a mixin may implement more than a single interface.
		/// In order to track that, we register them all here, and when we instansiate the proxy, we pass it the 
		/// mixins implementations, where each interface has an object that implements it.
		/// Example:
		/// FooBar foo implements IFoo and IBar
		/// 
		/// proxy ctor would be:
		/// 
		/// public Proxy(IFoo, IBar, IInterceptor[], object target)
		/// 
		/// And will be invoked with:
		/// new Proxy(foo, foo, inteceptors, target);
		/// </summary>
		/// <param name="mixin">The mixin.</param>
		private void AddMixinInterfaceImplementation(object mixin)
		{
			if (mixinsImpl == null)
			{
				mixinsImpl = new ArrayList();
			}
			mixinsImpl.Add(mixin);
		}

		internal object[] MixinInterfaceImplementationsAsArray()
		{
			if (mixinsImpl == null)
				return new object[0];
			return mixinsImpl.ToArray();
		}

		/// <summary>
		/// Because we need to cache the types based on the mixed in mixins, we do the following here:
		///  - Get all the mixin interfaces
		///  - Sort them by full name
		///  - Return them by position
		/// 
		/// The idea is to have reproducable behavior for the case that mixins are registered in different orders.
		/// This method is here because it is required 
		/// </summary>
		/// <returns></returns>
		private void InspectAndRegisterMixinInterfaces()
		{
			if (HasMixins == false)
				return;

			mixinsTypes = new List<Type>();
			Dictionary<Type, object> interface2Mixin = new Dictionary<Type, object>();

			foreach(object mixin in MixinsAsArray())
			{
				Type[] mixinInterfaces = mixin.GetType().GetInterfaces();

				foreach(Type inter in mixinInterfaces)
				{
					mixinsTypes.Add(inter);
					interface2Mixin[inter] = mixin;
				}
			}
			mixinsTypes.Sort(delegate(Type x, Type y) { return x.FullName.CompareTo(y.FullName); });
			mixinPositions = new Dictionary<Type, int>();
			for(int i = 0; i < mixinsTypes.Count; i++)
			{
				Type mixinType = mixinsTypes[i];
				object mixin = interface2Mixin[mixinType];
				mixinPositions[mixinType] = i;
				AddMixinInterfaceImplementation(mixin);
			}
			return;
		}

		internal void Initialize()
		{
			if (!HasMixins) //there is no initialization if you don't have mixins
				return;
			InspectAndRegisterMixinInterfaces();
		}

		public Dictionary<Type, int> GetMixinInterfacesAndPositions()
		{
			return mixinPositions;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("hook", hook);
			info.AddValue("selector", selector);
			info.AddValue("useSelector", useSelector);
			info.AddValue("mixins", mixins);
			// remaining mixin fields are not serialized, they are recalculated in the Initialize method called from the deserialization constructor

			// avoid serializing Type objects, they lead to problems with deserialization order, see SerializableClassTestCase.ProxyGenerationOptionsRespectedOnDeserializationComplex
			info.AddValue("baseTypeForInterfaceProxy", baseTypeForInterfaceProxy.AssemblyQualifiedName);
		}
	}
}