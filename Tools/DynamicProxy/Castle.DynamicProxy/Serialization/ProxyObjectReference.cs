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

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Builder.CodeGenerators;

	/// <summary>
	/// Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private static ModuleScope _scope = new ModuleScope();

		private Type _baseType;
		private Type[] _interfaces;
		private IInterceptor _interceptor;
		private object[] _mixins;
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
			_interceptor = (IInterceptor) info.GetValue("__interceptor", typeof(IInterceptor) );
			_baseType = (Type) info.GetValue("__baseType", typeof(Type) );
			_mixins = (object[]) info.GetValue("__mixins", typeof(object[]) );

			String[] _interfaceNames = (String[]) info.GetValue("__interfaces", typeof(String[]) );
 			
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

			GeneratorContext genContext = new GeneratorContext();

			foreach(object mixin in _mixins)
			{
				genContext.AddMixinInstance(mixin);
			}

			InterfaceProxyGenerator gen = new InterfaceProxyGenerator( _scope, genContext );

			object target = info.GetValue("__target", typeof(object));

			if (_mixins.Length == 0)
			{
				Type proxy_type = gen.GenerateCode( _interfaces, target.GetType());

				proxy = Activator.CreateInstance( proxy_type, new object[] { _interceptor, target } );
			}
			else
			{
				Type proxy_type = gen.GenerateCode( _interfaces, target.GetType() );

				proxy = Activator.CreateInstance( proxy_type, new object[] { _interceptor, target, genContext.MixinsAsArray() } );
			}

			return proxy;
		}

		public object RecreateClassProxy(SerializationInfo info, StreamingContext context)
		{
			bool delegateBaseSer = info.GetBoolean("__delegateToBase");

			if (!delegateBaseSer)
			{
				_data = (object[]) info.GetValue("__data", typeof(object[]) );
			}

			object proxy = null;

			GeneratorContext genContext = new GeneratorContext();
			
			if (_mixins.Length != 0)
			{
				foreach(object mixin in _mixins)
				{
					genContext.AddMixinInstance(mixin);
				}
			}

			ClassProxyGenerator cpGen = new ClassProxyGenerator( _scope, genContext );

			Type proxy_type;

			if (_mixins.Length == 0)
			{
				proxy_type = cpGen.GenerateCode( _baseType, _interfaces );
			}
			else
			{
				proxy_type = cpGen.GenerateCustomCode( _baseType, _interfaces );
			}

			if (delegateBaseSer)
			{
				proxy = Activator.CreateInstance( proxy_type, new object[] { info, context } );
			}
			else
			{
				if (_mixins.Length == 0)
				{
					proxy = Activator.CreateInstance( proxy_type, new object[] { _interceptor } );
				}
				else
				{
					ArrayList args = new ArrayList();
					args.Add(_interceptor);
					args.Add(genContext.MixinsAsArray());
					
					proxy = Activator.CreateInstance( proxy_type, args.ToArray() );
				}

				MemberInfo[] members = FormatterServices.GetSerializableMembers( _baseType );
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
