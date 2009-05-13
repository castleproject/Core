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

namespace EventWiring
{
	using System;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;


	public class App
	{
		public static void Main()
		{
			IWindsorContainer container = new WindsorContainer( new XmlInterpreter("../config.xml") );

			SimplePublisher source = container.Resolve<SimplePublisher>();
			source.Trigger();
		}
	}

	public class SimplePublisher
	{
		public event EventHandler Event;

		public SimplePublisher() {
		}

		public void Trigger() {
			if (Event != null) {
				Event(this, new EventArgs());
			}
		}
	}

	public class SimpleListener
	{
		public SimpleListener() {
		}

		public void OnPublishEvent(object sender, EventArgs e) {
			Console.WriteLine("OnPublishEvent");
		}
	}

}
