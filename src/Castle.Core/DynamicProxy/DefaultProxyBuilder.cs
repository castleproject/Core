// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Generators.Emitters;
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
				throw new GeneratorException(BuildInternalsVisibleMessageForType(target));
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

		private static string BuildInternalsVisibleMessageForType(Type target)
		{
			var targetAssembly = target.Assembly;

			string strongNamedOrNotIndicator = " not"; // assume not strong-named
			string assemblyToBeVisibleTo = "\"DynamicProxyGenAssembly2\""; // appropriate for non-strong-named

			if (targetAssembly.IsAssemblySigned())
			{
				strongNamedOrNotIndicator = "";
				if (ReferencesCastleCore(targetAssembly))
				{
					assemblyToBeVisibleTo = "InternalsVisible.ToDynamicProxyGenAssembly2";
				}
				else
				{
					assemblyToBeVisibleTo = '"' + InternalsVisible.ToDynamicProxyGenAssembly2 + '"';
				}
			}

			return string.Format("Type {0} is not visible to DynamicProxy. " +
			                     "Can not create proxy for types that are not accessible. " +
			                     "Make the type public, or internal and mark your assembly with " +
			                     "[assembly: InternalsVisibleTo({1})] attribute, because assembly {2} " +
			                     "is{3} strong-named.",
				target.FullName, assemblyToBeVisibleTo,
#if SILVERLIGHT
				//SILVERLIGHT is retarded and doesn't allow us to call assembly.GetName()
				GetAssemblyName(targetAssembly),
#else
				targetAssembly.GetName().Name,
#endif
				strongNamedOrNotIndicator);
		}

#if SILVERLIGHT
		private static string GetAssemblyName(Assembly targetAssembly)
		{
			var fullName = targetAssembly.FullName;
			if (string.IsNullOrEmpty(fullName))
			{
				return fullName;
			}
			var index = fullName.IndexOf(", Version=", StringComparison.OrdinalIgnoreCase);
			if (index > 0)
			{
				return fullName.Substring(0, index);
			}
			return fullName;
		}
#endif
		private static bool ReferencesCastleCore(Assembly inspectedAssembly)
		{
#if SILVERLIGHT
			// no way to check that in SILVELIGHT, so we just fall back to the solution that will definitely work
			return false;
#else
			return inspectedAssembly.GetReferencedAssemblies()
				.Any(r => r.FullName == Assembly.GetExecutingAssembly().FullName);
#endif
		}
	}
}