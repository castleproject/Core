namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary>  Method used for regular method invocation
	/// *
	/// $foo.bar()
	/// *
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: VelMethod.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public interface IVelMethod
	{
		/// <summary>  specifies if this VelMethod is cacheable and able to be
		/// reused for this class of object it was returned for
		/// *
		/// </summary>
		/// <returns> true if can be reused for this class, false if not
		/// 
		/// </returns>
		bool Cacheable { get; }

		/// <summary>  returns the method name used
		/// </summary>
		String MethodName { get; }

		/// <summary>  returns the return type of the method invoked
		/// </summary>
		Type ReturnType { get; }

		/// <summary>  invocation method - called when the method invocationshould be
		/// preformed and a value returned
		/// </summary>
		Object Invoke(Object o, Object[] paramsRenamed);
	}
}