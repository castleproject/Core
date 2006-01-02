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

using System.IO;

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Collections;
	using System.Threading;

	/// <summary>
	/// Summary description for ModuleScope.
	/// </summary>
	[CLSCompliant(false)]
	public class ModuleScope
	{
		public static readonly String FILE_NAME = "GeneratedAssembly.dll";
        public static readonly String ASSEMBLY_NAME = "DynamicAssemblyProxyGen";
        
		/// <summary>
		/// Avoid leaks caused by non disposal of generated types.
		/// </summary>
		private ModuleBuilder _moduleBuilderWithStrongName = null;
        private ModuleBuilder _moduleBuilder = null;

		/// <summary>
		/// Keep track of generated types
		/// </summary>
		private Hashtable _typeCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Used to lock the module builder creation
		/// </summary>
		private object _lockobj = new object();

		private ReaderWriterLock readerWriterLock = new ReaderWriterLock();

		private AssemblyBuilder _assemblyBuilder;

	    public ModuleBuilder ObtainDynamicModule()
	    {
            return ObtainDynamicModule(false);
	    }
	    
		public ModuleBuilder ObtainDynamicModule(bool signStrongName)
		{
			lock (_lockobj)
			{
                if (signStrongName && _moduleBuilderWithStrongName == null)
				{
                    _moduleBuilderWithStrongName =  CreateModule(signStrongName);
				}
			    else if(!signStrongName && _moduleBuilder == null)
			    {
                    _moduleBuilder = CreateModule(signStrongName);
			    }
			}
            return signStrongName ? _moduleBuilderWithStrongName : _moduleBuilder;
		}

	    private ModuleBuilder CreateModule(bool signStrongName)
	    {
	        AssemblyName assemblyName = new AssemblyName();
	        assemblyName.Name = ASSEMBLY_NAME;
	        if (signStrongName)
	        {
	            assemblyName.KeyPair = new StrongNameKeyPair(GetKeyPair());
	        }
#if ( PHYSICALASSEMBLY )
    _assemblyBuilder =
						AppDomain.CurrentDomain.DefineDynamicAssembly(
							assemblyName,
							AssemblyBuilderAccess.RunAndSave);
					_moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name, FILE_NAME);
#else
	        _assemblyBuilder =
	            AppDomain.CurrentDomain.DefineDynamicAssembly(
	                assemblyName,
	                AssemblyBuilderAccess.Run);
	        return _assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);
#endif
	    }

	    public ReaderWriterLock RWLock
		{
			get { return readerWriterLock; }
		}

		public bool SaveAssembly()
		{
#if ( PHYSICALASSEMBLY )
			_assemblyBuilder.Save(FILE_NAME);
			return true;
#else
			return false;
#endif
		}

		public Type this[String name]
		{
			get { return _typeCache[name] as Type; }
			set
			{
				_typeCache[name] = value;
				SaveAssembly();
			}
		}

        private static byte[] GetKeyPair()
        {
            byte[] keyPair;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Castle.DynamicProxy.DynProxy.snk"))
            {
                int length = (int)stream.Length;
                keyPair = new byte[length];
                stream.Read(keyPair, 0, length);
            }
            return keyPair;
        }
	}
}