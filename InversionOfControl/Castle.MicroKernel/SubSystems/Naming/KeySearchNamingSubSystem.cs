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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Collections;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// When requesting a component by service, KeySearchNamingSubSystem first 
	/// determines if more than one component has been registered for that service.  
	/// If not, Default resolution occurs.  If so, all of the registered keys for 
	/// that service are processed through the provided Predicate to determine which 
	/// key to use for service resolution.  If no Predicate matches, the default 
	/// resolution occurs.
	/// </summary>
	[Serializable]
	public class KeySearchNamingSubSystem : DefaultNamingSubSystem
	{
		protected readonly IDictionary service2Keys;
		protected readonly Predicate<string> keyPredicate;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeySearchNamingSubSystem"/> class.
		/// </summary>
		public KeySearchNamingSubSystem() : this(delegate { return true; })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeySearchNamingSubSystem"/> class.
		/// </summary>
		/// <param name="keyPredicate">The key predicate.</param>
		public KeySearchNamingSubSystem(Predicate<string> keyPredicate)
		{
			if (keyPredicate == null) throw new ArgumentNullException("keyPredicate");

			service2Keys = Hashtable.Synchronized(new Hashtable());
			this.keyPredicate = keyPredicate;
		}

		/// <summary>
		/// Registers the given handler with the give key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="handler">The handler.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void Register(string key, IHandler handler)
		{
			base.Register(key, handler);

			Type service = handler.ComponentModel.Service;

			IList keys;

			if (!service2Keys.Contains(service))
			{
				if (!service2Keys.Contains(service))
				{
					keys = ArrayList.Synchronized(new ArrayList());
					keys.Add(key);
					service2Keys[service] = keys;
				}
			}
			else
			{
				keys = (IList) service2Keys[service];

				if (!keys.Contains(key))
				{
					if (!keys.Contains(key))
					{
						keys.Add(key);
					}
				}
			}
		}

		/// <summary>
		/// Unregisters the handler associated with the given key
		/// </summary>
		/// <param name="key">The key.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void UnRegister(string key)
		{
			IHandler handler = key2Handler[key] as IHandler;

			base.UnRegister(key);

			if (handler != null)
			{
				IList keys = service2Keys[handler.ComponentModel.Service] as IList;
				if (keys != null)
				{
					keys.Remove(key);
				}
			}
		}

		/// <summary>
		/// Unregisters the handler associated with the given service
		/// </summary>
		/// <param name="service">The service.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void UnRegister(Type service)
		{
			base.UnRegister(service);
			service2Keys.Remove(service);
		}

		/// <summary>
		/// Executes the Predicate against all keys for the registered service to 
		/// determine which component to return.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override IHandler GetHandler(Type service)
		{
			IList keys = service2Keys[service] as IList;

			if (keys == null) return null;

			if (keys.Count == 1) return base.GetHandler(service);

			for(int i = 0; i < keys.Count; i++)
			{
				string key = (string) keys[i];
				if (keyPredicate(key))
				{
					return GetHandler(key);
				}
			}

			return base.GetHandler(service);
		}
	}
}
