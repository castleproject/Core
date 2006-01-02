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

namespace Castle.Components.Winforms.AssemblyResolver
{
	using System;
	using System.IO;
	using System.Reflection;


	internal abstract class AssemblyUtils
	{
		public static String Normalize(String assemblyFullName)
		{
			String name = assemblyFullName;
			
			int index = assemblyFullName.IndexOf(',');

			if (index != -1)
			{
				name = name.Substring(0, index);
			}

			return name;
		}

		public static Assembly TryToLoadAssembly( String path, String name )
		{
			name = Path.Combine( path, name );

			String nameVar1 = String.Format( "{0}.dll", name );
			String nameVar2 = String.Format( "{0}.exe", name );

			FileInfo info1 = new FileInfo( nameVar1 );
			FileInfo info2 = new FileInfo( nameVar2 );

			if (info1.Exists)
			{
				return Assembly.LoadFile( info1.FullName );
			}
			else if (info2.Exists)
			{
				return Assembly.LoadFile( info2.FullName );
			}

			return null;
		}
	}
}
