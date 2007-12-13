// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Tests.Registration
{
	using System.Collections;
	using Castle.Core;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Tests.ClassComponents;
	using Castle.MicroKernel.Tests.Lifestyle.Components;
	using NUnit.Framework;

	[TestFixture]
	public class ComponentRegistrationTestCase
	{
		private IKernel _kernel;

		[SetUp]
		public void Init()
		{
			_kernel = new DefaultKernel();
		}

		[Test]
		public void AddComponent_WithServiceOnly_RegisteredWithServiceTypeName()
		{
			_kernel.AddComponentEx<CustomerImpl>()
				.Register();

			IHandler handler = _kernel.GetHandler(typeof(CustomerImpl));
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Service);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Implementation);

			CustomerImpl customer = _kernel.Resolve<CustomerImpl>();
			Assert.IsNotNull(customer);

			object customer1 = _kernel[typeof(CustomerImpl).FullName];
			Assert.IsNotNull(customer1);
			Assert.AreSame(customer, customer1);
		}

		[Test]
		public void AddComponent_WithServiceAndName_RegisteredWithName()
		{
			_kernel.AddComponentEx<CustomerImpl>()
				.WithName("customer")
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual("customer", handler.ComponentModel.Name);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Service);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Implementation);

			CustomerImpl customer = (CustomerImpl)_kernel["customer"];
			Assert.IsNotNull(customer);
		}

		[Test]
		[ExpectedException(typeof(ComponentRegistrationException),
			"This component has already been assigned name 'customer'")]
		public void AddComponent_WithNameAlreadyAssigned_ThrowsException()
		{
			_kernel.AddComponentEx<CustomerImpl>()
				.WithName("customer")
				.WithName("customer1")
				.Register();
		}

		[Test]
		public void AddComponent_WithServiceAndClass_RegisteredWithClassTypeName()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithImplementation<CustomerImpl>()
				.Register();

			ICustomer customer = _kernel.Resolve<ICustomer>();
			Assert.IsNotNull(customer);

			object customer1 = _kernel[typeof(CustomerImpl).FullName];
			Assert.IsNotNull(customer1);
		}

		[Test]
		[ExpectedException(typeof(ComponentRegistrationException),
		   "This component has already been assigned implementation Castle.MicroKernel.Tests.ClassComponents.CustomerImpl")]
		public void AddComponent_WithImplementationAlreadyAssigned_ThrowsException()
		{
			_kernel.AddComponentEx<CustomerImpl>()
				.WithImplementation<CustomerImpl>()
				.WithImplementation<CustomerImpl2>()
				.Register();
		}

		[Test]
		public void AddComponent_WithInstance_UsesInstance()
		{
			CustomerImpl customer = new CustomerImpl();

			_kernel.AddComponentEx<ICustomer>()
				.WithName("key")
				.WithInstance(customer)
				.Register();				
			Assert.IsTrue(_kernel.HasComponent("key"));

			CustomerImpl customer2 = _kernel["key"] as CustomerImpl;
			Assert.AreSame(customer, customer2);

			customer2 = _kernel[typeof(ICustomer)] as CustomerImpl;
			Assert.AreSame(customer, customer2);
		}

		[Test]
		public void AddComponent_WithTransientLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.Transient
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithSingletonLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.Singleton
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Singleton, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithCustomLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.Custom<MyLifestyleHandler>()
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithThreadLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.PerThread
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Thread, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPerWebRequestLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.PerWebRequest
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.PerWebRequest, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPooledLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.Pooled
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Pooled, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPooledWithSizeLifestyle_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithLifestyle.PooledWithSize(5, 10)
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Pooled, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithActivator_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithName("customer")
				.WithImplementation<CustomerImpl>()
				.WithActivator<MyCustomerActivator>()
				.Register();

			IHandler handler = _kernel.GetHandler("customer");
			Assert.AreEqual(typeof(MyCustomerActivator), handler.ComponentModel.CustomComponentActivator);

			ICustomer customer = _kernel.Resolve<ICustomer>();
			Assert.AreEqual("James Bond", customer.Name);
		}


		[Test]
		public void AddComponent_WithExtendedProperties_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithImplementation<CustomerImpl>()
				.WithExtendedProperties(
					Property.WithKey("key1").Eq("value1"),
					Property.WithKey("key2").Eq("value2"))
				.Register();

			IHandler handler = _kernel.GetHandler(typeof(ICustomer));
			Assert.AreEqual("value1", handler.ComponentModel.ExtendedProperties["key1"]);
			Assert.AreEqual("value2", handler.ComponentModel.ExtendedProperties["key2"]);
		}

		[Test]
		public void AddComponent_WithCustomDependencies_WorksFine()
		{
			_kernel.AddComponentEx<ICustomer>()
				.WithImplementation<CustomerImpl>()
				.WithCustomDependencies(
					Property.WithKey("Name").Eq("Caption Hook"),
					Property.WithKey("Address").Eq("Fairyland"),
					Property.WithKey("Age").Eq(45))
				.Register();

			ICustomer customer = _kernel.Resolve<ICustomer>();
			Assert.AreEqual(customer.Name, "Caption Hook");
			Assert.AreEqual(customer.Address, "Fairyland");
			Assert.AreEqual(customer.Age, 45);
		}

		[Test]
		public void AddComponent_WithCustomDependenciesDictionary_WorksFine()
		{
			Hashtable customDependencies = new Hashtable();
			customDependencies["Name"] = "Caption Hook";
			customDependencies["Address"] = "Fairyland";
			customDependencies["Age"] = 45;

			_kernel.AddComponentEx<ICustomer>()
				.WithImplementation<CustomerImpl>()
				.WithCustomDependencies(customDependencies)
				.Register();

			ICustomer customer = _kernel.Resolve<ICustomer>();
			Assert.AreEqual(customer.Name, "Caption Hook");
			Assert.AreEqual(customer.Address, "Fairyland");
			Assert.AreEqual(customer.Age, 45);
		}

		[Test]
		public void AddComponent_WithServiceOverrides_WorksFine()
		{
			_kernel
				.AddComponentEx<ICustomer>()
					.WithName("customer1")
					.WithImplementation<CustomerImpl>()
					.WithCustomDependencies(
						Property.WithKey("Name").Eq("Caption Hook"),
						Property.WithKey("Address").Eq("Fairyland"),
						Property.WithKey("Age").Eq(45))
				.Register()
				.AddComponentEx<CustomerChain1>()
					.WithName("customer2")
					.WithCustomDependencies(
						Property.WithKey("Name").Eq("Bigfoot"),
						Property.WithKey("Address").Eq("Forest"),
						Property.WithKey("Age").Eq(100))
					.WithServiceOverrides(
						ServiceOverride.WithKey("customer").Eq("customer1"))
				.Register();

			CustomerChain1 customer = (CustomerChain1)_kernel["customer2"];
			Assert.IsNotNull(customer.CustomerBase);
			Assert.AreEqual(customer.CustomerBase.Name, "Caption Hook");
			Assert.AreEqual(customer.CustomerBase.Address, "Fairyland");
			Assert.AreEqual(customer.CustomerBase.Age, 45);
		}

		[Test]
		public void AddComponent_WithServiceOverridesDictionary_WorksFine()
		{
			Hashtable serviceOverrides = new Hashtable();
			serviceOverrides["customer"] = "customer1";

			_kernel
				.AddComponentEx<ICustomer>()
					.WithName("customer1")
					.WithImplementation<CustomerImpl>()
					.WithCustomDependencies(
						Property.WithKey("Name").Eq("Caption Hook"),
						Property.WithKey("Address").Eq("Fairyland"),
						Property.WithKey("Age").Eq(45))
				.Register()
				.AddComponentEx<CustomerChain1>()
					.WithName("customer2")
					.WithCustomDependencies(
						Property.WithKey("Name").Eq("Bigfoot"),
						Property.WithKey("Address").Eq("Forest"),
						Property.WithKey("Age").Eq(100))
					.WithServiceOverrides(serviceOverrides)
				.Register();

			CustomerChain1 customer = (CustomerChain1)_kernel["customer2"];
			Assert.IsNotNull(customer.CustomerBase);
			Assert.AreEqual(customer.CustomerBase.Name, "Caption Hook");
			Assert.AreEqual(customer.CustomerBase.Address, "Fairyland");
			Assert.AreEqual(customer.CustomerBase.Age, 45);
		}
	}
}