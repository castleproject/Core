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

namespace Castle.Windsor.Proxy
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy;
	using Castle.MicroKernel;

	/// <summary>
	/// This implementation of <see cref="IProxyFactory"/> relies 
	/// on DynamicProxy to expose proxy capabilies.
	/// </summary>
	/// <remarks>
	/// Note that only virtual methods can be intercepted in a 
	/// concrete class. However, if the component 
	/// was registered with a service interface, we proxy
	/// the interface and the methods don't need to be virtual,
	/// </remarks>
	[Serializable]
	public class DefaultProxyFactory : AbstractProxyFactory, IDeserializationCallback
	{
		[NonSerialized]
		protected ProxyGenerator generator;

		/// <summary>
		/// Constructs a DefaultProxyFactory
		/// </summary>
		public DefaultProxyFactory()
		{
			Init();
		}

		public override object Create(IKernel kernel, object target, ComponentModel model,
		                              params object[] constructorArguments)
		{
			IInterceptor[] interceptors = ObtainInterceptors(kernel, model);

			object proxy;

			if (model.Service.IsInterface)
			{
				ProxyGenerationOptions options = new ProxyGenerationOptions();

				options.BaseTypeForInterfaceProxy = typeof(MarshalByRefObject);
				
				proxy = generator.CreateInterfaceProxyWithTarget(model.Service,
																 CollectInterfaces(model.Service, model.Implementation), 
				                                                 target, options, interceptors);
			}
			else
			{
				proxy = generator.CreateClassProxy(model.Implementation, 
				                                   interceptors, constructorArguments);
			}

			return proxy;
		}

		public override bool RequiresTargetInstance(IKernel kernel, ComponentModel model)
		{
			return model.Service.IsInterface;
		}

		protected Type[] CollectInterfaces(Type serviceInterface, Type implementation)
		{
			return implementation.FindInterfaces(new TypeFilter(EmptyTypeFilter), serviceInterface);
		}

		private bool EmptyTypeFilter(Type type, object criteria)
		{
			Type mainInterface = (Type) criteria;

			return !type.IsAssignableFrom(mainInterface);
		}

		#region IDeserializationCallback

		public void OnDeserialization(object sender)
		{
			Init();
		}

		#endregion

		private void Init()
		{
			generator = new ProxyGenerator();
		}
	}
}
