using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace Castle.DynamicProxy.Generators
{
	public class MethodSignatureComparer : IEqualityComparer<MethodInfo>
	{
		public static readonly MethodSignatureComparer Instance = new MethodSignatureComparer ();

		public bool Equals (MethodInfo x, MethodInfo y)
		{
			if (x == null && y == null)
				return true;
			else if (x == null || y == null)
				return false;
			else
				return x.Name == y.Name && EqualGenericParameters (x, y) && EqualSignatureTypes (x.ReturnType, y.ReturnType) && EqualParameters (x, y);
		}

		private bool EqualGenericParameters (MethodInfo x, MethodInfo y)
		{
			if (x.IsGenericMethod != y.IsGenericMethod)
				return false;
			if (x.IsGenericMethod)
			{
				Type[] xArgs = x.GetGenericArguments ();
				Type[] yArgs = y.GetGenericArguments ();

				if (xArgs.Length != yArgs.Length)
					return false;

				for (int i = 0; i < xArgs.Length; ++i)
				{
					if (xArgs[i].IsGenericParameter != yArgs[i].IsGenericParameter)
						return false;

					if (!xArgs[i].IsGenericParameter && !xArgs[i].Equals (yArgs[i]))
						return false;
				}
			}
			return true;
		}

		private bool EqualParameters (MethodInfo x, MethodInfo y)
		{
			ParameterInfo[] xArgs = x.GetParameters ();
			ParameterInfo[] yArgs = y.GetParameters ();

			if (xArgs.Length != yArgs.Length)
					return false;

			for (int i = 0; i < xArgs.Length; ++i)
			{
				if (!EqualSignatureTypes(xArgs[i].ParameterType, yArgs[i].ParameterType))
					return false;
			}
			return true;
		}

		private static bool EqualSignatureTypes (Type x, Type y)
		{
			if (x.IsGenericParameter != y.IsGenericParameter)
				return false;

			if (x.IsGenericParameter)
			{
				if (x.GenericParameterPosition != y.GenericParameterPosition)
					return false;
			}
			else
			{
				if (!x.Equals (y))
					return false;
			}
			return true;
		}

		public int GetHashCode (MethodInfo obj)
		{
			return obj.Name.GetHashCode () ^ obj.GetParameters ().Length; // everything else would be too cumbersome
		}
	}
}
