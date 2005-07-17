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

namespace Castle.Components.Common.EmailSender
{
	using System;

	/// <summary>
	/// Abstracts an approach to send e-mails
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		/// Sends a message. 
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="messageText"></param>
		void Send(String from, String to, String subject, String messageText);

		/// <summary>
		/// Sends a message. 
		/// </summary>
		/// <param name="message"></param>
		void Send(Message message);

		/// <summary>
		/// Sends multiple messages. 
		/// </summary>
		/// <param name="messages"></param>
		void Send(Message[] messages);
	}
}
