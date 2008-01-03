namespace Castle.MonoRail.Framework.Tests.Providers
{
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultLayoutDescriptorProviderTestCase
	{
		private DefaultLayoutDescriptorProvider provider = new DefaultLayoutDescriptorProvider();

		[Test]
		public void CanCollectLayoutFromClass()
		{
			LayoutDescriptor desc = provider.CollectLayout(typeof(LayoutOnController));
	
			Assert.IsNotNull(desc);
			Assert.AreEqual("default", desc.LayoutNames[0]);
		}

		[Test]
		public void CanCollectLayoutFromMethod()
		{
			LayoutDescriptor desc = provider.CollectLayout(typeof(LayoutOnActionController).GetMethod("Action1"));

			Assert.IsNotNull(desc);
			Assert.AreEqual("action", desc.LayoutNames[0]);
		}

		#region Controllers

		[Layout("default")]
		public class LayoutOnController : Controller
		{
		}

		public class LayoutOnActionController : Controller
		{
			[Layout("action")]
			public void Action1()
			{
			}
		}

		#endregion
	}
}
