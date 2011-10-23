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
#if !SILVERLIGHT
	using System.Runtime.Serialization;
#endif

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Serialization;
	using Castle.DynamicProxy.Tokens;

	public abstract class ProxyInstanceContributor : ITypeContributor
	{
		// TODO: this whole type (and its descendants) should be #if !SILVERLIGHT... and empty type should be used instead for SL

		protected readonly Type targetType;
		private readonly string proxyTypeId;
		private readonly Type[] interfaces;

		protected ProxyInstanceContributor(Type targetType, Type[] interfaces, string proxyTypeId)
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
			foreach (var attribute in targetType.GetNonInheritableAttributes())
			{
				@class.DefineCustomAttribute(attribute);
			}
		}

		protected void ImplementProxyTargetAccessor(ClassEmitter emitter, FieldReference interceptorsField)
		{
			var dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetTargetReferenceExpression(emitter))));

			var getInterceptors = emitter.CreateMethod("GetInterceptors", typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

#if !SILVERLIGHT

		protected void ImplementGetObjectData(ClassEmitter emitter)
		{
			var getObjectData = emitter.CreateMethod("GetObjectData", typeof(void),
			                                         new[] { typeof(SerializationInfo), typeof(StreamingContext) });
			var info = getObjectData.Arguments[0];

			var typeLocal = getObjectData.CodeBuilder.DeclareLocal(typeof(Type));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					typeLocal,
					new MethodInvocationExpression(
						null,
						TypeMethods.StaticGetType,
						new ConstReference(typeof(ProxyObjectReference).AssemblyQualifiedName).ToExpression(),
						new ConstReference(1).ToExpression(),
						new ConstReference(0).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						info,
						SerializationInfoMethods.SetType,
						typeLocal.ToExpression())));

			foreach (var field in emitter.GetAllFields())
			{
				if (field.Reference.IsStatic)
				{
					continue;
				}
				if (field.Reference.IsNotSerialized)
				{
					continue;
				}
				AddAddValueInvocation(info, getObjectData, field);
			}

			var interfacesLocal = getObjectData.CodeBuilder.DeclareLocal(typeof(string[]));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					interfacesLocal,
					new NewArrayExpression(interfaces.Length, typeof(string))));

			for (var i = 0; i < interfaces.Length; i++)
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
						info,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__interfaces").ToExpression(),
						interfacesLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						info,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__baseType").ToExpression(),
						new ConstReference(emitter.BaseType.AssemblyQualifiedName).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						info,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__proxyGenerationOptions").ToExpression(),
						emitter.GetField("proxyGenerationOptions").ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(info,
					                               SerializationInfoMethods.AddValue_Object,
					                               new ConstReference("__proxyTypeId").ToExpression(),
					                               new ConstReference(proxyTypeId).ToExpression())));

			CustomizeGetObjectData(getObjectData.CodeBuilder, info, getObjectData.Arguments[1], emitter);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected virtual void AddAddValueInvocation(ArgumentReference serializationInfo, MethodEmitter getObjectData,
		                                             FieldReference field)
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

		protected abstract void CustomizeGetObjectData(AbstractCodeBuilder builder, ArgumentReference serializationInfo,
		                                               ArgumentReference streamingContext, ClassEmitter emitter);
#endif

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
		}
	}
}