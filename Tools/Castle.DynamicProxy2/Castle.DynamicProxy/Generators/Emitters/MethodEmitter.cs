 // Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Reflection.Emit;
	
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;


	public class MethodEmitter : IMemberEmitter
	{
		protected MethodBuilder builder;
		protected ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;
		private AbstractTypeEmitter maintype;
		private Type[] actualGenParameters;

		protected internal MethodEmitter()
		{
		}
		
		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       ReturnReferenceExpression returnRef, params ArgumentReference[] arguments) :
		                       	this(maintype, name, MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, returnRef, arguments)
		{
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name, MethodAttributes attrs)
		{
			this.maintype = maintype;

			builder = maintype.TypeBuilder.DefineMethod(name, attrs);
		}
		
		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       MethodAttributes attrs, ReturnReferenceExpression returnRef, 
		                       params ArgumentReference[] arguments) : this(maintype, name, attrs)
		{
			SetReturnType(returnRef.Type);
			SetParameters(ArgumentsUtil.InitializeAndConvert(arguments));
		}
		
		public void SetReturnType(Type returnType)
		{
			builder.SetReturnType(returnType);
		}

		public Type[] ActualGenericParameters
		{
			get { return actualGenParameters; }
		}

		/// <summary>
		/// Inspect the base method for generic definitions
		/// and set the return type and the parameters
		/// accordingly
		/// </summary>
		/// <param name="baseMethod"></param>
		public void CopyParametersAndReturnTypeFrom(MethodInfo baseMethod)
		{
			Type[] methodGenericArgs = baseMethod.GetGenericArguments();
			
			String[] names = new string[methodGenericArgs.Length];

			for (int i = 0; i < names.Length; i++)
			{
				names[i] = methodGenericArgs[i].Name;
			}

			actualGenParameters = new Type[0];

			if (methodGenericArgs.Length != 0)
			{
				actualGenParameters = builder.DefineGenericParameters(names);
			}

			// TODO: check if the return type is a generic
			// definition for the method
			SetReturnType(baseMethod.ReturnType);
			
			ParameterInfo[] baseMethodParameters = baseMethod.GetParameters();

			Type[] newParameters = new Type[baseMethodParameters.Length];

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				ParameterInfo param = baseMethodParameters[i];

				if (param.ParameterType.IsGenericParameter)
				{
					foreach (Type genParam in actualGenParameters)
					{
						if (genParam.Name == param.ParameterType.Name)
						{
							newParameters[i] = genParam;
							break;
						}
					}
				}

				if (newParameters[i] == null)
				{
					newParameters[i] = param.ParameterType;
				}
			}

			SetParameters(newParameters);

			DefineParameters(baseMethodParameters);
		}
		
		public void SetParameters(Type[] paramTypes)
		{
			builder.SetParameters(paramTypes);

			arguments = new ArgumentReference[paramTypes.Length];
			
			for(int i=0; i < paramTypes.Length; i++)
			{
				arguments[i] = new ArgumentReference(paramTypes[i]);
			}
			
			ArgumentsUtil.InitializeArgumentsByPosition(arguments);
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

		internal MethodBuilder MethodBuilder
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

		public void DefineParameters(ParameterInfo[] info)
		{
			foreach(ParameterInfo parameterInfo in info)
			{
				builder.DefineParameter(parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);
			}
		}
	}
}