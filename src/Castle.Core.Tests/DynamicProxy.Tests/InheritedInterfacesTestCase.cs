// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Reflection;

	using Interceptors;
	using NUnit.Framework;

	[TestFixture]
	public class InheritedInterfacesTestCase : BasePEVerifyTestCase
	{
		/// <summary>
		/// See DYNPROXY-ISSUE-58 and DYNPROXY-ISSUE-77.
		/// </summary>
		[Test]
		public void InheritedInterfaceWithTarget()
		{
			var proxiedFoo = (IFooExtended) generator.CreateInterfaceProxyWithTargetInterface(
			                                	typeof(IFooExtended), new ImplementedFooExtended(), new StandardInterceptor());
			proxiedFoo.FooExtended();
		}

		[Test]
		public void
			Should_not_have_duplicated_events_for_interface_proxy_with_inherited_target_and_two_inherited_additional_interfaces()
		{
			var target = new HasEventBar();
			object o = generator.CreateInterfaceProxyWithTarget(typeof(IHasEvent),
			                                                    new[] {typeof(IHasEventBar), typeof(IHasEventFoo)}, target,
			                                                    new StandardInterceptor());
			EventInfo[] events = o.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.AreEqual(3, events.Length);
		}

		[Test]
		public void Should_not_have_duplicated_properties_for_interface_proxy_with_inherited_target_and_two_inherited_additional_interfaces()
		{
			var target = new HasPropertyBar();
			object o = generator.CreateInterfaceProxyWithTarget(typeof(IHasProperty),
			                                                    new[] {typeof(IHasPropertyBar), typeof(IHasPropertyFoo)}, target,
			                                                    new StandardInterceptor());
			PropertyInfo[] properties = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.AreEqual(3, properties.Length);
		}

		[Test]
		public void ShouldGenerateProxyWithoutTargetAndWithDuplicatedBaseInterface()
		{
			var foo =
				(IHasMethod)
				generator.CreateInterfaceProxyWithoutTarget(typeof(IHasMethod), new[] {typeof(IFooExtended), typeof(IBarFoo)},
				                                            new DoNothingInterceptor());

			foo.Foo();
			((IFooExtended) foo).FooExtended();
			((IBarFoo) foo).Bar();
		}

		[Test]
		public void TargetImplementsOneInterfaceThatHasDuplicatedBaseInterfaceWithAdditionalProxiedInterfaces()
		{
			var target = new ImplementedFooExtended();

		    var foo =
		        (IHasMethod)
		        generator.CreateInterfaceProxyWithTarget(typeof (IHasMethod), new[] {typeof (IFooExtended), typeof (IBarFoo)},
		                                                 target,
		                                                 new ProceedOnTypeInterceptor(typeof (IBarFoo)));

			foo.Foo();
			((IFooExtended)foo).FooExtended();
			((IBarFoo)foo).Bar();
		}
	}

	public class HasPropertyBar : IHasPropertyBar
	{
		#region IHasPropertyBar Members

		public int Prop { get; set; }
		public string Bar { get; set; }

		#endregion
	}

	public interface IHasPropertyBar : IHasProperty
	{
		string Bar { get; set; }
	}

	public interface IHasPropertyFoo : IHasProperty
	{
		DateTime Foo { get; set; }
	}

	public interface IHasProperty
	{
		int Prop { get; set; }
	}

	public interface IHasEvent
	{
		event EventHandler MyEvent;
	}

	public interface IHasEventFoo : IHasEvent
	{
		event EventHandler EventFoo;
	}

	public interface IHasEventBar : IHasEvent
	{
		event EventHandler Bar;
	}

	public class HasEventBar : IHasEventBar
	{
		#region IHasEventBar Members

		public event EventHandler MyEvent;
		public event EventHandler Bar;

		#endregion

		public void RaiseMyEvent()
		{
			MyEvent(null, EventArgs.Empty);
		}

		public void RaiseBar()
		{
			Bar(null, EventArgs.Empty);
		}
	}

	public interface IHasMethod
	{
		void Foo();
	}

	public interface IFooExtended : IHasMethod
	{
		void FooExtended();
	}

	public interface IBarFoo : IHasMethod
	{
		void Bar();
	}

	public class ImplementedFooExtended : IFooExtended
	{
		#region IFooExtended Members

		public void FooExtended()
		{
		}

		public void Foo()
		{
		}

		#endregion
	}
}