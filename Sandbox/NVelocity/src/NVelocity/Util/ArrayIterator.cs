namespace NVelocity.Util
{
	using System;

	/// <summary>  <p>
	/// An Iterator wrapper for an Object[]. This will
	/// allow us to deal with all array like structures
	/// in a consistent manner.
	/// </p>
	/// <p>
	/// WARNING : this class's operations are NOT synchronized.
	/// It is meant to be used in a single thread, newly created
	/// for each use in the #foreach() directive.
	/// If this is used or shared, synchronize in the
	/// next() method.
	/// </p>
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ArrayIterator.cs,v 1.3 2004/12/27 05:55:07 corts Exp $
	///
	/// </version>
	public class ArrayIterator : Iterator
	{
		/// <summary> The objects to iterate.
		/// </summary>
		private Object array;

		/// <summary> The current position and size in the array.
		/// </summary>
		private int pos;

		private int size;

		/// <summary> Creates a new iterator instance for the specified array.
		/// *
		/// </summary>
		/// <param name="array">The array for which an iterator is desired.
		///
		/// </param>
		public ArrayIterator(Object array)
		{
			/*
	    * if this isn't an array, then throw.  Note that this is 
	    * for internal use - so this should never happen - if it does
	    *  we screwed up.
	    */

			if (!array.GetType().IsArray)
			{
				throw new ArgumentException("Programmer error : internal ArrayIterator invoked w/o array");
			}

			this.array = array;
			pos = 0;
			size = ((double[]) this.array).Length;
		}

		/// <summary> Move to next element in the array.
		/// *
		/// </summary>
		/// <returns>The next object in the array.
		///
		/// </returns>
		public Object next()
		{
			if (pos < size)
			{
				//UPGRADE_ISSUE: Method 'java.lang.reflect.Array.get' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangreflectArrayget_javalangObject_int"'
				return ((Array) array).GetValue(pos++);
				//return Array.get(array, pos++);
			}

			/*
	    *  we screwed up...
	    */

			throw new Exception("No more elements: " + pos + " / " + size);
		}

		/// <summary> Check to see if there is another element in the array.
		/// *
		/// </summary>
		/// <returns>Whether there is another element.
		///
		/// </returns>
		public bool hasNext()
		{
			return (pos < size);
		}

	}
}