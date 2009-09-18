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

using System.Collections.Generic;

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Threading;

	/// <summary>
	/// Default <see cref="INamingSubSystem"/> implementation.
	/// Keeps services map as a simple hash table.
	/// Keeps key map as a list dictionary to maintain order.
	/// Does not support a query string.
	/// </summary>
	[Serializable]
	public class DefaultNamingSubSystem : AbstractSubSystem, INamingSubSystem
	{
		/// <summary>
		/// Map(String, IHandler) to map component keys
		/// to <see cref="IHandler"/>
		/// Items in this dictionary are sorted in insertion order.
		/// </summary>
		protected IDictionary<string,IHandler> key2Handler;

		/// <summary>
		/// Map(Type, IHandler) to map a service
		/// to <see cref="IHandler"/>.
		/// If there is more than a single service of the type, only the first
		/// registered services is stored in this dictionary.
		/// It serve as a fast lookup for the common case of having a single handler for 
		/// a type.
		/// </summary>
		protected IDictionary<Type, IHandler> service2Handler;

		private readonly IDictionary<Type, IHandler[]> handlerListsByTypeCache = new Dictionary<Type, IHandler[]>();
		private readonly IDictionary<Type, IHandler[]> assignableHandlerListsByTypeCache = new Dictionary<Type, IHandler[]>();
		private IHandler[] allHandlersCache;
		private List<IHandler> allHandlers = new List<IHandler>();
		private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
		private readonly IList<IHandlerSelector> selectors = new List<IHandlerSelector>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultNamingSubSystem"/> class.
		/// </summary>
		public DefaultNamingSubSystem()
		{
			key2Handler = new Dictionary<string,IHandler>();
			service2Handler = new Dictionary<Type, IHandler>();
		}

		#region INamingSubSystem Members

		public virtual void Register(String key, IHandler handler)
		{
			Type service = handler.Service;

			locker.EnterWriteLock();
			try
			{
				if (key2Handler.ContainsKey(key))
				{
					throw new ComponentRegistrationException(
						String.Format("There is a component already registered for the given key {0}", key));
				}

				if (!service2Handler.ContainsKey(service))
				{
					this[service] = handler;
				}

				this[key] = handler;
				InvalidateCache();
			}
			finally
			{
				locker.ExitWriteLock();
			}
		}

		private void InvalidateCache()
		{
			handlerListsByTypeCache.Clear();
			assignableHandlerListsByTypeCache.Clear();
			allHandlersCache = null;
		}

		public virtual bool Contains(String key)
		{
			locker.EnterReadLock();
			try
			{
				return key2Handler.ContainsKey(key);
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public virtual bool Contains(Type service)
		{
			locker.EnterReadLock();
			try
			{
				return service2Handler.ContainsKey(service);
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public virtual void UnRegister(String key)
		{
			var writeLockHeld = locker.IsWriteLockHeld;
			if (writeLockHeld == false)
				locker.EnterWriteLock();
			try
			{
				IHandler value;
				if(key2Handler.TryGetValue(key, out value))
					allHandlers.Remove(value);
				key2Handler.Remove(key);
				InvalidateCache();
			}
			finally
			{
				if (writeLockHeld == false)
					locker.ExitWriteLock();
			}
		}

		public virtual void UnRegister(Type service)
		{
			locker.EnterWriteLock();
			try
			{
				service2Handler.Remove(service);
				InvalidateCache();
			}
			finally
			{
				locker.ExitWriteLock();
			}
		}

		public virtual int ComponentCount
		{
			get
			{
				locker.EnterReadLock();
				try
				{
					return allHandlers.Count;
				}
				finally
				{
					locker.ExitReadLock();
				}
			}
		}

		public virtual IHandler GetHandler(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			IHandler selectorsOpinion = GetSelectorsOpinion(key, null);
			if (selectorsOpinion != null)
				return selectorsOpinion;

			locker.EnterReadLock();
			try
			{
				IHandler value;
				key2Handler.TryGetValue(key, out value);
				return value;
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public virtual IHandler[] GetHandlers(String query)
		{
			throw new NotImplementedException();
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler selectorsOpinion = GetSelectorsOpinion(null, service);
			if (selectorsOpinion != null)
				return selectorsOpinion;

			locker.EnterReadLock();
			try
			{
				IHandler handler;

				service2Handler.TryGetValue(service, out handler);

				return handler;
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public virtual IHandler GetHandler(String key, Type service)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			IHandler selectorsOpinion = GetSelectorsOpinion(key, service);
			if (selectorsOpinion != null)
				return selectorsOpinion;

			locker.EnterReadLock();
			try
			{
				IHandler handler;

				key2Handler.TryGetValue(key, out handler);

				return handler;
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		public virtual IHandler[] GetHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler[] result;
			locker.EnterReadLock();
			try
			{
				if (handlerListsByTypeCache.TryGetValue(service, out result))
					return result;
			}
			finally
			{
				locker.ExitReadLock();
			}


			locker.EnterWriteLock();
			try
			{

				var handlers = GetHandlers();

				var list = new List<IHandler>(handlers.Length);
				foreach (IHandler handler in handlers)
				{
					if (service == handler.Service)
					{
						list.Add(handler);
					}
				}

				result = list.ToArray();

				handlerListsByTypeCache[service] = result;

			}
			finally
			{
				locker.ExitWriteLock();
			}

			return result;
		}

		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler[] result;
			locker.EnterReadLock();
			try
			{
				if (assignableHandlerListsByTypeCache.TryGetValue(service, out result))
					return result;
			}
			finally
			{
				locker.ExitReadLock();
			}

			locker.EnterWriteLock();
			try
			{

				var handlers = GetHandlers();
				var list = new List<IHandler>(handlers.Length);
				foreach (IHandler handler in handlers)
				{
					Type handlerService = handler.Service;
					if (service.IsAssignableFrom(handlerService))
					{
						list.Add(handler);
					}
					else
					{
						if (service.IsGenericType &&
							service.GetGenericTypeDefinition().IsAssignableFrom(handlerService))
						{
							list.Add(handler);
						}
					}
				}

				result = list.ToArray();
				assignableHandlerListsByTypeCache[service] = result;
			}
			finally
			{
				locker.ExitWriteLock();
			}

			return result;
		}

		public virtual IHandler[] GetHandlers()
		{
			var lockHeld = locker.IsReadLockHeld || 
				locker.IsUpgradeableReadLockHeld || 
				locker.IsWriteLockHeld;
			if (lockHeld == false)
				locker.EnterReadLock();
			try
			{

				if (allHandlersCache != null)
					return allHandlersCache;

				var list = new IHandler[key2Handler.Values.Count];

				key2Handler.Values.CopyTo(list, 0);

				allHandlersCache = list;

				return list;
			}
			finally
			{
				if (lockHeld == false)
					locker.ExitReadLock();
			}
		}

		public virtual IHandler this[Type service]
		{
			set
			{
				var writeLockHeld = locker.IsWriteLockHeld;
				if(writeLockHeld==false)
					locker.EnterWriteLock();
				try
				{
					service2Handler[service] = value;
				}
				finally
				{
					if (writeLockHeld == false)
						locker.ExitWriteLock();
				}
			}
		}

		public virtual IHandler this[String key]
		{
			set
			{
				var writeLockHeld = locker.IsWriteLockHeld;
				if (writeLockHeld == false)
					locker.EnterWriteLock();
				try
				{
					key2Handler[key] = value;
					allHandlers.Add(value);
				}
				finally
				{
					if (writeLockHeld == false)
						locker.ExitWriteLock();
				}
			}
		}

		public IDictionary<string,IHandler> GetKey2Handler()
		{
			return key2Handler;
		}

		public IDictionary<Type, IHandler> GetService2Handler()
		{
			return service2Handler;
		}

		public void AddHandlerSelector(IHandlerSelector selector)
		{
			selectors.Add(selector);
		}

		protected virtual IHandler GetSelectorsOpinion(string key, Type type)
		{
			type = type ?? typeof(object);// if type is null, we want everything, so object does well for that
			IHandler[] handlers = null;//only init if we have a selector with an opinion about this type
			foreach (IHandlerSelector selector in selectors)
			{
				if (selector.HasOpinionAbout(key, type) == false)
					continue;
				if (handlers == null)
					handlers = GetAssignableHandlers(type);
				IHandler handler = selector.SelectHandler(key, type, handlers);
				if (handler != null)
					return handler;
			}
			return null;
		}

		#endregion
	}
}
