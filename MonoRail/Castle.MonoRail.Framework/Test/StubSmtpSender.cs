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

namespace Castle.MonoRail.Framework.Test
{
	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// Represents a mock implementation of <see cref="IEmailSender"/> for unit test purposes.
	/// </summary>
	public class StubSmtpSender : IEmailSender
	{
		private readonly StubEngineContext context;

		/// <summary>
		/// Initializes a new instance of the <see cref="StubSmtpSender"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public StubSmtpSender(StubEngineContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <param name="from">From field</param>
		/// <param name="to">To field</param>
		/// <param name="subject">e-mail's subject</param>
		/// <param name="messageText">message's body</param>
		public void Send(string from, string to, string subject, string messageText)
		{
			Send(new Message(from, to, subject, messageText));
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <param name="message">Message instance</param>
		public void Send(Message message)
		{
			context.AddEmailMessageSent(message);
		}

		/// <summary>
		/// Sends multiple messages.
		/// </summary>
		/// <param name="messages">Array of messages</param>
		public void Send(Message[] messages)
		{
			foreach(Message message in messages)
			{
				context.AddEmailMessageSent(message);
			}
		}
	}
}