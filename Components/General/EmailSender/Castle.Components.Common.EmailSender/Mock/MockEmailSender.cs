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

namespace Castle.Components.Common.EmailSender.Mock
{
	using System;


	public class MockEmailSender : IEmailSender
	{
		public MockEmailSender()
		{
		}

		public void Send(String from, String to, String subject, String messageText)
		{
			
		}

		public void Send(Message message)
		{
			
		}

		public void Send(Message[] messages)
		{
			
		}
	}
}
