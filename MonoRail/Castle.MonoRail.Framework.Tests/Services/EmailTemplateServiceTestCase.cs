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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Castle.Components.Common.EmailSender;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;
	using Test;

	[TestFixture]
	public class EmailTemplateServiceTestCase
	{
		private readonly MockRepository mockRepository = new MockRepository();
		private EmailTemplateService service;
		private IViewEngineManager viewEngineManagerMock;
		private MockEngineContext engineContext;
		private DummyController controller;
		private ControllerContext controllerContext;

		[SetUp]
		public void Init()
		{
			viewEngineManagerMock = mockRepository.DynamicMock<IViewEngineManager>();

			service = new EmailTemplateService(viewEngineManagerMock);

			engineContext = new MockEngineContext(null, null, null, null);
			controller = new DummyController();
			controllerContext = new ControllerContext();
		}

		[Test]
		public void RenderMailMessage_BackwardCompatibility_PassOnControllerContext()
		{
			string templateName = "welcome";

			using(mockRepository.Record())
			{
				viewEngineManagerMock.Process(templateName, null, engineContext, controller, controllerContext);
				LastCall.Constraints(
					Is.Equal("mail\\" + templateName),
					Is.Anything(),
					Is.Same(engineContext),
					Is.Same(controller),
					Is.Same(controllerContext));
			}

			using(mockRepository.Playback())
			{
				service.RenderMailMessage(templateName, engineContext, controller, controllerContext, false);
			}
		}

		[Test]
		public void RenderMailMessage_BackwardCompatibility_UsesTemplateNameAsItIsIfStartsWithSlash()
		{
			string templateName = "/emailtemplates/welcome";

			using(mockRepository.Record())
			{
				viewEngineManagerMock.Process(templateName, null, engineContext, controller, controllerContext);
				LastCall.Constraints(
					Is.Equal(templateName),
					Is.Anything(),
					Is.Same(engineContext),
					Is.Same(controller),
					Is.Same(controllerContext));
			}

			using(mockRepository.Playback())
			{
				service.RenderMailMessage(templateName, engineContext, controller, controllerContext, false);
			}
		}

		[Test]
		public void RenderMailMessage_InvokesViewEngineManager()
		{
			string templateName = "welcome";
			Hashtable parameters = new Hashtable();

			using(mockRepository.Record())
			{
				viewEngineManagerMock.Process(templateName, "layout", null, null);
				LastCall.Constraints(
					Is.Equal("mail\\" + templateName),
					Is.Equal("layout"),
					Is.Anything(),
					Is.Anything());
			}

			using(mockRepository.Playback())
			{
				service.RenderMailMessage(templateName, "layout", parameters);
			}
		}

		[Test]
		public void RenderMailMessage_MessageIsConstructedCorrectly()
		{
			string templateName = "welcome";
			Hashtable parameters = new Hashtable();

			using(mockRepository.Record())
			{
				Expect.Call(delegate() { viewEngineManagerMock.Process(templateName, "layout", null, null); })
					.Constraints(
						Is.Equal("mail\\" + templateName),
						Is.Equal("layout"),
						Is.Anything(),
						Is.Anything())
					.Do(new Render(RendersEmail));
			}

			using(mockRepository.Playback())
			{
				Message message = service.RenderMailMessage(templateName, "layout", parameters);

				Assert.AreEqual("hammett@noemail.com", message.To);
				Assert.AreEqual("copied@noemail.com", message.Cc);
				Assert.AreEqual("bcopied@noemail.com", message.Bcc);
				Assert.AreEqual("contact@noemail.com", message.From);
				Assert.AreEqual("Hello!", message.Subject);
				Assert.AreEqual("This is the\r\nbody\r\n", message.Body);
				Assert.AreEqual(1, message.Headers.Count);
			}
		}

		public delegate void Render(
			string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters);

		public static void RendersEmail(string templateName, string layoutName, TextWriter output,
		                                IDictionary<string, object> parameters)
		{
			output.WriteLine("to: hammett@noemail.com");
			output.WriteLine("cc: copied@noemail.com");
			output.WriteLine("bcc: bcopied@noemail.com");
			output.WriteLine("from: contact@noemail.com");
			output.WriteLine("subject: Hello!");
			output.WriteLine("X-something: Mime-super-content");
			output.WriteLine("");
			output.WriteLine("This is the");
			output.WriteLine("body");
		}

		private class DummyController : Controller
		{
		}
	}
}