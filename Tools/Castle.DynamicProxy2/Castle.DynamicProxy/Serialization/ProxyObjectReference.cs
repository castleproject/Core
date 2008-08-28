// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.DynamicProxy;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators;

	/// <summary>
	/// Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable, IDeserializationCallback
	{
		private static ModuleScope _scope = new ModuleScope();

		private readonly SerializationInfo _info;
		private readonly StreamingContext _context;

		private readonly Type _baseType;
		private readonly Type[] _interfaces;
		private readonly object _proxy;
		private readonly ProxyGenerationOptions _proxyGenerationOptions;

		private bool _isInterfaceProxy;
		private bool _delegateToBase;

		/// <summary>
		/// Resets the <see cref="ModuleScope"/> used for deserialization to a new scope.
		/// </summary>
		/// <remarks>This is useful for test cases.</remarks>
		public static void ResetScope()
		{
			SetScope (new ModuleScope());
		}

		/// <summary>
		/// Resets the <see cref="ModuleScope"/> used for deserialization to a given <paramref name="scope"/>.
		/// </summary>
		/// <param name="scope">The scope to be used for deserialization.</param>
		/// <remarks>By default, the deserialization process uses a different scope than the rest of the application, which can lead to multiple proxies
		/// being generated for the same type. By explicitly setting the deserialization scope to the application's scope, this can be avoided.</remarks>
		public static void SetScope (ModuleScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException ("scope");
			_scope = scope;
		}

		/// <summary>
		/// Gets the <see cref="DynamicProxy.ModuleScope"/> used for deserialization.
		/// </summary>
		/// <value>As <see cref="ProxyObjectReference"/> has no way of automatically determining the scope used by the application (and the application
		/// might use more than one scope at the same time), <see cref="ProxyObjectReference"/> uses a dedicated scope instance for deserializing proxy
		/// types. This instance can be reset and set to a specific value via <see cref="ResetScope"/> and <see cref="SetScope"/>.</value>
		public static ModuleScope ModuleScope
		{
			get { return _scope; }
		}

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			_info = info;
			_context = context;

			_baseType = DeserializeTypeFromString("__baseType");

			String[] _interfaceNames = (String[]) info.GetValue("__interfaces", typeof(String[]));
			_interfaces = new Type[_interfaceNames.Length];

			for(int i = 0; i < _interfaceNames.Length; i++)
				_interfaces[i] = Type.GetType(_interfaceNames[i]);

			_proxyGenerationOptions = (ProxyGenerationOptions) info.GetValue ("__proxyGenerationOptions", typeof (ProxyGenerationOptions));
			_proxy = RecreateProxy ();

			// We'll try to deserialize as much of the proxy state as possible here. This is just best effort; due to deserialization dependency reasons,
			// we need to repeat this in OnDeserialization to guarantee correct state deserialization.
			DeserializeProxyState ();
		}

		private Type DeserializeTypeFromString (string key)
		{
			return Type.GetType(_info.GetString(key), true, false);
		}

		protected virtual object RecreateProxy ()
		{
			if (_baseType == typeof(object)) // TODO: replace this hack by serializing a flag or something
			{
				_isInterfaceProxy = true;
				return RecreateInterfaceProxy();
			}
			else
			{
				_isInterfaceProxy = false;
				return RecreateClassProxy();
			}
		}

		public object RecreateInterfaceProxy()
		{
			InterfaceGeneratorType generatorType = (InterfaceGeneratorType) _info.GetInt32("__interface_generator_type");
			
			Type theInterface = DeserializeTypeFromString ("__theInterface");
			Type targetType = DeserializeTypeFromString ("__targetFieldType");
			
			InterfaceProxyWithTargetGenerator generator;
			switch(generatorType)
			{
				case InterfaceGeneratorType.WithTarget:
					generator = new InterfaceProxyWithTargetGenerator(_scope, theInterface);
					break;
				case InterfaceGeneratorType.WithoutTarget:
					generator = new InterfaceProxyWithoutTargetGenerator(_scope, theInterface);
					break;
				case InterfaceGeneratorType.WithTargetInterface:
					generator = new InterfaceProxyWithTargetInterfaceGenerator(_scope, theInterface);
					break;
				default:
					throw new InvalidOperationException(
						string.Format(
							"Got value {0} for the interface generator type, which is not known for the purpose of serialization.",
							generatorType));
			}

			Type proxy_type = generator.GenerateCode(targetType, _interfaces, _proxyGenerationOptions);
			return FormatterServices.GetSafeUninitializedObject (proxy_type);
		}

		public object RecreateClassProxy()
		{
			_delegateToBase = _info.GetBoolean ("__delegateToBase");

			ClassProxyGenerator cpGen = new ClassProxyGenerator(_scope, _baseType);

			Type proxy_type = cpGen.GenerateCode(_interfaces, _proxyGenerationOptions);


			if (_delegateToBase)
			{
				return Activator.CreateInstance(proxy_type, new object[] {_info, _context});
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
			return _proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}

		public void OnDeserialization (object sender)
		{
			IInterceptor[] _interceptors = (IInterceptor[]) _info.GetValue ("__interceptors", typeof (IInterceptor[]));
			SetInterceptors (_interceptors);
			
			// mixins
			if (_proxyGenerationOptions.HasMixins)
			{
				foreach (Type type in _proxyGenerationOptions.MixinData.MixinInterfacesAndPositions.Keys)
				{
					string mixinFieldName = "__mixin_" + type.FullName.Replace(".", "_");

					FieldInfo mixinField = _proxy.GetType().GetField(mixinFieldName);
					if (mixinField == null)
					{
						throw new SerializationException(
							"The SerializationInfo specifies an invalid proxy type, which has no " + mixinFieldName + " field.");
					}

					mixinField.SetValue(_proxy, _info.GetValue(mixinFieldName, type));
				}
			}

			// Get the proxy state again, to get all those members we couldn't get in the constructor due to deserialization ordering.
			DeserializeProxyState();
			InvokeCallback (_proxy);
		}

		private void DeserializeProxyState ()
		{
			if (_isInterfaceProxy)
			{
				object target = _info.GetValue ("__target", typeof (object));
				SetTarget (target);
			}
			else if (!_delegateToBase)
			{
				object[] baseMemberData = (object[]) _info.GetValue ("__data", typeof (object[]));
				MemberInfo[] members = FormatterServices.GetSerializableMembers (_baseType);
				FormatterServices.PopulateObjectMembers (_proxy, members, baseMemberData);
			}
		}

		private void SetTarget (object target)
		{
			FieldInfo targetField = _proxy.GetType ().GetField ("__target");
			if (targetField == null)
			{
				throw new SerializationException (
					"The SerializationInfo specifies an invalid interface proxy type, which has no __target field.");
			}

			targetField.SetValue (_proxy, target);
		}

		private void SetInterceptors (IInterceptor[] interceptors)
		{
			FieldInfo interceptorField = _proxy.GetType ().GetField ("__interceptors");
			if (interceptorField == null)
			{
				throw new SerializationException (
					"The SerializationInfo specifies an invalid proxy type, which has no __interceptors field.");
			}

			interceptorField.SetValue (_proxy, interceptors);
		}
	}
}
