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
	using System;
	using Castle.Facilities.EventWiring.Tests.Model;
	using NUnit.Framework;

	[TestFixture]
	public class SingletonComponentsTestCase : WiringTestBase
	{
		protected override string GetConfigFile()
		{
			return "config/singleton.config";
		}

		[Test]
		public void MultiEvents()
		{
			MultiListener listener = (MultiListener)container["MultiListener"];
			
			
			PublisherListener publisherThatListens = (PublisherListener)container["PublisherListener"];

			publisherThatListens.Trigger1();
			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisherThatListens, listener.Sender);

			listener.Reset();
			Assert.IsFalse(publisherThatListens.Listened);
			Assert.IsNull(publisherThatListens.Sender);

			
			SimplePublisher anotherPublisher = (SimplePublisher)container["SimplePublisher"];
			
			anotherPublisher.Trigger();
			Assert.IsTrue(publisherThatListens.Listened);
			Assert.AreSame(anotherPublisher, publisherThatListens.Sender);

			listener.Reset();
			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			
			MultiPublisher publisher = (MultiPublisher)container["MultiPublisher"];
			
			publisher.Trigger1();
			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);

			listener.Reset();
			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.Trigger2();
			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void TriggerThenResolve()
		{
			SimplePublisher publisher = (SimplePublisher)container["SimplePublisher"];
			
			publisher.Trigger();
			
			SimpleListener listener = (SimpleListener)container["SimpleListener"];
			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}
	}
}
