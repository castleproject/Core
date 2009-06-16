// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework.Tests;
	using Castle.MonoRail.TestSupport;
	using Components.Common.EmailSender;
	using NUnit.Framework;

	[TestFixture]
	public class EmailTestCase : BaseViewOnlyTestFixture
	{		
		[Test]
		public void CorrectEmailRendering()
		{
			Message msg = Controller.RenderMailMessage("myemail1", null, new Hashtable());
			PropertyBag.Add("msg", msg);

			ProcessView_StripRailsExtension("email/sendemail.rails");
			AssertSuccess();
			AssertReplyEqualTo("Castle.Components.Common.EmailSender.Message\r\n" + 
				"hammett@gmail.com\r\nhammett@apache.org\r\nhammett@uol.com.br" + 
				"\r\nSome subject\r\nNormal\r\n0\r\nHtml\r\n\r\n<html>\r\nThis is the message content\r\n</html>\r\n");
		}

		[Test]
		public void CorrectEmailRendering2()
		{
			Message msg = Controller.RenderMailMessage("myemail2", null, new Hashtable());
			PropertyBag.Add("msg", msg);

			ProcessView_StripRailsExtension("email/sendemail.rails");
			AssertSuccess();
			AssertReplyEqualTo("Castle.Components.Common.EmailSender.Message\r\n" + 
				"hammett@gmail.com\r\n\r\n" + 
				"\r\nSome subject\r\nNormal\r\n0\r\nText\r\n\r\nThis is the message content\r\n");
		}
	}
}
