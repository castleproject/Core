namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;

	public class MethodMap
	{
		/// <summary> Keep track of all methods with the same name.</summary>
		internal IDictionary methodByNameMap = new Hashtable();

		private const int MORE_SPECIFIC = 0;
		private const int LESS_SPECIFIC = 1;
		private const int INCOMPARABLE = 2;

		/// <summary> Add a method to a list of methods by name.
		/// For a particular class we are keeping track
		/// of all the methods with the same name.
		/// </summary>
		public void Add(MethodInfo method)
		{
			String methodName = method.Name;

			IList l = Get(methodName);

			if (l == null)
			{
				l = new ArrayList();
				methodByNameMap.Add(methodName, l);
			}

			l.Add(method);
		}

		/// <summary>
		/// Return a list of methods with the same name.
		/// </summary>
		/// <param name="key">key</param>
		/// <returns> List list of methods</returns>
		public IList Get(String key)
		{
			return (IList) methodByNameMap[key];
		}

		/// <summary>
		/// Find a method.  Attempts to find the
		/// most specific applicable method using the
		/// algorithm described in the JLS section
		/// 15.12.2 (with the exception that it can't
		/// distinguish a primitive type argument from
		/// an object type argument, since in reflection
		/// primitive type arguments are represented by
		/// their object counterparts, so for an argument of
		/// type (say) java.lang.Integer, it will not be able
		/// to decide between a method that takes int and a
		/// method that takes java.lang.Integer as a parameter.
		/// 
		/// <para>
		/// This turns out to be a relatively rare case
		/// where this is needed - however, functionality
		/// like this is needed.
		/// </para>
		/// </summary>
		/// <param name="methodName">name of method</param>
		/// <param name="args">the actual arguments with which the method is called</param>
		/// <returns> the most specific applicable method, or null if no method is applicable.</returns>
		/// <exception cref="AmbiguousException">if there is more than one maximally specific applicable method</exception>
		public MethodInfo Find(String methodName, Object[] args)
		{
			IList methodList = Get(methodName);

			if (methodList == null)
			{
				return null;
			}

			int l = args.Length;
			Type[] classes = new Type[l];

			for (int i = 0; i < l; ++i)
			{
				Object arg = args[i];

				// if we are careful down below, a null argument goes in there
				// so we can know that the null was passed to the method
				classes[i] = arg == null ? null : arg.GetType();
			}

			return GetMostSpecific(methodList, classes);
		}

		/// <summary>
		/// simple distinguishable exception, used when
		/// we run across ambiguous overloading
		/// </summary>
		[Serializable]
		public class AmbiguousException : Exception
		{
			public AmbiguousException()
			{
			}

			public AmbiguousException(string message) : base(message)
			{
			}

			public AmbiguousException(string message, Exception innerException) : base(message, innerException)
			{
			}

			public AmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		private static MethodInfo GetMostSpecific(IList methods, Type[] classes)
		{
			ArrayList applicables = GetApplicables(methods, classes);

			if (applicables.Count == 0)
			{
				return null;
			}

			if (applicables.Count == 1)
			{
				return (MethodInfo) applicables[0];
			}

			// This list will contain the maximally specific methods. Hopefully at
			// the end of the below loop, the list will contain exactly one method,
			// (the most specific method) otherwise we have ambiguity.
			ArrayList maximals = new ArrayList();

			foreach (MethodInfo app in applicables)
			{
				ParameterInfo[] appArgs = app.GetParameters();
				bool lessSpecific = false;

				foreach (MethodInfo max in maximals)
				{
					switch (IsMoreSpecific(appArgs, max.GetParameters()))
					{
						case MORE_SPECIFIC:
						{
							// This method is more specific than the previously
							// known maximally specific, so remove the old maximum.
							maximals.Remove(max);
							break;
						}

						case LESS_SPECIFIC:
						{
							// This method is less specific than some of the
							// currently known maximally specific methods, so we
							// won't add it into the set of maximally specific
							// methods

							lessSpecific = true;
							break;
						}
					}
				}

				if (!lessSpecific)
				{
					maximals.Add(app);
				}
			}
			
			// In a last attempt we remove 
			// the methods found for interfaces
			if (maximals.Count > 1)
			{
				ArrayList newList = new ArrayList();
				
				foreach(MethodInfo method in maximals)
				{
					if (method.DeclaringType.IsInterface) continue;

					newList.Add(method);
				}
				
				maximals = newList;
			}

			if (maximals.Count > 1)
			{
				// We have more than one maximally specific method
				throw new AmbiguousException(CreateDescriptiveAmbiguousErrorMessage(maximals, classes));
			}

			return (MethodInfo) maximals[0];
		}

		/// <summary> Determines which method signature (represented by a class array) is more
		/// specific. This defines a partial ordering on the method signatures.
		/// </summary>
		/// <param name="c1">first signature to compare
		/// </param>
		/// <param name="c2">second signature to compare
		/// </param>
		/// <returns> MORE_SPECIFIC if c1 is more specific than c2, LESS_SPECIFIC if
		/// c1 is less specific than c2, INCOMPARABLE if they are incomparable.
		/// 
		/// </returns>
		private static int IsMoreSpecific(ParameterInfo[] c1, ParameterInfo[] c2)
		{
			bool c1MoreSpecific = false;
			bool c2MoreSpecific = false;

			for (int i = 0; i < c1.Length; ++i)
			{
				if (c1[i] != c2[i])
				{
					c1MoreSpecific = c1MoreSpecific || IsStrictMethodInvocationConvertible(c2[i], c1[i]);
					c2MoreSpecific = c2MoreSpecific || IsStrictMethodInvocationConvertible(c1[i], c2[i]);
				}
			}

			if (c1MoreSpecific)
			{
				if (c2MoreSpecific)
				{
					//  Incomparable due to cross-assignable arguments (i.e.
					// foo(String, Object) vs. foo(Object, String))

					return INCOMPARABLE;
				}

				return MORE_SPECIFIC;
			}

			if (c2MoreSpecific)
			{
				return LESS_SPECIFIC;
			}

			// Incomparable due to non-related arguments (i.e.
			// foo(Runnable) vs. foo(Serializable))
			
			return INCOMPARABLE;
		}

		/// <summary>
		/// Returns all methods that are applicable to actual argument types.
		/// </summary>
		/// <param name="methods">list of all candidate methods</param>
		/// <param name="classes">the actual types of the arguments</param>
		/// <returns> 
		/// a list that contains only applicable methods (number of 
		/// formal and actual arguments matches, and argument types are assignable
		/// to formal types through a method invocation conversion).
		/// </returns>
		/// TODO: this used to return a LinkedList -- changed to an ArrayList for now until I can figure out what is really needed
		private static ArrayList GetApplicables(IList methods, Type[] classes)
		{
			ArrayList list = new ArrayList();

			foreach (MethodInfo method in methods)
			{
				if (IsApplicable(method, classes))
					list.Add(method);
			}
			return list;
		}

		/// <summary>
		/// Returns true if the supplied method is applicable to actual
		/// argument types.
		/// </summary>
		private static bool IsApplicable(MethodInfo method, Type[] classes)
		{
			ParameterInfo[] methodArgs = method.GetParameters();

			int indexOfParamArray = Int32.MaxValue;

			for (int i = 0; i < methodArgs.Length; ++i)
			{
				ParameterInfo paramInfo = methodArgs[i];

				if (paramInfo.IsDefined( typeof(ParamArrayAttribute), false ))
				{
					indexOfParamArray = i; break;
				}
			}

			if (indexOfParamArray == Int32.MaxValue && methodArgs.Length != classes.Length)
			{
				return false;
			}

			for (int i = 0; i < classes.Length; ++i)
			{
				ParameterInfo paramInfo = null;
				if (i < indexOfParamArray)
					paramInfo = methodArgs[i];
				else
					paramInfo = methodArgs[indexOfParamArray];

				if (!IsMethodInvocationConvertible(paramInfo, classes[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Determines whether a type represented by a class object is
		/// convertible to another type represented by a class object using a
		/// method invocation conversion, treating object types of primitive
		/// types as if they were primitive types (that is, a Boolean actual
		/// parameter type matches boolean primitive formal type). This behavior
		/// is because this method is used to determine applicable methods for
		/// an actual parameter list, and primitive types are represented by
		/// their object duals in reflective method calls.
		/// </summary>
		/// <param name="formal">the formal parameter type to which the actual parameter type should be convertible</param>
		/// <param name="actual">the actual parameter type.</param>
		/// <returns>
		/// true if either formal type is assignable from actual type,
		/// or formal is a primitive type and actual is its corresponding object
		/// type or an object type of a primitive type that can be converted to
		/// the formal type.
		/// </returns>
		private static bool IsMethodInvocationConvertible(ParameterInfo formal, Type actual)
		{
			Type underlyingType = formal.ParameterType;

			if (formal.IsDefined( typeof(ParamArrayAttribute), false ))
			{
				underlyingType = formal.ParameterType.GetElementType();
			}

			// if it's a null, it means the arg was null
			if (actual == null && !underlyingType.IsPrimitive)
			{
				return true;
			}

			// Check for identity or widening reference conversion
			if (actual != null && underlyingType.IsAssignableFrom(actual))
			{
				return true;
			}

			// Check for boxing with widening primitive conversion.
			if (underlyingType.IsPrimitive)
			{
				if (underlyingType == typeof(Boolean) && actual == typeof(Boolean))
					return true;
				if (underlyingType == typeof(Char) && actual == typeof(Char))
					return true;
				if (underlyingType == typeof(Byte) && actual == typeof(Byte))
					return true;
				if (underlyingType == typeof(Int16) && (actual == typeof(Int16) || actual == typeof(Byte)))
					return true;
				if (underlyingType == typeof(Int32) && (actual == typeof(Int32) || actual == typeof(Int16) || actual == typeof(Byte)))
					return true;
				if (underlyingType == typeof(Int64) && (actual == typeof(Int64) || actual == typeof(Int32) || actual == typeof(Int16) || actual == typeof(Byte)))
					return true;
				if (underlyingType == typeof(Single) && (actual == typeof(Single) || actual == typeof(Int64) || actual == typeof(Int32) || actual == typeof(Int16) || actual == typeof(Byte)))
					return true;
				if (underlyingType == typeof(Double) && (actual == typeof(Double) || actual == typeof(Single) || actual == typeof(Int64) || actual == typeof(Int32) || actual == typeof(Int16) || actual == typeof(Byte)))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether a type represented by a class object is
		/// convertible to another type represented by a class object using a
		/// method invocation conversion, without matching object and primitive
		/// types. This method is used to determine the more specific type when
		/// comparing signatures of methods.
		/// </summary>
		/// <param name="formal">the formal parameter type to which the actual parameter type should be convertible</param>
		/// <param name="actual">the actual parameter type.</param>
		/// <returns>
		/// true if either formal type is assignable from actual type,
		/// or formal and actual are both primitive types and actual can be
		/// subject to widening conversion to formal.
		/// </returns>
		private static bool IsStrictMethodInvocationConvertible(ParameterInfo formal, ParameterInfo actual)
		{
			// we shouldn't get a null into, but if so
			if (actual == null && !formal.ParameterType.IsPrimitive)
				return true;

			// Check for identity or widening reference conversion
			if (formal.ParameterType.IsAssignableFrom(actual.ParameterType))
				return true;

			// Check for widening primitive conversion.
			if (formal.ParameterType.IsPrimitive)
			{
				if (formal.ParameterType == typeof(Int16) && (actual.ParameterType == typeof(Byte)))
					return true;
				if (formal.ParameterType == typeof(Int32) && (actual.ParameterType == typeof(Int16) || actual.ParameterType == typeof(Byte)))
					return true;
				if (formal.ParameterType == typeof(Int64) && (actual.ParameterType == typeof(Int32) || actual.ParameterType == typeof(Int16) || actual.ParameterType == typeof(Byte)))
					return true;
				if (formal.ParameterType == typeof(Single) && (actual.ParameterType == typeof(Int64) || actual.ParameterType == typeof(Int32) || actual.ParameterType == typeof(Int16) || actual.ParameterType == typeof(Byte)))
					return true;
				if (formal.ParameterType == typeof(Double) && (actual.ParameterType == typeof(Single) || actual.ParameterType == typeof(Int64) || actual.ParameterType == typeof(Int32) || actual.ParameterType == typeof(Int16) || actual.ParameterType == typeof(Byte)))
					return true;
			}
			return false;
		}
		
		private static string CreateDescriptiveAmbiguousErrorMessage(IList list, Type[] classes)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append("There are two or more methods that can be bound given the parameters types (");
			
			foreach(Type paramType in classes)
			{
				if (paramType == null)
				{
					sb.Append("null");
				}
				else
				{
					sb.Append(paramType.Name);
				}
				
				sb.Append(" ");
			}
			
			sb.Append(") Methods: ");
			
			foreach(MethodInfo method in list)
			{
				sb.AppendFormat(" {0}.{1}({2}) ", method.DeclaringType.Name, method.Name, 
				                CreateParametersDescription(method.GetParameters()));
			}
			
			return sb.ToString();
		}

		private static String CreateParametersDescription(ParameterInfo[] parameters)
		{
			String message = String.Empty;
			
			foreach(ParameterInfo param in parameters)
			{
				if (message != String.Empty)
				{
					message += ", ";
				}
				
				message += param.ParameterType.Name;
			}
			
			return message;
		}
	}
}