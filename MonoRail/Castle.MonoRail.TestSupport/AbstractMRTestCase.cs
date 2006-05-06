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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web.Hosting;
	using System.Configuration;
	
	using NUnit.Framework;

	/// <summary>
	/// Base class for tests cases using the ASP.Net Runtime 
	/// to run the web project
	/// </summary>
	public abstract class AbstractMRTestCase
	{
		private static readonly String PhysicalWebDirConfigKey = "web.physical.dir";
		private static readonly String VirtualWebDirConfigKey = "web.virtual.dir";

		private WebAppHost host;
		private TestRequest request;
		private TestResponse response;
		private StringBuilder outputBuffer = new StringBuilder();

		#region Test Lifecycle

		[TestFixtureSetUp]
		public virtual void FixtureInitialize()
		{
			String virDir = GetVirtualDir();
			String physicalDir = GetPhysicalDir();

			if (!Path.IsPathRooted(physicalDir))
			{
				DirectoryInfo dinfo = new DirectoryInfo( Path.Combine( 
					AppDomain.CurrentDomain.SetupInformation.ApplicationBase, physicalDir ) );

				physicalDir = dinfo.FullName;
			}

			if (!Directory.Exists( Path.Combine(physicalDir, "bin") ) || 
				!File.Exists( Path.Combine(physicalDir, "web.config") ) )
			{
				String message = String.Format("The path specified for the " + 
					"web project doesnt look as a web project dir (bin directory or web.config missing): {0}", physicalDir);
				throw new ConfigurationException(message);
			}

			host = (WebAppHost) ApplicationHost.CreateApplicationHost( 
				typeof(WebAppHost), virDir, physicalDir );

			host.Configure(virDir, physicalDir);
		}

		[SetUp]
		public virtual void Initialize()
		{
			request = new TestRequest();
		}

		[TearDown]
		public virtual void Terminate()
		{
			outputBuffer.Length = 0;
		}

		[TestFixtureTearDown]
		public virtual void FixtureTerminate()
		{
			if (host != null) host.Dispose();
		}

		#endregion

		#region Actions

		/// <summary>
		/// Performs a GET operation on 
		/// </summary>
		/// <param name="path">The resource being request, for example <c>home/index.rails</c></param>
		/// <param name="queryStringParams">A list of key/value pair, for example <c>name=johndoe</c></param>
		public void DoGet(String path, params String[] queryStringParams)
		{
			AssertPathIsValid(path);

			if (queryStringParams.Length != 0) Request.QueryStringParams = queryStringParams;

			outputBuffer.Length = 0;

			Request.Url = path;

			StringWriter writer = new StringWriter(outputBuffer);

			response = host.Process( Request, writer );
		}

		/// <summary>
		/// Performs a Post operation on 
		/// </summary>
		/// <param name="path">The resource being request, for example <c>home/index.rails</c></param>
		/// <param name="postStringParams">A list of key/value pair, for example <c>name=johndoe</c></param>
		public void DoPost(String path, params String[] postStringParams)
		{
			if (postStringParams.Length != 0) Request.PostParams = postStringParams;

			outputBuffer.Length = 0;
			
			int pos = path.IndexOf('?');
			if( pos > -1 )
			{
				string qs = path.Substring( pos + 1 );
				path = path.Substring(0, pos );
				Request.QueryStringParams = qs.Split('&');
			}

			Request.Url = path;
			Request.Verb = "POST";
			
			StringWriter writer = new StringWriter(outputBuffer);

			response = host.Process( Request, writer );
		}

        /// <summary>
        /// Performs a Head operation on 
        /// </summary>
        /// <param name="path">The resource being request, for example <c>home/index.rails</c></param>
        /// <param name="postStringParams">A list of key/value pair, for example <c>name=johndoe</c></param>
        public void DoHead(String path, params String[] postStringParams)
        {
            if (postStringParams.Length != 0) Request.PostParams = postStringParams;

            outputBuffer.Length = 0;

            int pos = path.IndexOf('?');
            if (pos > -1)
            {
                string qs = path.Substring(pos + 1);
                path = path.Substring(0, pos);
                Request.QueryStringParams = qs.Split('&');
            }

            Request.Url = path;
            Request.Verb = "HEAD";

            StringWriter writer = new StringWriter(outputBuffer);

            response = host.Process(Request, writer);
        }
		

		private void AssertPathIsValid(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path", "Can't test a null path");
			if(path.Length==0)
				throw new ArgumentException("Can't test an empty path","path");
			if(path[0] == '/')
				throw new ArgumentException("Path mustn't start with a '/'!");
			if(path.IndexOf('?')!=-1)
				throw new ArgumentException("Path cannot contain query parameters! Pass them seperatedly","path");
		}

		#endregion

		#region Properties

		public TestRequest Request
		{
			get { return request; }
		}

		public TestResponse Response
		{
			get { return response; }
		}

		public String Output
		{
			get { return outputBuffer.ToString(); }
		}

		#endregion

		#region Available Asserts

		protected void AssertStatusCode(int expectedCode)
		{
			Assert.IsNotNull(response, "No requests performed with DoGet or DoPost (?)");
			Assert.IsTrue(response.StatusCode == expectedCode, "Expecting status code {0} when it was in fact {1} {2}", 
				expectedCode, response.StatusCode, response.StatusDescription);
		}
		
		/// <summary>
		/// Asserts the return status code is less than 400
		/// </summary>
		protected void AssertSuccess()
		{
			Assert.IsNotNull(response, "No requests performed with DoGet or DoPost (?)");
			Assert.IsTrue(response.StatusCode < 400, "Expecting status code < 400 when it was in fact {0} - {1}", 
				response.StatusCode, response.StatusDescription);
		}

		/// <summary>
		/// Asserts that reply has exactly the samme 
		/// content of <c>expectedContents</c>
		/// </summary>
		/// <param name="expectedContents"></param>
		protected void AssertReplyEqualTo(String expectedContents)
		{
			Assert.AreEqual( expectedContents, Output, "Reply differs. Expecting {0} but was {1}", expectedContents, Output );
		}

		/// <summary>
		/// Asserts that reply starts with 
		/// <c>expectedContents</c>
		/// </summary>
		protected void AssertReplyStartsWith(String contents)
		{
			String buffer = Output;

			Assert.IsTrue( buffer.StartsWith(contents), 
				String.Format("Reply string did not start with '{0}'. It was '{1}'", Prepare(contents),
                    Prepare(buffer.Substring(0, Math.Min(contents.Length, buffer.Length)))));
		}

		/// <summary>
		/// Asserts that reply ends with 
		/// <c>expectedContents</c>
		/// </summary>
		protected void AssertReplyEndsWith(String contents)
		{
			String buffer = Output;

			Assert.IsTrue( buffer.EndsWith(contents), 
				"Reply string did not end with '{0}'. It was '{1}'", contents,
					buffer.Substring(0, Math.Min(contents.Length, buffer.Length) ) );
		}

		/// <summary>
		/// Asserts that reply contains the specified
		/// <c>expectedContents</c>
		/// </summary>
		protected void AssertReplyContains(String contents)
		{
			Assert.IsTrue( Output.IndexOf(contents) != -1, 
				"AssertReplyContains did not find the content '{0}'. Raw content '{1}'", contents, Output );
		}

		/// <summary>
		/// Asserts that reply have only whitespace characters
		/// </summary>
		protected void AssertReplyIsBlank()
		{
			string contents = Output.Trim();

			Assert.IsTrue( contents == String.Empty, 
				"AssertReplyIsBlank found not whitespace characters '{0}'", contents);
		}

		/// <summary>
		/// Asserts that reply contents match the specified pattern, ignoring any whitespaces
		/// <c>pattern</c>
		/// </summary>
		protected void AssertReplyMatch(String pattern)
		{
			AssertReplyMatch(pattern, true, RegexOptions.None);
		}

		/// <summary>
		/// Asserts that reply contents match the specified pattern
		/// <c>pattern</c>
		/// </summary>
		protected void AssertReplyMatch(String pattern, bool ignoreSpaces)
		{						
			AssertReplyMatch(pattern, ignoreSpaces, RegexOptions.None);
		}

		/// <summary>
		/// Asserts that reply contents match the specified pattern
		/// <c>pattern</c>
		/// </summary>
		protected void AssertReplyMatch(String pattern, bool ignoreSpaces, RegexOptions options)
		{
			string contents = Output;

			if (ignoreSpaces)
			{
				contents = Regex.Replace(contents, @"\s+", "");
				pattern  = Regex.Replace(pattern,  @"\s+", "");
			}

			Regex re = new Regex(pattern, options);

			Assert.IsTrue( re.IsMatch(contents), 
				"AssertReplyMatch did not match pattern '{0}'. Raw content {1}", pattern, contents );
		}

		/// <summary>
		/// Asserts that reply does not contain
		/// <c>expectedContents</c>
		/// </summary>
		protected void AssertReplyDoesNotContain(String contents)
		{
			Assert.IsTrue( Output.IndexOf(contents) == -1, 
				"AssertReplyDoNotContain found the content '{0}'", contents );
		}

		/// <summary>
		/// Asserts that the response was a redirect to the specified
		/// <c>url</c>
		/// </summary>
		protected void AssertRedirectedTo(String url)
		{
			Assert.AreEqual(302, Response.StatusCode, "Redirect status not used");
			AssertHasHeader("Location");
			Assert.AreEqual(url, Response.Headers["Location"]);
		}

		protected void AssertContentTypeEqualsTo(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.AreEqual(expectedContentType, Response.Headers["Content-Type"]);
		}

		protected void AssertContentTypeStartsWith(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.IsTrue( Response.Headers["Content-Type"].ToString().StartsWith(expectedContentType) );
		}

		protected void AssertContentTypeEndsWith(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.IsTrue( Response.Headers["Content-Type"].ToString().EndsWith(expectedContentType) );
		}

		protected void AssertHasHeader(String headerName)
		{
			Assert.IsTrue( Response.Headers[headerName] != null, 
				"Header '{0}' was not found", headerName );
		}

		protected void AssertPropertyBagContains(String entryKey)
		{
			Assert.IsNotNull(response.PropertyBag, "PropertyBag could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.PropertyBag.Contains(entryKey), "Entry {0} was not on PropertyBag", entryKey);
		}

		protected void AssertPropertyBagEntryEquals(String entryKey, object expectedValue)
		{
			AssertPropertyBagContains(entryKey);
			Assert.AreEqual(expectedValue, response.PropertyBag[entryKey], "PropertyBag entry differs from the expected");
		}

		protected void AssertFlashContains(String entryKey)
		{
			Assert.IsNotNull(response.Flash, "Flash could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.Flash.Contains(entryKey), "Entry {0} was not on Flash", entryKey);
		}

		protected void AssertFlashDoesNotContain(String entryKey)
		{
			Assert.IsNotNull(response.Flash, "Flash could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsFalse(response.Flash.Contains(entryKey), "Entry {0} was on Flash", entryKey);
		}

		protected void AssertFlashEntryEquals(String entryKey, object expectedValue)
		{
			AssertFlashContains(entryKey);
			Assert.AreEqual(expectedValue, response.Flash[entryKey], "Flash entry differs from the expected");
		}

		protected void AssertSessionContains(String entryKey)
		{
			Assert.IsNotNull(response.Session, "Session could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.Session.Contains(entryKey), "Entry {0} was not on Session", entryKey);
		}

		protected void AssertSessionDoesNotContain(String entryKey)
		{
			Assert.IsNotNull(response.Session, "Session could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsFalse(response.Session.Contains(entryKey), "Entry {0} was on Session", entryKey);
		}

		protected void AssertSessionEntryEqualsTo(String entryKey, object expectedValue)
		{
			AssertSessionContains(entryKey);
			Assert.AreEqual(expectedValue, response.Session[entryKey], "Session entry differs from the expected");
		}

		protected void AssertHasCookie(String cookieName)
		{
			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName)) return;
			}

			Assert.Fail( "Cookie '{0}' was not found", cookieName );
		}

		protected void AssertCookieValueEqualsTo(String cookieName, String expectedValue)
		{
			AssertHasCookie(cookieName);

			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName))
				{
					Assert.AreEqual(expectedValue, cookie.Value);
					break;
				}
			}
		}

		protected void AssertCookieExpirationEqualsTo(String cookieName, DateTime expectedExpiration)
		{
			AssertHasCookie(cookieName);

			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName))
				{
					Assert.AreEqual(expectedExpiration.Day, cookie.Expires.Day, "Expiration day differs. Expecting {0} but was {1}", expectedExpiration.Day, cookie.Expires.Day);
					Assert.AreEqual(expectedExpiration.Month, cookie.Expires.Month, "Expiration month differs. Expecting {0} but was {1}", expectedExpiration.Month, cookie.Expires.Month);
					Assert.AreEqual(expectedExpiration.Year, cookie.Expires.Year, "Expiration year differs. Expecting {0} but was {1}", expectedExpiration.Year, cookie.Expires.Year);
					Assert.AreEqual(expectedExpiration.Hour, cookie.Expires.Hour, "Expiration hour differs. Expecting {0} but was {1}", expectedExpiration.Hour, cookie.Expires.Hour);
					Assert.AreEqual(expectedExpiration.Minute, cookie.Expires.Minute, "Expiration minute differs. Expecting {0} but was {1}", expectedExpiration.Minute, cookie.Expires.Minute);
					// Assert.AreEqual(expectedExpiration.Second, cookie.Expires.Second, "Expiration second differs. Expecting {0} but was {1}", expectedExpiration.Second, cookie.Expires.Second);
					break;
				}
			}
		}

		#endregion

		#region Overridables

		protected virtual string GetPhysicalDir()
		{
			String dir = ConfigurationSettings.AppSettings[PhysicalWebDirConfigKey];

			if (dir == null)
			{
				String message = String.Format("Could not find a configuration key " + 
					"defining the web application physical directory. You must create " + 
					"a key ('{0}') on your configuration file or override the method " + 
					"AbstractMRTestCase.GetPhysicalDir", PhysicalWebDirConfigKey);

				throw new ConfigurationException(message);
			}

			return dir;
		}

		protected virtual string GetVirtualDir()
		{
			String dir = ConfigurationSettings.AppSettings[VirtualWebDirConfigKey];

			if (dir == null)
			{
				dir = "/";
			}

			return dir;
		}

		#endregion

        private String Prepare(String content)
        {
            if (content == null || content.Length == 0) return String.Empty;

            return content.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
	}
}
