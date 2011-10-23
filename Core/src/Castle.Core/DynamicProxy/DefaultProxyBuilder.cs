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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;

	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;


	/// <summary>
	///   Default implementation of <see cref = "IProxyBuilder" /> interface producing in-memory proxy assemblies.
	/// </summary>
	public class DefaultProxyBuilder : IProxyBuilder
	{
		private readonly ModuleScope scope;
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		///   Initializes a new instance of the <see cref = "DefaultProxyBuilder" /> class with new <see cref = "ModuleScope" />.
		/// </summary>
		public DefaultProxyBuilder()
			: this(new ModuleScope())
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "DefaultProxyBuilder" /> class.
		/// </summary>
		/// <param name = "scope">The module scope for generated proxy types.</param>
		public DefaultProxyBuilder(ModuleScope scope)
		{
			this.scope = scope;
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public ModuleScope ModuleScope
		{
			get { return scope; }
		}

		public Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new ClassProxyGenerator(scope, classToProxy) { Logger = logger };
			return generator.GenerateCode(additionalInterfacesToProxy, options);
		}

		public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                           ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);
			AssertValidTypes(additionalInterfacesToProxy);
			var generator = new ClassProxyWithTargetGenerator(scope, classToProxy, additionalInterfacesToProxy, options)
			{ Logger = logger };
			return generator.GetGeneratedType();
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                               Type targetType,
		                                               ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                        ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetInterfaceGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                  ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithoutTargetGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
		}

		private void AssertValidType(Type target)
		{
			if (target.IsGenericTypeDefinition)
			{
				throw new GeneratorException("Type " + target.FullName + " is a generic type definition. " +
				                             "Can not create proxy for open generic types.");
			}
			if (IsPublic(target) == false && IsAccessible(target) == false)
			{
				throw new GeneratorException("Type " + target.FullName + " is not visible to DynamicProxy. " +
				                             "Can not create proxy for types that are not accessible. " +
				                             "Make the type public, or internal and mark your assembly with " +
				                             "[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)] attribute.");
			}
		}

		private void AssertValidTypes(IEnumerable<Type> targetTypes)
		{
			if (targetTypes != null)
			{
				foreach (var t in targetTypes)
				{
					AssertValidType(t);
				}
			}
		}

		private bool IsAccessible(Type target)
		{
			return IsInternal(target) && target.Assembly.IsInternalToDynamicProxy();
		}

		private bool IsPublic(Type target)
		{
			return target.IsPublic || target.IsNestedPublic;
		}

		private static bool IsInternal(Type target)
		{
			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
			var isInternalNotNested = target.IsVisible == false && isTargetNested == false;

			return isInternalNotNested || isNestedAndInternal;
		}
	}
}