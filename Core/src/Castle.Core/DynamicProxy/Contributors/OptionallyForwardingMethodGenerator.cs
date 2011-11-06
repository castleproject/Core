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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class OptionallyForwardingMethodGenerator : MethodGenerator
	{
		// TODO: This class largely duplicates code from Forwarding and Minimalistic generators. Should be refactored to change that
		private readonly GetTargetReferenceDelegate getTargetReference;

		public OptionallyForwardingMethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod,
		                                           GetTargetReferenceDelegate getTargetReference)
			: base(method, overrideMethod)
		{
			this.getTargetReference = getTargetReference;
		}

		protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class,
		                                                        ProxyGenerationOptions options, INamingScope namingScope)
		{
			var targetReference = getTargetReference(@class, MethodToOverride);

			emitter.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new IfNullExpression(targetReference, IfNull(emitter.ReturnType), IfNotNull(targetReference))));
			return emitter;
		}

		private Expression IfNotNull(Reference targetReference)
		{
			var expression = new MultiStatementExpression();
			var arguments = ArgumentsUtil.ConvertToArgumentReferenceExpression(MethodToOverride.GetParameters());

			expression.AddStatement(new ReturnStatement(
			                        	new MethodInvocationExpression(
			                        		targetReference,
			                        		MethodToOverride,
			                        		arguments) { VirtualCall = true }));
			return expression;
		}

		private Expression IfNull(Type returnType)
		{
			var expression = new MultiStatementExpression();
			InitOutParameters(expression, MethodToOverride.GetParameters());

			if (returnType == typeof(void))
			{
				expression.AddStatement(new ReturnStatement());
			}
			else
			{
				expression.AddStatement(new ReturnStatement(new DefaultValueExpression(returnType)));
			}
			return expression;
		}

		private void InitOutParameters(MultiStatementExpression expression, ParameterInfo[] parameters)
		{
			for (var index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index];
				if (parameter.IsOut)
				{
					expression.AddStatement(
						new AssignArgumentStatement(new ArgumentReference(parameter.ParameterType, index + 1),
						                            new DefaultValueExpression(parameter.ParameterType)));
				}
			}
		}
	}
}