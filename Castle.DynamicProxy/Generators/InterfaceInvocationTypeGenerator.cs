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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Emitters;
	using Emitters.SimpleAST;
	using Tokens;

	public class InterfaceInvocationTypeGenerator : InvocationTypeGenerator
	{
		public InterfaceInvocationTypeGenerator(Type target, IProxyMethod method, MethodInfo callback, bool canChangeTarget)
			: base(target, method, callback, canChangeTarget)
		{
		}

		protected override void ImplementInvokeMethodOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, MethodEmitter method, MethodInfo callbackMethod, Reference targetField)
		{
			method.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(SelfReference.Self, InvocationMethods.EnsureValidTarget)));
			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = TypeUtil.GetClosedParameterType(nested, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
					method.CodeBuilder.AddStatement(
						new AssignStatement(localReference,
						                    new ConvertExpression(paramType.GetElementType(),
						                                          new MethodInvocationExpression(SelfReference.Self,
						                                                                         InvocationMethods.GetArgumentValue,
						                                                                         new LiteralIntExpression(i)))));
					ByRefReference byRefReference = new ByRefReference(localReference);
					args[i] = new ReferenceExpression(byRefReference);
					byRefArguments[i] = localReference;
				}
				else
				{
					args[i] =
						new ConvertExpression(paramType,
						                      new MethodInvocationExpression(SelfReference.Self,
						                                                     InvocationMethods.GetArgumentValue,
						                                                     new LiteralIntExpression(i)));
				}
			}

			if (callbackMethod.IsGenericMethod)
			{
				callbackMethod = callbackMethod.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod));
			}

			var methodOnTargetInvocationExpression = new MethodInvocationExpression(
				new AsTypeReference(targetField, callbackMethod.DeclaringType),
				callbackMethod,
				args) { VirtualCall = true };

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				Type returnType = TypeUtil.GetClosedParameterType(nested, callbackMethod.ReturnType);
				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(new AssignStatement(returnValue, methodOnTargetInvocationExpression));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(methodOnTargetInvocationExpression));
			}

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = byRefArgument.Key;
				LocalReference localReference = byRefArgument.Value;
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(SelfReference.Self,
						                               InvocationMethods.SetArgumentValue,
						                               new LiteralIntExpression(index),
						                               new ConvertExpression(typeof(object), localReference.Type,
						                                                     new ReferenceExpression(localReference)))
						));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
					                               InvocationMethods.SetReturnValue,
					                               new ConvertExpression(typeof(object), returnValue.Type, returnValue.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}

	}
}