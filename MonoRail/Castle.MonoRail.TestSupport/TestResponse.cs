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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Net;
	using System.Web;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework.Internal.Test;


	[Serializable]
	public class TestResponse : MarshalByRefObject
	{
		private int statusCode;
		private String statusDescription;
		private IDictionary propertyBag;
		private IDictionary flash;
		private IDictionary session;
		private IDictionary headers = new Hashtable();
		private CookieContainer cookies = new CookieContainer();

		public TestResponse()
		{
		}

		public int StatusCode
		{
			get { return statusCode; }
			set { statusCode = value; }
		}

		public string StatusDescription
		{
			get { return statusDescription; }
			set { statusDescription = value; }
		}

		public IDictionary Headers
		{
			get { return headers; }
		}

		public CookieContainer Cookies
		{
			get { return cookies; }
		}

		public IDictionary PropertyBag
		{
			get { return propertyBag; }
		}

		public IDictionary Flash
		{
			get { return flash; }
		}

		public IDictionary Session
		{
			get { return session; }
		}

		protected internal void Complete()
		{
			foreach(DictionaryEntry entry in headers)
			{
				String name = entry.Key.ToString();

				if ("Set-Cookie".Equals(name))
				{
					if (entry.Value is IList)
					{
						foreach(String value in (IList) entry.Value)
						{
							cookies.SetCookies(new Uri("http://localhost"), value);
						}
					}
					else
					{
						try
						{
							cookies.SetCookies(new Uri("http://localhost"), entry.Value.ToString());
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}

					break;
				}
			}

			HttpContext context = TestContextHolder.Context;

			flash = (IDictionary) context.Items["mr.flash"];
			session = (IDictionary) context.Items["mr.session"];
			propertyBag = (IDictionary) context.Items["mr.propertybag"];
		}
	}
}
