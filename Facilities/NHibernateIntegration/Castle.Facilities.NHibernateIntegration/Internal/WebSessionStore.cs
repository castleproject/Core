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

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using System.Web;
	using System.Collections;

	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Provides an implementation of <see cref="ISessionStore"/>
	/// which relies on <c>HttpContext</c>. Suitable for web projects.
	/// </summary>
	public class WebSessionStore : AbstractDictStackSessionStore
	{
		protected override IDictionary GetDictionary()
		{
			HttpContext curContext = ObtainSessionContext();

			return curContext.Items[SlotKey] as IDictionary;
		}

		protected override void StoreDictionary(IDictionary dictionary)
		{
			HttpContext curContext = ObtainSessionContext();

			curContext.Items[SlotKey] = dictionary;
		}

		private static HttpContext ObtainSessionContext()
		{
			HttpContext curContext = HttpContext.Current;
	
			if (curContext == null)
			{
				throw new FacilityException("WebSessionStore: Could not obtain reference to HttpContext");
			}
			return curContext;
		}
	}
}
