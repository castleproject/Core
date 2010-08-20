// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Security;
	using System.Security.Permissions;

	/// <summary>
	/// Delegate to represent access to an instance.
	/// </summary>
	/// <param name="target">The target instance.</param>
	/// <returns>The accessed value.</returns>
	public delegate object Accessor(object target);

	/// <summary>
	/// Utility for accessing parts of an instance.
	/// </summary>
	public static class AccessorUtil
	{
		/// <summary>
		/// Gets the accessor for the property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The property accessor.</returns>
		public static Accessor GetAccessor(PropertyInfo property)
		{
			if (IsReflectionEmitAllowed() && !property.DeclaringType.IsInterface)
			{
				// this doesn't seem to work when declaring type is an interface
				ILGenerator il;
				DynamicMethod method = CreateAccessorMethod(property.DeclaringType, out il);
				return CreateAccessor(GeneratePropertyIL(property, il), method, il);
			}

			return delegate(object target)
			{
				return GetPropertyValue(target, property);
			};
		}

		/// <summary>
		/// Gets the access to an an expression on a type.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="path">The path expression.</param>
		/// <returns>The expression accessor.</returns>
		public static Accessor GetAccessor(Type type, string path)
		{
			string[] parts = path.Split('.');
			
			if (IsReflectionEmitAllowed() && !type.IsInterface)
			{
				// this doesn't seem to work when declaring type is an interface
				ILGenerator il;
				DynamicMethod method = CreateAccessorMethod(type, out il);
				return CreateAccessor(GeneratePathIL(type, il, parts), method, il);
			}

			return delegate(object target)
			{
				return GetPathValue(target, parts);
			};
		}

		/// <summary>
		/// Obtains the value of a property on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <param name="property">The property to inspect.</param>
		/// <returns>The property value.</returns>
		public static object GetPropertyValue(object instance, PropertyInfo property)
		{
			return property.GetValue(instance, null);
		}

		/// <summary>
		/// Obtains the value of a property or field expression on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <param name="path">The path of the field or property to inspect.</param>
		/// <returns>The path value.</returns>
		public static object GetPathValue(object instance, string path)
		{
			return GetPathValue(instance, path.Split('.'));
		}

		/// <summary>
		/// Obtains the value of a property or field expression on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <param name="path">The path of the field or property to inspect.</param>
		/// <returns>The path value.</returns>
		private static object GetPathValue(object instance, string[] path)
		{
			foreach (string fieldOrProperty in path)
			{
				Type targetType = instance.GetType();

				PropertyInfo pi = targetType.GetProperty(fieldOrProperty, PublicBinding);

				if (pi == null)
				{
					FieldInfo fi = targetType.GetField(fieldOrProperty, PublicBinding);

					if (fi != null)
					{
						instance = fi.GetValue(instance);
					}
					else
					{
						throw new ValidationException(string.Format(
							"No field or property {0} found for type {1}",
							fieldOrProperty, targetType.FullName));
					}
				}
				else
				{
					instance = pi.GetValue(instance, null);
				}
			}

			return instance;
		}

		private static DynamicMethod CreateAccessorMethod(Type owner, out ILGenerator il)
		{
			DynamicMethod method = new DynamicMethod(string.Empty, typeof(object),
				new Type[] { typeof(object) }, owner, false);

			il = method.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);

			return method;
		}

		private static Accessor CreateAccessor(Type result, DynamicMethod method, ILGenerator il)
		{
			if (result.IsValueType)
			{
				il.Emit(OpCodes.Box, result);
			}

			il.Emit(OpCodes.Ret);

			return (Accessor)method.CreateDelegate(typeof(Accessor));
		}

		private static Type GeneratePathIL(Type type, ILGenerator il, string[] path)
		{
			foreach (string fieldOrProperty in path)
			{
				PropertyInfo property = type.GetProperty(fieldOrProperty, PublicBinding);

				if (property != null)
				{
					type = GeneratePropertyIL(property, il);
				}
				else
				{
					FieldInfo field = type.GetField(fieldOrProperty, PublicBinding);

					if (field != null)
					{
						type = GenerateFieldIL(field, il);
					}
					else
					{
						throw new ValidationException(string.Format(
							"No field or property {0} found for type {1}",
							fieldOrProperty, type.FullName));
					}
				}
			}

			return type;
		}

		private static bool IsReflectionEmitAllowed()
		{
			return SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.ReflectionEmit));
		}

		private static Type GeneratePropertyIL(PropertyInfo property, ILGenerator il)
		{
			il.Emit(OpCodes.Callvirt, property.GetGetMethod());
			return property.PropertyType;
		}

		private static Type GenerateFieldIL(FieldInfo field, ILGenerator il)
		{
			il.Emit(OpCodes.Ldfld, field);
			return field.FieldType;
		}

		private static readonly BindingFlags PublicBinding = 
			BindingFlags.Instance | BindingFlags.Public;
	}
}
