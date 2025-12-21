// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	internal class ConstructorEmitter
	{
		private readonly ConstructorBuilder builder;
		private readonly CodeBuilder codeBuilder;
		private readonly ClassEmitter mainType;

		protected internal ConstructorEmitter(ClassEmitter mainType, ConstructorBuilder builder)
		{
			this.mainType = mainType;
			this.builder = builder;
			codeBuilder = new CodeBuilder();
		}

		internal ConstructorEmitter(ClassEmitter mainType, params ArgumentReference[] arguments)
		{
			this.mainType = mainType;

			var args = ArgumentsUtil.InitializeAndConvert(arguments);

			builder = mainType.TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, args);
			codeBuilder = new CodeBuilder();
		}

		public CodeBuilder CodeBuilder
		{
			get { return codeBuilder; }
		}

		public ConstructorBuilder ConstructorBuilder
		{
			get { return builder; }
		}

		private bool ImplementedByRuntime
		{
			get
			{
				var attributes = builder.MethodImplementationFlags;
				return (attributes & MethodImplAttributes.Runtime) != 0;
			}
		}

		public virtual void Generate()
		{
			if (ImplementedByRuntime)
			{
				return;
			}

			if (CodeBuilder.IsEmpty)
			{
				if (builder.IsStatic == false)
				{
					CodeBuilder.AddStatement(new ConstructorInvocationStatement(mainType.BaseType));
				}

				CodeBuilder.AddStatement(new ReturnStatement());
			}

			CodeBuilder.Generate(builder.GetILGenerator());
		}
	}
}