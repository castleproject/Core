// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

#nullable enable

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	internal sealed class RecordCloningContributor : ITypeContributor
	{
		private readonly Type targetType;
		private readonly INamingScope namingScope;

		private MethodInfo? baseCloneMethod;
		private bool overridable;
		private bool shouldIntercept;

		public RecordCloningContributor(Type targetType, INamingScope namingScope)
		{
			this.targetType = targetType;
			this.namingScope = namingScope;
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
			baseCloneMethod = targetType.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance);
			if (baseCloneMethod == null)
			{
				return;
			}

			var cloneMetaMethod = model.FindMethod(baseCloneMethod);
			if (cloneMetaMethod != null)
			{
				// The target contributor may have chosen to generate interception code for this method.
				// We override that decision here. This effectively renders `<Clone>$` uninterceptable,
				// in favor of some default behavior provided by DynamicProxy. This may be a bad idea.
				cloneMetaMethod.Ignore = true;
			}

			overridable = baseCloneMethod.IsVirtual && !baseCloneMethod.IsFinal;
			shouldIntercept = overridable && hook.ShouldInterceptMethod(targetType, baseCloneMethod);
		}

		public void Generate(ClassEmitter @class)
		{
			if (baseCloneMethod == null) return;

			ImplementCopyConstructor(@class, out var copyCtor);
			ImplementCloneMethod(@class, copyCtor);
		}

		private void ImplementCopyConstructor(ClassEmitter @class, out ConstructorInfo copyCtor)
		{
			var other = new ArgumentReference(@class.TypeBuilder);
			var copyCtorEmitter = @class.CreateConstructor(other);
			var baseCopyCtor = targetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, [targetType], null);

			copyCtorEmitter.CodeBuilder.AddStatement(
				new ConstructorInvocationStatement(
					baseCopyCtor,
					other));

			foreach (var field in @class.GetAllFields())
			{
				if (field.Reference.IsStatic) continue;

				copyCtorEmitter.CodeBuilder.AddStatement(
					new AssignStatement(
						field,
						new FieldReference(
							field.Reference,
							other)));
			}

			copyCtorEmitter.CodeBuilder.AddStatement(ReturnStatement.Instance);

			copyCtor = copyCtorEmitter.ConstructorBuilder;
		}

		private void ImplementCloneMethod(ClassEmitter @class, ConstructorInfo copyCtor)
		{
			if (shouldIntercept)
			{
				var cloneCallbackMethod = CreateCallbackMethod(@class, copyCtor);
				var cloneMetaMethod = new MetaMethod(baseCloneMethod!, cloneCallbackMethod, true, true, true);
				var invocationType = CreateInvocationType(@class, cloneMetaMethod, cloneCallbackMethod);

				var cloneMethodGenerator = new MethodWithInvocationGenerator(
					cloneMetaMethod,
					@class.GetField("__interceptors"),
					invocationType,
					(c, m) => new TypeTokenExpression(@class.TypeBuilder),
					@class.CreateMethod,
					null);

				cloneMethodGenerator.Generate(@class, namingScope);
			}
			else if (overridable)
			{
				var cloneMethodEmitter = @class.CreateMethod(
					name: baseCloneMethod!.Name,
					attrs: (baseCloneMethod.Attributes & MethodAttributes.MemberAccessMask) | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual,
					returnType: baseCloneMethod.ReturnType,  // no need to use covariant return type
					argumentTypes: []);

				cloneMethodEmitter.CodeBuilder.AddStatement(
					new ReturnStatement(
						new NewInstanceExpression(
							copyCtor,
							ThisExpression.Instance)));
			}
		}

		private MethodInfo CreateCallbackMethod(ClassEmitter @class, ConstructorInfo copyCtor)
		{
			var callbackMethod = @class.CreateMethod(
				name: baseCloneMethod!.Name + "_callback",
				attrs: MethodAttributes.Public | MethodAttributes.HideBySig,
				returnType: copyCtor.DeclaringType,
				argumentTypes: Type.EmptyTypes);

			callbackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new NewInstanceExpression(
						copyCtor,
						ThisExpression.Instance)));

			return callbackMethod.MethodBuilder;
		}

		private Type CreateInvocationType(ClassEmitter @class, MetaMethod cloneMetaMethod, MethodInfo cloneCallbackMethod)
		{
			var generator = new InheritanceInvocationTypeGenerator(
				targetType,
				cloneMetaMethod,
				cloneCallbackMethod,
				null);

			return generator.Generate(@class, namingScope).BuildType();
		}
	}
}
