namespace Castle.MonoRail.Framework.Tests.Providers
{
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultFilterDescriptorProviderTestCase
	{
		private DefaultFilterDescriptorProvider provider = new DefaultFilterDescriptorProvider();

		[Test]
		public void ShouldReturnEmptyArrayForControllerWithNoFilterAttribute()
		{
			FilterDescriptor[] filterDesc = provider.CollectFilters(typeof(NoFilterController));
			Assert.IsNotNull(filterDesc);
			Assert.AreEqual(0, filterDesc.Length);
		}

		[Test]
		public void ShouldReturnDescriptorForControllerWithAFilterAttribute()
		{
			FilterDescriptor[] filterDesc = provider.CollectFilters(typeof(SingleFilterController));
			Assert.IsNotNull(filterDesc);
			Assert.AreEqual(1, filterDesc.Length);

			Assert.AreEqual(typeof(DummyFilter), filterDesc[0].FilterType);
			Assert.AreEqual(ExecuteWhen.Always, filterDesc[0].When);
			Assert.AreEqual(4, filterDesc[0].ExecutionOrder);
		}

		[Test]
		public void ShouldReturnDescriptorsForControllerWithMultiFilterAttribute()
		{
			FilterDescriptor[] filterDesc = provider.CollectFilters(typeof(MultiFilterController));
			Assert.IsNotNull(filterDesc);
			Assert.AreEqual(3, filterDesc.Length);
		}

		#region Controllers

		public class NoFilterController : Controller
		{
		}

		[Filter(ExecuteWhen.Always, typeof(DummyFilter), ExecutionOrder = 4)]
		public class SingleFilterController : Controller
		{
		}

		[Filter(ExecuteWhen.AfterAction, typeof(DummyFilter))]
		[Filter(ExecuteWhen.BeforeAction, typeof(DummyFilter))]
		[Filter(ExecuteWhen.AfterRendering, typeof(DummyFilter))]
		public class MultiFilterController : Controller
		{
		}

		public class DummyFilter : Filter
		{

		}

		#endregion
	}
}
