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
	using System.Reflection;
#if !SILVERLIGHT
	using System.Runtime.Serialization;
	using Serialization;
#endif
	using Core.Interceptor;
	using Generators.Emitters;
	using Generators.Emitters.CodeBuilders;
	using Generators.Emitters.SimpleAST;
	using Tokens;

	public abstract class ProxyInstanceContributor : ITypeContributor
	{
		// TODO: this whole type (and its descendants) should be #if !SILVERLIGHT... and empty type should be used instead for SL

		protected readonly Type targetType;

		protected ProxyInstanceContributor(Type targetType)
		{
			this.targetType = targetType;
		}

		public FieldReference ProxyGenerationOptions { get; set; }

		protected abstract Expression TargetReferenceExpression { get; }


		public virtual void Generate(ClassEmitter emitter, FieldReference interceptors, FieldReference[] mixins, Type[] interfaces)
		{
#if !SILVERLIGHT
			ImplementGetObjectData(emitter, interceptors, mixins, interfaces);
#endif
			ImplementProxyTargetAccessor(emitter, interceptors);
		}

		protected void ImplementProxyTargetAccessor(ClassEmitter emitter, FieldReference interceptorsField)
		{
			MethodAttributes attributes = MethodAttributes.Virtual | MethodAttributes.Public;

			MethodEmitter dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", attributes, typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, TargetReferenceExpression)));

			MethodEmitter getInterceptors = emitter.CreateMethod("GetInterceptors", attributes, typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

#if !SILVERLIGHT

		protected void ImplementGetObjectData(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			ArgumentReference serializationInfo = new ArgumentReference(typeof (SerializationInfo));
			ArgumentReference streamingContext = new ArgumentReference(typeof (StreamingContext));
			MethodEmitter getObjectData = emitter.CreateMethod("GetObjectData",
			                                                   typeof (void), serializationInfo, streamingContext);

			LocalReference typeLocal = getObjectData.CodeBuilder.DeclareLocal(typeof (Type));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					typeLocal,
					new MethodInvocationExpression(
						null,
						TypeMethods.StaticGetType,
						new ConstReference(typeof (ProxyObjectReference).AssemblyQualifiedName).ToExpression(),
						new ConstReference(1).ToExpression(),
						new ConstReference(0).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.SetType,
						typeLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__interceptors").ToExpression(),
						interceptorsField.ToExpression())));

			foreach (FieldReference mixinFieldReference in mixinFields)
			{
				getObjectData.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(
							serializationInfo,
							SerializationInfoMethods.AddValue_Object,
							new ConstReference(
								mixinFieldReference.Reference.Name).ToExpression(),
							mixinFieldReference.ToExpression())));
			}

			LocalReference interfacesLocal = getObjectData.CodeBuilder.DeclareLocal(typeof (string[]));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					interfacesLocal,
					new NewArrayExpression(interfaces.Length, typeof (string))));

			for (int i = 0; i < interfaces.Length; i++)
			{
				getObjectData.CodeBuilder.AddStatement(
					new AssignArrayStatement(
						interfacesLocal,
						i,
						new ConstReference(interfaces[i].AssemblyQualifiedName).ToExpression()));
			}

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__interfaces").ToExpression(),
						interfacesLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__baseType").ToExpression(),
						new ConstReference(emitter.BaseType.AssemblyQualifiedName).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__proxyGenerationOptions").ToExpression(),
						ProxyGenerationOptions.ToExpression())));

			CustomizeGetObjectData(getObjectData.CodeBuilder, serializationInfo, streamingContext);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected abstract void CustomizeGetObjectData(AbstractCodeBuilder builder, ArgumentReference serializationInfo, ArgumentReference streamingContext);
#endif
	}
}