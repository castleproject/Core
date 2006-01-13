namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// Returned the value of object property when executed.
	/// </summary>
	public class PropertyExecutor : AbstractExecutor
	{
		private String propertyUsed = null;
		protected Introspector introspector = null;

		public PropertyExecutor(RuntimeLogger r, Introspector i, Type clazz, String propertyName)
		{
			rlog = r;
			introspector = i;

			Discover(clazz, propertyName);
		}

		protected internal virtual void Discover(Type clazz, String propertyName)
		{
			// this is gross and linear, but it keeps it straightforward.
			try
			{
				propertyUsed = propertyName;
				property = introspector.GetProperty(clazz, propertyUsed);
				if (property != null)
					return;

				// now the convenience, flip the 1st character
				propertyUsed = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
				property = introspector.GetProperty(clazz, propertyUsed);
				if (property != null)
					return;

				propertyUsed = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
				property = introspector.GetProperty(clazz, propertyUsed);
				if (property != null)
					return;

				// check for a method that takes no arguments
				propertyUsed = propertyName;
				method = introspector.GetMethod(clazz, propertyUsed, new Object[0]);
				if (method != null)
					return;

				// check for a method that takes no arguments, flipping 1st character
				propertyUsed = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
				method = introspector.GetMethod(clazz, propertyUsed, new Object[0]);
				if (method != null)
					return;

				propertyUsed = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
				method = introspector.GetMethod(clazz, propertyUsed, new Object[0]);
				if (method != null)
					return;
			}
			catch (Exception e)
			{
				rlog.Error("PROGRAMMER ERROR : PropertyExector() : " + e);
			}
		}

		/// <summary>
		/// Execute property against context.
		/// </summary>
		public override Object Execute(Object o)
		{
			if (property == null && method == null)
				return null;

			if (property != null)
			{
				return property.GetValue(o, null);
			}
			else
			{
				return method.Invoke(o, new Object[0]);
			}
		}
	}
}