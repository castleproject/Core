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

namespace Castle.CastleOnRails.WindsorExtension.Tests
{
	using System;
	using System.IO;
	using System.Net;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine.Tests;

	[TestFixture]
	public class BasicFunctionalityTestCase : AbstractCassiniTestCase
	{
		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteWindsor" );
		}

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
		public void Filter()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/registration/index.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("Login page", response);
		}
	}
}
