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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Contributors
{
	using System;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	internal class InterfaceProxySerializableContributor : SerializableContributor
	{
		public InterfaceProxySerializableContributor(Type targetType, string proxyGeneratorId, Type[] interfaces)
			: base(targetType, interfaces, proxyGeneratorId)
		{
		}

		protected override void CustomizeGetObjectData(CodeBuilder codebuilder, ArgumentReference serializationInfo,
		                                               ArgumentReference streamingContext, ClassEmitter emitter)
		{
			var targetField = emitter.GetField("__target");

			codebuilder.AddStatement(
				new MethodInvocationExpression(
					serializationInfo,
					SerializationInfoMethods.AddValue_Object,
					new LiteralStringExpression("__targetFieldType"),
					new LiteralStringExpression(targetField.Reference.FieldType.AssemblyQualifiedName)));

			codebuilder.AddStatement(
				new MethodInvocationExpression(
					serializationInfo,
					SerializationInfoMethods.AddValue_Object,
					new LiteralStringExpression("__theInterface"),
					new LiteralStringExpression(targetType.AssemblyQualifiedName)));
		}
	}
}

#endif
