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
#if DOTNET2
	using System.Collections.Generic;
#endif

	[CLSCompliant(false)]
	public class ClassEmitter : AbstractTypeEmitter
	{
		private static IDictionary signedAssemblyCache = new Hashtable();

		public ClassEmitter(ModuleScope modulescope, String name, Type baseType, Type[] interfaces, bool serializable)
		{
			TypeAttributes flags = 
				TypeAttributes.Public|TypeAttributes.Class|TypeAttributes.Serializable;

			if (serializable)
			{
				flags |= TypeAttributes.Serializable;
			}

			bool isAssemblySigned = IsAssemblySigned(baseType);
	          
			typebuilder = modulescope.ObtainDynamicModule(isAssemblySigned).DefineType(name, flags);
			
			InitializeGenericArgumentsFromBases (ref baseType, ref interfaces);

			if (interfaces != null)
			{
				foreach(Type inter in interfaces)
				{
					typebuilder.AddInterfaceImplementation(inter);
				}
			}
			
			typebuilder.SetParent(baseType);
		}

		// The ambivalent generic parameter handling of base type and interfaces has been removed from the ClassEmitter, it isn't used by the proxy
		// generators anyway. If a concrete user needs to support generic bases, a subclass can override this method (and not call this base
		// implementation), call CreateGenericParameters and replace baseType and interfaces by versions bound to the newly created GenericTypeParams.
		protected virtual void InitializeGenericArgumentsFromBases (ref Type baseType, ref Type[] interfaces)
		{
			if (baseType.IsGenericTypeDefinition)
			{
				throw new NotSupportedException ("ClassEmitter does not support open generic base types. Type: " + baseType.FullName);
			}
			foreach (Type inter in interfaces)
			{
				if (inter.IsGenericTypeDefinition)
				{
					throw new NotSupportedException ("ClassEmitter does not support open generic interfaces. Type: " + inter.FullName);
				}
			}
		}

		private bool IsAssemblySigned(Type baseType)
		{
			if (baseType == typeof(object) || 
			    baseType == typeof(MarshalByRefObject) || 
			    baseType == typeof(ContextBoundObject))
			{
				return false;
			}
			
			lock(signedAssemblyCache)
			{
				if (signedAssemblyCache.Contains(baseType.Assembly) == false)
				{
					byte[] key = baseType.Assembly.GetName().GetPublicKey();
					bool isSigned = key != null  && key.Length != 0;
					signedAssemblyCache.Add(baseType.Assembly, isSigned);
				}
				return (bool) signedAssemblyCache[baseType.Assembly];
			}
		}
	}
}
