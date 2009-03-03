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

namespace Castle.ActiveRecord.Tests.Event
{
	using NUnit.Framework;
	using NHibernate.Event;
	using Castle.ActiveRecord.Framework;
	using System;
	using System.Collections.Generic;

	[TestFixture]
	public class NHEventListenersTest
	{
		[Test]
		public void GetEventListenerTypes_returns_at_least_one_type()
		{
			List<Type> types = new List<Type>(NHEventListeners.GetEventListenerTypes());
			Assert.Greater(types.Count, 0);
		}

		[Test]
		public void GetEventListenerTypes_returns_only_interfaces()
		{
			foreach (var type in NHEventListeners.GetEventListenerTypes())
			{
				Assert.IsTrue(type.IsInterface);
			}
		}

		[Test]
		public void GetEventListenerTypes_returns_only_event_listeners()
		{
			foreach (var type in NHEventListeners.GetEventListenerTypes())
			{
				Assert.IsTrue(type.Name.EndsWith("EventListener"));
			}
		}

		[Test]
		public void NHEventListeners_serves_collection_semantics_for_event_listeners()
		{
			var sut = new NHEventListeners();

			var listener1 = new StubEventListener();
			var listener2 = new StubEventListener();

			sut.Add(listener1);

			Assert.AreEqual(1, (new List<IPreUpdateEventListener>(sut.Enumerate<IPreUpdateEventListener>())).Count);
			Assert.IsTrue(sut.Contains(listener1));
			Assert.IsFalse(sut.Contains(listener2));

			sut.Remove(listener2);
			Assert.AreEqual(1, (new List<IPreUpdateEventListener>(sut.Enumerate<IPreUpdateEventListener>())).Count);
			Assert.IsTrue(sut.Contains(listener1));

			sut.Remove(listener1);
			Assert.AreEqual(0, (new List<IPreUpdateEventListener>(sut.Enumerate<IPreUpdateEventListener>())).Count);
			Assert.IsFalse(sut.Contains(listener1));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Add_works_only_for_event_listeners()
		{
			new NHEventListeners().Add("String instance");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Remove_works_only_for_event_listeners()
		{
			new NHEventListeners().Remove("String instance");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Contains_works_only_for_event_listeners()
		{
			new NHEventListeners().Contains("String instance");
		}

		private class StubEventListener : IPreUpdateEventListener
		{
			public bool OnPreUpdate(PreUpdateEvent @event)
			{
				return true;
			}
		}


	}
}
