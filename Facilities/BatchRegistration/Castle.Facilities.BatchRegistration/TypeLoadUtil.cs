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
	using System.Configuration;


	public abstract class TypeLoadUtil
	{
		public static Type GetType( Assembly assembly, String typeName )
		{
			Type type = assembly.GetType(typeName, false, false);

			if (type == null)
			{
				String message = 
					String.Format("Could not load type {0} from {1}", typeName, assembly.FullName);
				
				throw new ConfigurationException(message);
			}

			return type;			
		}

		public static Type GetType( String typeName )
		{
			Type type = Type.GetType(typeName, false, false);

			if (type == null)
			{
				String message = 
					String.Format("Could not load type {0}", typeName);
				
				throw new ConfigurationException(message);
			}

			return type;			
		}
	}
}
