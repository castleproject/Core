namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;
	using System.IO;
	using System.Net;
	using NUnit.Framework;
	using System.Diagnostics;

	[TestFixture, Explicit]
	public class AsyncIntegrationTestCase
	{
		private Process server;

		[TestFixtureSetUp]
		public void StartServer()
		{
			string webDevPath2005 =
				Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework\v2.0.50727\Webdev.WebServer.exe");
			string webDevPath2008 =
				Environment.ExpandEnvironmentVariables(@"%CommonProgramFiles%\microsoft shared\DevServer\9.0\WebDev.WebServer.exe");

			string webDevPath;

			if (File.Exists(webDevPath2005))
				webDevPath = webDevPath2005;
			else if (File.Exists(webDevPath2008))
				webDevPath = webDevPath2008;
			else
			{
				Assert.Ignore("Could not find WebDev path");
				return;
			}

			string path;
			string currentDirectory = Path.GetFileNameWithoutExtension((AppDomain.CurrentDomain.BaseDirectory));
			if (currentDirectory == "bin")
				path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../TestSiteBrail");
			else // assume that we are on the build directory
				path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../MonoRail/TestSiteBrail");

			server = Process.Start(webDevPath, "/port:9999 /path:\"" + Path.GetFullPath(path) + "\"");
		}

		[TestFixtureTearDown]
		public void StopServer()
		{
			server.Kill();
		}
		

		[Test]
		public void CanGetResponseFromAsyncController()
		{
			string output = GetResponse("http://localhost:9999/async/index.rails");
			Assert.AreEqual("value from async task", output);
		}


		[Test]
		public void CanGetResponseFromAsyncController_WhenActionHasView()
		{
			string output = GetResponse("http://localhost:9999/async/WithView.rails");
			Assert.IsTrue(output.Contains("<p>value from async task</p>"));
		}

		[Test]
		public void CanGetResponseFromAsyncController_WhenActionHasViewAndLayout()
		{
			string output = GetResponse("http://localhost:9999/async/WithView.rails");
			Assert.AreEqual(@"
Welcome!
<p><p>value from async task</p></p>
Footer", output);
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithRescue_OnBegin()
		{
			string output = GetErrorResponse("http://localhost:9999/async/WithRescueBegin.rails");
			Assert.AreEqual(@"
Welcome!
<p>test error on rescue</p>
Footer", output);
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithRescue_OnAsync()
		{
			string output = GetErrorResponse("http://localhost:9999/async/WithRescueAsync.rails");
			Assert.AreEqual(@"
Welcome!
<p>error from async</p>
Footer", output);
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithRescue_OnEnd()
		{
			string output = GetErrorResponse("http://localhost:9999/async/WithRescueEnd.rails");
			Assert.AreEqual(@"
Welcome!
<p>error from end</p>
Footer", output);
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithError_OnBegin()
		{
			string output = GetErrorResponse("http://localhost:9999/async/ErrorBegin.rails");
			Assert.IsTrue(output.Contains("test error on begin"));
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithError_OnEnd()
		{
			string output = GetErrorResponse("http://localhost:9999/async/ErrorEnd.rails");
			Assert.IsTrue(output.Contains("test error from end"));
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithError_OnAysnc()
		{
			string output = GetErrorResponse("http://localhost:9999/async/ErrorAsync.rails");
			Assert.IsTrue(output.Contains("error from async"));
		}

		[Test]
		public void CanGetResponseFromAsyncController_RescueOnBeginActionLayout()
		{
			string output = GetErrorResponse("http://localhost:9999/async/RescueOnBeginActionLayout.rails");
			Assert.AreEqual(@"start modified
blah
end", output);
		}


		[Test]
		public void CanGetResponseFromAsyncController_WithActionLayout()
		{
			string output = GetResponse("http://localhost:9999/async/WithActionLayout.rails");
			Assert.AreEqual(@"start modified
value from async task
end",output);
		}

		[Test]
		public void CanGetResponseFromAsyncController_WithParams()
		{
			string output = GetResponse("http://localhost:9999/async/WithParams.rails?id=15&name=ayende");
			Assert.AreEqual(@"
Welcome!
<p>id: 15, name: ayende, value: value from async task</p>
Footer", output);
		}

		private string GetResponse(string url)
		{
			return new WebClient().DownloadString(url);
		}

		private string GetErrorResponse(string url)
		{
			try
			{
				new WebClient().DownloadString(url);
				throw new AssertionException("Expceted an error before this");
			}
			catch(WebException e)
			{
				using (TextReader tr = new StreamReader(e.Response.GetResponseStream()))
					return tr.ReadToEnd();
			}
		}
	}
}
