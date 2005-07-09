namespace NVelocity.Util
{
	using System;

	#region CVS information

	/*
    * $Header: /cvsroot/nvelocity/NVelocity/src/Util/SimplePool.cs,v 1.4 2003/11/05 03:57:29 corts Exp $
    * $Revision: 1.4 $
    * $Date: 2003/11/05 03:57:29 $
    */

	#endregion

	/// <summary>
	/// Simple object pool. Based on ThreadPool and few other classes
	/// The pool will ignore overflow and return null if empty.
	/// </summary>
	/// <author>Gal Shachor</author>
	/// <author>Costin</author>
	/// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version>$Id: SimplePool.cs,v 1.4 2003/11/05 03:57:29 corts Exp $</version>
	public sealed class SimplePool
	{
		/*
	* Where the objects are held.
	*/
		private Object[] pool;

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
			pool = new Object[max];
		}

		/// <summary>
		/// Add the object to the pool, silent nothing if the pool is full
		/// </summary>
		public void put(Object o)
		{
			int idx = - 1;

			lock (this)
			{
				/*
		*  if we aren't full
		*/

				if (current < max - 1)
				{
					/*
		    *  then increment the 
		    *  current index.
		    */
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
			lock (this)
			{
				/*
		*  if we have any in the pool
		*/
				if (current >= 0)
				{
					/*
		     *  remove the current one
		     */

					Object o = pool[current];
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
		private Object[] getPool()
		{
			return pool;
		}

	}
}