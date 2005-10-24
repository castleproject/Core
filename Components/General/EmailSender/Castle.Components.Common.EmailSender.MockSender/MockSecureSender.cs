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

namespace Castle.Components.Common.EmailSender.MockSender
{
    using System;


    public class MockSecureSender : IEmailSender
    {
        public MockSecureSender()
        {
        }

        public void Send(String from, String to, String subject, String messageText)
        {
			
        }

        public void Send(Message message)
        {
			if(!CheckForAuthenticationHeaders(message))
			{
			    throw new ApplicationException("Email does not contain the necessary SMTP Authentication Fields. See http://www.systemwebmail.com/faq/3.8.aspx");
			}
        }

        public void Send(Message[] messages)
        {
			foreach(Message message in messages)
			{
			    Send(message);
			}

        }

        private bool CheckForAuthenticationHeaders(Message message)
        {
            bool result = false;

            if(message.Fields.Contains("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate") &&
                message.Fields.Contains("http://schemas.microsoft.com/cdo/configuration/sendusername") && 
                message.Fields.Contains("http://schemas.microsoft.com/cdo/configuration/sendpassword"))
            {
                result = true;
            }

            return result;
        }
    }
}