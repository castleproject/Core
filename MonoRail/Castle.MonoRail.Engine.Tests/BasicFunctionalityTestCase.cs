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

namespace Castle.MonoRail.Engine.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for BasicFunctionalityTestCase.
	/// </summary>
	[TestFixture]
	public class BasicFunctionalityTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void SimpleControllerAction()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/index.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for Home\\Index", response);
		}

		[Test]
		public void RenderView()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/welcome.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("Contents for heyhello View", response);
		}

		[Test]
		public void Redirect()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/redirectAction.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/home/index.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for Home\\Index", response);
		}

		[Test]
		public void RedirectForArea()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/redirectForOtherArea.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/subarea/home/index.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for SubArea\\Home\\Index", response);
		}

		[Test]
		public void PropertyBagNoFieldSetting()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/bag.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/home/bag.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("\r\nCustomer is hammett\r\n<br>\r\n123", response);
		}

		[Test]
		public void PropertyBagUsingFieldSetting()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/home/bag2.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/home/bag2.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("\r\nCustomer is hammett\r\n<br>\r\n123", response);
		}

		[Test]
		public void CreateCookie()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/cookies/addcookie.rails");
			myReq.AllowAutoRedirect = false;
			myReq.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/cookies/addcookie.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for Home\\Index", response);
			CookieCollection cookies = 
				myReq.CookieContainer.GetCookies( new Uri("http://localhost:8083/") );
			
			/// The server may use two headers for SetCookie
			Assert.IsTrue( cookies.Count == 1 || cookies.Count == 2 );
			foreach(Cookie cookie in cookies)
			{
				Assert.AreEqual( "cookiename", cookie.Name );
				Assert.AreEqual( "value", cookie.Value );
			}
		}

		[Test]
		public void CreateCookieRedirect()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/cookies/AddCookieRedirect.rails");
			myReq.AllowAutoRedirect = false;
			myReq.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);

			CookieCollection cookies = 
				myReq.CookieContainer.GetCookies( new Uri("http://localhost:8083/") );
			
			/// The server may use two headers for SetCookie
			Assert.IsTrue( cookies.Count == 1 || cookies.Count == 2 );
			foreach(Cookie cookie in cookies)
			{
				Assert.AreEqual( "cookiename", cookie.Name );
				Assert.AreEqual( "value", cookie.Value );
			}
		}

		[Test]
		public void CreateCookieExpirationRedirect()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/cookies/AddCookieExpirationRedirect.rails");
			myReq.AllowAutoRedirect = false;
			myReq.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);

			CookieCollection cookies = 
				myReq.CookieContainer.GetCookies( new Uri("http://localhost:8083/") );
			
			/// The server may use two headers for SetCookie
			Assert.IsTrue( cookies.Count == 1 || cookies.Count == 2 );
			foreach(Cookie cookie in cookies)
			{
				Assert.AreEqual( "cookiename2", cookie.Name );
				Assert.AreEqual( "value", cookie.Value );
				DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));
				Assert.AreEqual( twoWeeks.Day, cookie.Expires.Day );
				Assert.AreEqual( twoWeeks.Month, cookie.Expires.Month );
				Assert.AreEqual( twoWeeks.Year, cookie.Expires.Year );
			}
		}

		[Test]
		public void CreateCookieExpiration()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/cookies/AddCookieExpiration.rails");
			myReq.AllowAutoRedirect = false;
			myReq.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/cookies/AddCookieExpiration.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("My View contents for Home\\Index", response);
			CookieCollection cookies = 
				myReq.CookieContainer.GetCookies( new Uri("http://localhost:8083/") );
			
			/// The server may use two headers for SetCookie
			Assert.IsTrue( cookies.Count == 1 || cookies.Count == 2 );
			foreach(Cookie cookie in cookies)
			{
				Assert.AreEqual( "cookiename2", cookie.Name );
				Assert.AreEqual( "value", cookie.Value );
				DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));
				Assert.AreEqual( twoWeeks.Day, cookie.Expires.Day );
				Assert.AreEqual( twoWeeks.Month, cookie.Expires.Month );
				Assert.AreEqual( twoWeeks.Year, cookie.Expires.Year );
			}
		}
	}
}
