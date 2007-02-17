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


using Castle.DynamicProxy.Generators;

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.Core.Interceptor;

	/// <summary>
	/// Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private static ModuleScope _scope = new ModuleScope();

		private Type _baseType;
		private Type[] _interfaces;
		private IInterceptor[] _interceptors;
		private object[] _data;
		private object _proxy;

		/// <summary>
		/// Usefull for test cases
		/// </summary>
		public static void ResetScope()
		{
			_scope = new ModuleScope();
		}

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			_interceptors = (IInterceptor[])info.GetValue("__interceptors", typeof(IInterceptor[]));
			_baseType = (Type)info.GetValue("__baseType", typeof(Type));

			String[] _interfaceNames = (String[])info.GetValue("__interfaces", typeof(String[]));

			_interfaces = new Type[_interfaceNames.Length];

			for (int i = 0; i < _interfaceNames.Length; i++)
			{
				_interfaces[i] = Type.GetType(_interfaceNames[i]);
			}

			_proxy = RecreateProxy(info, context);

			InvokeCallback(_proxy);
		}

		protected virtual object RecreateProxy(SerializationInfo info, StreamingContext context)
		{
			if (_baseType == typeof(object))
			{
				return RecreateInterfaceProxy(info, context);
			}
			else
			{
				return RecreateClassProxy(info, context);
			}
		}

		public object RecreateInterfaceProxy(SerializationInfo info, StreamingContext context)
		{
			object proxy = null;

			object target = info.GetValue("__target", typeof(object));
			InterfaceGeneratorType generatorType = (InterfaceGeneratorType)info.GetInt32("__interface_generator_type");
			string interfaceName = info.GetString("__theInterface");
			Type theInterface = Type.GetType(interfaceName, true, false);
			InterfaceProxyWithTargetGenerator generator;
			switch (generatorType)
			{
				case InterfaceGeneratorType.WithTarget:
					generator = new  InterfaceProxyWithTargetGenerator(_scope,theInterface);
					break;
				case InterfaceGeneratorType.WithoutTarget:
					generator = new InterfaceProxyGeneratorWithoutTarget(_scope, theInterface);
					break;
				case InterfaceGeneratorType.WithTargetInterface:
					generator = new InterfaceProxyWithTargetInterfaceGenerator(_scope,  theInterface);
					break;
				default:
					throw new InvalidOperationException(string.Format("Got value {0} for the interface generator type, which is not known for the purpose of serialization.", generatorType));
			}

			Type proxy_type = generator.GenerateCode(target.GetType(), _interfaces , ProxyGenerationOptions.Default);

			proxy = Activator.CreateInstance(proxy_type, new object[] { _interceptors, target });
			
			return proxy;
		}

		public object RecreateClassProxy(SerializationInfo info, StreamingContext context)
		{
			bool delegateBaseSer = info.GetBoolean("__delegateToBase");

			if (!delegateBaseSer)
			{
				_data = (object[])info.GetValue("__data", typeof(object[]));
			}

			object proxy = null;

			ClassProxyGenerator cpGen = new ClassProxyGenerator(_scope, _baseType);

			Type proxy_type = cpGen.GenerateCode(_interfaces, ProxyGenerationOptions.Default);
			

			if (delegateBaseSer)
			{
				proxy = Activator.CreateInstance(proxy_type, new object[] { info, context });
			}
			else
			{
				proxy = Activator.CreateInstance(proxy_type, new object[] { _interceptors });

				MemberInfo[] members = FormatterServices.GetSerializableMembers(_baseType);
				FormatterServices.PopulateObjectMembers(proxy, members, _data);
			}

			return proxy;
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
			return _proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}
	}
}
