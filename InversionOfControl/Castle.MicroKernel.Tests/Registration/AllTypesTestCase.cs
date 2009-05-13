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
using System.Threading;

namespace Castle.MicroKernel.Tests.Registration
{
	using System;
	using System.Reflection;
	using Castle.Core;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;
	using System.Linq;

	[TestFixture]
	public class AllTypesTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[Test]
		public void RegisterAssemblyTypes_BasedOn_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of(typeof(ICommon))
			                	.FromAssembly(Assembly.GetExecutingAssembly())
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_NoService_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_FirstInterfaceService_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_LookupInterfaceService_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.WithService.FromInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetHandlers(typeof(ICommonSub1));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommonSub1));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetHandlers(typeof(ICommonSub2));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommonSub2));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_DefaultService_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.WithService.Base()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_WithConfiguration_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.Configure(delegate(ComponentRegistration component)
			                	           	{
			                	           		component.LifeStyle.Transient
			                	           			.Named(component.Implementation.FullName + "XYZ");
			                	           	})
				);

			foreach(IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
				Assert.AreEqual(handler.ComponentModel.Implementation.FullName + "XYZ", handler.ComponentModel.Name);
			}
		}

		[Test]
		public void RegisterAssemblyTypes_WithConfigurationBasedOnImplementation_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.Configure(delegate(ComponentRegistration component)
			                	           	{
			                	           		component.LifeStyle.Transient
			                	           			.Named(component.Implementation.FullName + "XYZ");
			                	           	})
			                	.ConfigureFor<CommonImpl1>(
			                	delegate(ComponentRegistration component) { component.DependsOn(Property.ForKey("key1").Eq(1)); })
			                	.ConfigureFor<CommonImpl2>(
			                	delegate(ComponentRegistration component) { component.DependsOn(Property.ForKey("key2").Eq(2)); })
				);

			foreach(IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
				Assert.AreEqual(handler.ComponentModel.Implementation.FullName + "XYZ", handler.ComponentModel.Name);

				if (handler.ComponentModel.Implementation == typeof(CommonImpl1))
				{
					Assert.AreEqual(1, handler.ComponentModel.CustomDependencies.Count);
					Assert.IsTrue(handler.ComponentModel.CustomDependencies.Contains("key1"));
				}
				else if (handler.ComponentModel.Implementation == typeof(CommonImpl2))
				{
					Assert.AreEqual(1, handler.ComponentModel.CustomDependencies.Count);
					Assert.IsTrue(handler.ComponentModel.CustomDependencies.Contains("key2"));
				}
			}
		}

		[Test]
		public void RegisterGenericTypes_BasedOnGenericDefinition_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Pick()
			                	.From(typeof(DefaultRepository<>))
			                	.WithService.FirstInterface()
				);

			IRepository<CustomerImpl> repository = kernel.Resolve<IRepository<CustomerImpl>>();
			Assert.IsNotNull(repository);
		}

		[Test]
		public void RegisterGenericTypes_WithGenericDefinition_RegisteredInContainer()
		{
			kernel.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
			                	.BasedOn(typeof(IValidator<>))
			                	.WithService.Base()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(IValidator<ICustomer>));
			Assert.AreNotEqual(0, handlers.Length);
			IValidator<ICustomer> validator = kernel.Resolve<IValidator<ICustomer>>();
			Assert.IsNotNull(validator);

			handlers = kernel.GetHandlers(typeof(IValidator<CustomerChain1>));
			Assert.AreNotEqual(0, handlers.Length);
			IValidator<CustomerChain1> validator2 = kernel.Resolve<IValidator<CustomerChain1>>();
			Assert.IsNotNull(validator2);
		}

		[Test]
		public void RegisterAssemblyTypes_ClosedGenericTypes_RegisteredInContainer()
		{
			kernel.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
			                	.BasedOn(typeof(IMapper<>))
			                	.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(IMapper<CommonImpl1>));
			Assert.AreEqual(1, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(IMapper<CommonImpl2>));
			Assert.AreEqual(1, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_IfCondition_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICustomer>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.If(t => t.FullName.Contains("Chain"))
				);

			IHandler[] handlers = kernel.GetAssignableHandlers(typeof(ICustomer));
			Assert.AreNotEqual(0, handlers.Length);

			foreach(IHandler handler in handlers)
			{
				Assert.IsTrue(handler.ComponentModel.Implementation.FullName.Contains("Chain"));
			}
		}

		[Test]
		public void RegisterAssemblyTypes_UnlessCondition_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICustomer>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.Unless(t => typeof(CustomerChain1).IsAssignableFrom(t))
				);

			foreach(IHandler handler in kernel.GetAssignableHandlers(typeof(ICustomer)))
			{
				Assert.IsFalse(typeof(CustomerChain1).IsAssignableFrom(handler.ComponentModel.Implementation));
			}
		}

		[Test]
		public void RegisterTypes_WithLinq_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<CustomerChain1>()
			                	.Pick(from type in Assembly.GetExecutingAssembly().GetExportedTypes()
			                	      where type.IsDefined(typeof(SerializableAttribute), true)
			                	      select type
			                	));

			IHandler[] handlers = kernel.GetAssignableHandlers(typeof(CustomerChain1));
			Assert.AreEqual(2, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_WithLinqConfiguration_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.Configure(component => component.LifeStyle.Transient
			                	                        	.Named(component.Implementation.FullName + "XYZ")
			                	)
				);

			foreach(IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
				Assert.AreEqual(handler.ComponentModel.Implementation.FullName + "XYZ", handler.ComponentModel.Name);
			}
		}

		[Test]
		public void RegisterAssemblyTypes_WithLinqConfigurationReturningValue_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ICommon>()
			                	.FromAssembly(Assembly.GetExecutingAssembly())
			                	.Configure(component => component.LifeStyle.Transient)
				);

			foreach(IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
			}
		}

		[Test]
		public void RegisterMultipleAssemblyTypes_BasedOn_RegisteredInContainer()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<ICommon>()
					.BasedOn<ICustomer>()
					.If(t => t.FullName.Contains("Chain"))
					.BasedOn<DefaultTemplateEngine>()
					.BasedOn<DefaultMailSenderService>()
					.BasedOn<DefaultSpamServiceWithConstructor>()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICustomer));
			Assert.AreNotEqual(0, handlers.Length);

			foreach(IHandler handler in handlers)
			{
				Assert.IsTrue(handler.ComponentModel.Implementation.FullName.Contains("Chain"));
			}

			handlers = kernel.GetAssignableHandlers(typeof(DefaultSpamServiceWithConstructor));
			Assert.AreEqual(1, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_WhereConditionSatisifed_RegisteredInContainer()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(t => t.Name == "CustomerImpl")
					.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICustomer));
			Assert.AreEqual(1, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_OnlyPublicTypes_WillNotRegisterNonPublicTypes()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<NonPublicComponent>()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(NonPublicComponent));
			Assert.AreEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_IncludeNonPublicTypes_WillNRegisterNonPublicTypes()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.IncludeNonPublicTypes()
					.BasedOn<NonPublicComponent>()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(NonPublicComponent));
			Assert.AreEqual(1, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_WhenTypeInNamespace_RegisteredInContainer()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(Component.IsInNamespace("Castle.MicroKernel.Tests.ClassComponents"))
					.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICustomer));
			Assert.IsTrue(handlers.Length > 0);
		}

		[Test]
		public void RegisterAssemblyTypes_WhenTypeInMissingNamespace_NotRegisteredInContainer()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(Component.IsInNamespace("Castle.MicroKernel.Tests.FooBar"))
					.WithService.FirstInterface()
				);

			Assert.AreEqual(0, kernel.GetAssignableHandlers(typeof(object)).Length);
		}

		[Test]
		public void RegisterAssemblyTypes_WhenTypeInSameNamespaceAsComponent_RegisteredInContainer()
		{
			kernel.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(Component.IsInSameNamespaceAs<CustomerImpl2>())
					.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICustomer));
			Assert.IsTrue(handlers.Length > 0);
		}

		[Test]
		public void RegisterSpecificTypes_WithGenericDefinition_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Pick()
			                	.From(typeof(CustomerRepository))
			                	.WithService.FirstInterface()
				);

			IRepository<ICustomer> repository = kernel.Resolve<IRepository<ICustomer>>();
			Assert.IsNotNull(repository);
		}

		[Test]
		public void RegisterGenericTypes_BasedOnGenericDefinitionUsingSelect_RegisteredInContainer()
		{
			kernel.Register(AllTypes.Of<ITask>().FromAssembly(Assembly.GetExecutingAssembly())
			                	.WithService.Select((t, b) =>
			                	                    from type in t.GetInterfaces()
			                	                    where !type.Equals(typeof(ITask))
			                	                    select type));
			Assert.IsNotNull(kernel.Resolve<ITask<object>>());
		}
	}
}