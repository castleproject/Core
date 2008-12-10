using System.Collections.Generic;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests.Bugs.Ioc113
{
	[TestFixture]
	public class IoC_113_When_resolving_initializable_disposable_and_startable_component
	{
		private IKernel	kernel;
		private StartableDisposableAndInitializableComponent component;
		private IList<SdiComponentMethods> calledMethods;

		[SetUp]
		public void SetUp()
		{
			kernel = new DefaultKernel();

			kernel.AddFacility<StartableFacility>();

			kernel.Register(
				Component.For<StartableDisposableAndInitializableComponent>()
					.LifeStyle.Transient
				);

			component = kernel.Resolve<StartableDisposableAndInitializableComponent>();
			component.DoSomething();
			kernel.ReleaseComponent(component);

			calledMethods = component.calledMethods;
		}

		[Test]
		public void Should_call_all_methods_once()
		{
			Assert.AreEqual(5, component.calledMethods.Count);
		}

		[Test]
		public void Should_call_initialize_before_start()
		{
			Assert.AreEqual(SdiComponentMethods.Initialize, calledMethods[0]);
			Assert.AreEqual(SdiComponentMethods.Start, calledMethods[1]);
		}

		[Test]
		public void Should_call_stop_before_dispose()
		{
			Assert.AreEqual(SdiComponentMethods.Stop, calledMethods[3]);
			Assert.AreEqual(SdiComponentMethods.Dispose, calledMethods[4]);
		}

		[Test]
		public void Should_call_DoSomething_between_start_and_stop()
		{
			Assert.AreEqual(SdiComponentMethods.DoSomething, calledMethods[2]);
		}
	}
}
