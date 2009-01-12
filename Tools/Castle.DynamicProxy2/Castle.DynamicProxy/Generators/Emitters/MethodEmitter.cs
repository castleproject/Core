// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class MethodEmitter : IMemberEmitter
	{
		private readonly MethodBuilder builder;
		private readonly AbstractTypeEmitter maintype;

		private ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;
		private GenericTypeParameterBuilder[] genericTypeParams;

		private Dictionary<String, GenericTypeParameterBuilder> name2GenericType =
			new Dictionary<string, GenericTypeParameterBuilder>();

		protected internal MethodEmitter(MethodBuilder builder)
		{
			this.builder = builder;
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name, MethodAttributes attrs)
			: this(maintype.TypeBuilder.DefineMethod(name, attrs))
		{
			this.maintype = maintype;
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

			// Disabled due to .Net 3.5 SP 1 bug
//			List<Type[]> paramModReq = new List<Type[]>();
//			List<Type[]> paramModOpt = new List<Type[]>();
//			foreach (ParameterInfo parameterInfo in baseMethodParameters)
//			{
//				paramModOpt.Add(parameterInfo.GetOptionalCustomModifiers());
//				paramModReq.Add(parameterInfo.GetRequiredCustomModifiers());
//			} 

			Type[] genericArguments = baseMethod.GetGenericArguments();

			genericTypeParams = GenericUtil.DefineGenericArguments(genericArguments, builder, name2GenericType);
			// Bind parameter types

			SetParameters(GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType));

			// TODO: check if the return type is a generic
			// definition for the method

			SetReturnType(GenericUtil.ExtractCorrectType(baseMethod.ReturnType, name2GenericType));

#if SILVERLIGHT
#warning What to do?
#else
			builder.SetSignature(
				returnType,
				// Disabled due to .Net 3.5 SP 1 bug
				//baseMethod.ReturnParameter.GetRequiredCustomModifiers(),
				//baseMethod.ReturnParameter.GetOptionalCustomModifiers(),
				Type.EmptyTypes,
				Type.EmptyTypes,
				parameters,
				null, null
//				 paramModReq.ToArray(),
//				 paramModOpt.ToArray()
				);
#endif

			DefineParameters(baseMethodParameters);
		}

		public void SetParameters(Type[] paramTypes)
		{
			builder.SetParameters(paramTypes);

			arguments = new ArgumentReference[paramTypes.Length];

			for (int i = 0; i < paramTypes.Length; i++)
			{
				arguments[i] = new ArgumentReference(paramTypes[i]);
			}

			ArgumentsUtil.InitializeArgumentsByPosition(arguments, MethodBuilder.IsStatic);
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (codebuilder == null)
				{
					codebuilder = new MethodCodeBuilder(
						maintype.BaseType, builder, builder.GetILGenerator());
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

		public void DefineCustomAttribute(Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);

			if (customAttributeBuilder == null)
			{
				return;
			}

			builder.SetCustomAttribute(customAttributeBuilder);
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
