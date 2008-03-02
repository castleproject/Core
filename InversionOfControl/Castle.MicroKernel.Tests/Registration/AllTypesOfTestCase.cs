// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using Castle.Core;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

#if DOTNET35
	using System.Linq;
#endif

	[TestFixture]
	public class AllTypesOfTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[Test]
		public void RegisterAssemblyTypes_NoService_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<ICommon>
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
			kernel.Register(AllTypesOf<ICommon>
				.FromAssembly(Assembly.GetExecutingAssembly())
				.WithService.FirstInterface()
				);

			IHandler[] handlers = kernel.GetHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);

			handlers = kernel.GetAssignableHandlers(typeof(ICommon));
			Assert.AreNotEqual(0, handlers.Length);
		}

		[Test]
		public void RegisterAssemblyTypes_DefaultService_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<ICommon>
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
			kernel.Register(AllTypesOf<ICommon>
				.FromAssembly(Assembly.GetExecutingAssembly())
				.Configure(delegate(ComponentRegistration component)
					{
						component.LifeStyle.Transient
							.Named(component.Implementation.FullName + "XYZ");
					})
				);

			foreach (IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
				Assert.AreEqual(handler.ComponentModel.Implementation.FullName + "XYZ", handler.ComponentModel.Name);
			}
		}

		[Test]
		public void RegisterGenericTypes_WithGenericDefinition_RegisteredInContainer()
		{
			kernel.Register(AllTypes
				.From(typeof(DefaultRepository<>))
				.WithService.FirstInterface()
				);

			Type t = typeof(IRepository<CustomerImpl>);

			IRepository<CustomerImpl> repository = kernel.Resolve<IRepository<CustomerImpl>>();
		}

#if DOTNET35

		[Test]
		public void RegisterAssemblyTypes_IfCondition_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<ICustomer>
				.FromAssembly(Assembly.GetExecutingAssembly())
				.If(t => t.FullName.Contains("Chain"))
				);

			IHandler[] handlers = kernel.GetAssignableHandlers(typeof(ICustomer));
			Assert.AreNotEqual(0, handlers.Length);
			
			foreach (IHandler handler in handlers)
			{
				Assert.IsTrue(handler.ComponentModel.Implementation.FullName.Contains("Chain"));
			}
		}

		[Test]
		public void RegisterAssemblyTypes_UnlessCondition_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<ICustomer>
				.FromAssembly(Assembly.GetExecutingAssembly())
				.Unless(t => typeof(CustomerChain1).IsAssignableFrom(t))
				);

			foreach (IHandler handler in kernel.GetAssignableHandlers(typeof(ICustomer)))
			{
				Assert.IsFalse(typeof(CustomerChain1).IsAssignableFrom(handler.ComponentModel.Implementation));
			}
		}
		
		[Test]
		public void RegisterTypes_WithLinq_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<CustomerChain1>
				.Pick(from type in Assembly.GetExecutingAssembly().GetExportedTypes()
					  where type.IsDefined(typeof(SerializableAttribute), true)
					  select type
					));

			IHandler[] handlers = kernel.GetAssignableHandlers(typeof(CustomerChain1));
			Assert.AreEqual(2, handlers.Length);		
		}

		[Test]
		public void RegisterAssemblyTypes_WithKLinqConfiguration_RegisteredInContainer()
		{
			kernel.Register(AllTypesOf<ICommon>
				.FromAssembly(Assembly.GetExecutingAssembly())
				.Configure(component => component.LifeStyle.Transient
							.Named(component.Implementation.FullName + "XYZ")
							)
				);

			foreach (IHandler handler in kernel.GetAssignableHandlers(typeof(ICommon)))
			{
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
				Assert.AreEqual(handler.ComponentModel.Implementation.FullName + "XYZ", handler.ComponentModel.Name);
			}
		}	
		
		#endif	
	}
}