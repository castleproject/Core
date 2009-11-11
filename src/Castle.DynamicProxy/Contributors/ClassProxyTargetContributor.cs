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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using Generators;
	using Generators.Emitters;
	using Generators.Emitters.SimpleAST;

	public /* internal? */ class ClassProxyTargetContributor : ITypeContributor
	{
		private readonly Type targetType;
		private readonly IList<MethodInfo> methodsToSkip;
		private readonly IDictionary<Type, InterfaceMapping> interfaces = new Dictionary<Type, InterfaceMapping>();
		private readonly ICollection<MembersCollector> targets = new List<MembersCollector>();
		private readonly INamingScope namingScope;

		public ClassProxyTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope)
		{
			this.targetType = targetType;
			this.namingScope = namingScope;
			this.methodsToSkip = methodsToSkip;
		}

		public void AddInterfaceMapping(Type @interface)
		{
			// TODO: this method is likely to be moved to the interface
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.ContainsKey(@interface), "!interfaces.ContainsKey(@interface)", "Shouldn't be adding same interface twice...");
			Debug.Assert(@interface.IsAssignableFrom(targetType), "@interface.IsAssignableFrom(targetType)",
						 "Shouldn't be adding mapping to interface that target does not implement...");

			interfaces.Add(@interface, targetType.GetInterfaceMap(@interface));
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			var targetItem = new ClassMembersCollector(targetType, this);
			targetItem.CollectMembersToProxy(hook);
			targets.Add(targetItem);

			foreach (var mapping in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(mapping.Key, this, true, mapping.Value);
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}

		}

		public void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var target in targets)
			{
				foreach (var method in target.Methods)
				{
					if (!method.Standalone || methodsToSkip.Contains(method.Method))
					{
						continue;
					}

					ImplementMethod(method,
					                @class,
					                options,
					                @class.CreateMethod);
				}

				foreach (var property in target.Properties)
				{
					ImplementProperty(@class, property, options);
				}

				foreach (var @event in target.Events)
				{
					ImplementEvent(@class, @event, options);
				}
			}
		}

		private void ImplementEvent(ClassEmitter emitter, EventToGenerate @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(emitter);
			ImplementMethod(@event.Adder, emitter, options, @event.Emitter.CreateAddMethod);
			ImplementMethod(@event.Remover, emitter, options, @event.Emitter.CreateRemoveMethod);

		}

		private void ImplementProperty(ClassEmitter emitter, PropertyToGenerate property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				ImplementMethod(property.Getter, emitter, options,
				                (name, atts) => property.Emitter.CreateGetMethod(name, atts));
			}

			if (property.CanWrite)
			{
				ImplementMethod(property.Setter, emitter, options,
				                (name, atts) => property.Emitter.CreateSetMethod(name, atts));
				
			}
		}

		private void ImplementMethod(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			MethodGenerator generator;
			if (method.Proxyable)
			{
				var methodInfo = method.Method;
				var callback = default(MethodInfo);
				var targetForInvocation = targetType;
				if (!method.MethodOnTarget.IsAbstract && !IsExplicitInterfaceImplementation(method.MethodOnTarget))
				{
					// NOTE: factor this out as well.
					callback = CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
					targetForInvocation = callback.DeclaringType;
				}
				var invocation = new InvocationTypeGenerator(targetForInvocation, method, callback, false)
					.Generate(@class, options, namingScope);

				var interceptors = @class.GetField("__interceptors");
				generator = new MethodWithCallbackGenerator(method, invocation, interceptors, createMethod);
			}
			else
			{
				generator = new MinimialisticMethodGenerator(method, createMethod, GeneratorUtil.ObtainClassMethodAttributes);
			}
			var proxyMethod = generator.Generate(@class, options, namingScope);
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(method.Method))
			{
				proxyMethod.DefineCustomAttribute(attribute);
			}
		}

		private bool IsExplicitInterfaceImplementation(MethodInfo methodInfo)
		{
			return methodInfo.IsPrivate && methodInfo.IsFinal;
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget ?? methodInfo;

			// MethodBuild creation

			MethodAttributes attributes = MethodAttributes.Family;

			MethodEmitter callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"), attributes);

			callBackMethod.CopyParametersAndReturnTypeFrom(targetMethod, emitter);

			// Generic definition

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams);
			}

			// Parameters exp

			Expression[] exps = new Expression[callBackMethod.Arguments.Length];

			for (int i = 0; i < callBackMethod.Arguments.Length; i++)
			{
				exps[i] = callBackMethod.Arguments[i].ToExpression();
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(new MethodInvocationExpression(SelfReference.Self, targetMethod, exps)));

			return callBackMethod.MethodBuilder;
		}
	}
}