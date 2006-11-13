namespace Castle.Facilities.EventWiring.Tests
{
	using Castle.Facilities.EventWiring.Tests.Model;
	using Castle.Windsor;
	using NUnit.Framework;

	public abstract class WiringTestBase
	{
		protected WindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer(ConfigHelper.ResolvePath(GetConfigFile()));
		}

		protected abstract string GetConfigFile();

		[Test]
		public void TriggerSimple()
		{
			SimplePublisher publisher = (SimplePublisher)container["SimplePublisher"];
			SimpleListener listener = (SimpleListener)container["SimpleListener"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void TriggerStaticEvent()
		{
			SimplePublisher publisher = (SimplePublisher)container["SimplePublisher"];
			SimpleListener listener = (SimpleListener)container["SimpleListener2"];

			Assert.IsFalse(listener.Listened);
			Assert.IsNull(listener.Sender);

			publisher.StaticTrigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}
	}
}
