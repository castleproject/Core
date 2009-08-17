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

namespace NVelocity.Util
{
	using System;

	/// <summary>
	/// Simple object pool. Based on ThreadPool and few other classes
	/// The pool will ignore overflow and return null if empty.
	/// </summary>
	public sealed class SimplePool<T> where T : class
	{
		/*
	     * Where the objects are held.
	     */
		private T[] pool;

		/// <summary>  max amount of objects to be managed
		/// set via CTOR
		/// </summary>
		private int maximum;

		/// <summary>  index of previous to next
		/// free slot
		/// </summary>
		private int current = - 1;

		public SimplePool(int max)
		{
			maximum = max;
			pool = new T[max];
		}

		/// <summary>
		/// Add the object to the pool, silent nothing if the pool is full
		/// </summary>
		public void put(T o)
		{
			int idx = - 1;

			lock(this)
			{
				if (current < maximum - 1)
				{
					idx = ++current;
				}

				if (idx >= 0)
				{
					pool[idx] = o;
				}
			}
		}

		/// <summary>
		/// Get an object from the pool, null if the pool is empty.
		/// </summary>
		public Object get()
		{
			lock(this)
			{
				if (current >= 0)
				{
					T o = pool[current];
					pool[current] = null;
					current--;

					return o;
				}
			}

			return null;
		}

		/// <summary>
		/// Return the size of the pool
		/// </summary>
		public int Maximum
		{
			get { return maximum; }
		}
	}
}