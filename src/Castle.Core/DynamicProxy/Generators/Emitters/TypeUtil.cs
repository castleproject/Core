// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;

	public static class TypeUtil
	{
		public static FieldInfo[] GetAllFields(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (type.IsClass == false)
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
				currentType = currentType.BaseType;
			}

			return fields.ToArray();
		}

		/// <summary>
		///   Returns list of all unique interfaces implemented given types, including their base interfaces.
		/// </summary>
		/// <param name = "types"></param>
		/// <returns></returns>
		public static ICollection<Type> GetAllInterfaces(params Type[] types)
		{
			if (types == null)
			{
				return Type.EmptyTypes;
			}

			var dummy = new object();
			// we should move this to HashSet once we no longer support .NET 2.0
			IDictionary<Type, object> interfaces = new Dictionary<Type, object>();
			foreach (var type in types)
			{
				if (type == null)
				{
					continue;
				}

				if (type.IsInterface)
				{
					interfaces[type] = dummy;
				}

				foreach (var @interface in type.GetInterfaces())
				{
					interfaces[@interface] = dummy;
				}
			}

			return Sort(interfaces.Keys);
		}

		public static ICollection<Type> GetAllInterfaces(this Type type)
		{
			return GetAllInterfaces(new[] { type });
		}

		public static Type GetClosedParameterType(this AbstractTypeEmitter type, Type parameter)
		{
			if (parameter.IsGenericTypeDefinition)
			{
				return parameter.GetGenericTypeDefinition().MakeGenericType(type.GetGenericArgumentsFor(parameter));
			}

			if (parameter.IsGenericType)
			{
				var arguments = parameter.GetGenericArguments();
				if (CloseGenericParametersIfAny(type, arguments))
				{
					return parameter.GetGenericTypeDefinition().MakeGenericType(arguments);
				}
			}

			if (parameter.IsGenericParameter)
			{
				return type.GetGenericArgument(parameter.Name);
			}

			if (parameter.IsArray)
			{
				var elementType = GetClosedParameterType(type, parameter.GetElementType());
				return elementType.MakeArrayType();
			}

			if (parameter.IsByRef)
			{
				var elementType = GetClosedParameterType(type, parameter.GetElementType());
				return elementType.MakeByRefType();
			}

			return parameter;
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
			var flags = additionalFlags | BindingFlags.Static | BindingFlags.SetField;

			try
			{
				type.InvokeMember(fieldName, flags, null, null, new[] { value });
			}
			catch (MissingFieldException e)
			{
				throw new ProxyGenerationException(
					string.Format(
						"Could not find field named '{0}' on type {1}. This is likely a bug in DynamicProxy. Please report it.",
						fieldName,
						type), e);
			}
			catch (TargetException e)
			{
				throw new ProxyGenerationException(
					string.Format(
						"There was an error trying to set field named '{0}' on type {1}. This is likely a bug in DynamicProxy. Please report it.",
						fieldName,
						type), e);
			}
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

		private static bool CloseGenericParametersIfAny(AbstractTypeEmitter emitter, Type[] arguments)
		{
			var hasAnyGenericParameters = false;
			for (var i = 0; i < arguments.Length; i++)
			{
				var newType = GetClosedParameterType(emitter, arguments[i]);
				if (!ReferenceEquals(newType, arguments[i]))
				{
					arguments[i] = newType;
					hasAnyGenericParameters = true;
				}
			}
			return hasAnyGenericParameters;
		}

		private static Type[] Sort(IEnumerable<Type> types)
		{
			var array = types.ToArray();
			//NOTE: is there a better, stable way to sort Types. We will need to revise this once we allow open generics
			Array.Sort(array, (l, r) => string.Compare(l.AssemblyQualifiedName, r.AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase));
			return array;
		}
	}
}