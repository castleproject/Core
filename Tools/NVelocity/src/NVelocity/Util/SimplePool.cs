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
		private int max;

		/// <summary>  index of previous to next
		/// free slot
		/// </summary>
		private int current = - 1;

		public SimplePool(int max)
		{
			this.max = max;
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
				if (current < max - 1)
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
		public int Max
		{
			get { return max; }
		}

		/// <summary>
		/// for testing purposes, so we can examine the pool
		/// </summary>
		private T[] getPool()
		{
			return pool;
		}
	}
}