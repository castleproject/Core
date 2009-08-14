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
	using System.Reflection;
	using System.Runtime.Serialization;
	using Core.Interceptor;
	using Generators.Emitters;
	using Generators.Emitters.CodeBuilders;
	using Generators.Emitters.SimpleAST;
	using Tokens;

	public class ClassProxyInstanceContributor : ProxyInstanceContributor
	{
		private readonly bool delegateToBaseGetObjectData;
		// TODO:this maybe should be changed to polymorphism...
		private readonly bool implementISerializable;
		private ConstructorInfo serializationConstructor;

		public ClassProxyInstanceContributor(Type targetType, IList<MethodInfo> methodsToSkip)
			: base(targetType)
		{
			// TODO: the methodsToSkip is temporary, until I refactor it further
#if !SILVERLIGHT
			if (targetType.IsSerializable)
			{
				implementISerializable = true;
				delegateToBaseGetObjectData = VerifyIfBaseImplementsGetObjectData(targetType, methodsToSkip);
			}
#endif
		}

		protected override Expression TargetReferenceExpression
		{
			get { return SelfReference.Self.ToExpression(); }
		}

		public override void Generate(ClassEmitter emitter, FieldReference interceptors, FieldReference[] mixins, Type[] interfaces)
		{
#if !SILVERLIGHT
			Constructor(emitter, interceptors, mixins);

			if (implementISerializable)
			{
				ImplementGetObjectData(emitter, interceptors, mixins, interfaces);
			}
#endif
			ImplementProxyTargetAccessor(emitter, interceptors);
		}
#if !SILVERLIGHT
		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo, ArgumentReference streamingContext)
		{
			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Bool,
			                         	                               new ConstReference("__delegateToBase").ToExpression(),
			                         	                               new ConstReference(delegateToBaseGetObjectData ? 1 : 0).
			                         	                               	ToExpression())));

			if (delegateToBaseGetObjectData)
			{
				MethodInfo baseGetObjectData = targetType.GetMethod("GetObjectData",
				                                                    new[] {typeof (SerializationInfo), typeof (StreamingContext)});

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(baseGetObjectData,
				                         	                               serializationInfo.ToExpression(),
				                         	                               streamingContext.ToExpression())));
			}
			else
			{
				LocalReference members_ref = codebuilder.DeclareLocal(typeof (MemberInfo[]));
				LocalReference data_ref = codebuilder.DeclareLocal(typeof (object[]));

				codebuilder.AddStatement(new AssignStatement(members_ref,
				                                             new MethodInvocationExpression(null,
				                                                                            FormatterServicesMethods.
				                                                                            	GetSerializableMembers,
				                                                                            new TypeTokenExpression(targetType))));

				codebuilder.AddStatement(new AssignStatement(data_ref,
				                                             new MethodInvocationExpression(null,
				                                                                            FormatterServicesMethods.GetObjectData,
				                                                                            SelfReference.Self.ToExpression(),
				                                                                            members_ref.ToExpression())));

				codebuilder.AddStatement(new ExpressionStatement(
				                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Object,
				                         	                               new ConstReference("__data").ToExpression(),
				                         	                               data_ref.ToExpression())));
			}
		}

		private void Constructor(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields)
		{
#if !SILVERLIGHT
			if (!delegateToBaseGetObjectData) return;
			GenerateSerializationConstructor(emitter, interceptorsField, mixinFields);
#endif
		}

		private void GenerateSerializationConstructor(ClassEmitter emitter, FieldReference interceptorField, FieldReference[] mixinFields)
		{
			var serializationInfo = new ArgumentReference(typeof(SerializationInfo));
			var streamingContext = new ArgumentReference(typeof(StreamingContext));

			var ctor = emitter.CreateConstructor(serializationInfo, streamingContext);

			ctor.CodeBuilder.AddStatement(
				new ConstructorInvocationStatement(serializationConstructor,
				                                   serializationInfo.ToExpression(), streamingContext.ToExpression()));

			MethodInvocationExpression getInterceptorInvocation =
				new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.GetValue,
				                               new ConstReference("__interceptors").ToExpression(),
				                               new TypeTokenExpression(typeof(IInterceptor[])));

			ctor.CodeBuilder.AddStatement(new AssignStatement(
			                              	interceptorField,
			                              	new ConvertExpression(typeof(IInterceptor[]), typeof(object),
			                              	                      getInterceptorInvocation)));

			// mixins
			foreach (FieldReference mixinFieldReference in mixinFields)
			{
				MethodInvocationExpression getMixinInvocation =
					new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.GetValue,
					                               new ConstReference(mixinFieldReference.Reference.Name).ToExpression(),
					                               new TypeTokenExpression(mixinFieldReference.Reference.FieldType));

				ctor.CodeBuilder.AddStatement(new AssignStatement(
				                              	mixinFieldReference,
				                              	new ConvertExpression(mixinFieldReference.Reference.FieldType, typeof(object),
				                              	                      getMixinInvocation)));
			}
			ctor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private bool VerifyIfBaseImplementsGetObjectData(Type baseType, IList<MethodInfo> methodsToSkip)
		{
			// If base type implements ISerializable, we have to make sure
			// the GetObjectData is marked as virtual
			if (typeof(ISerializable).IsAssignableFrom(baseType))
			{
				MethodInfo getObjectDataMethod = baseType.GetMethod("GetObjectData",
				                                                    new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

				if (getObjectDataMethod == null) //explicit interface implementation
				{
					return false;
				}

				if (!getObjectDataMethod.IsVirtual || getObjectDataMethod.IsFinal)
				{
					String message = String.Format("The type {0} implements ISerializable, but GetObjectData is not marked as virtual",
					                               baseType.FullName);
					throw new ArgumentException(message);
				}

				// TODO: This should be removed
				methodsToSkip.Add(getObjectDataMethod);

				serializationConstructor = baseType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
					null,
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) },
					null);

				if (serializationConstructor == null)
				{
					String message =
						String.Format("The type {0} implements ISerializable, but failed to provide a deserialization constructor",
						              baseType.FullName);
					throw new ArgumentException(message);
				}

				return true;
			}
			return false;
		}
#endif
	}
}