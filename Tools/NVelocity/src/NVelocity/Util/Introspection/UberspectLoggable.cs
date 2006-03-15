namespace NVelocity.Util.Introspection
{
	using NVelocity.Runtime;
	
	/// <summary>  
	/// Marker interface to let an uberspector indicate it can and wants to
	/// log
	/// *
	/// Thanks to Paulo for the suggestion
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: UberspectLoggable.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// *
	/// 
	/// </version>
	public interface UberspectLoggable
	{
		/// <summary>  Sets the logger.  This will be called before any calls to the
		/// uberspector
		/// </summary>
		IRuntimeLogger RuntimeLogger { set; }
	}
}
