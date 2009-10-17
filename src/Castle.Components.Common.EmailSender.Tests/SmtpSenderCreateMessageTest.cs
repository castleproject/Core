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

namespace Castle.Components.Common.EmailSender.Tests
{
	using System.IO;
	using System.Net.Mail;
	using System.Text;
	using NUnit.Framework;
	using Smtp;

	[TestFixture]
	public class SmtpSenderCreateMessageTest
	{
		private SmtpSender smtpSender;
		private readonly string attachmentFilename = Path.GetTempFileName();

		[SetUp]
		public void TestSetup()
		{
			smtpSender = new SmtpSender("localhost");
			File.WriteAllText(attachmentFilename, "Test Data");
		}

		[TearDown]
		public void TestCleanup()
		{
			if (File.Exists(attachmentFilename))
			{
				File.Delete(attachmentFilename);
			}
		}

		[Test]
		public void Ensure_CreateMessage_assigns_correct_mime_type_to_attachments_when_supplied_with_data_stream()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("Attachment.txt", "text/plain", GetTestData()));

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual("text/plain", mailMsg.Attachments[0].ContentType.MediaType);
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_filename_to_attachments_when_supplied_with_data_stream()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("Attachment.txt", "text/plain", GetTestData()));

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual("Attachment.txt", mailMsg.Attachments[0].Name);
			}
		}

		[Test]
		public void Ensure_CreateMessage_assigns_correct_mime_type_to_attachments_when_supplied_with_filename()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("text/plain", attachmentFilename));

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual("text/plain", mailMsg.Attachments[0].ContentType.MediaType);
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_filename_to_attachments_when_supplied_with_filename()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("text/plain", attachmentFilename));

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual(Path.GetFileName(attachmentFilename), mailMsg.Attachments[0].Name);
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_bcc_to_message_when_bcc_is_delimited()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body")
			{
				Bcc = "bcc@somewhere.com;also@somewhere.com"
			};

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(2, mailMsg.Bcc.Count, "Bcc parse count doesn't match");
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_cc_to_message_when_cc_is_delimited()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body")
			{
				Cc = "cc@somewhere.com;alsoTo@somewhere.com"
			};

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(2, mailMsg.CC.Count, "Cc parse count doesn't match");
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_cc_to_message_when_cc_is_not_delimited()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body")
			{
				Cc = "cc@somewhere.com"
			};

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.CC.Count, "Cc parse count doesn't match");
				Assert.AreEqual(new MailAddress("cc@somewhere.com"), mailMsg.CC[0], "Cc set doesn't match");
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_bcc_to_message_when_bcc_is_not_delimited()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body")
			{
				Bcc = "bcc@somewhere.com"
			};

			using (MailMessage mailMsg = smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Bcc.Count, "Bcc parse count doesn't match");
				Assert.AreEqual(new MailAddress("bcc@somewhere.com"), mailMsg.Bcc[0], "Bcc set doesn't match");
			}
		}

		private static Stream GetTestData()
		{
			byte[] b = Encoding.ASCII.GetBytes("Test Data");
			return new MemoryStream(b);
		}
	}
}