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

namespace AspectSharp.Core.Proxy
{
	using System;
	using System.Collections;
	using AspectSharp.Lang.AST;
	using AspectSharp.Core.Dispatcher;

	/// <summary>
	/// Default implementation of IProxyFactory which uses Castle.DynamicProxy to
	/// generate the proxies.
	/// </summary>
	public class DefaultProxyFactory : IProxyFactory
	{
		private CustomProxyGenerator _generator = new CustomProxyGenerator();

		private IInvocationDispatcherFactory _dispatcherFactory;

		private AspectEngine _engine;

		/// <summary>
		/// For caching, we associate the AspectDefinition with a
		/// dynamic Type to avoid regenerate it.
		/// </summary>
		private Hashtable _aspect2ProxyType = new Hashtable();

		/// <summary>
		/// Constructs a DefaultProxyFactory
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="dispatcherFactory"></param>
		public DefaultProxyFactory(AspectEngine engine, IInvocationDispatcherFactory dispatcherFactory)
		{
			AssertUtil.ArgumentNotNull(engine, "engine");
			AssertUtil.ArgumentNotNull(dispatcherFactory, "dispatcherFactory");

			_engine = engine;
			_dispatcherFactory = dispatcherFactory;
		}

		/// <summary>
		/// Constructs a DefaultProxyFactory
		/// </summary>
		/// <param name="engine"></param>
		public DefaultProxyFactory(AspectEngine engine) : this(engine, new DefaultDispatcherFactory())
		{
		}

		#region IProxyFactory Members

		public object CreateClassProxy(Type classType, AspectDefinition aspect)
		{
			AssertUtil.ArgumentNotNull(classType, "classType");
			AssertUtil.ArgumentNotNull(aspect, "aspect");

			IInvocationDispatcher dispatcher = _dispatcherFactory.Create(aspect, _engine);

			return CreateAndInstantiateClassProxy(classType, aspect, dispatcher);
		}

		public object CreateInterfaceProxy(Type inter, object target, AspectDefinition aspect)
		{
			AssertUtil.ArgumentNotNull(inter, "inter");
			AssertUtil.ArgumentNotNull(target, "target");
			AssertUtil.ArgumentNotNull(aspect, "aspect");

			IInvocationDispatcher dispatcher = _dispatcherFactory.Create(aspect, _engine);

			return CreateAndInstantiateInterfaceProxy(inter, target, aspect, dispatcher);
		}

		#endregion

		protected virtual object CreateAndInstantiateClassProxy(Type baseClass, AspectDefinition aspect, IInvocationDispatcher dispatcher)
		{
			object proxy = null;

			object[] mixins = InstantiateMixins(aspect.Mixins);
			proxy = ObtainClassProxyInstance(aspect, baseClass, mixins, dispatcher);
			InitializeMixins(proxy, mixins);

			return proxy;
		}

		protected virtual object CreateAndInstantiateInterfaceProxy(Type inter, object target, AspectDefinition aspect, IInvocationDispatcher dispatcher)
		{
			object proxy = null;

			object[] mixins = InstantiateMixins(aspect.Mixins);
			proxy = ObtainInterfaceProxyInstance(aspect, target, inter, mixins, dispatcher);
			InitializeMixins(proxy, mixins);

			return proxy;
		}

		private object ObtainClassProxyInstance(AspectDefinition aspect, Type baseClass, object[] mixins, IInvocationDispatcher dispatcher)
		{
			Type proxyType = GetProxyTypeFromCache(aspect, baseClass);

			if (proxyType != null)
			{
				return CreateClassProxyInstance(proxyType, mixins, dispatcher);
			}

			object proxy = _generator.CreateClassProxy(baseClass, mixins, dispatcher);
			RegisterProxyTypeInCache(aspect, baseClass, proxy.GetType());
			return proxy;
		}

		private object ObtainInterfaceProxyInstance(AspectDefinition aspect, object target, Type inter, object[] mixins, IInvocationDispatcher dispatcher)
		{
			Type proxyType = GetProxyTypeFromCache(aspect, target.GetType());

			if (proxyType != null)
			{
				return CreateInterfaceProxyInstance(proxyType, target, mixins, dispatcher);
			}

			object proxy = _generator.CreateProxy(inter, target, mixins, dispatcher);
			RegisterProxyTypeInCache(aspect, target.GetType(), proxy.GetType());
			return proxy;
		}

		#region Proxy instantiation related methods

		private object CreateClassProxyInstance(Type proxyType, object[] mixins, IInvocationDispatcher dispatcher)
		{
			object proxy;

			if (mixins.Length != 0)
			{
				proxy = Activator.CreateInstance(proxyType, new object[] {dispatcher, mixins});
			}
			else
			{
				proxy = Activator.CreateInstance(proxyType, new object[] {dispatcher});
			}

			return proxy;
		}

		private object CreateInterfaceProxyInstance(Type proxyType, object target, object[] mixins, IInvocationDispatcher dispatcher)
		{
			object proxy;

			if (mixins.Length != 0)
			{
				proxy = Activator.CreateInstance(proxyType, new object[] {dispatcher, target, mixins});
			}
			else
			{
				proxy = Activator.CreateInstance(proxyType, new object[] {dispatcher, target});
			}

			return proxy;
		}

		#endregion

		#region Caching related methods

		private Type GetProxyTypeFromCache(AspectDefinition aspect, Type baseType)
		{
			return _aspect2ProxyType[new AspectInstanceKey(aspect, baseType)] as Type;
		}

		private void RegisterProxyTypeInCache(AspectDefinition aspect, Type baseType, Type proxyType)
		{
			_aspect2ProxyType[new AspectInstanceKey(aspect, baseType)] = proxyType;
		}

		/// <summary>
		/// Key to identify cached aspect type instances
		/// </summary>
		private class AspectInstanceKey
		{
			private readonly String aspectName;
			private readonly String typeName;

			public AspectInstanceKey(AspectDefinition aspect, Type baseClass)
			{
				aspectName = aspect.Name;
				typeName = baseClass.FullName;
			}

			public override int GetHashCode()
			{
				return aspectName.GetHashCode() ^ typeName.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is AspectInstanceKey)
				{
					AspectInstanceKey k = obj as AspectInstanceKey;
					return (aspectName.Equals(k.aspectName) && typeName.Equals(k.typeName));
				}
				return false;
			}
		}

		#endregion

		#region Mixin related methods

		protected void InitializeMixins(object proxy, object[] mixins)
		{
			for (int i = 0; i < mixins.Length; i++)
			{
				object mixin = mixins[i];

				if (mixin is IProxyAware)
				{
					(mixin as IProxyAware).SetProxy(proxy);
				}
			}
		}

		protected object[] InstantiateMixins(MixinDefinitionCollection mixins)
		{
			object[] instances = new object[mixins.Count];

			for (int i = 0; i < mixins.Count; i++)
			{
				MixinDefinition definition = mixins[i];
				Type mixinType = definition.TypeReference.ResolvedType;

				try
				{
					instances[i] = Activator.CreateInstance(mixinType);
				}
				catch (Exception e)
				{
					throw new ProxyFactoryException("Could not instantiate mixin " + mixinType.FullName, e);
				}
			}

			return instances;
		}

		#endregion
	}
}