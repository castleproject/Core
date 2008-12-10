using System;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests.Bugs
{
	[TestFixture]
	public class IoC_108
	{
		[Test]
		public void Should_not_fail_when_constructor_parameter_and_public_property_with_private_setter_have_same_name()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddComponent<Service2>();
			kernel.AddComponent<Service1>();

			try
			{
				Service2 svc = kernel.Resolve<Service2>();
				Assert.IsNotNull(svc);
			}
			catch (NullReferenceException)
			{
				Assert.Fail("Should not have thrown a NullReferenceException");
			}
		}

		public class Service1
		{
			public void OpA() { }
			public void OpB() { }
		}

		public class Service2
		{
			private Service1 m_Service1;

			public Service1 Service1
			{
				get { return m_Service1; }
				private set { m_Service1 = value; }
			}

			public Service2(Service1 service1)
			{
				Service1 = service1;
			}
		}
	}

}
