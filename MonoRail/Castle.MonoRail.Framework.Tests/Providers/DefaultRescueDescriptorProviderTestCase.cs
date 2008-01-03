namespace Castle.MonoRail.Framework.Tests.Providers
{
	using System;
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultRescueDescriptorProviderTestCase
	{
		private DefaultRescueDescriptorProvider provider = new DefaultRescueDescriptorProvider();

		[Test]
		public void CanCollectLayoutFromClass()
		{
			RescueDescriptor[] descs = provider.CollectRescues(typeof(RescueOnController));

			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual("general", descs[0].ViewName);
			Assert.AreEqual(typeof(Exception), descs[0].ExceptionType);
		}

		[Test]
		public void CanCollectLayoutFromMethod()
		{
			RescueDescriptor[] descs = provider.CollectRescues(typeof(RescueOnActionController).GetMethod("Action1"));

			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual("action", descs[0].ViewName);
			Assert.AreEqual(typeof(ArgumentNullException), descs[0].ExceptionType);
		}

		#region Controllers

		[Rescue("general")]
		public class RescueOnController : Controller
		{
		}

		public class RescueOnActionController : Controller
		{
			[Rescue("action", typeof(ArgumentNullException))]
			public void Action1()
			{
			}
		}


		#endregion
	}
}
