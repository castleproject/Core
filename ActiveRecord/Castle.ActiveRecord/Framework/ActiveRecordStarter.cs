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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Reflection;

	using NHibernate.Cfg;


	/// <summary>
	/// 
	/// </summary>
	public class ActiveRecordStarter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="types"></param>
		public static void Initialize( params Type[] types )
		{
			NHibernateMappingEngine engine = new NHibernateMappingEngine();

			SessionFactoryHolder holder = new SessionFactoryHolder();

			ActiveRecordBase._holder = holder;

			foreach( Type type in types )
			{
				if ( !typeof(ActiveRecordBase).IsAssignableFrom( type ) )
				{
					continue;
				}
//
//				MethodInfo m = type.GetMethod("DefineConfiguration", 
//					BindingFlags.Static|BindingFlags.Public);
//				
//				if (m != null)
//				{
//					Console.WriteLine( "    {0}.{1}", t.Name, m.Name );
//
//					holder.Add( t, (Configuration) m.Invoke(null, new object[] { null }) );
//				}

				Configuration cfg = holder.GetConfiguration( type );

				if (cfg == null)
				{
					// Add to wait list
				}

				if (!type.IsAbstract)
				{
					engine.CreateMapping(type, cfg);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assembly"></param>
		public static void Initialize( Assembly assembly )
		{
			Type[] types = assembly.GetExportedTypes();

			ArrayList list = new ArrayList();

			foreach( Type type in types )
			{
				if ( !typeof(ActiveRecordBase).IsAssignableFrom( type ) )
				{
					continue;
				}

				list.Add(type);
			}

			Initialize( (Type[]) list.ToArray( typeof(Type) ) );
		}

		public static void Initialize( )
		{
			Initialize( Assembly.GetExecutingAssembly() );
		}
	}
}
