// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;

	using Castle.DynamicProxy.Generators;


	public class DefaultProxyBuilder : IProxyBuilder
	{
		private readonly ModuleScope scope = new ModuleScope();

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

//		public Type CreateInterfaceProxyType(Type theInterface, ProxyGenerationOptions options)
//		{
//			throw new NotImplementedException();
//		}

		public Type CreateInterfaceProxyTypeWithTarget(Type theInterface, Type targetType, ProxyGenerationOptions options)
		{
			AssertValidType(theInterface);

			InterfaceProxyWithTargetGenerator generator = new InterfaceProxyWithTargetGenerator(scope, theInterface);

			return generator.GenerateCode(targetType, options);
		}

		private static void AssertValidType(Type target)
		{
			if (!target.IsPublic)
			{
				throw new GeneratorException("Type is not public, so a proxy cannot be generated. Type: " + target.FullName);
			}
		}
	}
}
