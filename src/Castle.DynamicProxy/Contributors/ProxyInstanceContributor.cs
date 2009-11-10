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
		private readonly string proxyTypeId;
		private readonly Type[] interfaces;

		protected ProxyInstanceContributor(Type targetType, Type[] interfaces,string proxyTypeId)
		{
			this.targetType = targetType;
			this.proxyTypeId = proxyTypeId;
			this.interfaces = interfaces ?? Type.EmptyTypes;
		}

		protected abstract Expression GetTargetReferenceExpression(ClassEmitter emitter);


		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			var interceptors = @class.GetField("__interceptors");
#if !SILVERLIGHT
			ImplementGetObjectData(@class);
#endif
			ImplementProxyTargetAccessor(@class, interceptors);
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(targetType))
			{
				@class.DefineCustomAttribute(attribute);
			}
		}

		protected void ImplementProxyTargetAccessor(ClassEmitter emitter, FieldReference interceptorsField)
		{
			MethodAttributes attributes = MethodAttributes.Virtual | MethodAttributes.Public;

			MethodEmitter dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", attributes, typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetTargetReferenceExpression(emitter))));

			MethodEmitter getInterceptors = emitter.CreateMethod("GetInterceptors", attributes, typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

#if !SILVERLIGHT

		protected void ImplementGetObjectData(ClassEmitter emitter)
		{

			var serializationInfo = new ArgumentReference(typeof (SerializationInfo));
			var streamingContext = new ArgumentReference(typeof (StreamingContext));
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

			foreach (var field in emitter.GetAllFields())
			{
				if (field.Reference.IsStatic) continue;
				if (field.Reference.IsNotSerialized) continue;
				AddAddValueInvocation(serializationInfo, getObjectData, field);
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
						emitter.GetField("proxyGenerationOptions").ToExpression())));



			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(serializationInfo,
					                               SerializationInfoMethods.AddValue_Object,
					                               new ConstReference("__proxyTypeId").ToExpression(),
					                               new ConstReference(proxyTypeId).ToExpression())));

			CustomizeGetObjectData(getObjectData.CodeBuilder, serializationInfo, streamingContext,emitter);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected virtual void AddAddValueInvocation(ArgumentReference serializationInfo, MethodEmitter getObjectData, FieldReference field)
		{
			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference(field.Reference.Name).ToExpression(),
						field.ToExpression())));
			return;
		}

		protected abstract void CustomizeGetObjectData(AbstractCodeBuilder builder, ArgumentReference serializationInfo, ArgumentReference streamingContext, ClassEmitter emitter);
#endif
		public void CollectElementsToProxy(IProxyGenerationHook hook)
		{
		}
	}
}