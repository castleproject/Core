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

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	//TODO: this class should be factored out as soon as we get rid of the need for BackingField property
	public abstract class MixinContributorBase : ITypeContributor
	{
		protected FieldReference field;

		protected Type mixinInterface;
		protected GetTargetExpressionDelegate getTargetExpression;

		protected MixinContributorBase()
		{
			getTargetExpression = (c, i) => BackingField.ToExpression();
		}

		public FieldReference BackingField
		{
			get
			{
				return field;
			}
		}

		protected FieldReference BuildTargetField(ClassEmitter emitter, Type type)
		{
			return emitter.CreateField("__mixin_" + type.FullName.Replace(".", "_"), type);
		}

		public abstract void CollectElementsToProxy(IProxyGenerationHook hook);
		public abstract void Generate(ClassEmitter @class, ProxyGenerationOptions options);
	}
}