namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	public static class AttributesToAvoidReplicating
	{
		internal readonly static List<Type> attributes = new List<Type>();

		static AttributesToAvoidReplicating()
		{
			Add<ComImportAttribute>();
		}

		public static void Add(Type attribute)
		{
			if (attributes.Contains(attribute)==false)
				attributes.Add(attribute);
		}

		public static void Add<T>()
		{
			Add(typeof(T));
		}

		public static bool Contains(Type type)
		{
			return attributes.Contains(type);
		}
	}
}