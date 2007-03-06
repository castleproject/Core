// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Castle.DynamicProxy;

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using Castle.DynamicProxy.Generators;

	[CLSCompliant(false)]
	public class DefaultProxyBuilder : IProxyBuilder
	{
		private readonly ModuleScope scope = new ModuleScope();
#if DOTNET2
#endif

		public DefaultProxyBuilder()
		{
		}

		protected ModuleScope ModuleScope
		{
			get { return scope; }
		}

		public virtual Type CreateClassProxy(Type theClass, ProxyGenerationOptions options)
		{
			AssertValidType(theClass);

			ClassProxyGenerator generator = new ClassProxyGenerator(scope, theClass);

			return generator.GenerateCode(null, options);
		}

		public Type CreateClassProxy(Type theClass, Type[] interfaces, ProxyGenerationOptions options)
		{
			AssertValidType(theClass);

			ClassProxyGenerator generator = new ClassProxyGenerator(scope, theClass);

			return generator.GenerateCode(interfaces, options);
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options)
		{
			AssertValidType(theInterface);

			InterfaceProxyGeneratorWithoutTarget generatorWithoutTarget = new InterfaceProxyGeneratorWithoutTarget(scope, theInterface);

			return generatorWithoutTarget.GenerateCode(typeof (object), interfaces, options);
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type[] interfaces, Type targetType, ProxyGenerationOptions options)
		{
			AssertValidType(theInterface);

			InterfaceProxyWithTargetGenerator generator = new InterfaceProxyWithTargetGenerator(scope, theInterface);

			return generator.GenerateCode(targetType, interfaces, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type theInterface, ProxyGenerationOptions options)
		{
			AssertValidType(theInterface);

			InterfaceProxyWithTargetInterfaceGenerator generator = new InterfaceProxyWithTargetInterfaceGenerator(scope, theInterface);

			return generator.GenerateCode(theInterface, null, options);
		}

		private void AssertValidType(Type target)
		{
			bool isNestedAndInternal = target.IsNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
			bool isInternalNotNested = target.IsVisible ==false && target.IsNested==false;
			bool internalAndVisibleToDynProxy = (isInternalNotNested || isNestedAndInternal) && InternalsHelper.IsInternalToDynamicProxy(target.Assembly);
			if (!target.IsPublic && !target.IsNestedPublic && !internalAndVisibleToDynProxy)
			{
				throw new GeneratorException("Type is not public, so a proxy cannot be generated. Type: " + target.FullName);
			}
		}
	}
}