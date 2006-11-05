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

	/// <summary>
	/// Represents the SMTP configuration
	/// on the configuration file
	/// </summary>
	public class SmtpConfig : ISerializedConfig
	{
		private String host = "localhost";
		private int port = 25;
		private String username;
		private String password;

		/// <summary>
		/// Deserializes the specified smtp section.
		/// </summary>
		/// <param name="section">The smtp section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlAttribute smtpHostAtt = section.Attributes["smtpHost"];
			XmlAttribute smtpPortAtt = section.Attributes["smtpPort"];
			XmlAttribute smtpUserAtt = section.Attributes["smtpUsername"];
			XmlAttribute smtpPwdAtt = section.Attributes["smtpPassword"];

			if (smtpHostAtt != null && smtpHostAtt.Value != String.Empty)
			{
				host = smtpHostAtt.Value;
			}
			if (smtpPortAtt != null && smtpPortAtt.Value != String.Empty)
			{
				port = int.Parse(smtpPortAtt.Value);
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

		/// <summary>
		/// Gets or sets the smtp host.
		/// </summary>
		/// <value>The host.</value>
		public String Host
		{
			get { return host; }
			set { host = value; }
		}

		/// <summary>
		/// Gets or sets the smtp port.
		/// </summary>
		/// <value>The port.</value>
		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		/// <summary>
		/// Gets or sets the smtp username.
		/// </summary>
		/// <value>The username.</value>
		public String Username
		{
			get { return username; }
			set { username = value; }
		}

		/// <summary>
		/// Gets or sets the smtp password.
		/// </summary>
		/// <value>The password.</value>
		public String Password
		{
			get { return password; }
			set { password = value; }
		}
	}
}