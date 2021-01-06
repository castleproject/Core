// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;

	/// <summary>
	///   Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable, IDeserializationCallback
	{
		private static IProxyBuilder builder = new DefaultProxyBuilder();

		private readonly SerializationInfo info;
		private readonly StreamingContext context;

		private readonly Type baseType;
		private readonly Type[] interfaces;
		private readonly object proxy;
		private readonly ProxyGenerationOptions proxyGenerationOptions;

		private bool isInterfaceProxy;
		private bool delegateToBase;

		/// <summary>
		///   Resets the <see cref="IProxyBuilder" /> used for deserialization to a new scope.
		/// </summary>
		/// <remarks>
		///   This is useful for test cases.
		/// </remarks>
		public static void ResetProxyBuilder()
		{
			SetProxyBuilder(new DefaultProxyBuilder());
		}

		/// <summary>
		///   Resets the <see cref="IProxyBuilder" /> used for deserialization to a given <paramref name="builder" />.
		/// </summary>
		/// <param name="builder">The builder to be used for deserialization.</param>
		/// <remarks>
		///   By default, the deserialization process uses a different builder than the rest of the application,
		///   which can lead to multiple proxy types being generated for the same type. By explicitly setting
		///   the deserialization builder to the application's builder, this can be avoided.
		/// </remarks>
		public static void SetProxyBuilder(IProxyBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}
			ProxyObjectReference.builder = builder;
		}

		/// <summary>
		///   Gets the <see cref="IProxyBuilder" /> used for deserialization.
		/// </summary>
		/// <value>
		///   As <see cref="ProxyObjectReference" /> has no way of automatically determining the builder
		///   used by the application (and the application might use more than one scope at the same time),
		///   <see cref="ProxyObjectReference" /> uses a dedicated builder instance for deserializing proxy types.
		///   This instance can be reset and set to a specific value via <see cref="ResetProxyBuilder" />
		///   and <see cref="SetProxyBuilder" />.
		/// </value>
		public static IProxyBuilder ProxyBuilder
		{
			get { return builder; }
		}

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			this.info = info;
			this.context = context;

			baseType = DeserializeTypeFromString("__baseType");

			var _interfaceNames = (string[])info.GetValue("__interfaces", typeof(string[]));
			interfaces = new Type[_interfaceNames.Length];

			for (var i = 0; i < _interfaceNames.Length; i++)
			{
				interfaces[i] = Type.GetType(_interfaceNames[i]);
			}

			proxyGenerationOptions =
				(ProxyGenerationOptions)info.GetValue("__proxyGenerationOptions", typeof(ProxyGenerationOptions));
			proxy = RecreateProxy();

			// We'll try to deserialize as much of the proxy state as possible here. This is just best effort; due to deserialization dependency reasons,
			// we need to repeat this in OnDeserialization to guarantee correct state deserialization.
			DeserializeProxyState();
		}

		private Type DeserializeTypeFromString(string key)
		{
			return Type.GetType(info.GetString(key), true, false);
		}

		protected virtual object RecreateProxy()
		{
			var generatorType = GetValue<string>("__proxyTypeId");
			if (generatorType.Equals(ProxyTypeConstants.Class))
			{
				isInterfaceProxy = false;
				return RecreateClassProxy();
			}
			if (generatorType.Equals(ProxyTypeConstants.ClassWithTarget))
			{
				isInterfaceProxy = false;
				return RecreateClassProxyWithTarget();
			}
			isInterfaceProxy = true;
			return RecreateInterfaceProxy(generatorType);
		}

		private object RecreateClassProxyWithTarget()
		{
			var proxyType = builder.CreateClassProxyTypeWithTarget(baseType, interfaces, proxyGenerationOptions);
			return InstantiateClassProxy(proxyType);
		}

		public object RecreateInterfaceProxy(string generatorType)
		{
			var @interface = DeserializeTypeFromString("__theInterface");
			var targetType = DeserializeTypeFromString("__targetFieldType");

			Type proxyType;
			if (generatorType == ProxyTypeConstants.InterfaceWithTarget)
			{
				proxyType = builder.CreateInterfaceProxyTypeWithTarget(@interface, interfaces, targetType, proxyGenerationOptions);
			}
			else if (generatorType == ProxyTypeConstants.InterfaceWithoutTarget)
			{
				proxyType = builder.CreateInterfaceProxyTypeWithoutTarget(@interface, interfaces, proxyGenerationOptions);
			}
			else if (generatorType == ProxyTypeConstants.InterfaceWithTargetInterface)
			{
				proxyType = builder.CreateInterfaceProxyTypeWithTargetInterface(@interface, interfaces, proxyGenerationOptions);
			}
			else
			{
				throw new InvalidOperationException(
					string.Format(
						"Got value {0} for the interface generator type, which is not known for the purpose of serialization.",
						generatorType));
			}

			return FormatterServices.GetSafeUninitializedObject(proxyType);
		}

		public object RecreateClassProxy()
		{
			var proxyType = builder.CreateClassProxyType(baseType, interfaces, proxyGenerationOptions);
			return InstantiateClassProxy(proxyType);
		}

		private object InstantiateClassProxy(Type proxy_type)
		{
			delegateToBase = GetValue<bool>("__delegateToBase");
			if (delegateToBase)
			{
				return Activator.CreateInstance(proxy_type, new object[] { info, context });
			}
			else
			{
				return FormatterServices.GetSafeUninitializedObject(proxy_type);
			}
		}

		protected void InvokeCallback(object target)
		{
			if (target is IDeserializationCallback)
			{
				(target as IDeserializationCallback).OnDeserialization(this);
			}
		}

		public object GetRealObject(StreamingContext context)
		{
			return proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}

		public void OnDeserialization(object sender)
		{
			var interceptors = GetValue<IInterceptor[]>("__interceptors");
			SetInterceptors(interceptors);

			DeserializeProxyMembers();

			// Get the proxy state again, to get all those members we couldn't get in the constructor due to deserialization ordering.
			DeserializeProxyState();
			InvokeCallback(proxy);
		}

		private void DeserializeProxyMembers()
		{
			var proxyType = proxy.GetType();
			var members = FormatterServices.GetSerializableMembers(proxyType);

			var deserializedMembers = new List<MemberInfo>();
			var deserializedValues = new List<object>();
			for (var i = 0; i < members.Length; i++)
			{
				var member = members[i] as FieldInfo;
				// we get some inherited members...
				if (member.DeclaringType != proxyType)
				{
					continue;
				}

				Debug.Assert(member != null);
				var value = info.GetValue(member.Name, member.FieldType);
				deserializedMembers.Add(member);
				deserializedValues.Add(value);
			}
			FormatterServices.PopulateObjectMembers(proxy, deserializedMembers.ToArray(), deserializedValues.ToArray());
		}

		private void DeserializeProxyState()
		{
			if (isInterfaceProxy)
			{
				var target = GetValue<object>("__target");
				SetTarget(target);
			}
			else if (!delegateToBase)
			{
				var baseMemberData = GetValue<object[]>("__data");
				var members = FormatterServices.GetSerializableMembers(baseType);

				// Sort to keep order on both serialize and deserialize side the same, c.f DYNPROXY-ISSUE-127
				members = TypeUtil.Sort(members);

				FormatterServices.PopulateObjectMembers(proxy, members, baseMemberData);
			}
		}

		private void SetTarget(object target)
		{
			var targetField = proxy.GetType().GetField("__target", BindingFlags.Instance | BindingFlags.NonPublic);
			if (targetField == null)
			{
				throw new SerializationException(
					"The SerializationInfo specifies an invalid interface proxy type, which has no __target field.");
			}

			targetField.SetValue(proxy, target);
		}

		private void SetInterceptors(IInterceptor[] interceptors)
		{
			var interceptorField = proxy.GetType().GetField("__interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
			if (interceptorField == null)
			{
				throw new SerializationException(
					"The SerializationInfo specifies an invalid proxy type, which has no __interceptors field.");
			}

			interceptorField.SetValue(proxy, interceptors);
		}

		private T GetValue<T>(string name)
		{
			return (T)info.GetValue(name, typeof(T));
		}
	}
}

#endif