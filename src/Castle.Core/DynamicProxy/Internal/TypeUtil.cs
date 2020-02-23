// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters;

	public static class TypeUtil
	{
		public static bool IsNullableType(this Type type)
		{
			return type.GetTypeInfo().IsGenericType &&
			       type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static FieldInfo[] GetAllFields(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (type.GetTypeInfo().IsClass == false)
			{
				throw new ArgumentException(string.Format("Type {0} is not a class type. This method supports only classes", type));
			}

			var fields = new List<FieldInfo>();
			var currentType = type;
			while (currentType != typeof(object))
			{
				Debug.Assert(currentType != null);
				var currentFields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				fields.AddRange(currentFields);
				currentType = currentType.GetTypeInfo().BaseType;
			}

			return fields.ToArray();
		}

		/// <summary>
		///   Returns list of all unique interfaces implemented given types, including their base interfaces.
		/// </summary>
		public static Type[] GetAllInterfaces(params Type[] types)
		{
			if (types == null)
			{
				return Type.EmptyTypes;
			}

			var interfaces = new HashSet<Type>();
			for (var index = 0; index < types.Length; index++)
			{
				var type = types[index];
				if (type == null)
				{
					continue;
				}

				if (type.GetTypeInfo().IsInterface)
				{
					if (interfaces.Add(type) == false)
					{
						continue;
					}
				}

				var innerInterfaces = type.GetInterfaces();
				for (var i = 0; i < innerInterfaces.Length; i++)
				{
					var @interface = innerInterfaces[i];
					interfaces.Add(@interface);
				}
			}

			return Sort(interfaces);
		}

		public static Type[] GetAllInterfaces(this Type type)
		{
			return GetAllInterfaces(new[] { type });
		}

		public static Type GetClosedParameterType(this AbstractTypeEmitter type, Type parameter)
		{
			if (parameter.GetTypeInfo().IsGenericTypeDefinition)
			{
				return parameter.GetGenericTypeDefinition().MakeGenericType(type.GetGenericArgumentsFor(parameter));
			}

			if (parameter.GetTypeInfo().IsGenericType)
			{
				var arguments = parameter.GetGenericArguments();
				if (CloseGenericParametersIfAny(type, arguments))
				{
					return parameter.GetGenericTypeDefinition().MakeGenericType(arguments);
				}
			}

			if (parameter.GetTypeInfo().IsGenericParameter)
			{
				return type.GetGenericArgument(parameter.Name);
			}

			if (parameter.GetTypeInfo().IsArray)
			{
				var elementType = GetClosedParameterType(type, parameter.GetElementType());
				int rank = parameter.GetArrayRank();
				return rank == 1
					? elementType.MakeArrayType()
					: elementType.MakeArrayType(rank);
			}

			if (parameter.GetTypeInfo().IsByRef)
			{
				var elementType = GetClosedParameterType(type, parameter.GetElementType());
				return elementType.MakeByRefType();
			}

			return parameter;
		}

		public static Type GetTypeOrNull(object target)
		{
			if (target == null)
			{
				return null;
			}
			return target.GetType();
		}

		public static Type[] AsTypeArray(this GenericTypeParameterBuilder[] typeInfos)
		{
			Type[] types = new Type[typeInfos.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = typeInfos[i].AsType();
			}
			return types;
		}

		public static bool IsFinalizer(this MethodInfo methodInfo)
		{
			return string.Equals("Finalize", methodInfo.Name) && methodInfo.GetBaseDefinition().DeclaringType == typeof(object);
		}

		public static bool IsGetType(this MethodInfo methodInfo)
		{
			return methodInfo.DeclaringType == typeof(object) && string.Equals("GetType", methodInfo.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsMemberwiseClone(this MethodInfo methodInfo)
		{
			return methodInfo.DeclaringType == typeof(object) && string.Equals("MemberwiseClone", methodInfo.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static void SetStaticField(this Type type, string fieldName, BindingFlags additionalFlags, object value)
		{
			var flags = additionalFlags | BindingFlags.Static;

			FieldInfo field = type.GetField(fieldName, flags);
			if (field == null)
			{
				throw new ProxyGenerationException(string.Format(
					"Could not find field named '{0}' on type {1}. This is likely a bug in DynamicProxy. Please report it.",
					fieldName, type));
			}

			try
			{
				field.SetValue(null, value);
			}
			catch (MissingFieldException e)
			{
				throw new ProxyGenerationException(
					string.Format(
						"Could not find field named '{0}' on type {1}. This is likely a bug in DynamicProxy. Please report it.",
						fieldName,
						type), e);
			}
#if FEATURE_TARGETEXCEPTION
			catch (TargetException e)
			{
				throw new ProxyGenerationException(
					string.Format(
						"There was an error trying to set field named '{0}' on type {1}. This is likely a bug in DynamicProxy. Please report it.",
						fieldName,
						type), e);
			}
#endif
			catch (TargetInvocationException e) // yes, this is not documented in MSDN. Yay for documentation
			{
				if ((e.InnerException is TypeInitializationException) == false)
				{
					throw;
				}
				throw new ProxyGenerationException(
					string.Format(
						"There was an error in static constructor on type {0}. This is likely a bug in DynamicProxy. Please report it.",
						type), e);
			}
		}

		public static MemberInfo[] Sort(MemberInfo[] members)
		{
			var sortedMembers = new MemberInfo[members.Length];
			Array.Copy(members, sortedMembers, members.Length);
			Array.Sort(sortedMembers, (l, r) => string.Compare(l.Name, r.Name, StringComparison.OrdinalIgnoreCase));
			return sortedMembers;
		}

		/// <summary>
		///   Checks whether the specified <paramref name="type"/> is a delegate type (i.e. a direct subclass of <see cref="MulticastDelegate"/>).
		/// </summary>
		internal static bool IsDelegateType(this Type type)
		{
			return type.GetTypeInfo().BaseType == typeof(MulticastDelegate);
		}

		private static bool CloseGenericParametersIfAny(AbstractTypeEmitter emitter, Type[] arguments)
		{
			var hasAnyGenericParameters = false;
			for (var i = 0; i < arguments.Length; i++)
			{
				var newType = GetClosedParameterType(emitter, arguments[i]);
				if (newType != null && !ReferenceEquals(newType, arguments[i]))
				{
					arguments[i] = newType;
					hasAnyGenericParameters = true;
				}
			}
			return hasAnyGenericParameters;
		}

		private static Type[] Sort(ICollection<Type> types)
		{
			var array = new Type[types.Count];
			types.CopyTo(array, 0);
			//NOTE: is there a better, stable way to sort Types. We will need to revise this once we allow open generics
			Array.Sort(array, TypeNameComparer.Instance);
			//                ^^^^^^^^^^^^^^^^^^^^^^^^^
			// Using a `IComparer<T>` object instead of a `Comparison<T>` delegate prevents
			// an unnecessary level of indirection inside the framework (as the latter get
			// wrapped as `IComparer<T>` objects).
			return array;
		}

		private sealed class TypeNameComparer : IComparer<Type>
		{
			public static readonly TypeNameComparer Instance = new TypeNameComparer();

			public int Compare(Type x, Type y)
			{
				// Comparing by `type.AssemblyQualifiedName` would give the same result,
				// but it performs a hidden concatenation (and therefore string allocation)
				// of `type.FullName` and `type.Assembly.FullName`. We can avoid this
				// overhead by comparing the two properties separately.
				int result = string.CompareOrdinal(x.FullName, y.FullName);
				return result != 0 ? result : string.CompareOrdinal(x.GetTypeInfo().Assembly.FullName, y.GetTypeInfo().Assembly.FullName);
			}
		}
	}
}