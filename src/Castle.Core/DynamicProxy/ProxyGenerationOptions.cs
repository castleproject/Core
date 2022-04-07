// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
#if FEATURE_SERIALIZATION
	using System.Runtime.Serialization;
#endif

	using Castle.DynamicProxy.Internal;

	/// <summary>
	///   <see cref="ProxyGenerationOptions"/> allows customization of the behavior of proxies created by
	///   an <see cref="IProxyGenerator"/> (or proxy types generated by an <see cref="IProxyBuilder"/>).
	///   <para>
	///     You should not modify an instance of <see cref="ProxyGenerationOptions"/> once it has been
	///     used to create a proxy (or proxy type).
	///   </para>
	/// </summary>
#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class ProxyGenerationOptions
#if FEATURE_SERIALIZATION
		: ISerializable
#endif
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private List<object> mixins;
		private readonly IList<CustomAttributeInfo> additionalAttributes = new List<CustomAttributeInfo>();

#if FEATURE_SERIALIZATION
		[NonSerialized]
#endif
		private MixinData mixinData; // this is calculated dynamically on proxy type creation

		/// <summary>
		///   Initializes a new instance of the <see cref = "ProxyGenerationOptions" /> class.
		/// </summary>
		/// <param name = "hook">The hook.</param>
		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			BaseTypeForInterfaceProxy = typeof(object);
			Hook = hook;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "ProxyGenerationOptions" /> class.
		/// </summary>
		public ProxyGenerationOptions()
			: this(new AllMethodsHook())
		{
		}

#if FEATURE_SERIALIZATION
		private ProxyGenerationOptions(SerializationInfo info, StreamingContext context)
		{
			Hook = (IProxyGenerationHook)info.GetValue("hook", typeof(IProxyGenerationHook));
			Selector = (IInterceptorSelector)info.GetValue("selector", typeof(IInterceptorSelector));
			mixins = (List<object>)info.GetValue("mixins", typeof(List<object>));
			BaseTypeForInterfaceProxy = Type.GetType(info.GetString("baseTypeForInterfaceProxy.AssemblyQualifiedName"));
		}
#endif

		public void Initialize()
		{
			if (mixinData == null)
			{
				try
				{
					mixinData = new MixinData(mixins);
				}
				catch (ArgumentException ex)
				{
					throw new InvalidOperationException(
						"There is a problem with the mixins added to this ProxyGenerationOptions. See the inner exception for details.", ex);
				}
			}
		}

#if FEATURE_SERIALIZATION
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("hook", Hook);
			info.AddValue("selector", Selector);
			info.AddValue("mixins", mixins);
			info.AddValue("baseTypeForInterfaceProxy.AssemblyQualifiedName", BaseTypeForInterfaceProxy.AssemblyQualifiedName);
		}
#endif

		/// <summary>
		///   Gets or sets the <see cref="IProxyGenerationHook"/> that should be used during proxy type
		///   generation. Defaults to an instance of <see cref="AllMethodsHook"/>.
		///   <para>
		///     You should not modify this property once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		public IProxyGenerationHook Hook { get; set; }

		/// <summary>
		///   Gets or sets the <see cref="IInterceptorSelector"/> that should be used by created proxies
		///   to determine which interceptors to use for an interception. If set to <see langword="null"/>
		///   (which is the default), created proxies will not use any selector.
		///   <para>
		///     You should not modify this property once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		public IInterceptorSelector Selector { get; set; }

		/// <summary>
		///   Gets or sets the class type from which generated interface proxy types will be derived.
		///   Defaults to <c><see langword="typeof"/>(<see langword="object"/>)</c>.
		///   <para>
		///     You should not modify this property once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		public Type BaseTypeForInterfaceProxy { get; set; }

		/// <summary>
		///   Gets the collection of additional custom attributes that will be put on generated proxy types.
		///   This collection is initially empty.
		///   <para>
		///     You should not modify this collection once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		public IList<CustomAttributeInfo> AdditionalAttributes
		{
			get { return additionalAttributes; }
		}

		public MixinData MixinData
		{
			get
			{
				if (mixinData == null)
				{
					throw new InvalidOperationException("Call Initialize before accessing the MixinData property.");
				}
				return mixinData;
			}
		}

		/// <summary>
		///   Adds a delegate type to the list of mixins that will be added to generated proxies.
		///   That is, generated proxies will have a `Invoke` method with a signature matching that
		///   of the specified <paramref name="delegateType"/>.
		///   <para>
		///     You should not call this method once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		/// <param name="delegateType">The delegate type whose `Invoke` method should be reproduced in generated proxies.</param>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> is not a delegate type.</exception>
		public void AddDelegateTypeMixin(Type delegateType)
		{
			if (delegateType == null) throw new ArgumentNullException(nameof(delegateType));
			if (!delegateType.IsDelegateType()) throw new ArgumentException("Type must be a delegate type.", nameof(delegateType));

			AddMixinImpl(delegateType);
		}

		/// <summary>
		///   Adds a delegate to be mixed into generated proxies. The <paramref name="delegate"/>
		///   will act as the target for calls to a `Invoke` method with a signature matching that
		///   of the delegate.
		///   <para>
		///     You should not call this method once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		/// <param name="delegate">The delegate that should act as the target for calls to `Invoke` methods with a matching signature.</param>
		/// <exception cref="ArgumentNullException"><paramref name="delegate"/> is <see langword="null"/>.</exception>
		public void AddDelegateMixin(Delegate @delegate)
		{
			if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

			AddMixinImpl(@delegate);
		}

		/// <summary>
		///   Mixes the interfaces implemented by the specified <paramref name="instance"/> object into
		///   created proxies, and uses <paramref name="instance"/> as the target for these mixed-in interfaces.
		///   <para>
		///     You should not call this method once this <see cref="ProxyGenerationOptions"/> instance
		///     has been used to create a proxy.
		///   </para>
		/// </summary>
		/// <param name="instance">The object that should act as the target for all of its implemented interfaces' methods.</param>
		/// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="instance"/> is an instance of <see cref="Type"/>.</exception>
		public void AddMixinInstance(object instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			if (instance is Type) throw new ArgumentException("You may not mix in types using this method.", nameof(instance));

			AddMixinImpl(instance);
		}

		private void AddMixinImpl(object instanceOrType)
		{
			if (mixins == null)
			{
				mixins = new List<object>();
			}

			mixins.Add(instanceOrType);
			mixinData = null;
		}

		public object[] MixinsAsArray()
		{
			if (mixins == null)
			{
				return new object[0];
			}

			return mixins.ToArray();
		}

		public bool HasMixins
		{
			get { return mixins != null && mixins.Count != 0; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (ReferenceEquals(proxyGenerationOptions, null))
			{
				return false;
			}

			// ensure initialization before accessing MixinData
			Initialize();
			proxyGenerationOptions.Initialize();

			if (!Equals(Hook, proxyGenerationOptions.Hook))
			{
				return false;
			}
			if (!Equals(Selector == null, proxyGenerationOptions.Selector == null))
			{
				return false;
			}
			if (!Equals(MixinData, proxyGenerationOptions.MixinData))
			{
				return false;
			}
			if (!Equals(BaseTypeForInterfaceProxy, proxyGenerationOptions.BaseTypeForInterfaceProxy))
			{
				return false;
			}
			if (!HasEquivalentAdditionalAttributes(proxyGenerationOptions))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			// ensure initialization before accessing MixinData
			Initialize();

			var result = Hook != null ? Hook.GetType().GetHashCode() : 0;
			result = 29*result + (Selector != null ? 1 : 0);
			result = 29*result + MixinData.GetHashCode();
			result = 29*result + (BaseTypeForInterfaceProxy != null ? BaseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29*result + GetAdditionalAttributesHashCode();
			return result;
		}

		private int GetAdditionalAttributesHashCode()
		{
			var result = 0;
			for (var i = 0; i < additionalAttributes.Count; i++)
			{
				if (additionalAttributes[i] != null)
				{
					// simply add since order does not matter
					result += additionalAttributes[i].GetHashCode();
				}
			}

			return result;
		}

		private bool HasEquivalentAdditionalAttributes(ProxyGenerationOptions other)
		{
			var listA = additionalAttributes;
			var listB = other.additionalAttributes;

			if (listA.Count != listB.Count)
			{
				return false;
			}

			// copy contents to another list so that contents can be removed as they are found,
			// in order to consider duplicates
			var listBAvailableContents = listB.ToList();

			// order is not important, just make sure that each entry in A is also found in B
			for (var i = 0; i < listA.Count; i++)
			{
				var found = false;

				for (var j = 0; j < listBAvailableContents.Count; j++)
				{
					if (Equals(listA[i], listBAvailableContents[j]))
					{
						found = true;
						listBAvailableContents.RemoveAt(j);
						break;
					}
				}

				if (!found)
				{
					return false;
				}
			}

			return true;
		}
	}
}