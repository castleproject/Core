namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class GenericInterfaceWithGenericMethod
	{
		ProxyGenerator _proxyGenerator;
		ProxyGenerationOptions _options;

		[SetUp]
		public void setup()
		{
			_proxyGenerator = new ProxyGenerator();
			_options = new ProxyGenerationOptions();
		}

		[Test]
		public void FailingCastleProxyCase()
		{
			var type = typeof(IMinimumFailure<string>);
			var result = _proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[0], _options);

			Assert.That(result as IMinimumFailure<string>, Is.Not.Null);
		}

		public interface IMinimumFailure<T>
		{
			void NormalMethod();
			IEnumerable<T> FailingMethod<T2>(T2 pred);
		}

	}
}