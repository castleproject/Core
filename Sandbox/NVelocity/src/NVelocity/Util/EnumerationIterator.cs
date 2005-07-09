namespace NVelocity.Util
{
	using System;
	using System.Collections;

	/// <summary>
	/// An Iterator wrapper for an Enumeration.
	/// </summary>
	/// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: EnumerationIterator.cs,v 1.3 2004/12/27 05:55:07 corts Exp $</version>
	public class EnumerationIterator : Iterator
	{
		/// <summary>
		/// The enumeration to iterate.
		/// </summary>
		private IEnumerator enum_Renamed = null;

		/// <summary>
		/// Creates a new iteratorwrapper instance for the specified
		/// Enumeration.
		/// </summary>
		/// <param name="enum"> The Enumeration to wrap.
		/// </param>
		public EnumerationIterator(IEnumerator enum_Renamed)
		{
			this.enum_Renamed = enum_Renamed;
		}

		/// <summary>
		/// Move to next element in the array.
		/// </summary>
		/// <returns>The next object in the array.
		/// </returns>
		public Object next()
		{
			//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
			return enum_Renamed.Current;
		}

		/// <summary>
		/// Check to see if there is another element in the array.
		/// </summary>
		/// <returns>Whether there is another element.
		/// </returns>
		public bool hasNext()
		{
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			return enum_Renamed.MoveNext();
		}

	}
}