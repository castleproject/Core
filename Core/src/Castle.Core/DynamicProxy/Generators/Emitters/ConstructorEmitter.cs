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
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class ConstructorEmitter : IMemberEmitter
	{
		private readonly ConstructorBuilder builder;
		private readonly AbstractTypeEmitter maintype;

		private ConstructorCodeBuilder constructorCodeBuilder;

		protected internal ConstructorEmitter(AbstractTypeEmitter maintype, ConstructorBuilder builder)
		{
			this.maintype = maintype;
			this.builder = builder;
		}

		internal ConstructorEmitter(AbstractTypeEmitter maintype, params ArgumentReference[] arguments)
		{
			this.maintype = maintype;

			var args = ArgumentsUtil.InitializeAndConvert(arguments);

			builder = maintype.TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, args);
		}

		public virtual ConstructorCodeBuilder CodeBuilder
		{
			get
			{
				if (constructorCodeBuilder == null)
				{
					constructorCodeBuilder = new ConstructorCodeBuilder(
						maintype.BaseType, builder.GetILGenerator());
				}
				return constructorCodeBuilder;
			}
		}

		public ConstructorBuilder ConstructorBuilder
		{
			get { return builder; }
		}

		public MemberInfo Member
		{
			get { return builder; }
		}

		public Type ReturnType
		{
			get { return typeof(void); }
		}

		private bool ImplementedByRuntime
		{
			get
			{
				var attributes = builder.GetMethodImplementationFlags();
				return (attributes & MethodImplAttributes.Runtime) != 0;
			}
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (ImplementedByRuntime == false && CodeBuilder.IsEmpty)
			{
				CodeBuilder.InvokeBaseConstructor();
				CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public virtual void Generate()
		{
			if (ImplementedByRuntime)
			{
				return;
			}

			CodeBuilder.Generate(this, builder.GetILGenerator());
		}
	}
}