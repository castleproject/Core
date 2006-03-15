namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary>
	/// Holds information for node-local context data introspection
	/// information.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: IntrospectionCacheData.cs,v 1.3 2003/10/27 13:54:12 corts Exp $ </version>
	public class IntrospectionCacheData
	{
		public IntrospectionCacheData()
		{
		}

		public IntrospectionCacheData(Type contextData, object thingy)
		{
			this.Thingy = thingy;
			this.ContextData = contextData;
		}

		/// <summary>
		/// Object to pair with class - currently either a Method or
		/// AbstractExecutor. It can be used in any way the using node
		/// wishes.
		/// </summary>
		public Object Thingy;

		/// <summary>
		/// Class of context data object associated with the 
		/// introspection information
		/// </summary>
		public Type ContextData;
	}
}