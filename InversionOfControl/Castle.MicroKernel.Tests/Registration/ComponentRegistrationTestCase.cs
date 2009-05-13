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


namespace Castle.MicroKernel.Tests.Registration
{
	using System;
	using System.Collections;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable;
	using Castle.Facilities.Startable.Tests.Components;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Tests.Lifestyle.Components;
	using Castle.MicroKernel.Tests.Configuration.Components;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

	[TestFixture]
	public class ComponentRegistrationTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void AddComponent_WhichIsNull_ThrowsNullArgumentException()
		{
			// Previously the kernel assummed everything was OK, and null reffed instead.
			kernel.Register(Component.For(Type.GetType("NonExistentType, WohooAssembly")));
		}

		[Test]
		public void AddComponent_WithServiceOnly_RegisteredWithServiceTypeName()
		{
			kernel.Register(
				Component.For<CustomerImpl>());

			IHandler handler = kernel.GetHandler(typeof(CustomerImpl));
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Service);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Implementation);

			CustomerImpl customer = kernel.Resolve<CustomerImpl>();
			Assert.IsNotNull(customer);

			object customer1 = kernel[typeof(CustomerImpl).FullName];
			Assert.IsNotNull(customer1);
			Assert.AreSame(customer, customer1);
		}

		[Test]
		public void AddComponent_WithServiceAndName_RegisteredNamed()
		{
			kernel.Register(
				Component.For<CustomerImpl>()
					.Named("customer")
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual("customer", handler.ComponentModel.Name);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Service);
			Assert.AreEqual(typeof(CustomerImpl), handler.ComponentModel.Implementation);

			CustomerImpl customer = (CustomerImpl) kernel["customer"];
			Assert.IsNotNull(customer);
		}

		[Test]
		[ExpectedException(typeof(ComponentRegistrationException),
			ExpectedMessage = "This component has already been assigned name 'customer'")]
		public void AddComponent_NamedAlreadyAssigned_ThrowsException()
		{
			kernel.Register(
				Component.For<CustomerImpl>()
					.Named("customer")
					.Named("customer1")
					);
		}

		[Test]
		[ExpectedException(typeof(ComponentRegistrationException),
			ExpectedMessage = "There is a component already registered for the given key customer")]
		public void AddComponent_WithSameName_ThrowsException()
		{
			kernel.Register(
				Component.For<CustomerImpl>()
					.Named("customer"),
				Component.For<CustomerImpl>()
					.Named("customer")
				);
		}

		[Test]
		public void AddComponent_WithServiceAndClass_RegisteredWithClassTypeName()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>());

			ICustomer customer = kernel.Resolve<ICustomer>();
			Assert.IsNotNull(customer);

			object customer1 = kernel[typeof(CustomerImpl).FullName];
			Assert.IsNotNull(customer1);
		}

		[Test]
		[ExpectedException(typeof(ComponentRegistrationException),
			ExpectedMessage = "This component has already been assigned implementation Castle.MicroKernel.Tests.ClassComponents.CustomerImpl")]
		public void AddComponent_WithImplementationAlreadyAssigned_ThrowsException()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.ImplementedBy<CustomerImpl2>()
					);
		}

		[Test]
		public void AddComponent_Instance_UsesInstance()
		{
			CustomerImpl customer = new CustomerImpl();

			kernel.Register(
				Component.For<ICustomer>()
					.Named("key")
					.Instance(customer)
					);
			Assert.IsTrue(kernel.HasComponent("key"));
			IHandler handler = kernel.GetHandler("key");
			Assert.AreEqual(customer.GetType(), handler.ComponentModel.Implementation);

			CustomerImpl customer2 = kernel["key"] as CustomerImpl;
			Assert.AreSame(customer, customer2);

			customer2 = kernel[typeof(ICustomer)] as CustomerImpl;
			Assert.AreSame(customer, customer2);
		}

		[Test]
		public void AddComponent_Instance_UsesInstanceWithParameters()
		{
			CustomerImpl2 customer = new CustomerImpl2("ernst","delft", 29);

			kernel.Register(
				Component.For<ICustomer>()
					.Named("key")
					.Instance(customer)
					);
			Assert.IsTrue(kernel.HasComponent("key"));
			IHandler handler = kernel.GetHandler("key");
			Assert.AreEqual(customer.GetType(), handler.ComponentModel.Implementation);

			CustomerImpl2 customer2 = kernel["key"] as CustomerImpl2;
			Assert.AreSame(customer, customer2);

			customer2 = kernel[typeof(ICustomer)] as CustomerImpl2;
			Assert.AreSame(customer, customer2);
		}

		[Test]
		public void AddComponent_WithExplicitLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.Is(LifestyleType.Transient)
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithTransientLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.Transient
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithSingletonLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.Singleton
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Singleton, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithCustomLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.Custom<MyLifestyleHandler>()
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithThreadLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.PerThread
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Thread, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPerWebRequestLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.PerWebRequest
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.PerWebRequest, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPooledLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.Pooled
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Pooled, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_WithPooledWithSizeLifestyle_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.LifeStyle.PooledWithSize(5, 10)
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(LifestyleType.Pooled, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void AddComponent_Activator_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer")
					.ImplementedBy<CustomerImpl>()
					.Activator<MyCustomerActivator>()
					);

			IHandler handler = kernel.GetHandler("customer");
			Assert.AreEqual(typeof(MyCustomerActivator), handler.ComponentModel.CustomComponentActivator);

			ICustomer customer = kernel.Resolve<ICustomer>();
			Assert.AreEqual("James Bond", customer.Name);
		}

		[Test]
		public void AddComponent_ExtendedProperties_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.ExtendedProperties(
						Property.ForKey("key1").Eq("value1"),
						Property.ForKey("key2").Eq("value2")
						)
					);

			IHandler handler = kernel.GetHandler(typeof(ICustomer));
			Assert.AreEqual("value1", handler.ComponentModel.ExtendedProperties["key1"]);
			Assert.AreEqual("value2", handler.ComponentModel.ExtendedProperties["key2"]);
		}

		[Test]
		public void AddComponent_ExtendedProperties_UsingAnonymousType()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.ExtendedProperties(new { key1 = "value1", key2 = "value2" }));

			IHandler handler = kernel.GetHandler(typeof(ICustomer));
			Assert.AreEqual("value1", handler.ComponentModel.ExtendedProperties["key1"]);
			Assert.AreEqual("value2", handler.ComponentModel.ExtendedProperties["key2"]);
		}

		[Test]
		public void AddComponent_CustomDependencies_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.DependsOn(
						Property.ForKey("Name").Eq("Caption Hook"),
						Property.ForKey("Address").Eq("Fairyland"),
						Property.ForKey("Age").Eq(45)
						)
					);

			ICustomer customer = kernel.Resolve<ICustomer>();
			Assert.AreEqual(customer.Name, "Caption Hook");
			Assert.AreEqual(customer.Address, "Fairyland");
			Assert.AreEqual(customer.Age, 45);
		}

		[Test]
		public void AddComponent_CustomDependencies_UsingAnonymousType()
		{
				kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.DependsOn(new { Name = "Caption Hook", Address = "Fairyland", Age = 45 }));

			ICustomer customer = kernel.Resolve<ICustomer>();
			Assert.AreEqual(customer.Name, "Caption Hook");
			Assert.AreEqual(customer.Address, "Fairyland");
			Assert.AreEqual(customer.Age, 45);
		}

		[Test]
		public void AddComponent_CustomDependenciesDictionary_WorksFine()
		{
			Hashtable customDependencies = new Hashtable();
			customDependencies["Name"] = "Caption Hook";
			customDependencies["Address"] = "Fairyland";
			customDependencies["Age"] = 45;

			kernel.Register(
				Component.For<ICustomer>()
					.ImplementedBy<CustomerImpl>()
					.DependsOn(customDependencies)
					);

			ICustomer customer = kernel.Resolve<ICustomer>();
			Assert.AreEqual(customer.Name, "Caption Hook");
			Assert.AreEqual(customer.Address, "Fairyland");
			Assert.AreEqual(customer.Age, 45);
		}

		[Test]
		public void AddComponent_ServiceOverrides_WorksFine()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer1")
					.ImplementedBy<CustomerImpl>()
					.DependsOn(
						Property.ForKey("Name").Eq("Caption Hook"),
						Property.ForKey("Address").Eq("Fairyland"),
						Property.ForKey("Age").Eq(45)
						),
				Component.For<CustomerChain1>()
					.Named("customer2")
					.DependsOn(
						Property.ForKey("Name").Eq("Bigfoot"),
						Property.ForKey("Address").Eq("Forest"),
						Property.ForKey("Age").Eq(100)
						)
					.ServiceOverrides(
						ServiceOverride.ForKey("customer").Eq("customer1")
						)
				);

			CustomerChain1 customer = (CustomerChain1) kernel["customer2"];
			Assert.IsNotNull(customer.CustomerBase);
			Assert.AreEqual(customer.CustomerBase.Name, "Caption Hook");
			Assert.AreEqual(customer.CustomerBase.Address, "Fairyland");
			Assert.AreEqual(customer.CustomerBase.Age, 45);
		}

		[Test]
		public void AddComponent_ArrayServiceOverrides_WorksFine()
		{
			kernel.Register(
				Component.For<ICommon>()
					.Named("common1")
					.ImplementedBy<CommonImpl1>(),
				Component.For<ICommon>()
					.Named("common2")
					.ImplementedBy<CommonImpl2>(),
				Component.For<ClassWithArrayConstructor>()
					.ServiceOverrides(
						ServiceOverride.ForKey("first").Eq("common2"),
						ServiceOverride.ForKey("services").Eq("common1", "common2")
					)
				);

			ICommon common1 = (ICommon) kernel["common1"];
			ICommon common2 = (ICommon) kernel["common2"];
			ClassWithArrayConstructor component = kernel.Resolve<ClassWithArrayConstructor>();
			Assert.AreSame(common2, component.First);
			Assert.AreEqual(2, component.Services.Length);
			Assert.AreSame(common1, component.Services[0]);
			Assert.AreSame(common2, component.Services[1]);
		}

		[Test]
		public void AddComponent_GenericListServiceOverrides_WorksFine()
		{
			kernel.Register(
				Component.For<ICommon>()
					.Named("common1")
					.ImplementedBy<CommonImpl1>(),
				Component.For<ICommon>()
					.Named("common2")
					.ImplementedBy<CommonImpl2>(),
				Component.For<ClassWithListConstructor>()
					.ServiceOverrides(
						ServiceOverride.ForKey("services").Eq<ICommon>("common1", "common2")
					)
				);

			ICommon common1 = (ICommon)kernel["common1"];
			ICommon common2 = (ICommon)kernel["common2"];
			ClassWithListConstructor component = kernel.Resolve<ClassWithListConstructor>();
			Assert.AreEqual(2, component.Services.Count);
			Assert.AreSame(common1, component.Services[0]);
			Assert.AreSame(common2, component.Services[1]);
		}

		[Test]
		public void AddComponent_ServiceOverrides_UsingAnonymousType()
		{
			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer1")
					.ImplementedBy<CustomerImpl>()
					.DependsOn(
						Property.ForKey("Name").Eq("Caption Hook"),
						Property.ForKey("Address").Eq("Fairyland"),
						Property.ForKey("Age").Eq(45)
						),
				Component.For<CustomerChain1>()
					.Named("customer2")
					.DependsOn(
						Property.ForKey("Name").Eq("Bigfoot"),
						Property.ForKey("Address").Eq("Forest"),
						Property.ForKey("Age").Eq(100)
						)
					.ServiceOverrides(new { customer = "customer1" })
				);

			CustomerChain1 customer = (CustomerChain1) kernel["customer2"];
			Assert.IsNotNull(customer.CustomerBase);
			Assert.AreEqual(customer.CustomerBase.Name, "Caption Hook");
			Assert.AreEqual(customer.CustomerBase.Address, "Fairyland");
			Assert.AreEqual(customer.CustomerBase.Age, 45);
		}

		[Test]
		public void AddComponent_ServiceOverridesDictionary_WorksFine()
		{
			Hashtable serviceOverrides = new Hashtable();
			serviceOverrides["customer"] = "customer1";

			kernel.Register(
				Component.For<ICustomer>()
					.Named("customer1")
					.ImplementedBy<CustomerImpl>()
					.DependsOn(
						Property.ForKey("Name").Eq("Caption Hook"),
						Property.ForKey("Address").Eq("Fairyland"),
						Property.ForKey("Age").Eq(45)
						),
				Component.For<CustomerChain1>()
					.Named("customer2")
					.DependsOn(
						Property.ForKey("Name").Eq("Bigfoot"),
						Property.ForKey("Address").Eq("Forest"),
						Property.ForKey("Age").Eq(100)
						)
					.ServiceOverrides(serviceOverrides)
				);

			CustomerChain1 customer = (CustomerChain1) kernel["customer2"];
			Assert.IsNotNull(customer.CustomerBase);
			Assert.AreEqual(customer.CustomerBase.Name, "Caption Hook");
			Assert.AreEqual(customer.CustomerBase.Address, "Fairyland");
			Assert.AreEqual(customer.CustomerBase.Age, 45);
		}

		[Test]
		public void AddComponent_ArrayConfigurationParameters_WorksFine()
		{
			MutableConfiguration list = new MutableConfiguration("list");
			list.Attributes.Add("type", typeof(ICommon).AssemblyQualifiedName);
			list.Children.Add(new MutableConfiguration("item", "${common1}"));
			list.Children.Add(new MutableConfiguration("item", "${common2}"));

			kernel.Register(
				Component.For<ICommon>()
					.Named("common1")
					.ImplementedBy<CommonImpl1>(),
				Component.For<ICommon>()
					.Named("common2")
					.ImplementedBy<CommonImpl2>(),
				Component.For<ClassWithArrayConstructor>()
					.Parameters(
						Parameter.ForKey("first").Eq("${common2}"),
						Parameter.ForKey("services").Eq(list)
					)
				);

			ICommon common1 = (ICommon)kernel["common1"];
			ICommon common2 = (ICommon)kernel["common2"];
			ClassWithArrayConstructor component = kernel.Resolve<ClassWithArrayConstructor>();
			Assert.AreSame(common2, component.First);
			Assert.AreEqual(2, component.Services.Length);
			Assert.AreSame(common1, component.Services[0]);
			Assert.AreSame(common2, component.Services[1]);
		}

		[Test]
		public void AddComponent_ListConfigurationParameters_WorksFine()
		{
			MutableConfiguration list = new MutableConfiguration("list");
			list.Attributes.Add("type", typeof(ICommon).AssemblyQualifiedName);
			list.Children.Add(new MutableConfiguration("item", "${common1}"));
			list.Children.Add(new MutableConfiguration("item", "${common2}"));

			kernel.Register(
				Component.For<ICommon>()
					.Named("common1")
					.ImplementedBy<CommonImpl1>(),
				Component.For<ICommon>()
					.Named("common2")
					.ImplementedBy<CommonImpl2>(),
				Component.For<ClassWithListConstructor>()
					.Parameters(
						Parameter.ForKey("services").Eq(list)
					)
				);

			ICommon common1 = (ICommon)kernel["common1"];
			ICommon common2 = (ICommon)kernel["common2"];
			ClassWithListConstructor component = kernel.Resolve<ClassWithListConstructor>();
			Assert.AreEqual(2, component.Services.Count);
			Assert.AreSame(common1, component.Services[0]);
			Assert.AreSame(common2, component.Services[1]);
		}

		[Test]
		public void AddComponent_WithComplexConfiguration_WorksFine()
		{
			kernel.Register(
				Component.For<ClassWithComplexParameter>()
					.Configuration(
						Child.ForName("parameters").Eq(
							Attrib.ForName("notUsed").Eq(true),
							Child.ForName("complexparam").Eq(
								Child.ForName("complexparametertype").Eq(
									Child.ForName("mandatoryvalue").Eq("value1"),
									Child.ForName("optionalvalue").Eq("value2")
									)
								)
							)
						)
				);

			ClassWithComplexParameter component = kernel.Resolve<ClassWithComplexParameter>();
			Assert.IsNotNull(component);
			Assert.IsNotNull(component.ComplexParam);
			Assert.AreEqual("value1", component.ComplexParam.MandatoryValue);
			Assert.AreEqual("value2", component.ComplexParam.OptionalValue);			
		}
		
		[Test]
		public void CanUseExistingComponentModelWithComponentRegistration()
		{
			kernel.Register(Component.For<ICustomer>()
				.ImplementedBy<CustomerImpl>()
				);

			IHandler handler = kernel.GetHandler(typeof(ICustomer));
			ComponentRegistration component = Component.For(handler.ComponentModel);

			Assert.AreEqual(typeof(ICustomer), component.ServiceType);			
			Assert.AreEqual(typeof(CustomerImpl), component.Implementation);
		}

		[Test]
		public void AddGenericComponent_WithParameters()
		{
			kernel.Register(Component.For(typeof(IGenericClassWithParameter<>))
				.ImplementedBy(typeof(GenericClassWithParameter<>))
				.Parameters(Parameter.ForKey("name").Eq("NewName"))
				);

			IGenericClassWithParameter<int> instance = kernel.Resolve<IGenericClassWithParameter<int>>();
			Assert.AreEqual("NewName", instance.Name);
		}

		[Test]
		public void AddComponent_UnlessCondition_SkipsRegistration()
		{
			kernel.Register(Component.For<ICustomer>()
				.ImplementedBy<CustomerImpl>()
				);

			kernel.Register(Component.For<ICustomer>()
				.ImplementedBy<CustomerChain1>()
				.Unless(Component.ServiceAlreadyRegistered)
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICustomer));
			Assert.AreEqual(1, handlers.Length);
		}

		[Test]
		public void AddComponent_IfCondition_RegistersComponent()
		{
			kernel.Register(Component.For<ICustomer>()
				.ImplementedBy<CustomerImpl>()
				);

			kernel.Register(Component.For<ICustomer>()
				.ImplementedBy<CustomerChain1>()
				.If(Component.ServiceAlreadyRegistered)
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICustomer));
			Assert.AreEqual(2, handlers.Length);
		}

		[Test]
		public void AddComponent_StartableWithInterface_StartsComponent()
		{
			kernel.AddFacility<StartableFacility>()
				.Register(Component.For<StartableComponent>());

			StartableComponent component = kernel.Resolve<StartableComponent>();

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}

		[Test]
		public void AddComponent_StartableWithoutInterface_StartsComponent()
		{
			kernel.AddFacility<StartableFacility>()
				.Register(Component.For<NoInterfaceStartableComponent>()
					.StartUsingMethod("Start")
					.StopUsingMethod("Stop")
					);

			NoInterfaceStartableComponent component = kernel.Resolve<NoInterfaceStartableComponent>();

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}
	}
}