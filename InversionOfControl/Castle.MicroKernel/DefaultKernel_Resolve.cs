// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using Core;
	using Handlers;

#if !SILVERLIGHT
	public partial class DefaultKernel : MarshalByRefObject, IKernel, IKernelEvents, IDeserializationCallback
#else
	public partial class DefaultKernel : IKernel, IKernelEvents
#endif
	{
		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public T Resolve<T>(IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType, arguments);
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Resolve<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// </summary>
		/// <returns></returns>
		public T Resolve<T>()
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key">Component's key</param>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>The Component instance</returns>
		public T Resolve<T>(String key)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="key">Component's key</param>
		/// <param name="arguments"></param>
		/// <returns>The Component instance</returns>
		public T Resolve<T>(String key, IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType, arguments);
		}

		public virtual object this[String key]
		{
			get
			{
				if (key == null) throw new ArgumentNullException("key");

				if (!HasComponent(key))
				{
					throw new ComponentNotFoundException(key);
				}

				IHandler handler = GetHandler(key);

				return ResolveComponent(handler);
			}
		}

		public virtual object this[Type service]
		{
			get
			{
				if (service == null) throw new ArgumentNullException("service");

				if (!HasComponent(service))
				{
					throw new ComponentNotFoundException(service);
				}

				IHandler handler = GetHandler(service);

				return ResolveComponent(handler, service);
			}
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// </summary>
		public object Resolve(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			return this[service];
		}

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		public Array ResolveAll(Type service)
		{
			return ResolveAll(service, new Hashtable());
		}

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="arguments">Arguments to resolve the services</param>
		public Array ResolveAll(Type service, IDictionary arguments)
		{
			Dictionary<IHandler, object> resolved = new Dictionary<IHandler, object>();

			foreach(IHandler handler in GetAssignableHandlers(service))
			{
				if (handler.CurrentState != HandlerState.Valid)
					continue;

				IHandler actualHandler = handler;

				if (handler is ForwardingHandler)
				{
					actualHandler = ((ForwardingHandler)handler).Target;
				}

				if (!resolved.ContainsKey(actualHandler))
				{
					object component = ResolveComponent(actualHandler, service, arguments);
					resolved.Add(actualHandler, component);
				}
			}

			Array components = Array.CreateInstance(service, resolved.Count);
			((ICollection)resolved.Values).CopyTo(components, 0);
			return components;
		}

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}


		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(object argumentsAsAnonymousType)
		{
			return (TService[])ResolveAll(typeof(TService), argumentsAsAnonymousType);
		}

		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(IDictionary arguments)
		{
			return (TService[])ResolveAll(typeof(TService), arguments);
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(Type service, IDictionary arguments)
		{
			if (service == null) throw new ArgumentNullException("service");
			if (arguments == null) throw new ArgumentNullException("arguments");

			if (!HasComponent(service))
			{
				throw new ComponentNotFoundException(service);
			}

			IHandler handler = GetHandler(service);

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(string key, IDictionary arguments)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (arguments == null) throw new ArgumentNullException("arguments");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, arguments);
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(string key, object argumentsAsAnonymousType)
		{
			return Resolve(key, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, service);
		}


		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		public TService[] ResolveAll<TService>()
		{
			return (TService[])ResolveAll(typeof(TService), new Hashtable());
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, IDictionary arguments)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, object argumentsAsAnonymousType)
		{
			return Resolve(key, service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}
	}
}
