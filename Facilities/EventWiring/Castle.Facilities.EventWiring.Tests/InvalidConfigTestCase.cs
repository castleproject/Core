namespace Castle.Facilities.EventWiring.Tests
{
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class InvalidConfigTestCase
	{
		private WindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer(ConfigHelper.ResolvePath("config/Invalid.config"));
		}
		
		[Test, ExpectedException(typeof(EventWiringException))]
		public void InvalidConfigured()
		{
			container.Resolve("SimplePublisher");
		}
	}
}
