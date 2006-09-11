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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Xml;

	public class SmtpConfig : ISerializedConfig
	{
		private String host = "localhost";
		private String username, password;

		public void Deserialize(XmlNode section)
		{
			XmlAttribute smtpHostAtt = section.Attributes["smtpHost"];
			XmlAttribute smtpUserAtt = section.Attributes["smtpUsername"];
			XmlAttribute smtpPwdAtt = section.Attributes["smtpPassword"];

			if (smtpHostAtt != null && smtpHostAtt.Value != String.Empty)
			{
				host = smtpHostAtt.Value;
			}
			if (smtpUserAtt != null && smtpUserAtt.Value != String.Empty)
			{
				username = smtpUserAtt.Value;
			}
			if (smtpPwdAtt != null && smtpPwdAtt.Value != String.Empty)
			{
				password = smtpPwdAtt.Value;
			}		
		}

		public String Host
		{
			get { return host; }
			set { host = value; }
		}

		public String Username
		{
			get { return username; }
			set { username = value; }
		}

		public String Password
		{
			get { return password; }
			set { password = value; }
		}
	}
}
