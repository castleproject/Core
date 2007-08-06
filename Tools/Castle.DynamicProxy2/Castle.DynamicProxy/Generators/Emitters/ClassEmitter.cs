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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	[CLSCompliant(false)]
	public class ClassEmitter : AbstractTypeEmitter
	{
		private static IDictionary signedAssemblyCache = new Hashtable();

		public ClassEmitter (ModuleScope modulescope, String name, Type baseType, Type[] interfaces)
			: this (modulescope, name, baseType, interfaces, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable)
		{
		}

		public ClassEmitter(ModuleScope modulescope, String name, Type baseType, Type[] interfaces, TypeAttributes flags)
		{
			bool isAssemblySigned = IsAssemblySigned(baseType);
			foreach(Type type in interfaces)
			{
				isAssemblySigned &= IsAssemblySigned(type);
			}

			typebuilder = modulescope.ObtainDynamicModule(isAssemblySigned).DefineType(name, flags);

			InitializeGenericArgumentsFromBases(ref baseType, ref interfaces);

			if (interfaces != null)
			{
				foreach(Type inter in interfaces)
				{
					typebuilder.AddInterfaceImplementation(inter);
				}
			}

			typebuilder.SetParent(baseType);
		}

		public ClassEmitter (TypeBuilder typeBuilder)
		{
			this.typebuilder = typeBuilder;
		}

		// The ambivalent generic parameter handling of base type and interfaces has been removed from the ClassEmitter, it isn't used by the proxy
		// generators anyway. If a concrete user needs to support generic bases, a subclass can override this method (and not call this base
		// implementation), call CreateGenericParameters and replace baseType and interfaces by versions bound to the newly created GenericTypeParams.
		protected virtual void InitializeGenericArgumentsFromBases(ref Type baseType, ref Type[] interfaces)
		{
			if (baseType.IsGenericTypeDefinition)
			{
				throw new NotSupportedException("ClassEmitter does not support open generic base types. Type: " + baseType.FullName);
			}
			foreach(Type inter in interfaces)
			{
				if (inter.IsGenericTypeDefinition)
				{
					throw new NotSupportedException("ClassEmitter does not support open generic interfaces. Type: " + inter.FullName);
				}
			}
		}

		private bool IsAssemblySigned(Type baseType)
		{
			lock(signedAssemblyCache)
			{
				if (signedAssemblyCache.Contains(baseType.Assembly) == false)
				{
					bool isSigned = ContainsPublicKey(baseType.Assembly);
					signedAssemblyCache.Add(baseType.Assembly, isSigned);
				}
				return (bool) signedAssemblyCache[baseType.Assembly];
			}
		}

		public static bool ContainsPublicKey (Assembly assembly)
		{
			byte[] key = assembly.GetName ().GetPublicKey ();
			return key != null && key.Length != 0;
		}
	}
}