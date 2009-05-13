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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;
	using Castle.DynamicProxy.Generators;
#if SILVERLIGHT
	using Castle.DynamicProxy.SilverlightExtensions;
#endif

	/// <summary>
	/// Default implementation of <see cref="IProxyBuilder"/> interface producing in-memory proxy assemblies.
	/// </summary>
	public class DefaultProxyBuilder : IProxyBuilder
	{
		private readonly ModuleScope scope;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultProxyBuilder"/> class with new <see cref="DynamicProxy.ModuleScope"/>.
		/// </summary>
		public DefaultProxyBuilder()
			: this(new ModuleScope())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultProxyBuilder"/> class.
		/// </summary>
		/// <param name="scope">The module scope for generated proxy types.</param>
		public DefaultProxyBuilder(ModuleScope scope)
		{
			this.scope = scope;
		}

		public ModuleScope ModuleScope
		{
			get { return scope; }
		}

		public virtual Type CreateClassProxy(Type classToProxy, ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);

			ClassProxyGenerator generator = new ClassProxyGenerator(scope, classToProxy);

			return generator.GenerateCode(null, options);
		}

		public Type CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			ClassProxyGenerator generator = new ClassProxyGenerator(scope, classToProxy);

			return generator.GenerateCode(additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			InterfaceProxyWithoutTargetGenerator generatorWithoutTarget =
				new InterfaceProxyWithoutTargetGenerator(scope, interfaceToProxy);

			return generatorWithoutTarget.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, Type targetType,
													   ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			InterfaceProxyWithTargetGenerator generator = new InterfaceProxyWithTargetGenerator(scope, interfaceToProxy);

			return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
																ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			InterfaceProxyWithTargetInterfaceGenerator generator =
				new InterfaceProxyWithTargetInterfaceGenerator(scope, interfaceToProxy);

			return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		private void AssertValidType(Type target)
		{
#if SILVERLIGHT
			bool isTargetNested = target.IsNested();
#else
			bool isTargetNested = target.IsNested;
#endif

			bool isNestedAndInternal = isTargetNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
			bool isInternalNotNested = target.IsVisible == false && isTargetNested == false;

			bool internalAndVisibleToDynProxy = (isInternalNotNested || isNestedAndInternal) &&
												InternalsHelper.IsInternalToDynamicProxy(target.Assembly);
			if (!target.IsPublic && !target.IsNestedPublic && !internalAndVisibleToDynProxy)
			{
				throw new GeneratorException("Type is not public, so a proxy cannot be generated. Type: " + target.FullName);
			}
			if (target.IsGenericTypeDefinition)
			{
				throw new GeneratorException("Type is a generic type definition, so a proxy cannot be generated. Type: " +
											 target.FullName);
			}
		}

		private void AssertValidTypes(IEnumerable<Type> targetTypes)
		{
			if (targetTypes != null)
			{
				foreach (Type t in targetTypes)
				{
					AssertValidType(t);
				}
			}
		}
	}
}
