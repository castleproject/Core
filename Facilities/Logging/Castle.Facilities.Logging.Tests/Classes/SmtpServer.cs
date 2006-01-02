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

namespace Castle.Facilities.Logging.Tests.Classes
{
	using System;

	using Castle.Services.Logging;


	public class SmtpServer : ISmtpServer
	{
		private ILogger logger;

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public void Start()
		{
			Logger.Debug("Started");
		}

		public void Stop()
		{
			Logger.Debug("Stopped");
		}

		public void InternalSend(String from, String to, String contents)
		{
			Logger.Info("InternalSend {0} {1} {2}", from, to, contents);
		}
	}
}
