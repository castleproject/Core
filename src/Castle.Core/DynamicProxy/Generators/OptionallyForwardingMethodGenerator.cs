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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	internal class OptionallyForwardingMethodGenerator : MethodGenerator
	{
		// TODO: This class largely duplicates code from Forwarding and Minimalistic generators. Should be refactored to change that
		private readonly GetTargetExpressionDelegate getTarget;

		public OptionallyForwardingMethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod,
		                                           GetTargetExpressionDelegate getTarget)
			: base(method, overrideMethod)
		{
			this.getTarget = getTarget;
		}

		protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class,
		                                                        INamingScope namingScope)
		{
			var target = getTarget(@class, MethodToOverride);

			emitter.CodeBuilder.AddStatement(
				new IfNullExpression(
					target,
					IfNull(emitter.ReturnType),
					IfNotNull(target)));

			return emitter;
		}

		private IStatement IfNotNull(IExpression target)
		{
			var statements = new BlockStatement();
			var arguments = ArgumentsUtil.ConvertToArgumentReferenceExpression(MethodToOverride.GetParameters());

			statements.AddStatement(new ReturnStatement(
			                        	new MethodInvocationExpression(
			                        		target,
			                        		MethodToOverride,
			                        		arguments) { VirtualCall = true }));
			return statements;
		}

		private IStatement IfNull(Type returnType)
		{
			var statements = new BlockStatement();
			InitOutParameters(statements, MethodToOverride.GetParameters());

			if (returnType == typeof(void))
			{
				statements.AddStatement(new ReturnStatement());
			}
			else
			{
				statements.AddStatement(new ReturnStatement(new DefaultValueExpression(returnType)));
			}
			return statements;
		}

		private void InitOutParameters(BlockStatement statements, ParameterInfo[] parameters)
		{
			for (var index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index];
				if (parameter.IsOut)
				{
					statements.AddStatement(
						new AssignStatement(
							new IndirectReference(
								new ArgumentReference(parameter.ParameterType, index + 1)),
							new DefaultValueExpression(parameter.ParameterType.GetElementType())));
				}
			}
		}
	}
}