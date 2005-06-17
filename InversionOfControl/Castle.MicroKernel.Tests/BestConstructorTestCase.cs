using Castle.Model.Configuration;
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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.SubSystems.Configuration;

	public class ServiceUser
	{
		private A _a;
		private B _b;
		private C _c;

		public ServiceUser(A a)
		{
			if (a == null) throw new ArgumentNullException();
			_a = a;
		}

		public ServiceUser(A a, B b) : this(a)
		{
			if (b == null) throw new ArgumentNullException();
			_b = b;
		}

		public ServiceUser(A a, B b, C c) : this(a, b)
		{
			if (c == null) throw new ArgumentNullException();
			_c = c;
		}

		public A AComponent
		{
			get { return _a; }
		}

		public B BComponent
		{
			get { return _b; }
		}

		public C CComponent
		{
			get { return _c; }
		}
	}

	public class ServiceUser2 : ServiceUser
	{
		private string _name;
		private int _port;
		private int _scheduleinterval;

		public ServiceUser2(A a, String name, int port) : base(a)
		{
			_name = name;
			_port = port;
		}

		public ServiceUser2(A a, String name, int port, int scheduleinterval) : this(a, name, port)
		{
			_scheduleinterval = scheduleinterval;
		}

		public String Name
		{
			get { return _name; }
		}

		public int Port
		{
			get { return _port; }
		}

		public int ScheduleInterval
		{
			get { return _scheduleinterval; }
		}
	}

	/// <summary>
	/// Summary description for BestConstructorTestCase.
	/// </summary>
	[TestFixture]
	public class BestConstructorTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void ConstructorWithMoreArguments()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "c", typeof(C) );
			kernel.AddComponent( "service", typeof(ServiceUser) );

			ServiceUser service = (ServiceUser) kernel["service"];

			Assert.IsNotNull( service );
			Assert.IsNotNull( service.AComponent );
			Assert.IsNotNull( service.BComponent );
			Assert.IsNotNull( service.CComponent );
		}

		[Test]
		public void ConstructorWithTwoArguments()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "b", typeof(B) );
			kernel.AddComponent( "service", typeof(ServiceUser) );

			ServiceUser service = (ServiceUser) kernel["service"];

			Assert.IsNotNull( service );
			Assert.IsNotNull( service.AComponent );
			Assert.IsNotNull( service.BComponent );
			Assert.IsNull( service.CComponent );
		}

		[Test]
		public void ConstructorWithOneArgument()
		{
			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "service", typeof(ServiceUser) );

			ServiceUser service = (ServiceUser) kernel["service"];

			Assert.IsNotNull( service );
			Assert.IsNotNull( service.AComponent );
			Assert.IsNull( service.BComponent );
			Assert.IsNull( service.CComponent );
		}

		[Test]
		public void ParametersAndServicesBestCase()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();

			MutableConfiguration config = new MutableConfiguration("component");
			MutableConfiguration parameters = (MutableConfiguration) 
				config.Children.Add( new MutableConfiguration("parameters") );
			parameters.Children.Add( new MutableConfiguration("name", "hammett") );
			parameters.Children.Add( new MutableConfiguration("port", "120") );

			store.AddComponentConfiguration("service", config);

			kernel.ConfigurationStore = store;

			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "service", typeof(ServiceUser2) );

			ServiceUser2 service = (ServiceUser2) kernel["service"];

			Assert.IsNotNull( service );
			Assert.IsNotNull( service.AComponent );
			Assert.IsNull( service.BComponent );
			Assert.IsNull( service.CComponent );
			Assert.AreEqual( "hammett", service.Name );
			Assert.AreEqual( 120, service.Port );
		}

		[Test]
		public void ParametersAndServicesBestCase2()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();

			MutableConfiguration config = new MutableConfiguration("component");
			MutableConfiguration parameters = (MutableConfiguration) 
				config.Children.Add( new MutableConfiguration("parameters") );
			parameters.Children.Add( new MutableConfiguration("name", "hammett") );
			parameters.Children.Add( new MutableConfiguration("port", "120") );
			parameters.Children.Add( new MutableConfiguration("Scheduleinterval", "22") );

			store.AddComponentConfiguration("service", config);

			kernel.ConfigurationStore = store;

			kernel.AddComponent( "a", typeof(A) );
			kernel.AddComponent( "service", typeof(ServiceUser2) );

			ServiceUser2 service = (ServiceUser2) kernel["service"];

			Assert.IsNotNull( service );
			Assert.IsNotNull( service.AComponent );
			Assert.IsNull( service.BComponent );
			Assert.IsNull( service.CComponent );
			Assert.AreEqual( "hammett", service.Name );
			Assert.AreEqual( 120, service.Port );
			Assert.AreEqual( 22, service.ScheduleInterval );
		}
	}
}
