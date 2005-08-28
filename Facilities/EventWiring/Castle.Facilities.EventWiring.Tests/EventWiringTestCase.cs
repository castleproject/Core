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

namespace Castle.Facilities.EventWiring.Tests
{
	using Castle.Facilities.EventWiring.Tests.Model;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Configuration.Sources;
	using NUnit.Framework;

	[TestFixture]
	public class EventWiringTestCase
	{
		private WindsorContainer _container;

		public EventWiringTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			_container = new WindsorContainer(new XmlInterpreter(new AppDomainConfigSource()));

			_container.AddFacility("eventwiring", new EventWiringFacility());

			_container.AddComponent("SimpleListener", typeof(SimpleListener));
			_container.AddComponent("SimplePublisher", typeof(SimplePublisher));
			_container.AddComponent("MultiPublisher", typeof(MultiPublisher));
			_container.AddComponent("MultiListener", typeof(MultiListener));
			_container.AddComponent("PublisherListener", typeof(PublisherListener), typeof(PublisherListener));
			_container.AddComponent("BadConfig", typeof(SimpleListener));
		}

		[Test]
		public void TriggerSimple()
		{
			SimplePublisher publisher = (SimplePublisher)_container["SimplePublisher"];
			SimpleListener listener = (SimpleListener)_container["SimpleListener"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test, ExpectedException(typeof(EventWiringException))]
		public void EventNotFound()
		{
			object badConfigured = _container["BadConfig"];

			Assert.Fail();
		}

		[Test]
		public void MultiEvents()
		{
			MultiListener listener = (MultiListener)_container["MultiListener"];
			MultiPublisher publisher = (MultiPublisher)_container["MultiPublisher"];
			PublisherListener publisherThatListens = (PublisherListener)_container["PublisherListener"];
			SimplePublisher anotherPublisher = (SimplePublisher)_container["SimplePublisher"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.Trigger1();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);

			listener.Reset();

			publisher.Trigger2();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);

			listener.Reset();

			publisherThatListens.Trigger1();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisherThatListens, listener.Sender);


			Assert.IsFalse(publisherThatListens.Listened);
			Assert.IsNull(publisherThatListens.Sender);

			anotherPublisher.Trigger();

			Assert.IsTrue(publisherThatListens.Listened);
			Assert.AreSame(anotherPublisher, publisherThatListens.Sender);

		}
	}
}
