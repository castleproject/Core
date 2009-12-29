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

namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;

	public abstract class MethodGenerator : IGenerator<MethodEmitter>
	{
		protected MethodInfo MethodToOverride
		{
			get { return method.Method; }
		}

		protected MethodGenerator(MethodToGenerate method, CreateMethodDelegate createMethod, GetMethodAttributesAndNameDelegate getAttributesAndName)
		{
			this.method = method;
			this.createMethod = createMethod;
			this.getAttributesAndName = getAttributesAndName;
		}

		private readonly MethodToGenerate method;
		private readonly CreateMethodDelegate createMethod;
		private readonly GetMethodAttributesAndNameDelegate getAttributesAndName;

		public MethodEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var attributesAndName = getAttributesAndName(method);
			var methodEmitter = createMethod(attributesAndName.Second, attributesAndName.First);
			methodEmitter.CopyParametersAndReturnTypeFrom(MethodToOverride, @class);
			var proxiedMethod = BuildProxiedMethodBody(methodEmitter, @class, options, namingScope);

			if (MethodToOverride.DeclaringType.IsInterface)
			{
				@class.TypeBuilder.DefineMethodOverride(proxiedMethod.MethodBuilder, MethodToOverride);
			}

			return proxiedMethod;
		}

		protected abstract MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope);
	}
}