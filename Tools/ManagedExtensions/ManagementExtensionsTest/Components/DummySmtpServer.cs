// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Test.Components
{
	using System;

	using Castle.ManagementExtensions;

	/// <summary>
	/// Summary description for DummySmtpServer.
	/// </summary>
	public class DummySmtpServer : MDynamicSupport
	{
		protected bool Started = false;
		protected int port = 1088;

		public DummySmtpServer()
		{
		}

		public void Start()
		{
			Started = true;
		}

		public void Stop()
		{
			Started = false;
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}
	
		#region MDynamicSupport Members

		public object Invoke(string action, object[] args, Type[] signature)
		{
			if (action.Equals("Stop"))
			{
				Stop();
			}
			else if (action.Equals("Start"))
			{
				Start();
			}
			
			return null;
		}

		public void SetAttributeValue(string name, object value)
		{
			if (name.Equals("Port"))
			{
				Port = (int) value;
			}
		}

		public ManagementInfo Info
		{
			get
			{
				ManagementInfo info = new ManagementInfo();

				info.Operations.Add(new ManagementOperation("Start"));
				info.Operations.Add(new ManagementOperation("Stop"));
				info.Attributes.Add(new ManagementAttribute("Port", typeof(int)));

				return info;
			}
		}

		public object GetAttributeValue(string name)
		{
			if (name.Equals("Port"))
			{
				return Port;
			}

			return null;
		}

		#endregion
	}
}
