using NUnit.Framework;

namespace Castle.Windsor.Tests
{
	[TestFixture]
	public class RegisteringTwoServicesForSameType
	{
		public interface IService{}
		public class Srv1 : IService{}
		public class Srv2 : IService { }


		[Test]
		public void ResolvingComponentIsDoneOnFirstComeBasis()
		{
			var windsor = new WindsorContainer();
			windsor.AddComponent<IService, Srv1>("1");
			windsor.AddComponent<IService, Srv1>("2");

			Assert.IsInstanceOf<Srv1>(windsor.Resolve<IService>());
		}

		[Test]
		public void ResolvingComponentIsDoneOnFirstComeBasisWhenNamesAreNotOrdered()
		{
			var windsor = new WindsorContainer();
			windsor.AddComponent<IService, Srv1>("3");
			windsor.AddComponent<IService, Srv1>("2");

			Assert.IsInstanceOf<Srv1>(windsor.Resolve<IService>());
		}
	}
}