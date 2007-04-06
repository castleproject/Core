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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Collections;

	/// <summary>
	/// Default <see cref="INamingSubSystem"/> implementation.
	/// Keeps services and key maps as simple hash tables. Does not
	/// support a query string.
	/// </summary>
	[Serializable]
	public class DefaultNamingSubSystem : AbstractSubSystem, INamingSubSystem
	{
		/// <summary>
		/// Map(String, IHandler) to map component keys
		/// to <see cref="IHandler"/>
		/// </summary>
		protected IDictionary key2Handler;

		/// <summary>
		/// Map(Type, IHandler) to map services 
		/// to <see cref="IHandler"/>
		/// </summary>
		protected IDictionary service2Handler;

		private object locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultNamingSubSystem"/> class.
		/// </summary>
		public DefaultNamingSubSystem()
		{
			key2Handler = Hashtable.Synchronized(new Hashtable());
			service2Handler = Hashtable.Synchronized(new Hashtable());
		}

		#region INamingSubSystem Members

		public virtual void Register(String key, IHandler handler)
		{
			Type service = handler.ComponentModel.Service;

			lock(locker)
			{
				if (key2Handler.Contains(key))
				{
					throw new ComponentRegistrationException(
						String.Format("There is a component already registered for the given key {0}", key));
				}

				if (!service2Handler.Contains(service))
				{
					this[service] = handler;
				}

				this[key] = handler;
			}

		}

		public virtual bool Contains(String key)
		{
			lock(locker)
			{
				return key2Handler.Contains(key);
			}
		}

		public virtual bool Contains(Type service)
		{
			lock(locker)
			{
				return service2Handler.Contains(service);
			}
		}

		public virtual void UnRegister(String key)
		{
			lock (locker)
			{
				key2Handler.Remove(key);
			}
		}

		public virtual void UnRegister(Type service)
		{
			lock(locker)
			{
				service2Handler.Remove(service);
			}
		}

		public virtual int ComponentCount
		{
			get
			{
				lock(locker)
				{
					return key2Handler.Count;
				}
			}
		}

		public virtual IHandler GetHandler(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			lock (locker)
			{
				return key2Handler[key] as IHandler;
			}
		}

		public virtual IHandler[] GetHandlers(String query)
		{
			throw new NotImplementedException();
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			lock (locker)
			{
				IHandler handler = service2Handler[service] as IHandler;

				return handler;
			}
		}

		public virtual IHandler GetHandler(String key, Type service)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			lock (locker)
			{
				IHandler handler = key2Handler[key] as IHandler;

				return handler;
			}
		}

		public virtual IHandler[] GetHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			ArrayList list = new ArrayList();

			foreach(IHandler handler in GetHandlers())
			{
				if (service == handler.ComponentModel.Service)
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray(typeof(IHandler));
		}

		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			ArrayList list = new ArrayList();

			foreach(IHandler handler in GetHandlers())
			{
				if (service.IsAssignableFrom(handler.ComponentModel.Service))
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray(typeof(IHandler));
		}

		public virtual IHandler[] GetHandlers()
		{
			lock (locker)
			{
				IHandler[] list = new IHandler[key2Handler.Values.Count];

				int index = 0;

				foreach(IHandler handler in key2Handler.Values)
				{
					list[index++] = handler;
				}

				return list;
			}
		}

		public virtual IHandler this[Type service]
		{
			set
			{
				lock(locker)
				{
					service2Handler[service] = value;
				}
			}
		}

		public virtual IHandler this[String key]
		{
			set
			{
				lock (locker)
				{
					key2Handler[key] = value;
				}
			}
		}

		public IDictionary GetKey2Handler()
		{
			return key2Handler;
		}

		public IDictionary GetService2Handler()
		{
			return service2Handler;
		}

		#endregion
	}
}