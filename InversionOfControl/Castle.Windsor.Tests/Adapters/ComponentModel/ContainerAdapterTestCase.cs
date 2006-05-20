#region Copyright
// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

#endregion

namespace Castle.Windsor.Tests.Adapters.ComponentModel
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.Design;

	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.Windsor.Adapters.ComponentModel;

	using Castle.Windsor.Tests.Components;
    using System.Collections;

	[TestFixture]
	public class ContainerAdapterTestCase
	{
		private bool disposed;
		private int calledCount;
		private IContainerAdapter container;

		[SetUp]
		public void Init()
		{
			calledCount = 0;
			disposed = false;
			container = new ContainerAdapter();
		}

		[TearDown]
		public void Dispose()
		{
			container.Dispose();
		}

		[Test]
		public void GetIntrinsicServices()
		{
			Assert.IsNotNull( container.GetService( typeof(IContainer) ) );
			Assert.IsNotNull( container.GetService( typeof(IServiceContainer) ) );
			Assert.IsNotNull( container.GetService( typeof(IWindsorContainer) ) );
			Assert.IsNotNull( container.GetService( typeof(IKernel) ) );
		}

		[Test]
		[ExpectedException( typeof(ArgumentException),
			 "A service for type 'Castle.Windsor.IWindsorContainer' already exists")]
		public void AddIntrinsicService()
		{
			container.AddService( typeof(IWindsorContainer), new WindsorContainer() );
		}

		[Test]
		[ExpectedException( typeof(ArgumentException),  "Cannot remove an instrinsic service")]
		public void RemoveInstrinsicService()
		{
			container.RemoveService( typeof(IWindsorContainer) );
		}

		[Test]
		public void GetExistingServiceFromKernel()
		{
			WindsorContainer windsor = new WindsorContainer();
			
			windsor.AddComponent( "calculator", typeof(ICalcService), typeof(CalculatorService) );

			IContainerAdapter adapter = new ContainerAdapter( windsor );

			ICalcService service = (ICalcService) adapter.GetService( typeof(ICalcService) );

			Assert.IsNotNull( service );
		}

		[Test]
		public void AddUnamedComponent()
		{
			IComponent component = new Component();

			container.Add( component );
			ISite site = component.Site;
			Assert.IsNotNull( site );
			Assert.AreSame( container, site.Container );
			Assert.AreEqual( 1, container.Components.Count );
			Assert.IsNotNull( site.GetService( typeof(IHandler) ) );

			IComponent component2 = container.Components[0];
			Assert.AreSame( component, component2 );
			Assert.AreSame( container, component2.Site.Container );
		}

		[Test]
		public void AddNamedComponent()
		{
			IComponent component = new Component();

			container.Add( component, "myComponent" );
			ISite site = component.Site;
			Assert.IsNotNull( site );
			Assert.AreSame( container, site.Container );
			Assert.AreEqual( 1, container.Components.Count );
			Assert.IsNotNull( container.Components["myComponent"] );
			Assert.IsTrue( container.Container.Kernel.HasComponent( "myComponent" ) );
			Assert.IsNotNull( site.GetService( typeof(IHandler) ) );

			IComponent component2 = container.Components["myComponent"];
			Assert.AreSame( component2, component );
			Assert.AreSame( component2, container.Container["myComponent"] );
			Assert.AreSame( container, component2.Site.Container );
		}

		[Test]
		[ExpectedException( typeof(ArgumentException),
			 "There is a component already registered for the given key myComponent" )]
		public void AddDuplicateComponent()
		{
			container.Add( new Component(), "myComponent" );
			container.Add( new Component(), "myComponent" );
		}

		[Test]
		public void RemoveUnnamedComponent()
		{
			IComponent component = new Component();

			container.Add( component );
			IContainerAdapterSite site = component.Site as IContainerAdapterSite;
			Assert.IsNotNull( site );

			container.Remove( component );
			Assert.IsNull( component.Site );
			Assert.AreEqual( 0, container.Components.Count );
			Assert.IsFalse( container.Container.Kernel.HasComponent( site.EffectiveName ) );
		}

		[Test]
		public void RemoveNamedComponent()
		{
			IComponent component = new Component();

			container.Add( component, "myComponent" );
			ISite site = component.Site as ISite;
			Assert.IsNotNull( site );

			container.Remove( component );
			Assert.IsNull( component.Site );
			Assert.AreEqual( 0, container.Components.Count );
			Assert.IsFalse( container.Container.Kernel.HasComponent( "myComponent" ) );
		}

		[Test]
		public void RemoveComponentFromWindsor()
		{
			IComponent component = new Component();

			container.Add( component, "myComponent" );
			IContainerAdapterSite site = component.Site as IContainerAdapterSite;

			container.Container.Kernel.RemoveComponent( site.EffectiveName );
			Assert.AreEqual( 0, container.Components.Count );
		}

		[Test]
		public void GetComponentHandlers()
		{
			IComponent component = new Component();

			container.Add( component );

            IHandler[] handlers = container.Container.Kernel.GetHandlers(typeof(IComponent));
            Assert.AreEqual(component, handlers[0].Resolve());
		}

		[Test]
		public void AddServiceInstance()
		{
			ICalcService service = new CalculatorService();
			
			container.AddService( typeof(ICalcService), service );

			Assert.AreSame( container.GetService( typeof(ICalcService) ), service );
			Assert.AreSame( container.Container[typeof(ICalcService)], service );
		}

		[Test]
		public void AddPromotedServiceInstance()
		{
			ContainerAdapter child = new ContainerAdapter();
			container.Add( child );

			ICalcService service = new CalculatorService();
			
			child.AddService( typeof(ICalcService), service, true );

			Assert.AreSame( child.GetService( typeof(ICalcService) ), service );
			Assert.AreSame( container.GetService( typeof(ICalcService) ), service );

			container.Remove( child );
			Assert.IsNull( child.GetService( typeof(ICalcService) ) );
			Assert.AreSame( container.GetService( typeof(ICalcService) ), service );
		}

		[Test]
		[ExpectedException( typeof(ArgumentException),
			 "A service for type 'Castle.Windsor.Tests.Components.ICalcService' already exists")]
		public void AddExistingServiceInstance()
		{
			container.AddService( typeof(ICalcService), new CalculatorService() );
			container.AddService( typeof(ICalcService), new CalculatorService() );
		}

		[Test]
		public void AddServiceCreatorCallback()
		{
			ServiceCreatorCallback callback = new ServiceCreatorCallback(CreateCalculatorService);

			container.AddService( typeof(ICalcService), callback );

			ICalcService service = (ICalcService) container.GetService( typeof(ICalcService) );

			Assert.IsNotNull( service );
			Assert.AreSame( service, container.Container[typeof(ICalcService)] );

			service = (ICalcService) container.GetService( typeof(ICalcService) );
			Assert.AreEqual( 1, calledCount );
		}

		[Test]
		public void AddPromotedServiceCreatorCallback()
		{
			ContainerAdapter child = new ContainerAdapter();
			container.Add( child );

			ServiceCreatorCallback callback = new ServiceCreatorCallback(CreateCalculatorService);

			child.AddService( typeof(ICalcService), callback, true );

			ICalcService service = (ICalcService) child.GetService( typeof(ICalcService) );
			Assert.IsNotNull( service );

			ICalcService promotedService = (ICalcService) container.GetService( typeof(ICalcService) );
			Assert.IsNotNull( service );

			Assert.AreSame( service, promotedService );

			container.Remove( child );
			Assert.IsNull( child.GetService( typeof(ICalcService) ) );
			Assert.AreSame( container.GetService( typeof(ICalcService) ), service );
		}

		[Test]
		public void RemoveServiceInstance()
		{
			ICalcService service = new CalculatorService();
			
			container.AddService( typeof(ICalcService), service );
			container.RemoveService( typeof(ICalcService ) );
			Assert.IsNull( container.GetService( typeof(ICalcService) ) );
			Assert.IsFalse( container.Container.Kernel.HasComponent( typeof(ICalcService) ));
		}

		[Test]
		public void RemovePromotedServiceInstance()
		{
			ContainerAdapter child = new ContainerAdapter();
			container.Add( child );

			ICalcService service = new CalculatorService();
			
			child.AddService( typeof(ICalcService), service, true );
			Assert.IsNotNull( child.GetService( typeof(ICalcService) ) );

			child.RemoveService( typeof(ICalcService), true );
			Assert.IsNull( child.GetService( typeof(ICalcService) ) );
			Assert.IsNull( container.GetService( typeof(ICalcService) ) );
		}

		public void ChainContainers()
		{
			ICalcService service = new CalculatorService();
			container.AddService( typeof(ICalcService), service );

			IContainerAdapter adapter = new ContainerAdapter( container );

			Assert.AreSame( service, container.GetService( typeof(ICalcService) ) );
		}

		[Test]
		public void ComponentLifecyle()
		{
			TestComponent component = new TestComponent();
			Assert.IsFalse( component.IsSited );
			Assert.IsFalse( component.IsDisposed );

			container.Add( component );
			Assert.IsTrue( component.IsSited );

			container.Dispose();
			Assert.IsTrue( component.IsDisposed );
		}

		[Test]
		public void ContainerLifecyle()
		{
			container.Disposed += new EventHandler(Container_Disposed);
			Assert.IsFalse(disposed);

			container.Dispose();
			Assert.IsTrue(disposed);
		}

		[Test]
		public void DisposeWindsorContainer()
		{
			container.Disposed += new EventHandler(Container_Disposed);
			Assert.IsFalse(disposed);

			container.Container.Dispose();
			Assert.IsTrue(disposed);
		}

		private object CreateCalculatorService(IServiceContainer container, Type serviceType)
		{
			++calledCount;
			return new CalculatorService();
		}

		private void Container_Disposed(object source, EventArgs args)
		{
			disposed = true;
		}
	}
}
