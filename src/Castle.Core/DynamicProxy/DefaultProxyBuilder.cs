// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;

	using Castle.Core.Internal;
	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;

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
			AssertValidType(classToProxy, nameof(classToProxy));
			AssertValidTypes(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

			var generator = new ClassProxyGenerator(scope, classToProxy) { Logger = logger };
			return generator.GenerateCode(additionalInterfacesToProxy, options);
		}

		public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
		                                           ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy, nameof(classToProxy));
			AssertValidTypes(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

			var generator = new ClassProxyWithTargetGenerator(scope, classToProxy, additionalInterfacesToProxy, options)
			{ Logger = logger };
			return generator.GetGeneratedType();
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                               Type targetType,
		                                               ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy, nameof(interfaceToProxy));
			AssertValidTypes(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

			var generator = new InterfaceProxyWithTargetGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                        ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy, nameof(interfaceToProxy));
			AssertValidTypes(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

			var generator = new InterfaceProxyWithTargetInterfaceGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
		                                                  ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy, nameof(interfaceToProxy));
			AssertValidTypes(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

			var generator = new InterfaceProxyWithoutTargetGenerator(scope, interfaceToProxy) { Logger = logger };
			return generator.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
		}

		private void AssertValidType(Type target, string paramName)
		{
			AssertValidTypeForTarget(target, target, paramName);
		}

		private void AssertValidTypeForTarget(Type type, Type target, string paramName)
		{
			if (type.IsGenericTypeDefinition)
			{
				throw new ArgumentException(
					$"Can not create proxy for type {target.GetBestName()} because type {type.GetBestName()} is an open generic type.",
					paramName);
			}
			if (ProxyUtil.IsAccessibleType(type) == false)
			{
				throw new ArgumentException(ExceptionMessageBuilder.CreateMessageForInaccessibleType(type, target), paramName);
			}
			foreach (var typeArgument in type.GetGenericArguments())
			{
				AssertValidTypeForTarget(typeArgument, target, paramName);
			}
		}

		private void AssertValidTypes(IEnumerable<Type> targetTypes, string paramName)
		{
			if (targetTypes != null)
			{
				foreach (var t in targetTypes)
				{
					AssertValidType(t, paramName);
				}
			}
		}
	}
}