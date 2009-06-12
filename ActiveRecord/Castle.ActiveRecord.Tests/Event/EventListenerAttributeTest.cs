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

using System.Reflection;

namespace Castle.ActiveRecord.Tests.Event
{
	using Iesi.Collections;
	using System;
	using NUnit.Framework;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Tests.Model;
	using NHibernate.Event;

	[TestFixture]
	public class EventListenerAttributeTest : AbstractEventListenerTest
	{
		[Test]
		public void C1_ListenerMustBeAddedWhenInitialized()
		{
			InitializeSingleBase(typeof(SamplePreInsertListener));
			AssertListenerWasRegistered<SamplePreInsertListener>(e => e.PreInsertEventListeners);
		}

		[Test]
		public void C1_Listener_must_be_added_when_initialized_with_assembly()
		{
			ActiveRecordStarter.Initialize(Assembly.GetAssembly(typeof(AttributedPreLoadListener)), GetConfigSource());
			AssertListenerWasRegistered<AttributedPreLoadListener>(e=>e.PreLoadEventListeners);
		}

		[Test]
		public void C5_Expected_default_listener_is_present_in_configuration()
		{
			InitializeSingleBase();
			AssertListenerWasRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void C5_Default_listener_is_present_in_configuration_after_adding_custom_listener()
		{
			InitializeSingleBase(typeof(AdditionalLoadListener));
			AssertListenerWasRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<AdditionalLoadListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void U4_Default_listener_is_replaced_by_custom_listener_when_explicitly_demanded()
		{
			InitializeSingleBase(typeof(ReplacementLoadListener));
			AssertListenerWasNotRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<ReplacementLoadListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void U4_All_specified_listeners_are_present_when_replacing_existing_listeners()
		{
			InitializeSingleBase(typeof(ReplacementLoadListener), typeof(AdditionalLoadListener));
			AssertListenerWasNotRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<ReplacementLoadListener>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<AdditionalLoadListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void U2_Listeners_should_be_ignored_when_specified_through_attribute()
		{
			InitializeSingleBase(typeof(IgnoredListener));
			AssertListenerWasNotRegistered<IgnoredListener>(e => e.PreLoadEventListeners);
		}

		[Test]
		public void U2_Ignored_listeners_should_never_replace_existing_listeners()
		{
			InitializeSingleBase(typeof(IgnoredReplacementListener));
			AssertListenerWasRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
			AssertListenerWasNotRegistered<IgnoredReplacementListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void U3ab_One_listener_should_serve_multiple_events()
		{
			InitializeSingleBase(typeof(MultipleListener));
			AssertListenerWasRegistered<MultipleListener>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<MultipleListener>(e => e.PostLoadEventListeners);
		}

		[Test]
		public void U3c_Single_events_should_be_skipped()
		{
			InitializeSingleBase(typeof(MultipleSkippedListener));
			AssertListenerWasNotRegistered<MultipleSkippedListener>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<MultipleSkippedListener>(e => e.PostLoadEventListeners);
		}

		[Test]
		public void U3c_Skipped_events_must_not_replace_default_events_even_if_specified()
		{
			InitializeSingleBase(typeof(MultipleSkippedReplacementListener));
			AssertListenerWasNotRegistered<MultipleSkippedReplacementListener>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<MultipleSkippedReplacementListener>(e => e.DeleteEventListeners);
			AssertListenerWasRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e => e.LoadEventListeners);
		}

		[Test]
		public void U3d_Multiple_events_are_served_by_different_instances_by_default()
		{
			InitializeSingleBase(typeof(MultipleListener));
			Assert.AreNotSame(Array.Find(GetRegisteredListeners(e => e.PreLoadEventListeners), l => (l is MultipleListener)),
			                  Array.Find(GetRegisteredListeners(e => e.PostLoadEventListeners), l => (l is MultipleListener)));
		}

		[Test]
		public void U3d_Multiple_events_are_served_by_the_same_instance_when_singleton_is_defined()
		{
			InitializeSingleBase(typeof(MultipleSingletonListener));
			Assert.AreSame(Array.Find(GetRegisteredListeners(e => e.PreLoadEventListeners), l => (l is MultipleSingletonListener)),
							  Array.Find(GetRegisteredListeners(e => e.PostLoadEventListeners), l => (l is MultipleSingletonListener)));
		}

        [Test]
        public void Event_listeners_are_registered_only_once()
        {
            InitializeSingleBase(typeof(SamplePostInsertListener), typeof(SamplePostUpdateListener), typeof(SamplePostDeleteListener));
            Assert.AreEqual(1, Array.FindAll(GetRegisteredListeners(e => e.PostInsertEventListeners), l => (l is SamplePostInsertListener)).Length);
            Assert.AreEqual(1, Array.FindAll(GetRegisteredListeners(e => e.PostUpdateEventListeners), l => (l is SamplePostUpdateListener)).Length);
            Assert.AreEqual(1, Array.FindAll(GetRegisteredListeners(e => e.PostDeleteEventListeners), l => (l is SamplePostDeleteListener)).Length);
        }

		[Test]
		public void U1_Listeners_are_registered_for_all_configurations()
		{
			InitializeMultipleBases(typeof(SamplePreInsertListener));
			AssertListenerWasRegistered<SamplePreInsertListener, ActiveRecordBase>(e => e.PreInsertEventListeners);
			AssertListenerWasRegistered<SamplePreInsertListener, Test2ARBase>(e=>e.PreInsertEventListeners);
		}

		[Test]
		public void U1_Included_Listeners_are_registered()
		{
			InitializeMultipleBases(typeof(FirstBaseIncludeListener), typeof(SecondBaseIncludeListener));
			AssertListenerWasRegistered<FirstBaseIncludeListener, ActiveRecordBase>(e => e.PreLoadEventListeners);
			AssertListenerWasNotRegistered<SecondBaseIncludeListener, ActiveRecordBase>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<SecondBaseIncludeListener, Test2ARBase>(e => e.PreLoadEventListeners);
			AssertListenerWasNotRegistered<FirstBaseIncludeListener, Test2ARBase>(e => e.PreLoadEventListeners);
		}

		[Test]
		public void U1_Excluded_Listeners_are_not_registered()
		{
			InitializeMultipleBases(typeof(FirstBaseExcludeListener), typeof(SecondBaseExcludeListener));
			AssertListenerWasNotRegistered<FirstBaseExcludeListener, ActiveRecordBase>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<SecondBaseExcludeListener, ActiveRecordBase>(e => e.PreLoadEventListeners);
			AssertListenerWasNotRegistered<SecondBaseExcludeListener, Test2ARBase>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<FirstBaseExcludeListener, Test2ARBase>(e => e.PreLoadEventListeners);
		}

		[Test]
		public void U1_C5_U4_No_replacement_on_excluded_configs()
		{
			InitializeMultipleBases(typeof (SecondBaseReplacementLoadListener));
			AssertListenerWasNotRegistered<SecondBaseReplacementLoadListener, ActiveRecordBase>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<SecondBaseReplacementLoadListener, Test2ARBase>(e => e.LoadEventListeners);
			AssertListenerWasRegistered<NHibernate.Event.Default.DefaultLoadEventListener, ActiveRecordBase>(e => e.LoadEventListeners);
			AssertListenerWasNotRegistered<NHibernate.Event.Default.DefaultLoadEventListener, Test2ARBase>(e => e.LoadEventListeners);
		}

		#region Listeners

		[EventListener]
		private class SamplePreInsertListener : IPreInsertEventListener
		{
			public bool OnPreInsert(PreInsertEvent @event) {return true; }
		}

        [EventListener]
        private class SamplePostInsertListener : IPostInsertEventListener
        {
            public void OnPostInsert(PostInsertEvent @event) { }
        }

        [EventListener]
        private class SamplePostUpdateListener : IPostUpdateEventListener
        {
            public void OnPostUpdate(PostUpdateEvent @event) { }
        }

        [EventListener]
        private class SamplePostDeleteListener : IPostDeleteEventListener
        {
            public void OnPostDelete(PostDeleteEvent @event) { }
        }

		[EventListener]
		private class AdditionalLoadListener : ILoadEventListener
		{
			public void OnLoad(LoadEvent @event, LoadType loadType){}
		}

		[EventListener(ReplaceExisting = true)]
		private class ReplacementLoadListener : ILoadEventListener
		{
			public void OnLoad(LoadEvent @event, LoadType loadType){}
		}

		[EventListener(Ignore = true)]
		private class IgnoredListener : IPreLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event){}
		}

		[EventListener(Ignore = true, ReplaceExisting = true)]
		private class IgnoredReplacementListener : ILoadEventListener
		{
			public void OnLoad(LoadEvent @event, LoadType loadType){}
		}

		[EventListener]
		private class MultipleListener : IPreLoadEventListener, IPostLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event){}
            public void OnPostLoad(PostLoadEvent @event){}
		}

		[EventListener(SkipEvent = new[] {typeof (IPreLoadEventListener)})]
		private class MultipleSkippedListener : IPreLoadEventListener, IPostLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event){}
			public void OnPostLoad(PostLoadEvent @event){}
		}

		[EventListener(SkipEvent = new[] {typeof (ILoadEventListener)})]
		private class MultipleSkippedReplacementListener : ILoadEventListener, IDeleteEventListener
		{
			public void OnLoad(LoadEvent @event, LoadType loadType){}
			public void OnDelete(DeleteEvent @event){}
			public void OnDelete(DeleteEvent @event, ISet transientEntities){}
		}

		[EventListener(Singleton = true)]
		private class MultipleSingletonListener : IPreLoadEventListener, IPostLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event){}
			public void OnPostLoad(PostLoadEvent @event){}
		}

		[EventListener(Include=new[]{typeof(ActiveRecordBase)})]
		private class FirstBaseIncludeListener : IPreLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event) { }
		}

		[EventListener(Include = new[] { typeof(Test2ARBase) })]
		private class SecondBaseIncludeListener : IPreLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event) { }
		}

		[EventListener(Exclude = new[] { typeof(ActiveRecordBase) })]
		private class FirstBaseExcludeListener : IPreLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event) { }
		}

		[EventListener(Exclude = new[] { typeof(Test2ARBase) })]
		private class SecondBaseExcludeListener : IPreLoadEventListener
		{
			public void OnPreLoad(PreLoadEvent @event) { }
		}

		[EventListener(ReplaceExisting = true, Include = new[] { typeof(Test2ARBase) })]
		private class SecondBaseReplacementLoadListener : ILoadEventListener
		{
			public void OnLoad(LoadEvent @event, LoadType loadType) { }
		}

		#endregion

		protected static void InitializeSingleBase(params Type[] listenerTypes)
		{
			InitializeWithListeners(listenerTypes, new[] {typeof (Blog), typeof (Post)});
		}

		private static void InitializeWithListeners(Type[] listenerTypes, Type[] arTypes)
		{
			var typesToRegister = new Type[listenerTypes.Length + arTypes.Length];
			Array.ConstrainedCopy(arTypes,0,typesToRegister,0,arTypes.Length);
			Array.ConstrainedCopy(listenerTypes,0,typesToRegister,arTypes.Length,listenerTypes.Length);

			ActiveRecordStarter.Initialize(GetConfigSource(),typesToRegister);
		}

		protected static void InitializeMultipleBases(params Type[] listenerTypes)
		{
			InitializeWithListeners(listenerTypes, new[]
			                                       	{
			                                       		typeof(Blog),
														typeof(Post),
														typeof(Test2ARBase),
														typeof(OtherDbBlog),
														typeof(OtherDbPost)
			                                       	});
		}
	}
}