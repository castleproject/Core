// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Encapsulates the information needed to build an attribute.
	/// </summary>
	/// <remarks>
	/// Arrays passed to this class as constructor arguments or property or field values become owned by this class.
	/// They should not be mutated after creation.
	/// </remarks>
	public class CustomAttributeInfo : IEquatable<CustomAttributeInfo>
	{
		// Cached empty arrays to avoid unnecessary allocations
		private static readonly PropertyInfo[] EmptyProperties = new PropertyInfo[0];
		private static readonly FieldInfo[] EmptyFields = new FieldInfo[0];
		private static readonly object[] EmptyValues = new object[0];

		private static readonly IEqualityComparer<object> ValueComparer = new AttributeArgumentValueEqualityComparer();

		private readonly CustomAttributeBuilder builder;
		private readonly ConstructorInfo constructor;
		private readonly object[] constructorArgs;
		private readonly IDictionary<string, object> properties;
		private readonly IDictionary<string, object> fields;

		public CustomAttributeInfo(
			ConstructorInfo constructor,
			object[] constructorArgs,
			PropertyInfo[] namedProperties,
			object[] propertyValues,
			FieldInfo[] namedFields,
			object[] fieldValues)
		{
			// Will take care of validating the arguments
			this.builder = new CustomAttributeBuilder(constructor, constructorArgs, namedProperties, propertyValues, namedFields, fieldValues);

			this.constructor = constructor;
			this.constructorArgs = constructorArgs.Length == 0 ? EmptyValues : constructorArgs.ToArray();
			this.properties = MakeNameValueDictionary(namedProperties, propertyValues);
			this.fields = MakeNameValueDictionary(namedFields, fieldValues);
		}

		public CustomAttributeInfo(
			ConstructorInfo constructor,
			object[] constructorArgs,
			PropertyInfo[] namedProperties,
			object[] propertyValues)
			: this(constructor, constructorArgs, namedProperties, propertyValues, EmptyFields, EmptyValues)
		{			
		}

		public CustomAttributeInfo(
			ConstructorInfo constructor,
			object[] constructorArgs,
			FieldInfo[] namedFields,
			object[] fieldValues)
			: this(constructor, constructorArgs, EmptyProperties, EmptyValues, namedFields, fieldValues)
		{
		}

		public CustomAttributeInfo(
			ConstructorInfo constructor,
			object[] constructorArgs)
			: this(constructor, constructorArgs, EmptyProperties, EmptyValues, EmptyFields, EmptyValues)
		{
		}

		public static CustomAttributeInfo FromExpression(Expression<Func<Attribute>> expression)
		{
			var namedProperties = new List<PropertyInfo>();
			var propertyValues = new List<object>();
			var namedFields = new List<FieldInfo>();
			var fieldValues = new List<object>();

			var body = UnwrapBody(expression.Body);

			var newExpression = body as NewExpression;
			if (newExpression == null)
			{
				var memberInitExpression = body as MemberInitExpression;
				if (memberInitExpression == null)
				{
					throw new ArgumentException("The expression must be either a simple constructor call or an object initializer expression");
				}

				newExpression = memberInitExpression.NewExpression;

				foreach (var binding in memberInitExpression.Bindings)
				{
					var assignment = binding as MemberAssignment;
					if (assignment == null)
					{
						throw new ArgumentException("Only assignment bindings are supported");
					}

					object value = GetAttributeArgumentValue(assignment.Expression, allowArray: true);

					var property = assignment.Member as PropertyInfo;
					if (property != null)
					{
						namedProperties.Add(property);
						propertyValues.Add(value);
					}
					else
					{
						var field = assignment.Member as FieldInfo;
						if (field != null)
						{
							namedFields.Add(field);
							fieldValues.Add(value);
						}
						else
						{
							throw new ArgumentException("Only property and field assignments are supported");
						}
					}
				}
			}

			var ctorArguments = new List<object>();
			foreach (var arg in newExpression.Arguments)
			{
				object value = GetAttributeArgumentValue(arg, allowArray: true);
				ctorArguments.Add(value);
			}

			return new CustomAttributeInfo(
				newExpression.Constructor,
				ctorArguments.ToArray(),
				namedProperties.ToArray(),
				propertyValues.ToArray(),
				namedFields.ToArray(),
				fieldValues.ToArray());
		}

		private static Expression UnwrapBody(Expression body)
		{
			// In VB.NET, a lambda expression like `Function() New MyAttribute()` introduces
			// a conversion to the return type. We need to remove this conversion expression
			// to get the actual constructor call.

			var convertExpression = body as UnaryExpression;
			if (convertExpression != null && convertExpression.NodeType == ExpressionType.Convert)
			{
				return convertExpression.Operand;
			}
			return body;
		}

		private static object GetAttributeArgumentValue(Expression arg, bool allowArray)
		{
			switch (arg.NodeType)
			{
				case ExpressionType.Constant:
					return ((ConstantExpression)arg).Value;
				case ExpressionType.MemberAccess:
					var memberExpr = (MemberExpression) arg;
					if (memberExpr.Member is FieldInfo field)
					{
						if (memberExpr.Expression is ConstantExpression constant &&
						    IsCompilerGenerated(constant.Type) &&
						    constant.Value != null)
						{
							return field.GetValue(constant.Value);
						}
					}
					break;
				case ExpressionType.NewArrayInit:
					if (allowArray)
					{
						var newArrayExpr = (NewArrayExpression) arg;
						var array = Array.CreateInstance(newArrayExpr.Type.GetElementType(), newArrayExpr.Expressions.Count);
						int index = 0;
						foreach (var expr in newArrayExpr.Expressions)
						{
							object value = GetAttributeArgumentValue(expr, allowArray: false);
							array.SetValue(value, index);
							index++;
						}
						return array;
					}
					break;
			}
			
			throw new ArgumentException("Only constant, local variables, method parameters and single-dimensional array expressions are supported");
		}

		private static bool IsCompilerGenerated(Type type)
		{
			return type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute));
		}

		internal CustomAttributeBuilder Builder
		{
			get { return builder; }
		}

		public bool Equals(CustomAttributeInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return constructor.Equals(other.constructor) &&
				constructorArgs.SequenceEqual(other.constructorArgs, ValueComparer) &&
				AreMembersEquivalent(properties, other.properties) &&
				AreMembersEquivalent(fields, other.fields);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CustomAttributeInfo)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = constructor.GetHashCode();
				hashCode = (hashCode*397) ^ CombineHashCodes(constructorArgs);
				hashCode = (hashCode*397) ^ CombineMemberHashCodes(properties);
				hashCode = (hashCode*397) ^ CombineMemberHashCodes(fields);
				return hashCode;
			}
		}

		private static bool AreMembersEquivalent(IDictionary<string, object> x, IDictionary<string, object> y)
		{
			if (x.Count != y.Count)
				return false;

			foreach (var kvp in x)
			{
				object value;
				if (!y.TryGetValue(kvp.Key, out value))
					return false;
				if (!ValueComparer.Equals(kvp.Value, value))
					return false;
			}
			return true;
		}

		private static int CombineHashCodes(IEnumerable<object> values)
		{
			unchecked
			{
				int hashCode = 173;
				foreach (object value in values)
				{
					hashCode = (hashCode*397) ^ ValueComparer.GetHashCode(value);
				}
				return hashCode;
			}
		}

		private static int CombineMemberHashCodes(IDictionary<string, object> dict)
		{
			unchecked
			{
				// Just sum the hashcodes of all key-value pairs, because
				// we don't want to take order into account.

				int hashCode = 0;
				foreach (var kvp in dict)
				{
					int keyHashCode = kvp.Key.GetHashCode();
					int valueHashCode = ValueComparer.GetHashCode(kvp.Value);
					hashCode += (keyHashCode*397) ^ valueHashCode;
				}
				return hashCode;
			}
		}

		private IDictionary<string, object> MakeNameValueDictionary<T>(T[] members, object[] values)
			where T : MemberInfo
		{
			var dict = new Dictionary<string, object>();
			for (int i = 0; i < members.Length; i++)
			{
				dict.Add(members[i].Name, values[i]);
			}
			return dict;
		}

		private class AttributeArgumentValueEqualityComparer : IEqualityComparer<object>
		{
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				if (ReferenceEquals(x, y))
					return true;
				if (x == null || y == null)
					return false;
				if (x.GetType() != y.GetType())
					return false;

				if (x.GetType().IsArray)
				{
					return AsObjectEnumerable(x).SequenceEqual(AsObjectEnumerable(y));
				}

				return x.Equals(y);
			}

			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				if (obj == null)
					return 0;
				if (obj.GetType().IsArray)
				{
					return CombineHashCodes(AsObjectEnumerable(obj));
				}
				return obj.GetHashCode();
			}

			private static IEnumerable<object> AsObjectEnumerable(object array)
			{
				// Covariance doesn't work for value types
				if (array.GetType().GetElementType().GetTypeInfo().IsValueType)
					return ((Array)array).Cast<object>();

				return (IEnumerable<object>)array;
			}
		}
	}
}
