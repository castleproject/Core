// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	[DebuggerDisplay("{builder.Name}")]
	public class MethodEmitter : IMemberEmitter
	{
		private readonly MethodBuilder builder;

		private ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;
		private GenericTypeParameterBuilder[] genericTypeParams;

		private readonly Dictionary<String, GenericTypeParameterBuilder> name2GenericType =
			new Dictionary<string, GenericTypeParameterBuilder>();

		protected internal MethodEmitter(MethodBuilder builder)
		{
			this.builder = builder;
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name, MethodAttributes attrs)
			: this(maintype.TypeBuilder.DefineMethod(name, attrs))
		{
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       MethodAttributes attrs, Type returnType,
		                       params Type[] argumentTypes)
			: this(maintype, name, attrs)
		{
			SetParameters(argumentTypes);
			SetReturnType(returnType);
		}

		public void SetReturnType(Type returnType)
		{
			builder.SetReturnType(returnType);
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		/// <summary>
		/// Inspect the base method for generic definitions
		/// and set the return type and the parameters
		/// accordingly
		/// </summary>
		public void CopyParametersAndReturnTypeFrom(MethodInfo baseMethod, AbstractTypeEmitter parentEmitter)
		{
			GenericUtil.PopulateGenericArguments(parentEmitter, name2GenericType);
			Type returnType = GenericUtil.ExtractCorrectType(baseMethod.ReturnType, name2GenericType);
			ParameterInfo[] baseMethodParameters = baseMethod.GetParameters();
			Type[] parameters = GenericUtil.ExtractParametersTypes
				(baseMethodParameters, name2GenericType);

			//// Disabled for .NET due to .Net 3.5 SP 1 bug
			//List<Type[]> paramModReq = new List<Type[]>();
			//List<Type[]> paramModOpt = new List<Type[]>();
			//foreach (ParameterInfo parameterInfo in baseMethodParameters)
			//{
			//    paramModOpt.Add(parameterInfo.GetOptionalCustomModifiers());
			//    paramModReq.Add(parameterInfo.GetRequiredCustomModifiers());
			//} 

			genericTypeParams = GenericUtil.CopyGenericArguments(baseMethod, builder, name2GenericType);
			// Bind parameter types

			SetParameters(GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType));

			// TODO: check if the return type is a generic
			// definition for the method

			SetReturnType(GenericUtil.ExtractCorrectType(baseMethod.ReturnType, name2GenericType));

			builder.SetSignature(
				returnType,
				// Disabled due to .Net 3.5 SP 1 bug and Silverlight does not have this API
				//baseMethod.ReturnParameter.GetRequiredCustomModifiers(),
				//baseMethod.ReturnParameter.GetOptionalCustomModifiers(),
				Type.EmptyTypes,
				Type.EmptyTypes,
				parameters,
				null, null
//				 paramModReq.ToArray(),
//				 paramModOpt.ToArray()
				);


			DefineParameters(baseMethodParameters);
		}

		public void SetParameters(Type[] paramTypes)
		{
			builder.SetParameters(paramTypes);
			arguments = ArgumentsUtil.ConvertToArgumentReference(paramTypes);
			ArgumentsUtil.InitializeArgumentsByPosition(arguments, MethodBuilder.IsStatic);
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

		public ArgumentReference[] Arguments
		{
			get { return arguments; }
		}

		public MethodBuilder MethodBuilder
		{
			get { return builder; }
		}

		public Type ReturnType
		{
			get { return builder.ReturnType; }
		}

		public MemberInfo Member
		{
			get { return builder; }
		}

		public virtual void Generate()
		{
			codebuilder.Generate(this, builder.GetILGenerator());
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (CodeBuilder.IsEmpty)
			{
				CodeBuilder.AddStatement(new NopStatement());
				CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			builder.SetCustomAttribute(attribute);
		}

		private void DefineParameters(ParameterInfo[] info)
		{
			foreach (ParameterInfo parameterInfo in info)
			{
				builder.DefineParameter(parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);
			}
		}
	}
}
