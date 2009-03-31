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
	using System;
	using NHibernate.Event;
	using NUnit.Framework;

	public abstract class AbstractEventListenerTest:AbstractActiveRecordTest
	{
		protected static void AssertListenerWasRegistered<TEventListener>(Func<EventListeners, object[]> listenerTypeSelector)
		{
			AssertListenerWasRegistered<TEventListener, ActiveRecordBase>(listenerTypeSelector);
		}

		protected static void AssertListenerWasRegistered<TEventListener, TBaseClass>(
			Func<EventListeners, object[]> listenerTypeSelector)
		{
			Assert.IsTrue(
				ListenerWasRegistered<TEventListener, TBaseClass>(listenerTypeSelector),
				"Event listener class {0} was not registered", typeof (TEventListener).FullName);
		}

		protected static void AssertListenerWasNotRegistered<TEventListener>(Func<EventListeners, object[]> listenerTypeSelector)
		{
			AssertListenerWasNotRegistered<TEventListener, ActiveRecordBase>(listenerTypeSelector);
		}

		protected static void AssertListenerWasNotRegistered<TEventListener, TBaseClass>(
			Func<EventListeners, object[]> listenerTypeSelector)
		{
			Assert.IsFalse(
				ListenerWasRegistered<TEventListener, TBaseClass>(listenerTypeSelector),
				"Event listener class {0} was registered, but should not be", typeof (TEventListener).FullName);
		}

		private bool ListenerWasRegistered<TEventListener>(Func<EventListeners, object[]> listenerTypeSelector)
		{
			return ListenerWasRegistered<TEventListener, ActiveRecordBase>(listenerTypeSelector);
		}

		private static bool ListenerWasRegistered<TEventListener, TBaseClass>(Func<EventListeners, object[]> listenerTypeSelector)
		{
			object[] registeredListeners = GetRegisteredListeners<TBaseClass>(listenerTypeSelector);
			var found = false;
			foreach (var listener in registeredListeners)
			{
				if (typeof (TEventListener).Equals(listener.GetType()))
					found = true;
			}
			return found;
		}

		protected static object[] GetRegisteredListeners(Func<EventListeners, object[]> listenerTypeSelector)
		{
			return listenerTypeSelector(
				ActiveRecordMediator.GetSessionFactoryHolder().GetConfiguration(typeof (ActiveRecordBase)).EventListeners);
		}

		private static object[] GetRegisteredListeners<TBaseClass>(Func<EventListeners, object[]> listenerTypeSelector)
		{
			return listenerTypeSelector(
				ActiveRecordMediator.GetSessionFactoryHolder().GetConfiguration(typeof (TBaseClass)).EventListeners);
		}

		public override void Init()
		{
			base.Init();
			ActiveRecordStarter.ClearContributors();
		}

		public override void Drop()
		{
			ActiveRecordStarter.ClearContributors();
			base.Drop();
		}
	}
}