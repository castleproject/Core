namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class GenericInterfaceWithGenericMethod
	{
		ProxyGenerator proxyGenerator;
		ProxyGenerationOptions options;

		public GenericInterfaceWithGenericMethod()
		{
			proxyGenerator = new ProxyGenerator();
			options = new ProxyGenerationOptions();
		}

		[Fact]
		public void FailingCastleProxyCase()
		{
			var type = typeof(IMinimumFailure<string>);
			var result = proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[0], options);

			Assert.True(result as IMinimumFailure<string> != null);
		}

		public interface IMinimumFailure<T>
		{
			void NormalMethod();
			IEnumerable<T> FailingMethod<T2>(T2 pred);
		}

	}
}