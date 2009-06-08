using Castle.MicroKernel.Handlers;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests.Bugs
{
	[TestFixture]
	public class IoC_141
	{
		[Test]
		public void Can_resolve_open_generic_service_with_closed_generic_parameter()
		{
			var kernel = new DefaultKernel();
			kernel.AddComponent("processor", typeof(IProcessor<>), typeof(DefaultProcessor<>));
			kernel.AddComponent("assembler", typeof(IAssembler<object>), typeof(ObjectAssembler));
			Assert.IsInstanceOf(typeof(DefaultProcessor<object>), kernel.Resolve<IProcessor<object>>());
		}

		[Test]
		public void Can_resolve_service_with_open_generic_parameter_with_closed_generic_parameter()
		{
			var kernel = new DefaultKernel();
			kernel.AddComponent("service1", typeof(IService), typeof(Service1));
			kernel.AddComponent("processor", typeof(IProcessor<>), typeof(DefaultProcessor<>));
			kernel.AddComponent("assembler", typeof(IAssembler<object>), typeof(ObjectAssembler));
			Assert.IsInstanceOf(typeof(Service1), kernel.Resolve<IService>());
		}

		[Test]
		[ExpectedException(typeof(HandlerException))]
		public void Throws_right_exception_when_not_found_matching_generic_service()
		{
			var kernel = new DefaultKernel();
			kernel.AddComponent("processor", typeof(IProcessor<>), typeof(DefaultProcessor<>));
			kernel.AddComponent("assembler", typeof(IAssembler<object>), typeof(ObjectAssembler));
			kernel.Resolve<IProcessor<int>>();
		}

		public interface IService
		{
		}

		public interface IProcessor<T>
		{
		}

		public interface IAssembler<T>
		{
		}

		public class Service1 : IService
		{
			public Service1(IProcessor<object> processor)
			{
			}
		}

		public class DefaultProcessor<T> : IProcessor<T>
		{
			public DefaultProcessor(IAssembler<T> assembler)
			{
			}
		}

		public class ObjectAssembler : IAssembler<object>
		{
		}
	}

}
