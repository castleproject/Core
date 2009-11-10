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
	using System.Diagnostics;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public abstract class InterfaceMethodGeneratorBase : MethodGenerator
	{
		private readonly MethodToGenerate method;

		protected MethodToGenerate Method
		{
			get { return method; }
		}

		private readonly CreateMethodDelegate createMethod;

		protected InterfaceMethodGeneratorBase(MethodToGenerate method, CreateMethodDelegate createMethod)
		{
			Debug.Assert(method.Method.DeclaringType.IsInterface, "method.Method.DeclaringType.IsInterface");
			this.method = method;
			this.createMethod = createMethod;
		}


		public override MethodEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			string name;
			MethodAttributes atts = ObtainMethodAttributes(out name);
			MethodEmitter methodEmitter = createMethod(name, atts);
			MethodEmitter proxiedMethod = ImplementProxiedMethod(methodEmitter, @class,options,namingScope);

			@class.TypeBuilder.DefineMethodOverride(methodEmitter.MethodBuilder, method.Method);
			return proxiedMethod;
		}

		private MethodAttributes ObtainMethodAttributes(out string name)
		{
			var methodInfo = method.Method;
			name = methodInfo.DeclaringType.Name + "." + methodInfo.Name;
			var attributes = MethodAttributes.Virtual |
			                 MethodAttributes.Private |
			                 MethodAttributes.HideBySig |
			                 MethodAttributes.NewSlot |
			                 MethodAttributes.Final;

			if (method.Standalone == false)
			{
				attributes |= MethodAttributes.SpecialName;
			}
			return attributes;
		}

		protected abstract MethodEmitter ImplementProxiedMethod(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope);
	}
}