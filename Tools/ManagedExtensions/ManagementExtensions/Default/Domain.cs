// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for Domain.
	/// </summary>
	public class Domain : DictionaryBase
	{
		protected String name;

		public Domain()
		{
			Name = "default";
		}

		public Domain(String name)
		{
			Name = name;
		}

		public void Add(ManagedObjectName objectName, Entry instance)
		{
			lock(this)
			{
				InnerHashtable.Add(objectName, instance);
			}
		}

		public bool Contains(ManagedObjectName objectName)
		{
			return InnerHashtable.ContainsKey(objectName);
		}

		public void Remove(ManagedObjectName objectName)
		{
			lock(this)
			{
				InnerHashtable.Remove(objectName);
			}
		}

		public String Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public Entry this[ManagedObjectName objectName]
		{
			get
			{
				return (Entry) InnerHashtable[objectName];
			}
		}

		public ManagedObjectName[] ToArray()
		{
			lock(this)
			{
				int index = 0;
				ManagedObjectName[] names = new ManagedObjectName[ Count ];
				foreach(ManagedObjectName name in InnerHashtable.Keys)
				{
					names[index++] = name;
				}
				return names;
			}
		}
	}
}
