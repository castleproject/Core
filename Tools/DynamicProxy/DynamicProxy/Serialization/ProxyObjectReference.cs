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

namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;

	/// <summary>
	/// Handles the deserialization of proxies.
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private Type _baseType;
		private Type[] _interfaces;
		private IInterceptor _interceptor;
		private object[] _mixins;
		private object[] _data;
		private object _proxy;

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

			// TODO: ProxyGenerator.Current
			ProxyGenerator generator = new ProxyGenerator();

			object target = info.GetValue("__target", typeof(object));

			if (_mixins.Length == 0)
			{
				proxy = generator.CreateProxy( _interfaces, _interceptor, target);
			}
			else
			{
				GeneratorContext genContext = new GeneratorContext();
				
				foreach(object mixin in _mixins)
				{
					genContext.AddMixinInstance(mixin);
				}
				
				proxy = generator.CreateCustomProxy( _interfaces, _interceptor, target, genContext );
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

			// TODO: ProxyGenerator.Current
			ProxyGenerator generator = new ProxyGenerator();

			if (delegateBaseSer)
			{
				if (_mixins.Length == 0)
				{
					Type proxy_type = generator.ProxyBuilder.CreateClassProxy( _baseType );
					proxy = Activator.CreateInstance( proxy_type, new object[] { info, context } );
				} 
				else
				{
					GeneratorContext genContext = new GeneratorContext();
					
					foreach(object mixin in _mixins)
					{
						genContext.AddMixinInstance(mixin);
					}
					
					Type proxy_type = generator.ProxyBuilder.CreateCustomClassProxy( 
						_baseType, genContext );
					proxy = Activator.CreateInstance( proxy_type, new object[] { info, context } );
				}
			}
			else
			{
				if (_mixins.Length == 0)
				{
					proxy = generator.CreateClassProxy( _baseType, _interceptor );
				}
				else
				{
					GeneratorContext genContext = new GeneratorContext();
					
					foreach(object mixin in _mixins)
					{
						genContext.AddMixinInstance(mixin);
					}
					
					proxy = generator.CreateCustomClassProxy( _baseType, _interceptor, genContext );
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
