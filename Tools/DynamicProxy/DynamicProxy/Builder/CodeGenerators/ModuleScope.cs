// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Collections;

	/// <summary>
	/// Summary description for ModuleScope.
	/// </summary>
	public class ModuleScope
	{
		public static readonly String FILE_NAME = "GeneratedAssembly.dll";

		/// <summary>
		/// Avoid leaks caused by non disposal of generated types.
		/// </summary>
		private ModuleBuilder m_moduleBuilder = null;

		/// <summary>
		/// Keep track of generated types
		/// </summary>
		private Hashtable m_typeCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Used to lock the module builder creation
		/// </summary>
		private object m_lockobj = new object();

		private int innerTypeCounter;

		private AssemblyBuilder m_assemblyBuilder;

		public ModuleBuilder ObtainDynamicModule()
		{
			lock (m_lockobj)
			{
				if (m_moduleBuilder == null)
				{
					AssemblyName assemblyName = new AssemblyName();
					assemblyName.Name = "DynamicAssemblyProxyGen";

#if ( DEBUG )
					m_assemblyBuilder =
						AppDomain.CurrentDomain.DefineDynamicAssembly(
							assemblyName,
							AssemblyBuilderAccess.RunAndSave);
					m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(assemblyName.Name, FILE_NAME);
#else
					m_assemblyBuilder =
						AppDomain.CurrentDomain.DefineDynamicAssembly(
							assemblyName,
							AssemblyBuilderAccess.Run);
					m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);
#endif
				}
			}

			return m_moduleBuilder;
		}

		public int InnerTypeCounter
		{
			get { return innerTypeCounter++; }
		}

		public bool SaveAssembly()
		{
#if ( DEBUG )
			m_assemblyBuilder.Save(FILE_NAME);
			return true;
#else
			return false;
#endif
		}

		public Type this[String name]
		{
			get { return m_typeCache[name] as Type; }
			set
			{
				m_typeCache[name] = value;
			}
		}
	}
}