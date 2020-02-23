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
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

	[DebuggerDisplay("{builder.Name}")]
	public class MethodEmitter : IMemberEmitter
	{
		private readonly MethodBuilder builder;
		private readonly GenericTypeParameterBuilder[] genericTypeParams;

		private ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;

		protected internal MethodEmitter(MethodBuilder builder)
		{
			this.builder = builder;
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name, MethodAttributes attributes)
			: this(owner.TypeBuilder.DefineMethod(name, attributes))
		{
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name,
		                       MethodAttributes attributes, Type returnType,
		                       params Type[] argumentTypes)
			: this(owner, name, attributes)
		{
			SetParameters(argumentTypes);
			SetReturnType(returnType);
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name,
		                       MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
			: this(owner, name, attributes)
		{
			var name2GenericType = GenericUtil.GetGenericArgumentsMap(owner);

			var returnType = GenericUtil.ExtractCorrectType(methodToUseAsATemplate.ReturnType, name2GenericType);
			var baseMethodParameters = methodToUseAsATemplate.GetParameters();
			var parameters = GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType);

			genericTypeParams = GenericUtil.CopyGenericArguments(methodToUseAsATemplate, builder, name2GenericType);
			SetParameters(parameters);
			SetReturnType(returnType);
			SetSignature(returnType, methodToUseAsATemplate.ReturnParameter, parameters, baseMethodParameters);
			DefineParameters(baseMethodParameters);
		}

		public ArgumentReference[] Arguments
		{
			get { return arguments; }
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (codebuilder == null)
				{
					codebuilder = new MethodCodeBuilder(builder.GetILGenerator());
				}
				return codebuilder;
			}
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		public MethodBuilder MethodBuilder
		{
			get { return builder; }
		}

		public MemberInfo Member
		{
			get { return builder; }
		}

		public Type ReturnType
		{
			get { return builder.ReturnType; }
		}

		private bool ImplementedByRuntime
		{
			get
			{
#if FEATURE_LEGACY_REFLECTION_API
				var attributes = builder.GetMethodImplementationFlags();
#else
				var attributes = builder.MethodImplementationFlags;
#endif
				return (attributes & MethodImplAttributes.Runtime) != 0;
			}
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			builder.SetCustomAttribute(attribute);
		}

		public void SetParameters(Type[] paramTypes)
		{
			builder.SetParameters(paramTypes);
			arguments = ArgumentsUtil.ConvertToArgumentReference(paramTypes);
			ArgumentsUtil.InitializeArgumentsByPosition(arguments, MethodBuilder.IsStatic);
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (ImplementedByRuntime == false && CodeBuilder.IsEmpty)
			{
				CodeBuilder.AddStatement(new NopStatement());
				CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public virtual void Generate()
		{
			if (ImplementedByRuntime)
			{
				return;
			}

			codebuilder.Generate(this, builder.GetILGenerator());
		}

		private void DefineParameters(ParameterInfo[] parameters)
		{
			foreach (var parameter in parameters)
			{
				var parameterBuilder = builder.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
				foreach (var attribute in parameter.GetNonInheritableAttributes())
				{
					parameterBuilder.SetCustomAttribute(attribute.Builder);
				}

				// If a parameter has a default value, that default value needs to be replicated.
				// Default values as reported by `ParameterInfo.[Raw]DefaultValue` have two possible origins:
				//
				// 1. A `[DecimalConstant]` or `[DateTimeConstant]` custom attribute attached to the parameter.
				//    Attribute-based default values have already been copied above.
				//    (Note that another attribute type, `[DefaultParameterValue]`, only appears in source
				//    code. The compiler replaces it with another metadata construct:)
				//
				// 2. A `Constant` metadata table entry whose parent is the parameter.
				//    Constant-based default values need more work. We can detect this case by checking
				//    whether the `ParameterAttributes.HasDefault` flag is set. (NB: This is not the same
				//    as querying `ParameterInfo.HasDefault`, which would also return true for case (1)!)
				if ((parameter.Attributes & ParameterAttributes.HasDefault) != 0)
				{
					try
					{
						CopyDefaultValueConstant(from: parameter, to: parameterBuilder);
					}
					catch
					{
						// Default value replication is a nice-to-have feature but not essential,
						// so if it goes wrong for one parameter, just continue.
					}
				}
			}
		}

		private void CopyDefaultValueConstant(ParameterInfo from, ParameterBuilder to)
		{
			Debug.Assert(from != null);
			Debug.Assert(to != null);
			Debug.Assert((from.Attributes & ParameterAttributes.HasDefault) != 0);

			object defaultValue;
			try
			{
				defaultValue = from.DefaultValue;
			}
			catch (FormatException) when (from.ParameterType == typeof(DateTime))
			{
				// This catch clause guards against a CLR bug that makes it impossible to query
				// the default value of an optional DateTime parameter. For the CoreCLR, see
				// https://github.com/dotnet/corefx/issues/26164.

				// If this bug is present, it is caused by a `null` default value:
				defaultValue = null;
			}
			catch (FormatException) when (from.ParameterType.GetTypeInfo().IsEnum)
			{
				// This catch clause guards against a CLR bug that makes it impossible to query
				// the default value of a (closed generic) enum parameter. For the CoreCLR, see
				// https://github.com/dotnet/corefx/issues/29570.

				// If this bug is present, it is caused by a `null` default value:
				defaultValue = null;
			}

			if (defaultValue is Missing)
			{
				// It is likely that we are reflecting over invalid metadata if we end up here.
				// At this point, `to.Attributes` will have the `HasDefault` flag set. If we do
				// not call `to.SetConstant`, that flag will be reset when creating the dynamic
				// type, so `to` will at least end up having valid metadata. It is quite likely
				// that the `Missing.Value` will still be reproduced because the `Parameter-
				// Builder`'s `ParameterAttributes.Optional` is likely set. (If it isn't set,
				// we'll be causing a default value of `DBNull.Value`, but there's nothing that
				// can be done about that, short of recreating a new `ParameterBuilder`.)
				return;
			}

			try
			{
				to.SetConstant(defaultValue);
			}
			catch (ArgumentException)
			{
				var parameterType = from.ParameterType;
				var parameterNonNullableType = parameterType;

				if (defaultValue == null)
				{
					if (parameterType.IsNullableType())
					{
						// This guards against a Mono bug that prohibits setting default value `null`
						// for a `Nullable<T>` parameter. See https://github.com/mono/mono/issues/8504.
						//
						// If this bug is present, luckily we still get `null` as the default value if
						// we do nothing more (which is probably itself yet another bug, as the CLR
						// would "produce" a default value of `Missing.Value` in this situation).
						return;
					}
					else if (parameterType.GetTypeInfo().IsValueType)
					{
						// This guards against a CLR bug that prohibits replicating `null` default
						// values for non-nullable value types (which, despite the apparent type
						// mismatch, is perfectly legal and something that the Roslyn compilers do).
						// For the CoreCLR, see https://github.com/dotnet/corefx/issues/26184.

						// If this bug is present, the best we can do is to not set the default value.
						// This will cause a default value of `Missing.Value` (if `ParameterAttributes-
						// .Optional` is set) or `DBNull.Value` (otherwise, unlikely).
						return;
					}
				}
				else if (parameterType.IsNullableType())
				{
					parameterNonNullableType = from.ParameterType.GetGenericArguments()[0];
					if (parameterNonNullableType.GetTypeInfo().IsEnum || parameterNonNullableType.IsAssignableFrom(defaultValue.GetType()))
					{
						// This guards against two bugs:
						//
						// * On the CLR and CoreCLR, a bug that makes it impossible to use `ParameterBuilder-
						//   .SetConstant` on parameters of a nullable enum type. For CoreCLR, see
						//   https://github.com/dotnet/coreclr/issues/17893.
						//
						//   If this bug is present, there is no way to faithfully reproduce the default
						//   value. This will most likely cause a default value of `Missing.Value` or
						//   `DBNull.Value`. (To better understand which of these, see comment above).
						//
						// * On Mono, a bug that performs a too-strict type check for nullable types. The
						//   value passed to `ParameterBuilder.SetConstant` must have a type matching that
						//   of the parameter precisely. See https://github.com/mono/mono/issues/8597.
						//
						//   If this bug is present, there's no way to reproduce the default value because
						//   we cannot actually create a value of type `Nullable<>`.
						return;
					}
				}

				// Finally, we might have got here because the metadata constant simply doesn't match
				// the parameter type exactly. Some code generators other than the .NET compilers
				// might produce such metadata. Make a final attempt to coerce it to the required type:
				try
				{
					var coercedDefaultValue = Convert.ChangeType(defaultValue, parameterNonNullableType, CultureInfo.InvariantCulture);
					to.SetConstant(coercedDefaultValue);

					return;
				}
				catch
				{
					// We don't care about the error thrown by an unsuccessful type coercion.
				}

				throw;
			}
		}

		private void SetReturnType(Type returnType)
		{
			builder.SetReturnType(returnType);
		}

		private void SetSignature(Type returnType, ParameterInfo returnParameter, Type[] parameters,
		                          ParameterInfo[] baseMethodParameters)
		{
			Type[] returnRequiredCustomModifiers;
			Type[] returnOptionalCustomModifiers;
			Type[][] parametersRequiredCustomModifiers;
			Type[][] parametersOptionalCustomModifiers;

#if FEATURE_CUSTOMMODIFIERS
			returnRequiredCustomModifiers = returnParameter.GetRequiredCustomModifiers();
			Array.Reverse(returnRequiredCustomModifiers);

			returnOptionalCustomModifiers = returnParameter.GetOptionalCustomModifiers();
			Array.Reverse(returnOptionalCustomModifiers);

			int parameterCount = baseMethodParameters.Length;
			parametersRequiredCustomModifiers = new Type[parameterCount][];
			parametersOptionalCustomModifiers = new Type[parameterCount][];
			for (int i = 0; i < parameterCount; ++i)
			{
				parametersRequiredCustomModifiers[i] = baseMethodParameters[i].GetRequiredCustomModifiers();
				Array.Reverse(parametersRequiredCustomModifiers[i]);

				parametersOptionalCustomModifiers[i] = baseMethodParameters[i].GetOptionalCustomModifiers();
				Array.Reverse(parametersOptionalCustomModifiers[i]);
			}
#else
			returnRequiredCustomModifiers = null;
			returnOptionalCustomModifiers = null;
			parametersRequiredCustomModifiers = null;
			parametersOptionalCustomModifiers = null;
#endif

			builder.SetSignature(
				returnType,
				returnRequiredCustomModifiers,
				returnOptionalCustomModifiers,
				parameters,
				parametersRequiredCustomModifiers,
				parametersOptionalCustomModifiers);
		}
	}
}