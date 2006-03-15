namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary>
	/// Interface defining a 'getter'.  For uses when looking for resolution of
	/// property references
	/// <code>
	/// $foo.bar
	/// </code>
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: VelPropertyGet.cs,v 1.1 2004/12/27 05:55:08 corts Exp $ </version>
	public interface IVelPropertyGet
	{
		/// <summary>
		/// specifies if this VelPropertyGet is cacheable and able to be
		/// reused for this class of object it was returned for
		/// </summary>
		/// <returns>true if can be reused for this class, false if not</returns>
		bool Cacheable { get; }

		/// <summary>
		/// returns the method name used to return this 'property'
		/// </summary>
		String MethodName { get; }

		/// <summary>
		/// invocation method - called when the 'get action' should be
		/// preformed and a value returned
		/// </summary>
		Object Invoke(Object o);
	}
}