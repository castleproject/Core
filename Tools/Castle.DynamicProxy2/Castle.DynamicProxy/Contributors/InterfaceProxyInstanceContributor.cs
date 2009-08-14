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
	using Generators;
	using Generators.Emitters.CodeBuilders;
	using Generators.Emitters.SimpleAST;
	using Tokens;

	public class InterfaceProxyInstanceContributor : ProxyInstanceContributor
	{
		private readonly InterfaceGeneratorType generatorType;

		public FieldReference TargetField { private get; set; }

		protected override Expression TargetReferenceExpression
		{
			get { return TargetField.ToExpression(); }
		}

		public InterfaceProxyInstanceContributor(Type targetType, InterfaceGeneratorType generatorType) : base(targetType)
		{
			this.generatorType = generatorType;
		}
		
#if !SILVERLIGHT
		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo, ArgumentReference streamingContext)
		{
			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Object,
			                         	                               new ConstReference("__target").ToExpression(),
			                         	                               TargetField.ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Object,
			                         	                               new ConstReference("__targetFieldType").ToExpression(),
			                         	                               new ConstReference(
			                         	                               	TargetField.Reference.FieldType.AssemblyQualifiedName).
			                         	                               	ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Int32,
			                         	                               new ConstReference("__interface_generator_type").
			                         	                               	ToExpression(),
			                         	                               new ConstReference((int) generatorType).ToExpression())));

			codebuilder.AddStatement(new ExpressionStatement(
			                         	new MethodInvocationExpression(serializationInfo, SerializationInfoMethods.AddValue_Object,
			                         	                               new ConstReference("__theInterface").ToExpression(),
			                         	                               new ConstReference(targetType.AssemblyQualifiedName).
			                         	                               	ToExpression())));
		}
#endif
	}
}