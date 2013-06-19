namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class GenericInterfaceWithGenericMethod
	{
		ProxyGenerator proxyGenerator;
		ProxyGenerationOptions options;

		[SetUp]
		public void setup()
		{
			proxyGenerator = new ProxyGenerator();
			options = new ProxyGenerationOptions();
		}

		[Test]
		public void FailingCastleProxyCase()
		{
			var type = typeof(IMinimumFailure<string>);
			var result = proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[0], options);

			Assert.That(result as IMinimumFailure<string>, Is.Not.Null);
		}

		public interface IMinimumFailure<T>
		{
			void NormalMethod();
			IEnumerable<T> FailingMethod<T2>(T2 pred);
		}

	}
}