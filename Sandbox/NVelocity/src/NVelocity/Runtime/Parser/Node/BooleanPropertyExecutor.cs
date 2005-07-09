namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Util.Introspection;

	/// <summary>  Handles discovery and valuation of a
	/// boolean object property, of the
	/// form public boolean is<property> when executed.
	///
	/// We do this separately as to preserve the current
	/// quasi-broken semantics of get<as is property>
	/// get< flip 1st char> get("property") and now followed
	/// by is<Property>
	/// </summary>
	public class BooleanPropertyExecutor : PropertyExecutor
	{
		public BooleanPropertyExecutor(RuntimeLogger r, Introspector i, Type clazz, String propertyName) : base(r, i, clazz, propertyName)
		{
		}

		protected internal override void discover(Type clazz, String propertyName)
		{
			try
			{
				property = introspector.getProperty(clazz, propertyName);
				if (property != null)
				{
					if (property.PropertyType.Equals(typeof(Boolean)))
					{
						return;
					}
				}

				/*
		*  now the convenience, flip the 1st character
		*/
				propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
				property = introspector.getProperty(clazz, propertyName);
				if (property != null)
					if (property.PropertyType.Equals(typeof(Boolean)))
					{
						return;
					}

				property = null;
			}
			catch (Exception e)
			{
				rlog.error("PROGRAMMER ERROR : BooleanPropertyExector() : " + e);
			}
		}
	}
}