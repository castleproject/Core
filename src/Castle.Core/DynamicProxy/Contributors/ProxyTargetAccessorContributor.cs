// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	///   Adds an implementation for <see cref="IProxyTargetAccessor"/> to the proxy type.
	/// </summary>
	internal sealed class ProxyTargetAccessorContributor : ITypeContributor
	{
		private readonly Func<Reference> getTargetReference;
		private readonly Type targetType;

		public ProxyTargetAccessorContributor(Func<Reference> getTargetReference, Type targetType)
		{
			this.getTargetReference = getTargetReference;
			this.targetType = targetType;
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
		}

		public void Generate(ClassEmitter emitter)
		{
			var interceptorsField = emitter.GetField("__interceptors");
			var targetReference = getTargetReference();

			var dynProxyGetTarget = emitter.CreateMethod(nameof(IProxyTargetAccessor.DynProxyGetTarget), typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, targetReference.ToExpression())));

			var dynProxySetTarget = emitter.CreateMethod(nameof(IProxyTargetAccessor.DynProxySetTarget), typeof(void), typeof(object));

			// we can only change the target of the interface proxy
			if (targetReference is FieldReference targetField)
			{
				dynProxySetTarget.CodeBuilder.AddStatement(
					new AssignStatement(targetField,
						new ConvertExpression(targetField.Fieldbuilder.FieldType, dynProxySetTarget.Arguments[0].ToExpression())));
			}
			else
			{
				dynProxySetTarget.CodeBuilder.AddStatement(
					new ThrowStatement(typeof(InvalidOperationException), "Cannot change the target of the class proxy."));
			}

			dynProxySetTarget.CodeBuilder.AddStatement(new ReturnStatement());

			var getInterceptors = emitter.CreateMethod(nameof(IProxyTargetAccessor.GetInterceptors), typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}
	}
}
