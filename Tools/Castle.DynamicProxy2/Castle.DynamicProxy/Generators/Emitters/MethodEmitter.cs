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

		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       ReturnReferenceExpression returnRef, params ArgumentReference[] arguments) :
		                       	this(maintype, name,
		                       	     MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
		                       	     returnRef, arguments)
		{
		}

		internal MethodEmitter(AbstractTypeEmitter maintype, String name,
		                       MethodAttributes attrs,
		                       ReturnReferenceExpression returnRef, params ArgumentReference[] arguments)
		{
			this.maintype = maintype;
			this.arguments = arguments;

			Type returnType = returnRef.Type;
			Type[] args = ArgumentsUtil.InitializeAndConvert(arguments);

			this.builder = maintype.TypeBuilder.DefineMethod(name, attrs,
			                                             returnType, args);
		}

		protected internal MethodEmitter()
		{
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