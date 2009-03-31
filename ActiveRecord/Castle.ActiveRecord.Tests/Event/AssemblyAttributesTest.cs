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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using Attributes;
	using Model;
	using NHibernate.Event;
	using NHibernate.Event.Default;
	using NUnit.Framework;

	[TestFixture]
	public class AssemblyAttributesTest: AbstractEventListenerTest	
	{
		[Test]
		public void Check_that_attribute_is_created()
		{
			var attribute = new AddEventListenerAttribute("Castle.ActiveRecord.Tests");

			var assembly = BuildAssembly(attribute);
			var foundAttributes = (AddEventListenerAttribute[]) assembly.GetCustomAttributes(typeof (AddEventListenerAttribute), false);

			Assert.AreEqual(1, foundAttributes.Length);
			Assert.AreEqual(Assembly.GetAssembly(GetType()), foundAttributes[0].Assembly);
		}

		[Test]
		public void C2_Basic_usage_specifying_an_assembly_name_adds_all_listeners()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute("Castle.ActiveRecord.Tests.Model")));
			AssertListenerWasRegistered<AttributedPreLoadListener>(l => l.PreLoadEventListeners);
			AssertListenerWasRegistered<NonAttributedPreLoadListener>(l => l.PreLoadEventListeners);
		}

		[Test]
		public void C2_Basic_usage_specifying_a_type_adds_only_the_type()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(NonAttributedPreLoadListener))));
			AssertListenerWasRegistered<NonAttributedPreLoadListener>(l => l.PreLoadEventListeners);
			AssertListenerWasNotRegistered<AttributedPreLoadListener>(l => l.PreLoadEventListeners);
		}

		[Test]
		public void C5_Existing_listeners_are_not_removed_when_added_per_assembly_attribute()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(TestLoadListener))));
			AssertListenerWasRegistered<TestLoadListener>(l => l.LoadEventListeners);
			AssertListenerWasRegistered<DefaultLoadEventListener>(l => l.LoadEventListeners);
		}

		[Test]
		public void U3b_Listener_will_be_registered_for_all_events_it_can_react_to()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(MultipleEventListener))));
			AssertListenerWasRegistered<MultipleEventListener>(l => l.PreUpdateEventListeners);
			AssertListenerWasRegistered<MultipleEventListener>(l => l.PreInsertEventListeners);
		}

		[Test]
		public void C4_Source_code_event_listeners_can_be_overridden_per_assembly_specification()
		{
			Initialize(Assembly.Load("Castle.ActiveRecord.Tests.Model"),
				BuildAssembly(new IgnoreEventListenerAttribute("Castle.ActiveRecord.Tests.Model")));
			AssertListenerWasNotRegistered<AttributedPreLoadListener>(l => l.PreLoadEventListeners);
		}

		[Test]
		public void C4_Source_code_event_listeners_can_be_overridden_per_type_specification()
		{
			Initialize(Assembly.Load("Castle.ActiveRecord.Tests.Model"),
				BuildAssembly(new IgnoreEventListenerAttribute(typeof(AttributedPreLoadListener))));
			AssertListenerWasNotRegistered<AttributedPreLoadListener>(l => l.PreLoadEventListeners);
		}

		[Test]
		public void C4_Source_code_event_listeners_can_be_overridden_and_readded()
		{
			Initialize(Assembly.Load("Castle.ActiveRecord.Tests.Model"),
				BuildAssembly(new IgnoreEventListenerAttribute("Castle.ActiveRecord.Tests.Model"),
				new AddEventListenerAttribute(typeof(AttributedPreLoadListener))));
			AssertListenerWasRegistered<AttributedPreLoadListener>(l => l.PreLoadEventListeners);
		}

		[Test]
		public void U1_Listeners_are_registered_for_all_connections()
		{
			InitializeMultiple(BuildAssembly(new AddEventListenerAttribute(typeof (NonAttributedPreLoadListener))));
			AssertListenerWasRegistered<NonAttributedPreLoadListener>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<NonAttributedPreLoadListener, Test2ARBase>(e=>e.PreLoadEventListeners);
		}

		[Test]
		public void U1_Listeners_be_excluded_from_connections()
		{
			InitializeMultiple(BuildAssembly(new AddEventListenerAttribute(typeof(NonAttributedPreLoadListener)){Exclude = new Type[] {typeof(Test2ARBase)}}));
			AssertListenerWasRegistered<NonAttributedPreLoadListener>(e => e.PreLoadEventListeners);
			AssertListenerWasNotRegistered<NonAttributedPreLoadListener, Test2ARBase>(e=>e.PreLoadEventListeners);
		}

		[Test]
		public void U1_Listeners_be_included_to_specific_connections_only()
		{
			InitializeMultiple(BuildAssembly(new AddEventListenerAttribute(typeof(NonAttributedPreLoadListener)) { Include = new Type[] { typeof(Test2ARBase) } }));
			AssertListenerWasNotRegistered<NonAttributedPreLoadListener>(e => e.PreLoadEventListeners);
			AssertListenerWasRegistered<NonAttributedPreLoadListener, Test2ARBase>(e => e.PreLoadEventListeners);
		}

		[Test]
		public void U3c_Listeners_can_be_included_for_specific_events_only()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(MultipleEventListener)) { IncludeEvent = new Type[] { typeof(IPreUpdateEventListener) } }));
			AssertListenerWasNotRegistered<MultipleEventListener>(e => e.PreInsertEventListeners);
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreUpdateEventListeners);
		}

		[Test]
		public void U3c_Listeners_can_be_excluded_from_specific_events()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(MultipleEventListener)) { ExcludeEvent = new Type[] { typeof(IPreUpdateEventListener) } }));
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreInsertEventListeners);
			AssertListenerWasNotRegistered<MultipleEventListener>(e => e.PreUpdateEventListeners);
		}

		[Test]
		public void U4_Listeners_can_be_configured_to_be_used_as_singleton()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(MultipleEventListener)) { Singleton = true }));
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreInsertEventListeners);
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreUpdateEventListeners);
			Assert.AreSame(GetRegisteredListeners(e=>e.PreInsertEventListeners)[0], GetRegisteredListeners(e=>e.PreUpdateEventListeners)[0]);
		}

		[Test]
		public void U4_Listeners_are_not_singletons_by_default()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(MultipleEventListener))));
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreInsertEventListeners);
			AssertListenerWasRegistered<MultipleEventListener>(e => e.PreUpdateEventListeners);
			Assert.AreNotSame(GetRegisteredListeners(e => e.PreInsertEventListeners)[0], GetRegisteredListeners(e => e.PreUpdateEventListeners)[0]);
		}

		[Test]
		public void U5_Listeners_can_be_replaced()
		{
			Initialize(BuildAssembly(new AddEventListenerAttribute(typeof(TestLoadListener)){ReplaceExisting=true}));
			AssertListenerWasRegistered<TestLoadListener>(e=>e.LoadEventListeners);
			AssertListenerWasNotRegistered<NHibernate.Event.Default.DefaultLoadEventListener>(e=>e.LoadEventListeners);
		}

		private static void Initialize(params Assembly[] assemblies)
		{
			ActiveRecordStarter.Initialize(assemblies,GetConfigSource());
		}

		private static void InitializeMultiple(params Assembly[] assemblies)
		{
			ActiveRecordStarter.Initialize(assemblies, GetConfigSource(), typeof(Test2ARBase));
		}

		private static Assembly BuildAssembly(params EventListenerAssemblyAttribute[] attributes)
		{
			var assemblyName = "AttributeHolder." + new StackTrace().GetFrame(1).GetMethod().Name;
			var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Save);

			Type[] constructorArgs = new Type[0];
			object[] constructorValues = new object[0];
			PropertyInfo[] properties = new PropertyInfo[0];
			object[] propertyValues = new object[0];

			foreach(var attribute in attributes)
			{
				if (attribute.Type!= null)
				{
					constructorArgs = new Type[] {typeof(Type)};
					constructorValues = new object[] {attribute.Type};
				} 
				else if (attribute.Assembly != null)
				{
					constructorArgs = new Type[] { typeof(string) };
					constructorValues = new object[] {attribute.Assembly.FullName};
				}
				
				if (attribute is AddEventListenerAttribute)
				{
					var att = (AddEventListenerAttribute) attribute;
					var names = new List<PropertyInfo>();
					var values = new List<object>();

					if (att.Exclude != null)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("Exclude"));
						values.Add(att.Exclude);
					}
					if (att.Include != null)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("Include"));
						values.Add(att.Include);
					}
					if (att.ExcludeEvent != null)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("ExcludeEvent"));
						values.Add(att.ExcludeEvent);
					}
					if (att.IncludeEvent != null)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("IncludeEvent"));
						values.Add(att.IncludeEvent);
					}

					if (att.Singleton)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("Singleton"));
						values.Add(true);
					}

					if (att.ReplaceExisting)
					{
						names.Add(typeof(AddEventListenerAttribute).GetProperty("ReplaceExisting"));
						values.Add(true);
					}

					properties = names.ToArray();
					propertyValues = values.ToArray();
				}

				assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(attribute.GetType().GetConstructor(constructorArgs), constructorValues, properties, propertyValues));				
			}
			assemblyBuilder.Save(assemblyName+".dll");
			return Assembly.Load(assemblyName);
		}
	}
}