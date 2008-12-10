using System;
using Castle.MicroKernel.ComponentActivator;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests.Bugs
{
	/// <summary>
	/// For IoC-120 also
	/// </summary>
	[TestFixture]
	public class IoC_83
	{
		[Test]
		public void When_attemting_to_resolve_component_with_nonpublic_ctor_should_throw_meaningfull_exception()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddComponent<WithNonPublicCtor>();

			bool exceptionThrown = false;

			try
			{
				WithNonPublicCtor svc = kernel.Resolve<WithNonPublicCtor>();
				Assert.IsNotNull(svc);
			}
			catch (ComponentActivatorException exception)
			{
				ComponentActivatorException inner = exception.InnerException as ComponentActivatorException;
				Assert.IsNotNull(inner);
				StringAssert.Contains("public", inner.Message, "Exception should say that constructor has to be public.");

				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown, "A {0} should have been thrown.", typeof(ComponentActivatorException).Name);
		}

		public class WithNonPublicCtor
		{
			protected WithNonPublicCtor()
			{
			}
		}
	}

}
