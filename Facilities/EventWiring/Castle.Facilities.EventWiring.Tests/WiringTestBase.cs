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

namespace Castle.Facilities.EventWiring.Tests
{
	using Castle.Facilities.EventWiring.Tests.Model;
	using Castle.Windsor;
	using NUnit.Framework;

	public abstract class WiringTestBase
	{
		protected WindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer(ConfigHelper.ResolvePath(GetConfigFile()));
		}

		protected abstract string GetConfigFile();

		[Test]
		public void TriggerSimple()
		{
			SimplePublisher publisher = (SimplePublisher)container["SimplePublisher"];
			SimpleListener listener = (SimpleListener)container["SimpleListener"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void TriggerStaticEvent()
		{
			SimplePublisher publisher = (SimplePublisher)container["SimplePublisher"];
			SimpleListener listener = (SimpleListener)container["SimpleListener2"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.StaticTrigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}
	}
}
