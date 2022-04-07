// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Internal;

	internal class ClassEmitter : AbstractTypeEmitter
	{
		internal const TypeAttributes DefaultAttributes =
			TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

		private readonly ModuleScope moduleScope;

		public ClassEmitter(ModuleScope modulescope, string name, Type baseType, IEnumerable<Type> interfaces)
			: this(modulescope, name, baseType, interfaces, DefaultAttributes, forceUnsigned: false)
		{
		}

		public ClassEmitter(ModuleScope modulescope, string name, Type baseType, IEnumerable<Type> interfaces,
		                    TypeAttributes flags,
		                    bool forceUnsigned)
			: this(CreateTypeBuilder(modulescope, name, baseType, interfaces, flags, forceUnsigned))
		{
			interfaces = InitializeGenericArgumentsFromBases(ref baseType, interfaces);

			if (interfaces != null)
			{
				foreach (var inter in interfaces)
				{
					if (inter.IsInterface)
					{
						TypeBuilder.AddInterfaceImplementation(inter);
					}
					else
					{
						Debug.Assert(inter.IsDelegateType());
					}
				}
			}

			TypeBuilder.SetParent(baseType);
			moduleScope = modulescope;
		}

		public ClassEmitter(TypeBuilder typeBuilder)
			: base(typeBuilder)
		{
		}

		public ModuleScope ModuleScope
		{
			get { return moduleScope; }
		}

		internal bool InStrongNamedModule
		{
			get { return StrongNameUtil.IsAssemblySigned(TypeBuilder.Assembly); }
		}

		protected virtual IEnumerable<Type> InitializeGenericArgumentsFromBases(ref Type baseType,
		                                                                        IEnumerable<Type> interfaces)
		{
			if (baseType != null && baseType.IsGenericTypeDefinition)
			{
				throw new NotSupportedException("ClassEmitter does not support open generic base types. Type: " + baseType.FullName);
			}

			if (interfaces == null)
			{
				return interfaces;
			}

			foreach (var inter in interfaces)
			{
				if (inter.IsGenericTypeDefinition)
				{
					throw new NotSupportedException("ClassEmitter does not support open generic interfaces. Type: " + inter.FullName);
				}
			}
			return interfaces;
		}

		private static TypeBuilder CreateTypeBuilder(ModuleScope modulescope, string name, Type baseType,
		                                             IEnumerable<Type> interfaces,
		                                             TypeAttributes flags, bool forceUnsigned)
		{
			var isAssemblySigned = !forceUnsigned && !StrongNameUtil.IsAnyTypeFromUnsignedAssembly(baseType, interfaces);
			return modulescope.DefineType(isAssemblySigned, name, flags);
		}
	}
}