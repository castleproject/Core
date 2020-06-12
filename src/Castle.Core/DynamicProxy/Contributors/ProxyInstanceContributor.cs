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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

	internal abstract class ProxyInstanceContributor : ITypeContributor
	{
		protected readonly Type targetType;

		protected ProxyInstanceContributor(Type targetType)
		{
			this.targetType = targetType;
		}

		protected abstract Reference GetTargetReference(ClassEmitter emitter);

		private Expression GetTargetReferenceExpression(ClassEmitter emitter)
		{
			return GetTargetReference(emitter).ToExpression();
		}

		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			var interceptors = @class.GetField("__interceptors");
			ImplementProxyTargetAccessor(@class, interceptors);
			foreach (var attribute in targetType.GetNonInheritableAttributes())
			{
				@class.DefineCustomAttribute(attribute.Builder);
			}
		}

		protected void ImplementProxyTargetAccessor(ClassEmitter emitter, FieldReference interceptorsField)
		{
			var dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetTargetReferenceExpression(emitter))));

			var dynProxySetTarget = emitter.CreateMethod("DynProxySetTarget", typeof(void), typeof(object));

			// we can only change the target of the interface proxy
			var targetField = GetTargetReference(emitter) as FieldReference;
			if (targetField != null)
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

			var getInterceptors = emitter.CreateMethod("GetInterceptors", typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
		}
	}
}