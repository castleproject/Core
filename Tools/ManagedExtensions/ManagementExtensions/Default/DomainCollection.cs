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
	/// Summary description for DomainCollection.
	/// </summary>
	public class DomainCollection : NameObjectCollectionBase, IEnumerable
	{
		public DomainCollection() : 
			base(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default)
		{
		}

		public void Add(Domain domain)
		{
			lock(this)
			{
				base.BaseAdd(domain.Name, domain);
			}
		}

		public Domain this[String domainName]
		{
			get
			{
				return base.BaseGet(domainName) as Domain;
			}
		}

		public String[] ToArray()
		{
			lock(this)
			{
				String[] names = new String[base.Keys.Count];
				int index = 0;
				foreach(String key in base.Keys)
				{
					names[index++] = key;
				}
				return names;
			}
		}

		#region IEnumerable Members

		public new IEnumerator GetEnumerator()
		{
			return base.BaseGetAllValues().GetEnumerator();
		}

		#endregion
	}
}
