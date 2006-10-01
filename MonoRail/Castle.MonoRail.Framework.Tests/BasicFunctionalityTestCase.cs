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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	
	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;


	[TestFixture]
	public class BasicFunctionalityTestCase : AbstractTestCase
	{
		[Test]
		public void SimpleControllerAction()
		{
			DoGet("home/index.rails");

			AssertSuccess();

			AssertReplyEqualTo( "My View contents for Home\\Index" );
		}

		[Test]
		public void RenderView()
		{
			DoGet("home/welcome.rails");

			AssertSuccess();

			AssertReplyEqualTo( "Contents for heyhello View" );
		}

		[Test]
		public void Flash()
		{
			DoGet("home/flash1.rails");

			AssertSuccess();

			AssertFlashEntryEquals("errormessage", "Some error");
		}

		[Test]
		public void Redirect()
		{
			DoGet("home/redirectAction.rails");

			AssertSuccess();

			AssertRedirectedTo( "/home/index.rails" );
		}

		[Test]
		public void RedirectForArea()
		{
			DoGet("home/redirectforotherarea.rails");

			AssertSuccess();

			AssertRedirectedTo( "/subarea/home/index.rails" );
		}

		[Test]
		public void PropertyBagNoFieldSetting()
		{
			DoGet("home/bag.rails");

			AssertSuccess();

			AssertPropertyBagContains( "CustomerName" );
			AssertPropertyBagEntryEquals( "CustomerName", "hammett" );
			AssertReplyEqualTo( "\r\nCustomer is hammett\r\n<br>\r\n123" );
		}

		[Test]
		public void PropertyBagUsingFieldSetting()
		{
			DoGet("home/bag2.rails");

			AssertSuccess();

			AssertPropertyBagContains( "CustomerName" );
			AssertPropertyBagEntryEquals( "CustomerName", "hammett" );
			AssertReplyEqualTo( "\r\nCustomer is hammett\r\n<br>\r\n123" );
		}

		[Test]
		public void CreateCookie()
		{
			DoGet("cookies/addcookie.rails");

			AssertSuccess();

			AssertReplyEqualTo( @"My View contents for Cookies\Index" );
			
			/// One of the cookies will be the asp.net used by the flash property
			///Assert.AreEqual( 3, Response.Cookies.Count );

			AssertHasCookie( "cookiename" );
			AssertHasCookie( "cookiename2" );
			AssertCookieValueEqualsTo( "cookiename", "value" );
			AssertCookieValueEqualsTo( "cookiename2", "value2" );
		}

		[Test]
		public void CreateCookieRedirect()
		{
			DoGet("cookies/AddCookieRedirect.rails");

			AssertSuccess();
			
			AssertRedirectedTo("/cookies/index.rails");

			AssertCookieValueEqualsTo("cookiename", "value");
		}

		[Test]
		public void CreateCookieExpirationRedirect()
		{
			DoGet("cookies/AddCookieExpirationRedirect.rails");

			AssertSuccess();
			
			AssertRedirectedTo("/cookies/index.rails");

			DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));

			AssertCookieExpirationEqualsTo("cookiename2", twoWeeks);
			AssertCookieValueEqualsTo("cookiename2", "value");
		}

		[Test]
		public void CreateCookieExpiration()
		{
			DoGet("cookies/AddCookieExpiration.rails");

			AssertSuccess();
			
			AssertReplyEqualTo(@"My View contents for Cookies\Index");

			DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));

			AssertCookieExpirationEqualsTo("cookiename2", twoWeeks);
			AssertCookieValueEqualsTo("cookiename2", "value");
		}

		[Test]
		public void FormPostSmartDispatcher()
		{
			DoPost( "registration/posthere.rails", "p1=foo", "p2=123" );
			
			AssertSuccess();

			AssertReplyContains( "param1=foo" );
			AssertReplyContains( "param2=123" );
		}

		[Test]
		public void OverloadNullSmartDispatcher()
		{
			DoPost("registration/posthere.rails", "p1=foo", "p2=123");

			AssertSuccess();

			AssertReplyContains("param1=foo");
			AssertReplyContains("param2=123");
		}	
	}
}
