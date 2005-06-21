using System.IO;
using System.Net;
using Castle.MonoRail.Engine.Tests;
using NUnit.Framework;
// ${Copyrigth}

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;

	[TestFixture]
	public class ResourceTestCase : AbstractCassiniTestCase
	{
		public ResourceTestCase()
		{
		}

		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteNVelocity" );
		}

		[Test]
		public void GetResources()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/resourced/getresources.rails");
			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/resourced/getresources.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("testValue", response);
		}
	}
}
