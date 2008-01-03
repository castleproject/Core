namespace Castle.MonoRail.Framework.Tests.Providers
{
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using Helpers;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultHelperDescriptorProviderTestCase
	{
		private DefaultHelperDescriptorProvider provider = new DefaultHelperDescriptorProvider();

		[Test]
		public void ShouldReturnEmptyArrayForControllerWithNoHelperAttribute()
		{
			HelperDescriptor[] descs = provider.CollectHelpers(typeof(NoHelperController));
			Assert.IsNotNull(descs);
			Assert.AreEqual(0, descs.Length);
		}

		[Test]
		public void ShouldReturnSingleDescriptorForControllerWithAHelperAttribute()
		{
			HelperDescriptor[] descs = provider.CollectHelpers(typeof(SingleHelperController));
			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual(typeof(DummyHelper), descs[0].HelperType);
			Assert.AreEqual("abc", descs[0].Name);
		}

		[Test]
		public void ShouldReturnDescriptorsForControllerWithMultiHelperAttributes()
		{
			HelperDescriptor[] descs = provider.CollectHelpers(typeof(MultiHelperController));
			Assert.IsNotNull(descs);
			Assert.AreEqual(2, descs.Length);
		}

		#region Controllers

		public class NoHelperController : Controller
		{
		}

		[Helper(typeof(DummyHelper), "abc")]
		public class SingleHelperController : Controller
		{
		}

		[Helper(typeof(DummyHelper))]
		[Helper(typeof(DummyHelper))]
		public class MultiHelperController : Controller
		{
		}

		public class DummyHelper : AbstractHelper
		{

		}

		#endregion
	}
}
