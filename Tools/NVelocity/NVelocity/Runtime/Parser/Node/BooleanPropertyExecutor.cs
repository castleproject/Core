namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Util.Introspection;

	/// <summary>  Handles discovery and valuation of a
	/// boolean object property, of the
	/// form public boolean is&lt;property&gt; when executed.
	///
	/// We do this separately as to preserve the current
	/// quasi-broken semantics of get&lt;as is property&gt;
	/// get&lt; flip 1st char&gt; get("property") and now followed
	/// by is&lt;Property&gt;
	/// </summary>
	public class BooleanPropertyExecutor : PropertyExecutor
	{
		public BooleanPropertyExecutor(IRuntimeLogger r, Introspector i, Type type, String propertyName)
			: base(r, i, type, propertyName)
		{
		}

		protected internal override void Discover(Type type, String propertyName)
		{
			try
			{
				property = introspector.GetProperty(type, propertyName);
				if (property != null && property.PropertyType.Equals(typeof(Boolean)))
				{
					return;
				}

				// now the convenience, flip the 1st character
				propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
				property = introspector.GetProperty(type, propertyName);
				if (property != null && property.PropertyType.Equals(typeof(Boolean)))
				{
					return;
				}

				property = null;
			}
			catch(Exception e)
			{
				runtimeLogger.Error(string.Format("PROGRAMMER ERROR : BooleanPropertyExecutor() : {0}", e));
			}
		}
	}
}