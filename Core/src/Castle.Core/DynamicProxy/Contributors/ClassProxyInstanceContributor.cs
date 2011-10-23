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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	public class ClassProxyInstanceContributor : ProxyInstanceContributor
	{
#if !SILVERLIGHT
		private readonly bool delegateToBaseGetObjectData;
		private readonly bool implementISerializable;
		private ConstructorInfo serializationConstructor;
		private readonly IList<FieldReference> serializedFields = new List<FieldReference>();
#endif

		public ClassProxyInstanceContributor(Type targetType, IList<MethodInfo> methodsToSkip, Type[] interfaces,
		                                     string typeId)
			: base(targetType, interfaces, typeId)
		{
#if !SILVERLIGHT
			if (targetType.IsSerializable)
			{
				implementISerializable = true;
				delegateToBaseGetObjectData = VerifyIfBaseImplementsGetObjectData(targetType, methodsToSkip);
			}
#endif
		}

		protected override Expression GetTargetReferenceExpression(ClassEmitter emitter)
		{
			return SelfReference.Self.ToExpression();
		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			var interceptors = @class.GetField("__interceptors");
#if !SILVERLIGHT
			if (implementISerializable)
			{
				ImplementGetObjectData(@class);
				Constructor(@class);
			}
#endif
			ImplementProxyTargetAccessor(@class, interceptors);
			foreach (var attribute in targetType.GetNonInheritableAttributes())
			{
				@class.DefineCustomAttribute(attribute);
			}
		}

#if !SILVERLIGHT

		protected override void AddAddValueInvocation(ArgumentReference serializationInfo, MethodEmitter getObjectData,
		                                              FieldReference field)
		{
			serializedFields.Add(field);
			base.AddAddValueInvocation(serializationInfo, getObjectData, field);
		}

		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo,
		                                               ArgumentReference streamingContext, ClassEmitter emitter)
		{
			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(
			                         		serializationInfo,
			                         		SerializationInfoMethods.AddValue_Bool,
			                         		new ConstReference("__delegateToBase").ToExpression(),
			                         		new ConstReference(delegateToBaseGetObjectData).
			                         			ToExpression())));

			if (delegateToBaseGetObjectData == false)
			{
				EmitCustomGetObjectData(codebuilder, serializationInfo);
				return;
			}

			EmitCallToBaseGetObjectData(codebuilder, serializationInfo, streamingContext);
		}

		private void EmitCustomGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo)
		{
			var members = codebuilder.DeclareLocal(typeof(MemberInfo[]));
			var data = codebuilder.DeclareLocal(typeof(object[]));

			var getSerializableMembers = new MethodInvocationExpression(
				null,
				FormatterServicesMethods.GetSerializableMembers,
				new TypeTokenExpression(targetType));
			codebuilder.AddStatement(new AssignStatement(members, getSerializableMembers));

			// Sort to keep order on both serialize and deserialize side the same, c.f DYNPROXY-ISSUE-127
			var callSort = new MethodInvocationExpression(
				null,
				TypeUtilMethods.Sort,
				members.ToExpression());
			codebuilder.AddStatement(new AssignStatement(members, callSort));

			var getObjectData = new MethodInvocationExpression(
				null,
				FormatterServicesMethods.GetObjectData,
				SelfReference.Self.ToExpression(),
				members.ToExpression());
			codebuilder.AddStatement(new AssignStatement(data, getObjectData));

			var addValue = new MethodInvocationExpression(
				serializationInfo,
				SerializationInfoMethods.AddValue_Object,
				new ConstReference("__data").ToExpression(),
				data.ToExpression());
			codebuilder.AddStatement(new ExpressionStatement(addValue));
		}

		private void EmitCallToBaseGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo,
		                                         ArgumentReference streamingContext)
		{
			var baseGetObjectData = targetType.GetMethod("GetObjectData",
			                                             new[] { typeof(SerializationInfo), typeof(StreamingContext) });

			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(baseGetObjectData,
			                         	                               serializationInfo.ToExpression(),
			                         	                               streamingContext.ToExpression())));
		}

		private void Constructor(ClassEmitter emitter)
		{
			if (!delegateToBaseGetObjectData)
			{
				return;
			}
			GenerateSerializationConstructor(emitter);
		}

		private void GenerateSerializationConstructor(ClassEmitter emitter)
		{
			var serializationInfo = new ArgumentReference(typeof(SerializationInfo));
			var streamingContext = new ArgumentReference(typeof(StreamingContext));

			var ctor = emitter.CreateConstructor(serializationInfo, streamingContext);

			ctor.CodeBuilder.AddStatement(
				new ConstructorInvocationStatement(serializationConstructor,
				                                   serializationInfo.ToExpression(),
				                                   streamingContext.ToExpression()));

			foreach (var field in serializedFields)
			{
				var getValue = new MethodInvocationExpression(serializationInfo,
				                                              SerializationInfoMethods.GetValue,
				                                              new ConstReference(field.Reference.Name).ToExpression(),
				                                              new TypeTokenExpression(field.Reference.FieldType));
				ctor.CodeBuilder.AddStatement(new AssignStatement(
				                              	field,
				                              	new ConvertExpression(field.Reference.FieldType,
				                              	                      typeof(object),
				                              	                      getValue)));
			}
			ctor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private bool VerifyIfBaseImplementsGetObjectData(Type baseType, IList<MethodInfo> methodsToSkip)
		{
			if (!typeof(ISerializable).IsAssignableFrom(baseType))
			{
				return false;
			}

			if (IsDelegate(baseType))
			{
				//working around bug in CLR which returns true for "does this type implement ISerializable" for delegates
				return false;
			}

			// If base type implements ISerializable, we have to make sure
			// the GetObjectData is marked as virtual
			var getObjectDataMethod = baseType.GetInterfaceMap(typeof(ISerializable)).TargetMethods[0];
			if (getObjectDataMethod.IsPrivate) //explicit interface implementation
			{
				return false;
			}

			if (!getObjectDataMethod.IsVirtual || getObjectDataMethod.IsFinal)
			{
				var message = String.Format("The type {0} implements ISerializable, but GetObjectData is not marked as virtual. " +
				                            "Dynamic Proxy needs types implementing ISerializable to mark GetObjectData as virtual " +
				                            "to ensure correct serialization process.",
				                            baseType.FullName);
				throw new ArgumentException(message);
			}

			methodsToSkip.Add(getObjectDataMethod);

			serializationConstructor = baseType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public |
				BindingFlags.NonPublic,
				null,
				new[] { typeof(SerializationInfo), typeof(StreamingContext) },
				null);

			if (serializationConstructor == null)
			{
				var message = String.Format("The type {0} implements ISerializable, " +
				                            "but failed to provide a deserialization constructor",
				                            baseType.FullName);
				throw new ArgumentException(message);
			}

			return true;
		}

		private bool IsDelegate(Type baseType)
		{
			return baseType.BaseType == typeof(MulticastDelegate);
		}
#endif
	}
}