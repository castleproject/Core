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

namespace ComplexConfiguration.Components
{
	using System;

	/// <summary>
	/// Summary description for SmtpEmailSender.
	/// </summary>
	public class SmtpEmailSender : IEmailSender
	{
		private String[] _hosts;

		public SmtpEmailSender(String[] hosts)
		{
			if (hosts == null) throw new ArgumentNullException("hosts");
			if (hosts.Length == 0) throw new ArgumentException("hosts is empty");

			_hosts = hosts;
		}

		public virtual void Send(String from, String to, String message)
		{
			Console.WriteLine( "Sending e-mail from {0} to {1} with message '{2}'", 
				from, to, message );
		}
	}
}
