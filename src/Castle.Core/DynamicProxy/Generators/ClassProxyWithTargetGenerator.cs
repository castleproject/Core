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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

#if FEATURE_SERIALIZATION
	using System.Xml.Serialization;
#endif

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Serialization;

	internal sealed class ClassProxyWithTargetGenerator : BaseClassProxyGenerator
	{
		private FieldReference targetField;

		public ClassProxyWithTargetGenerator(ModuleScope scope, Type targetType, Type[] interfaces,
		                                     ProxyGenerationOptions options)
			: base(scope, targetType, interfaces, options)
		{
		}

		protected override FieldReference TargetField => targetField;

		protected override CacheKey GetCacheKey()
		{
			return new CacheKey(targetType, targetType, interfaces, ProxyGenerationOptions);
		}

		protected override void CreateFields(ClassEmitter emitter)
		{
			base.CreateFields(emitter);
			CreateTargetField(emitter);
		}

#if FEATURE_SERIALIZATION
		protected override SerializableContributor GetSerializableContributor()
		{
			return new ClassProxySerializableContributor(targetType, interfaces, ProxyTypeConstants.ClassWithTarget);
		}
#endif

		protected override CompositeTypeContributor GetProxyTargetContributor(INamingScope namingScope)
		{
			return new ClassProxyWithTargetTargetContributor(targetType, namingScope) { Logger = Logger };
		}

		protected override ProxyTargetAccessorContributor GetProxyTargetAccessorContributor()
		{
			return new ProxyTargetAccessorContributor(
				getTargetReference: () => targetField,
				targetType);
		}

		private void CreateTargetField(ClassEmitter emitter)
		{
			targetField = emitter.CreateField("__target", targetType);
#if FEATURE_SERIALIZATION
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
#endif
		}
	}
}
