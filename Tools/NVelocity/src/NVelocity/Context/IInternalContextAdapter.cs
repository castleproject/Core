namespace NVelocity.Context
{
	using System;
	using System.Collections;

	/// <summary>  interface to bring all necessary internal and user contexts together.
	/// this is what the AST expects to deal with.  If anything new comes
	/// along, add it here.
	/// *
	/// I will rename soon :)
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: InternalContextAdapter.cs,v 1.3 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	public interface IInternalContextAdapter : IInternalHousekeepingContext, IContext, IInternalWrapperContext, IInternalEventContext, IDictionary
	{
		/// <summary>
		/// Need to define this method here otherwise since both IDicationary and IContext
		/// contains a Remove(Object key) method we will need to cast the object to either interface
		/// before calling this method, for backward compabillity we make the IContext.Remove the default
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		new Object Remove(object key);
	}
}
