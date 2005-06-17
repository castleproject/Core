// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.BatchRegistration
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Configuration;

	using Castle.Model;


	public sealed class ComponentScanner
	{
		private bool _useAttributes;
		private String _assemblyName;
		private ArrayList _includes;
		private ArrayList _excludes;
		private ArrayList _result;

		public ComponentScanner(String assemblyName)
		{
			_includes = new ArrayList();
			_excludes = new ArrayList();
			_result= new ArrayList();
			_assemblyName = assemblyName;
		}

		public bool UseAttributes
		{
			get { return _useAttributes; }
			set { _useAttributes = value; }
		}

		public void AddInclude(String key, String service, String className)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (className == null) throw new ArgumentNullException("className");

			_includes.Add(new ComponentDefinition(key, service, className));
		}

		public void AddExclude(String className)
		{
			if (className == null) throw new ArgumentNullException("className");

			_excludes.Add(className);
		}

		public ComponentDefinition[] Process()
		{
			Assembly assembly = LoadAssembly();

			SortExcludes();

			InspectTypes(assembly);

			ProcessIncludes(assembly);

			return (ComponentDefinition[]) _result.ToArray( typeof(ComponentDefinition) );
		}

		private void InspectTypes(Assembly assembly)
		{
			if (_useAttributes)
			{
				foreach(Type type in assembly.GetExportedTypes())
				{
					ProcessType(type);
				}
			}
		}

		private Assembly LoadAssembly()
		{
			try
			{
				return Assembly.Load(_assemblyName);
			}
			catch(Exception ex)
			{
				String message = 
					String.Format("Could not load the specified assembly {0}", _assemblyName);
				
				throw new ConfigurationException(message, ex);
			}
		}

		private void ProcessIncludes(Assembly assembly)
		{
			foreach(ComponentDefinition def in _includes)
			{
				if (_excludes.BinarySearch( def.ClassName ) == -1)
				{
					AddToResult( assembly, def );
				}
			}
		}

		private void SortExcludes()
		{
			_excludes.Sort( CaseInsensitiveComparer.Default );
		}

		private void ProcessType(Type type)
		{
			if (type.IsDefined( typeof(CastleComponentAttribute), false ))
			{
				if (_excludes.BinarySearch( type.FullName ) == -1)
				{
					CastleComponentAttribute attribute = (CastleComponentAttribute)
						type.GetCustomAttributes( typeof(CastleComponentAttribute), false )[0];

					AddToResult( type, attribute );
				}
			}
		}

		private void AddToResult(Assembly assembly, ComponentDefinition def)
		{
			_result.Add( new ComponentDefinition( 
				def.Key, 
				ObtainType(assembly, def.Service), 
				InferType(assembly, def.ClassName) ) );
		}

		private void AddToResult(Type type, CastleComponentAttribute attribute)
		{
			_result.Add( new ComponentDefinition( attribute.Key, attribute.Service, type ) );
		}

		/// <summary>
		/// This method always tries to obtain the type
		/// from the specified assembly.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="assembly"></param>
		/// <returns></returns>
		private Type InferType(Assembly assembly, String typeName)
		{
			return TypeLoadUtil.GetType(assembly, typeName);
		}

		/// <summary>
		/// Obtains the <c>Type</c> by checking if the 
		/// <c>typeName</c> is possible a full type name, or 
		/// just an namespace.typename and for the later case, 
		/// it tries to load the type from the specified assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private Type ObtainType(Assembly assembly, String typeName)
		{
			if (typeName == null) return null;

			if (typeName.IndexOf(',') == -1)
			{
				return InferType(assembly, typeName);
			}
			else
			{
				return TypeLoadUtil.GetType(typeName);
			}
		}
	}
}
