using Castle.Model;
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

namespace Castle.MicroKernel.Lifestyle.Pool
{
	using System;
	using System.Threading;
	using System.Collections;

	[Serializable]
	public class DefaultPool : IPool, IDisposable
	{
		private readonly Stack _available = Stack.Synchronized(new Stack());
		private readonly IList _inUse = ArrayList.Synchronized(new ArrayList());
		private readonly int _initialsize;
		private readonly int _maxsize;
		private readonly ReaderWriterLock _lock;
		private readonly IComponentActivator _componentActivator;

		public DefaultPool(int initialsize, int maxsize, IComponentActivator componentActivator)
		{
			_initialsize = initialsize;
			_maxsize = maxsize;
			_componentActivator = componentActivator;

			_lock = new ReaderWriterLock();

			// Thread thread = new Thread(new ThreadStart(InitPool));
			// thread.Start();
			InitPool();
		}

		#region IPool Members

		public virtual object Request()
		{
			_lock.AcquireWriterLock(-1);

			object instance = null;

			try
			{

				if (_available.Count != 0)
				{
					instance = _available.Pop();

					if (instance == null)
					{
						throw new PoolException("Invalid instance on the pool stack");
					}
				}
				else
				{
					instance = _componentActivator.Create();

					if (instance == null)
					{
						throw new PoolException("Activator didn't return a valid instance");
					}
				}

				_inUse.Add(instance);
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}

			return instance;
		}

		public virtual void Release(object instance)
		{
			_lock.AcquireWriterLock(-1);

			try
			{
				if (!_inUse.Contains(instance))
				{
					throw new PoolException("Trying to release a component that does not belong to this pool");
				}

				_inUse.Remove(instance);

				if (_available.Count < _maxsize)
				{
					if (instance is IRecyclable)
					{
						(instance as IRecyclable).Recycle();
					}

					_available.Push(instance);
				}
				else
				{
					// Pool is full
					_componentActivator.Destroy(instance);
				}
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			// Release all components 

			foreach(object instance in _available)
			{
				_componentActivator.Destroy(instance);
			}
		}

		#endregion

		/// <summary>
		/// Initializes the pool to a initial size by requesting
		/// n components and then releasing them.
		/// </summary>
		private void InitPool()
		{
			ArrayList tempInstance = new ArrayList();

			for(int i=0; i < _initialsize; i++)
			{
				tempInstance.Add( Request() );
			}

			for(int i=0; i < _initialsize; i++)
			{
				Release( tempInstance[i] );
			}
		}
	}
}
