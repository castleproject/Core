// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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


#if !SILVERLIGHT

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Security;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
#if DOTNET40
#endif

	/// <summary>
	///   Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable, IDeserializationCallback
	{
		private static ModuleScope scope = new ModuleScope();

		private readonly SerializationInfo info;
		private readonly StreamingContext context;

		private readonly Type baseType;
		private readonly Type[] interfaces;
		private readonly object proxy;
		private readonly ProxyGenerationOptions proxyGenerationOptions;

		private bool isInterfaceProxy;
		private bool delegateToBase;

		/// <summary>
		///   Resets the <see cref = "ModuleScope" /> used for deserialization to a new scope.
		/// </summary>
		/// <remarks>
		///   This is useful for test cases.
		/// </remarks>
		public static void ResetScope()
		{
			SetScope(new ModuleScope());
		}

		/// <summary>
		///   Resets the <see cref = "ModuleScope" /> used for deserialization to a given <paramref name = "scope" />.
		/// </summary>
		/// <param name = "scope">The scope to be used for deserialization.</param>
		/// <remarks>
		///   By default, the deserialization process uses a different scope than the rest of the application, which can lead to multiple proxies
		///   being generated for the same type. By explicitly setting the deserialization scope to the application's scope, this can be avoided.
		/// </remarks>
		public static void SetScope(ModuleScope scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			ProxyObjectReference.scope = scope;
		}

		/// <summary>
		///   Gets the <see cref = "ModuleScope" /> used for deserialization.
		/// </summary>
		/// <value>As <see cref = "ProxyObjectReference" /> has no way of automatically determining the scope used by the application (and the application
		///   might use more than one scope at the same time), <see cref = "ProxyObjectReference" /> uses a dedicated scope instance for deserializing proxy
		///   types. This instance can be reset and set to a specific value via <see cref = "ResetScope" /> and <see
		///    cref = "SetScope" />.</value>
		public static ModuleScope ModuleScope
		{
			get { return scope; }
		}

#if DOTNET40
		[SecurityCritical]
#endif
		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			this.info = info;
			this.context = context;

			baseType = DeserializeTypeFromString("__baseType");

			var _interfaceNames = (String[])info.GetValue("__interfaces", typeof(String[]));
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

#if DOTNET40
		[SecurityCritical]
#endif
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

#if DOTNET40
		[SecurityCritical]
#endif
		private object RecreateClassProxyWithTarget()
		{
			var generator = new ClassProxyWithTargetGenerator(scope, baseType, interfaces, proxyGenerationOptions);
			var proxyType = generator.GetGeneratedType();
			return InstantiateClassProxy(proxyType);
		}

#if DOTNET40
		[SecurityCritical]
#endif
		public object RecreateInterfaceProxy(string generatorType)
		{
			var @interface = DeserializeTypeFromString("__theInterface");
			var targetType = DeserializeTypeFromString("__targetFieldType");

			InterfaceProxyWithTargetGenerator generator;
			if (generatorType == ProxyTypeConstants.InterfaceWithTarget)
			{
				generator = new InterfaceProxyWithTargetGenerator(scope, @interface);
			}
			else if (generatorType == ProxyTypeConstants.InterfaceWithoutTarget)
			{
				generator = new InterfaceProxyWithoutTargetGenerator(scope, @interface);
			}
			else if (generatorType == ProxyTypeConstants.InterfaceWithTargetInterface)
			{
				generator = new InterfaceProxyWithTargetInterfaceGenerator(scope, @interface);
			}
			else
			{
				throw new InvalidOperationException(
					string.Format(
						"Got value {0} for the interface generator type, which is not known for the purpose of serialization.",
						generatorType));
			}

			var proxyType = generator.GenerateCode(targetType, interfaces, proxyGenerationOptions);
			return FormatterServices.GetSafeUninitializedObject(proxyType);
		}

#if DOTNET40
		[SecurityCritical]
#endif
		public object RecreateClassProxy()
		{
			var generator = new ClassProxyGenerator(scope, baseType);
			var proxyType = generator.GenerateCode(interfaces, proxyGenerationOptions);
			return InstantiateClassProxy(proxyType);
		}

#if DOTNET40
		[SecurityCritical]
#endif
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

#if DOTNET40
		[SecurityCritical]
#endif
		public object GetRealObject(StreamingContext context)
		{
			return proxy;
		}

#if DOTNET40
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}

#if DOTNET40
		[SecuritySafeCritical]
#endif
		public void OnDeserialization(object sender)
		{
			var interceptors = GetValue<IInterceptor[]>("__interceptors");
			SetInterceptors(interceptors);

			DeserializeProxyMembers();

			// Get the proxy state again, to get all those members we couldn't get in the constructor due to deserialization ordering.
			DeserializeProxyState();
			InvokeCallback(proxy);
		}

#if DOTNET40
		[SecurityCritical]
#endif
		private void DeserializeProxyMembers()
		{
			var proxyType = proxy.GetType();
			var members = FormatterServices.GetSerializableMembers(proxyType);

			var deserializedMembers = new List<MemberInfo>();
			var deserializedValues = new List<Object>();
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

#if DOTNET40
		[SecurityCritical]
#endif
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
			var targetField = proxy.GetType().GetField("__target");
			if (targetField == null)
			{
				throw new SerializationException(
					"The SerializationInfo specifies an invalid interface proxy type, which has no __target field.");
			}

			targetField.SetValue(proxy, target);
		}

		private void SetInterceptors(IInterceptor[] interceptors)
		{
			var interceptorField = proxy.GetType().GetField("__interceptors");
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