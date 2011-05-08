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

namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;

	public abstract class MethodGenerator : IGenerator<MethodEmitter>
	{
		private readonly MetaMethod method;
		private readonly OverrideMethodDelegate overrideMethod;

		protected MethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod)
		{
			this.method = method;
			this.overrideMethod = overrideMethod;
		}

		protected MethodInfo MethodOnTarget
		{
			get { return method.MethodOnTarget; }
		}

		protected MethodInfo MethodToOverride
		{
			get { return method.Method; }
		}

		protected abstract MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class,
		                                                        ProxyGenerationOptions options, INamingScope namingScope);

		public MethodEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var methodEmitter = overrideMethod(method.Name, method.Attributes, MethodToOverride);
			var proxiedMethod = BuildProxiedMethodBody(methodEmitter, @class, options, namingScope);

			if (MethodToOverride.DeclaringType.IsInterface)
			{
				@class.TypeBuilder.DefineMethodOverride(proxiedMethod.MethodBuilder, MethodToOverride);
			}

			return proxiedMethod;
		}
	}
}