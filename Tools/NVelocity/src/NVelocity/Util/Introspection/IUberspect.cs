namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary> 'Federated' introspection/reflection interface to allow the introspection
	/// behavior in Velocity to be customized.
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@apache.org">Geir Magusson Jr.</a>
	/// </author>
	/// <version>  $Id: Uberspect.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public interface IUberspect
	{
		/// <summary>  Initializer - will be called before use
		/// </summary>
		void Init();

		/// <summary>  Returns a general method, corresponding to $foo.bar( $woogie )
		/// </summary>
		IVelMethod GetMethod(Object obj, String method, Object[] args, Info info);

		/// <summary> Property getter - returns VelPropertyGet appropos for #set($foo = $bar.woogie)
		/// </summary>
		IVelPropertyGet GetPropertyGet(Object obj, String identifier, Info info);

		/// <summary> Property setter - returns VelPropertySet appropos for #set($foo.bar = "geir")
		/// </summary>
		IVelPropertySet GetPropertySet(Object obj, String identifier, Object arg, Info info);
	}
}