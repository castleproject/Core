// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Handlers
{
	using System;
	using System.IO;
	using System.Web;
	using Castle.Core.Resource;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class ResourceFileHandlerTestCase
	{
		[Test]
		public void IfFileDoesNotExistsSetsStatusTo404()
		{
			DefaultStaticResourceRegistry registry = new DefaultStaticResourceRegistry();

			ResourceFileHandler handler = new ResourceFileHandler(new UrlInfo("", "controller", "action"), registry);

			StringWriter writer = new StringWriter();

			HttpResponse res = new HttpResponse(writer);
			HttpRequest req = new HttpRequest(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt"),
					"http://localhost:1333/controller/action", "");

			handler.ProcessRequest(new HttpContext(req, res));

			Assert.AreEqual(404, res.StatusCode);
		}

		[Test]
		public void ReturnsResourceContentAndSetMimeType()
		{
			DefaultStaticResourceRegistry registry = new DefaultStaticResourceRegistry();
			registry.RegisterCustomResource("key", null, null, new StaticContentResource("js"), "text/javascript", null);

			ResourceFileHandler handler = new ResourceFileHandler(new UrlInfo("", "controller", "key"), registry);

			StringWriter writer = new StringWriter();

			HttpResponse res = new HttpResponse(writer);
			HttpRequest req = new HttpRequest(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory, "Handlers/Files/simplerequest.txt"),
					"http://localhost:1333/controller/action", "");

			handler.ProcessRequest(new HttpContext(req, res));

			Assert.AreEqual(200, res.StatusCode);
			Assert.AreEqual("text/javascript", res.ContentType);
			Assert.AreEqual("js", writer.GetStringBuilder().ToString());
		}
	}
}